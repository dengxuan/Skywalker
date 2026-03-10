// Licensed to the Gordon

using System.IO.Pipelines;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

/// <summary>
/// Stream wrapper that create specific encryption stream only if necessary.
/// </summary>
internal sealed class ResponseEncryptionBody : Stream, IHttpResponseBodyFeature//, IHttpsCompressionFeature
{
    private readonly HttpContext _context;
    private readonly IResponseEncryptionProvider _provider;
    private readonly IHttpResponseBodyFeature _innerBodyFeature;
    private readonly Stream _innerStream;

    private IEncryptionProvider? _encryptionProvider;
    private bool _encryptionChecked;
    private CryptoStream? _encryptionStream;
    private PipeWriter? _pipeAdapter;
    private bool _providerCreated;
    private bool _autoFlush;
    private bool _complete;

    internal ResponseEncryptionBody(HttpContext context, IResponseEncryptionProvider provider, IHttpResponseBodyFeature innerBodyFeature)
    {
        _context = context;
        _provider = provider;
        _innerBodyFeature = innerBodyFeature;
        _innerStream = innerBodyFeature.Stream;
    }

    internal async Task FinishEncryptionAsync()
    {
        if (_complete)
        {
            return;
        }

        _complete = true;

        if (_pipeAdapter != null)
        {
            await _pipeAdapter.CompleteAsync();
        }

        if (_encryptionStream != null)
        {
            if (!_encryptionStream.HasFlushedFinalBlock)
            {
                await _encryptionStream.FlushFinalBlockAsync();
            }
            await _encryptionStream.DisposeAsync();
        }

        // Adds the compression headers for HEAD requests even if the body was not used.
        if (!_encryptionChecked && HttpMethods.IsHead(_context.Request.Method))
        {
            InitializeCompressionHeaders();
        }
    }

    //HttpsCompressionMode IHttpsCompressionFeature.Mode { get; set; } = HttpsCompressionMode.Default;

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => _innerStream.CanWrite;

    public override long Length
    {
        get { throw new NotSupportedException(); }
    }

    public override long Position
    {
        get { throw new NotSupportedException(); }
        set { throw new NotSupportedException(); }
    }

    public Stream Stream => this;

    public PipeWriter Writer
    {
        get
        {
            _pipeAdapter ??= PipeWriter.Create(Stream, new StreamPipeWriterOptions(leaveOpen: true));

            return _pipeAdapter;
        }
    }

    public override void Flush()
    {
        if (!_encryptionChecked)
        {
            OnWrite();
            // Flush the original stream to send the headers. Flushing the compression stream won't
            // flush the original stream if no data has been written yet.
            _innerStream.Flush();
            return;
        }

        if (_encryptionStream != null)
        {
            _encryptionStream.Flush();
        }
        else
        {
            _innerStream.Flush();
        }
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        if (!_encryptionChecked)
        {
            OnWrite();
            // Flush the original stream to send the headers. Flushing the compression stream won't
            // flush the original stream if no data has been written yet.
            return _innerStream.FlushAsync(cancellationToken);
        }

        if (_encryptionStream != null)
        {
            return _encryptionStream.FlushAsync(cancellationToken);
        }

        return _innerStream.FlushAsync(cancellationToken);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        OnWrite();

        if (_encryptionStream != null)
        {
            _encryptionStream.Write(buffer, offset, count);
            if (_autoFlush)
            {
                _encryptionStream.Flush();
            }
        }
        else
        {
            _innerStream.Write(buffer, offset, count);
        }
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        => TaskToApm.Begin(WriteAsync(buffer, offset, count, CancellationToken.None), callback, state);

    public override void EndWrite(IAsyncResult asyncResult)
        => TaskToApm.End(asyncResult);

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => await WriteAsync(buffer.AsMemory(offset, count), cancellationToken);

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        OnWrite();

        if (_encryptionStream != null)
        {
            await _encryptionStream.WriteAsync(buffer, cancellationToken);
            if (_autoFlush)
            {
                await _encryptionStream.FlushAsync(cancellationToken);
            }
        }
        else
        {
            await _innerStream.WriteAsync(buffer, cancellationToken);
        }
    }

    /// <summary>
    /// Checks if the response should be compressed and sets the response headers.
    /// </summary>
    /// <returns>The compression provider to use if compression is enabled, otherwise null.</returns>
    private IEncryptionProvider? InitializeCompressionHeaders()
    {
        var headers = _context.Response.Headers;

        var compressionProvider = ResolveCompressionProvider();
        if (compressionProvider != null)
        {
            // Can't use += as StringValues does not override operator+
            // and the implict conversions will cause an incorrect string concat https://github.com/dotnet/runtime/issues/52507
            //headers.ContentEncoding = StringValues.Concat(headers.ContentEncoding, compressionProvider.EncodingName);
            headers.ContentMD5 = default; // Reset the MD5 because the content changed.
            headers.ContentLength = default;
        }

        return compressionProvider;

    }

    private void OnWrite()
    {
        if (!_encryptionChecked)
        {
            _encryptionChecked = true;

            var compressionProvider = InitializeCompressionHeaders();

            if (compressionProvider != null)
            {
                _encryptionStream = compressionProvider.CreateStream(_innerStream).GetAwaiter().GetResult();
            }
        }
    }

    private IEncryptionProvider? ResolveCompressionProvider()
    {
        if (!_providerCreated)
        {
            _providerCreated = true;
            _encryptionProvider = _provider.GetEncryptionProvider(_context);
        }

        return _encryptionProvider;
    }

    // For this to be effective it needs to be called before the first write.
    public void DisableBuffering()
    {
        _autoFlush = true;
        _innerBodyFeature.DisableBuffering();
    }

    public Task SendFileAsync(string path, long offset, long? count, CancellationToken cancellation)
    {
        OnWrite();

        if (_encryptionStream != null)
        {
            return SendFileFallback.SendFileAsync(Stream, path, offset, count, cancellation);
        }

        return _innerBodyFeature.SendFileAsync(path, offset, count, cancellation);
    }

    public Task StartAsync(CancellationToken token = default)
    {
        OnWrite();
        return _innerBodyFeature.StartAsync(token);
    }

    public async Task CompleteAsync()
    {
        if (_complete)
        {
            return;
        }

        await FinishEncryptionAsync(); // Sets _complete
        await _innerBodyFeature.CompleteAsync();
    }
}

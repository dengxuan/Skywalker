using Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Skywalker.Extensions.AspNetCore.Security.Abstractions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Streams
{
    /// <summary>
    /// Stream wrapper that create specific encryption stream only if necessary.
    /// </summary>
    internal class BodyWrapperStream : Stream, IHttpBufferingFeature, IHttpSendFileFeature
    {
        private readonly HttpContext? _context;
        private readonly Stream? _bodyOriginalStream;
        private readonly IResponseEncrpytionProvider? _provider;
        private readonly IHttpBufferingFeature? _innerBufferFeature;
        private readonly IHttpSendFileFeature? _innerSendFileFeature;

        private IEncryptionProvider? _encryptionProvider;
        private bool _encryptionChecked;
        private Stream? _encryptionStream;
        private bool _providerCreated;
        private bool _autoFlush;

        internal BodyWrapperStream(HttpContext? context, Stream? bodyOriginalStream, IResponseEncrpytionProvider? provider, IHttpBufferingFeature? innerBufferFeature, IHttpSendFileFeature? innerSendFileFeature)
        {
            _context = context;
            _bodyOriginalStream = bodyOriginalStream;
            _provider = provider;
            _innerBufferFeature = innerBufferFeature;
            _innerSendFileFeature = innerSendFileFeature;
        }

        protected override void Dispose(bool disposing)
        {
            if (_encryptionStream != null)
            {
                //_encryptionStream.Dispose();
                _encryptionStream = null;
            }
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => _bodyOriginalStream!.CanWrite;

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void Flush()
        {
            if (!_encryptionChecked)
            {
                OnWrite();
                // Flush the original stream to send the headers. Flushing the compression stream won't
                // flush the original stream if no data has been written yet.
                _bodyOriginalStream!.Flush();
                return;
            }

            if (_encryptionStream != null)
            {
                _encryptionStream!.Flush();
            }
            else
            {
                _bodyOriginalStream!.Flush();
            }
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (!_encryptionChecked)
            {
                OnWrite();
                // Flush the original stream to send the headers. Flushing the compression stream won't
                // flush the original stream if no data has been written yet.
                return _bodyOriginalStream!.FlushAsync(cancellationToken);
            }

            if (_encryptionStream != null)
            {
                return _encryptionStream!.FlushAsync(cancellationToken);
            }
            return _bodyOriginalStream!.FlushAsync(cancellationToken);
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
                _encryptionStream!.Write(buffer, offset, count);
                if (_autoFlush)
                {
                    _encryptionStream!.Flush();
                }
            }
            else
            {
                _bodyOriginalStream!.Write(buffer, offset, count);
            }
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            var tcs = new TaskCompletionSource<object?>(state);
            InternalWriteAsync(buffer, offset, count, callback, tcs);
            return tcs!.Task;
        }

        private async void InternalWriteAsync(byte[] buffer, int offset, int count, AsyncCallback? callback, TaskCompletionSource<object?> tcs)
        {
            try
            {
                await WriteAsync(buffer, offset, count);
                tcs!.TrySetResult(null);
            }
            catch (Exception ex)
            {
                tcs!.TrySetException(ex);
            }

            if (callback != null)
            {
                // Offload callbacks to avoid stack dives on sync completions.
                var ignored = Task.Run(() =>
                {
                    try
                    {
                        callback(tcs!.Task);
                    }
                    catch (Exception)
                    {
                        // Suppress exceptions on background threads.
                    }
                });
            }
        }

        public override void EndWrite(IAsyncResult? asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            var task = (Task)asyncResult;
            task!.GetAwaiter().GetResult();
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            OnWrite();

            if (_encryptionStream != null)
            {
                await _encryptionStream!.WriteAsync(buffer, offset, count, cancellationToken);
                if (_autoFlush)
                {
                    await _encryptionStream!.FlushAsync(cancellationToken);
                }
            }
            else
            {
                await _bodyOriginalStream!.WriteAsync(buffer, offset, count, cancellationToken);
            }
        }

        private void OnWrite()
        {
            if (!_encryptionChecked)
            {
                _encryptionChecked = true;
                if (_provider!.ShouldEncryptionResponse(_context!))
                {
                    var encryptionProvider = ResolveEncryptionProvider();
                    if (encryptionProvider != null)
                    {
                        _context!.Response.Headers.Append(EncryptionHeaderNames.EncryptionEncoding, encryptionProvider!.EncodingName);
                        _context!.Response.Headers.Remove(HeaderNames.ContentMD5); // Reset the MD5 because the content changed.
                        _context!.Response.Headers.Remove(HeaderNames.ContentLength);

                        _encryptionStream = encryptionProvider!.CreateStream(_bodyOriginalStream!);
                    }
                }
            }
        }

        private IEncryptionProvider ResolveEncryptionProvider()
        {
            if (!_providerCreated)
            {
                _providerCreated = true;
                _encryptionProvider = _provider!.GetEncryptionProvider(_context!);
            }

            return _encryptionProvider!;
        }

        public void DisableRequestBuffering()
        {
            // Unrelated
            _innerBufferFeature?.DisableRequestBuffering();
        }

        // For this to be effective it needs to be called before the first write.
        public void DisableResponseBuffering()
        {
            if (ResolveEncryptionProvider()?.SupportsFlush == false)
            {
                // Don't compress, some of the providers don't implement Flush (e.g. .NET 4.5.1 GZip/Deflate stream)
                // which would block real-time responses like SignalR.
                _encryptionChecked = true;
            }
            else
            {
                _autoFlush = true;
            }
            _innerBufferFeature?.DisableResponseBuffering();
        }

        // The IHttpSendFileFeature feature will only be registered if _innerSendFileFeature exists.
        public Task SendFileAsync(string path, long offset, long? count, CancellationToken cancellation)
        {
            OnWrite();

            if (_encryptionStream != null)
            {
                return InnerSendFileAsync(path, offset, count, cancellation);
            }

            return _innerSendFileFeature!.SendFileAsync(path, offset, count, cancellation);
        }

        private async Task InnerSendFileAsync(string path, long offset, long? count, CancellationToken cancellation)
        {
            cancellation!.ThrowIfCancellationRequested();

            var fileInfo = new FileInfo(path);
            if (offset < 0 || offset > fileInfo!.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, string.Empty);
            }
            if (count!.HasValue &&
                (count!.Value < 0 || count!.Value > fileInfo!.Length - offset))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, string.Empty);
            }

            int bufferSize = 1024 * 16;

            var fileStream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite,
                bufferSize: bufferSize,
                options: FileOptions.Asynchronous | FileOptions.SequentialScan);

            using (fileStream)
            {
                fileStream!.Seek(offset, SeekOrigin.Begin);
                await StreamCopyOperation.CopyToAsync(fileStream, _encryptionStream, count, cancellation);

                if (_autoFlush)
                {
                    await _encryptionStream!.FlushAsync(cancellation);
                }
            }
        }
    }
}

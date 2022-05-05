using System.Security.Cryptography;
using System.Text;
using Abstractions;
using Microsoft.Extensions.Options;

namespace Skywalker.Extensions.AspNetCore.Security.Abstractions;

public class TripleDESEncryptionProvider : IEncryptionProvider
{
    /// <summary>
    /// Creates a new instance of GzipCompressionProvider with options.
    /// </summary>
    /// <param name="options"></param>
    public TripleDESEncryptionProvider(IOptions<TripleDESEncryptionProviderOptions> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        Options = options.Value;
    }

    private TripleDESEncryptionProviderOptions Options { get; }

    /// <inheritdoc />
    public string EncodingName => "3des";

    /// <inheritdoc />
    public bool SupportsFlush
    {
        get
        {
            return true;
        }
    }

    /// <inheritdoc />
    public Stream CreateStream(Stream outputStream)
    {
        var tripleDES = TripleDES.Create();
        var cryptoTransform = tripleDES.CreateEncryptor(Encoding.Default.GetBytes("123456781234567812345678"), Encoding.Default.GetBytes("12345678"));
        return new CryptoStream(outputStream, cryptoTransform, CryptoStreamMode.Write);
    }
}

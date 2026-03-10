// Licensed to the Gordon

using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.AspNetCore.ResponseEncryption;

namespace Skywalker.Ddd.AspNetCore.RequestDecryption;

/// <inheritdoc />
internal sealed partial class DefaultRequestDecryptionProvider : IRequestDecryptionProvider
{
    private readonly ILogger<DefaultRequestDecryptionProvider> _logger;

    public DefaultRequestDecryptionProvider(ILogger<DefaultRequestDecryptionProvider> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Stream?> GetDecryptionStream(HttpContext context)
    {
        var encryptionFeature = context.Features.Get<IEncryptionFeature>();
        if (encryptionFeature is null)
        {
            _logger.LogDebug("No encryption feature found.");
            return null;
        }
        var decryptionProvider = context.RequestServices.GetKeyedService<IDecryptionProvider>(encryptionFeature.CryptoName);
        if (decryptionProvider == null)
        {
            _logger.LogDebug("No matching request decompression provider found.");
            return null;
        }
        _logger.LogDebug("The request will be decompressed with '{ContentEncoding}'.", "3des");
        decryptionProvider.Key = Encoding.UTF8.GetBytes(encryptionFeature.Key);
        decryptionProvider.Mode = encryptionFeature.Mode;
        decryptionProvider.Padding = encryptionFeature.Padding;
        var stream = await decryptionProvider.GetDecryptionStream(context.Request.Body);
        return stream;
    }
}

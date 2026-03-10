// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

/// <inheritdoc />
public class ResponseEncryptionProvider(ILogger<ResponseEncryptionProvider> logger) : IResponseEncryptionProvider
{

    /// <inheritdoc />
    public virtual IEncryptionProvider? GetEncryptionProvider(HttpContext context)
    {
        var encryptionFeature = context.Features.Get<IEncryptionFeature>();
        if (encryptionFeature == null)
        {
            logger.NoCompressionProvider();
            return null;
        }

        var selectedProvider = context.RequestServices.GetKeyedService<IEncryptionProvider>(encryptionFeature.CryptoName);
        if (selectedProvider == null)
        {
            logger.NoCompressionProvider();
            return null;
        }

        logger.CompressingWith(encryptionFeature.CryptoName);
        selectedProvider.Key = Encoding.UTF8.GetBytes(encryptionFeature.Key);
        selectedProvider.Mode = encryptionFeature.Mode;
        selectedProvider.Padding = encryptionFeature.Padding;
        return selectedProvider;
    }

}

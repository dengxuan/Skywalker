using Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Skywalker.Extensions.AspNetCore.Security.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Extensions.AspNetCore.Security
{
    public class ResponseEncryptionProvider : IResponseEncrpytionProvider
    {
        private readonly IEncryptionProvider[] _providers;
        private readonly bool _enableForHttps;
        private readonly HashSet<string> _mimeTypes;

        public ResponseEncryptionProvider(IServiceProvider services, IOptions<SafetyOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _providers = options.Value.Providers.ToArray();
            if (_providers.Length == 0)
            {
                // Use the factory so it can resolve IOptions<TripleDESEncryptionProviderOptions> from DI.
                _providers = new IEncryptionProvider[] { new EncryptionProviderFactory(typeof(TripleDESEncryptionProvider)) };
            }
            for (var i = 0; i < _providers.Length; i++)
            {
                if (_providers[i] is EncryptionProviderFactory factory)
                {
                    _providers[i] = factory.CreateInstance(services);
                }
            }

            var mimeTypes = options.Value.MimeTypes;
            if (mimeTypes == null || !mimeTypes.Any())
            {
                mimeTypes = ResponseEncryptionDefaults.MimeTypes;
            }
            _mimeTypes = new HashSet<string>(mimeTypes, StringComparer.OrdinalIgnoreCase);

            _enableForHttps = options.Value.EnableForHttps;
        }

        public bool CheckRequestAcceptsEncryption(HttpContext context)
        {
            if (context.Request.IsHttps && !_enableForHttps)
            {
                return false;
            }
            return !string.IsNullOrEmpty(context.Request.Headers[EncryptionHeaderNames.AcceptEncryption]);
        }

        public IEncryptionProvider? GetEncryptionProvider(HttpContext context)
        {
            // e.g. Accept-Encoding: 3des, des, aes
            var accept = context.Request.Headers[EncryptionHeaderNames.AcceptEncryption];
            if (!StringValues.IsNullOrEmpty(accept)
                && StringWithQualityHeaderValue.TryParseList(accept, out IList<StringWithQualityHeaderValue> unsorted)
                && unsorted != null && unsorted.Count > 0)
            {
                // TODO PERF: clients don't usually include quality values so this sort will not have any effect. Fast-path?
                var sorted = unsorted
                    .Where(s => s.Quality.GetValueOrDefault(1) > 0)
                    .OrderByDescending(s => s.Quality.GetValueOrDefault(1));

                foreach (var encoding in sorted)
                {
                    // There will rarely be more than three providers, and there's only one by default
                    foreach (var provider in _providers)
                    {
                        if (StringSegment.Equals(provider.EncodingName, encoding.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            return provider;
                        }
                    }

                    // Uncommon but valid options
                    if (StringSegment.Equals("*", encoding.Value, StringComparison.Ordinal))
                    {
                        // Any
                        return _providers[0];
                    }
                    if (StringSegment.Equals("identity", encoding.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        // No compression
                        return null;
                    }
                }
            }

            // Default
            return _providers[0];
        }

        public bool ShouldEncryptionResponse(HttpContext context)
        {
            if (context.Response.Headers.ContainsKey(HeaderNames.ContentRange))
            {
                return false;
            }

            var mimeType = context.Response.ContentType;

            if (string.IsNullOrEmpty(mimeType))
            {
                return false;
            }

            var separator = mimeType.IndexOf(';');
            if (separator >= 0)
            {
                // Remove the content-type optional parameters
                mimeType = mimeType.Substring(0, separator);
                mimeType = mimeType.Trim();
            }

            // TODO PERF: StringSegments?
            return _mimeTypes.Contains(mimeType);
        }
    }
}

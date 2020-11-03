using System.Collections.Generic;

namespace Skywalker.Extensions.AspNetCore.Security.Abstractions
{
    public class SafetyOptions
    {
        /// <summary>
        /// Response Content-Type MIME types to compress.
        /// </summary>
        public IEnumerable<string>? MimeTypes { get; set; }

        public EncryptionProviderCollection Providers { get; } = new EncryptionProviderCollection();

        public bool EnableForHttps { get; set; } = false;
    }
}

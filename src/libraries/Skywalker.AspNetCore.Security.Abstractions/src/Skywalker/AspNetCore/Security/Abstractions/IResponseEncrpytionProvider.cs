using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.Security.Abstractions
{
    public interface IResponseEncrpytionProvider
    {
        /// <summary>
        /// Examines the request and selects an acceptable compression provider, if any.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>A compression provider or null if compression should not be used.</returns>
        IEncryptionProvider? GetEncryptionProvider(HttpContext context);

        /// <summary>
        /// Examines the response on first write to see if compression should be used.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool ShouldEncryptionResponse(HttpContext context);

        /// <summary>
        /// Examines the request to see if compression should be used for response.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool CheckRequestAcceptsEncryption(HttpContext context);
    }
}

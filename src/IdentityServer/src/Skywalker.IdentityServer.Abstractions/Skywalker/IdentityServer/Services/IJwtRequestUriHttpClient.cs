using System.Threading.Tasks;
using Skywalker.IdentityServer.Models;

namespace Skywalker.IdentityServer.Services
{
    /// <summary>
    /// Models making HTTP requests for JWTs from the authorize endpoint.
    /// </summary>
    public interface IJwtRequestUriHttpClient
    {
        /// <summary>
        /// Gets a JWT from the url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        Task<string> GetJwtAsync(string url, Client client);
    }
}
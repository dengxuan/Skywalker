using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Transfer.Abstractions
{
    public interface IAuthenticationProvider
    {
        Task<bool> CanExecuteAsync(HttpContext httpContext);

        Task ExecuteAsync(HttpContext httpContext);
    }
}

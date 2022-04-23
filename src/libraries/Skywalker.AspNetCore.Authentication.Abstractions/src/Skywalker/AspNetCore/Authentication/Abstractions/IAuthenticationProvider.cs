using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.Authentication.Abstractions;

public interface IAuthenticationProvider
{
    Task<bool> CanExecuteAsync(HttpContext httpContext);

    Task ExecuteAsync(HttpContext httpContext);
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Authentication.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        [Route("sign-in")]
        public async Task<string> SignIn(string username, string password, string provider)
        {
            string appId = "57a81748-d2ba-4dd1-b641-b44170ee8ba8";
            var identity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
            identity.AddClaim(new Claim(ClaimTypes.Sid, appId));
            await HttpContext.SignInAsync(SkywalkerAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return appId;
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task SignUp(string username, string password, string provider)
        {
            var identity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
            identity.AddClaim(new Claim(ClaimTypes.Sid, username));
            identity.AddClaim(new Claim(ClaimTypes.Name, username));
            identity.AddClaim(new Claim(ClaimTypes.Role, provider));
            await HttpContext.SignInAsync(SkywalkerAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Authentication.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        [HttpPost]
        public async Task<SkywalkerToken> SignIn(string username, string password)
        {
            var identity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Sid, username));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
            identity.AddClaim(new Claim(ClaimTypes.Role, password));
            await HttpContext.SignInAsync(SkywalkerAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return new SkywalkerToken("");
        }
    }
}

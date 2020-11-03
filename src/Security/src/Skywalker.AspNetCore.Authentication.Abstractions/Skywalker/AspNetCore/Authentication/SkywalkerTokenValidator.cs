using Skywalker.AspNetCore.Authentication.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Authentication
{
    public class SkywalkerTokenValidator : ISkywalkerTokenValidator
    {
        public async Task<ClaimsPrincipal> ValidateTokenAsync(string securityToken)
        {
            await Task.CompletedTask;
            var claimIdentity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
            claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")));
            claimIdentity.AddClaim(new Claim(ClaimTypes.Name, Guid.NewGuid().ToString("N")));
            return new ClaimsPrincipal(claimIdentity);
        }
    }
}

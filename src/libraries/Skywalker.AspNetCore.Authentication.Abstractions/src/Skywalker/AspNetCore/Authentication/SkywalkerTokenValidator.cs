using Skywalker.AspNetCore.Authentication.Abstractions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Authentication
{
    public class SkywalkerTokenValidator : ISkywalkerTokenValidator
    {
        public Task<ClaimsPrincipal> ValidateTokenAsync(string securityToken)
        {
            return Task.Run(()=>
            {
                JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(securityToken);
                var claimIdentity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
                claimIdentity.AddClaims(jwtSecurityToken.Claims);
                return new ClaimsPrincipal(claimIdentity);
            });
        }
    }
}

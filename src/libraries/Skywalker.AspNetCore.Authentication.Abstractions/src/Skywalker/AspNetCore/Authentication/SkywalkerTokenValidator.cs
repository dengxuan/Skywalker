using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/* Unmerged change from project 'Skywalker.AspNetCore.Authentication.Abstractions(netstandard2.1)'
Before:
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
After:
namespace Skywalker.AspNetCore.Authentication;

public class SkywalkerTokenValidator : ISkywalkerTokenValidator
{
    public Task<ClaimsPrincipal> ValidateTokenAsync(string securityToken)
    {
        return Task.Run(()=>
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(securityToken);
            var claimIdentity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
            claimIdentity.AddClaims(jwtSecurityToken.Claims);
            return new ClaimsPrincipal(claimIdentity);
        });
*/

/* Unmerged change from project 'Skywalker.AspNetCore.Authentication.Abstractions(netcoreapp3.1)'
Before:
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
After:
namespace Skywalker.AspNetCore.Authentication;

public class SkywalkerTokenValidator : ISkywalkerTokenValidator
{
    public Task<ClaimsPrincipal> ValidateTokenAsync(string securityToken)
    {
        return Task.Run(()=>
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(securityToken);
            var claimIdentity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
            claimIdentity.AddClaims(jwtSecurityToken.Claims);
            return new ClaimsPrincipal(claimIdentity);
        });
*/

/* Unmerged change from project 'Skywalker.AspNetCore.Authentication.Abstractions(net5.0)'
Before:
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
After:
namespace Skywalker.AspNetCore.Authentication;

public class SkywalkerTokenValidator : ISkywalkerTokenValidator
{
    public Task<ClaimsPrincipal> ValidateTokenAsync(string securityToken)
    {
        return Task.Run(()=>
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(securityToken);
            var claimIdentity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
            claimIdentity.AddClaims(jwtSecurityToken.Claims);
            return new ClaimsPrincipal(claimIdentity);
        });
*/

/* Unmerged change from project 'Skywalker.AspNetCore.Authentication.Abstractions(net6.0)'
Before:
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
After:
namespace Skywalker.AspNetCore.Authentication;

public class SkywalkerTokenValidator : ISkywalkerTokenValidator
{
    public Task<ClaimsPrincipal> ValidateTokenAsync(string securityToken)
    {
        return Task.Run(()=>
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(securityToken);
            var claimIdentity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
            claimIdentity.AddClaims(jwtSecurityToken.Claims);
            return new ClaimsPrincipal(claimIdentity);
        });
*/
using System.Threading.Tasks;
using Skywalker.AspNetCore.Authentication.Abstractions;

namespace Skywalker.AspNetCore.Authentication;

public class SkywalkerTokenValidator : ISkywalkerTokenValidator
{
    public Task<ClaimsPrincipal> ValidateTokenAsync(string securityToken)
    {
        return Task.Run(()=>
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(securityToken);
            var claimIdentity = new ClaimsIdentity(SkywalkerAuthenticationDefaults.AuthenticationScheme);
            claimIdentity.AddClaims(jwtSecurityToken.Claims);
            return new ClaimsPrincipal(claimIdentity);
        });
    }
}

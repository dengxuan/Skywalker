using Skywalker.AspNetCore.Transfer.Abstractions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Transfer
{
    public class SkywalkerTokenValidator : ISkywalkerTokenValidator
    {
        public Task<ClaimsPrincipal> ValidateTokenAsync(string securityToken)
        {
            return Task.Run(()=>
            {
                return new ClaimsPrincipal();
            });
        }
    }
}

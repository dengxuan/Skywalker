using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Skywalker.AspNetCore.Transfer.Abstractions
{
    public interface ISkywalkerTokenValidator
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(string securityToken);
    }
}

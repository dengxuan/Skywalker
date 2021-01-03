using Skywalker.Application.Services.Contracts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Simple.Application.Abstractions
{
    public interface ISimpleUserApplicationService : IApplicationService
    {
        Task<UserDto> CreateUserAsync([NotNull] string name);

        Task<List<UserDto>> GetUsersAsync();
        Task<List<UserDto>> FindUsersAsync([NotNull] string name);
    }
}

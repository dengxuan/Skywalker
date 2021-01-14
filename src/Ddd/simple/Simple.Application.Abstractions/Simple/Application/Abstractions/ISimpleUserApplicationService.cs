using Skywalker.Application.Services.Contracts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Simple.Application.Abstractions
{
    public interface ISimpleUserApplicationService : IApplicationService
    {
        Task<UserDto> CreateUserAsync([NotNull] string name);

        Task<List<UserDto>> BatchCreateUsersAsync([NotNull] string name, int count);

        Task<List<UserDto>> FindUsersAsync();
        Task<List<UserDto>> FindUsersAsync([NotNull] string name);
    }
}

using Skywalker.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Simple.Application.Abstractions
{
    public interface ISimpleUserApplicationService : IApplicationService
    {
        Task<UserDto> CreateUserAsync([NotNull] string name);

        Task<List<UserDto>> FindUsersAsync();
    }
}

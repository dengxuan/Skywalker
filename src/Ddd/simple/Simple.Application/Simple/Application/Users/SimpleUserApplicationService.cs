using Microsoft.Extensions.DependencyInjection;
using Simple.Application.Abstractions;
using Simple.Domain.Users;
using Skywalker.Ddd.UnitOfWork;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Simple.Application
{
    [UnitOfWork]
    public class SimpleUserApplicationService : SimpleApplicationService, ISimpleUserApplicationService
    {
        private readonly IUserManager _userManager;

        public SimpleUserApplicationService(ILazyLoader lazyLoader, IUserManager userManager) : base(lazyLoader)
        {
            _userManager = userManager;
        }

        public async Task<List<UserDto>> BatchCreateUsersAsync([NotNull] string name, int count)
        {
            List<User> users = await _userManager.BatchCreateUser(name,count);
            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }

        public async Task<UserDto> CreateUserAsync([NotNull] string name)
        {
            User user = null;
            for (int i = 0; i < 100; i++)
            {
                user = await _userManager.CreateUser(name);
            }
            return ObjectMapper.Map<User, UserDto>(user);
        }

        public async Task<List<UserDto>> FindUsersAsync()
        {
            List<User> users = await _userManager.GetUsersAsync();
            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }

        public async Task<List<UserDto>> FindUsersAsync([NotNull]string name)
        {
            List<User> users = await _userManager.FindUsersAsync(name);
            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }
    }
}

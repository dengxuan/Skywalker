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
        private IUserManager UserManager => LazyLoader.GetRequiredService<IUserManager>();

        public SimpleUserApplicationService(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public async Task<List<UserDto>> BatchCreateUsersAsync([NotNull] string name, int count)
        {
            List<User> users = await UserManager.BatchCreateUser(name,count);
            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }

        public async Task<UserDto> CreateUserAsync([NotNull] string name)
        {
            User? user = null;
            for (int i = 0; i < 100; i++)
            {
                user = await UserManager.CreateUser(name);
            }
            return ObjectMapper.Map<User?, UserDto>(user);
        }

        public async Task<List<UserDto>> FindUsersAsync()
        {
            List<User> users = await UserManager.GetUsersAsync();
            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }

        public async Task<List<UserDto>> FindUsersAsync([NotNull]string name)
        {
            List<User> users = await UserManager.FindUsersAsync(name);
            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }
    }
}

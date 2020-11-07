using Microsoft.Extensions.DependencyInjection;
using Simple.Application.Abstractions;
using Simple.Domain.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Simple.Application
{
    public class SimpleUserApplicationService : SimpleApplicationService, ISimpleUserApplicationService
    {
        protected UserManager UserManager => LazyLoader.GetRequiredService<UserManager>();

        public SimpleUserApplicationService(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public async Task<UserDto> CreateUserAsync([NotNull] string name)
        {
            MongoUser user = await UserManager.CreateUser(name);
            //return new UserDto(user.Name!) { };
            return ObjectMapper.Map<MongoUser, UserDto>(user);
        }

        public async Task<List<UserDto>> FindUsersAsync()
        {
            List<MongoUser> users = await UserManager.FindUsersAsync();
            return ObjectMapper.Map<List<MongoUser>, List<UserDto>>(users);
        }
    }
}

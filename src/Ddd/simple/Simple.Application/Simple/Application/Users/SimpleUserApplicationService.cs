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
        private UserManager _userManager => LazyLoader.GetRequiredService<UserManager>();

        public SimpleUserApplicationService(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public async Task<UserDto> CreateUserAsync([NotNull] string name)
        {
            MongoUser user = await _userManager.CreateUser(new MongoUser(name) { Value = new UserValue("name") });
            return ObjectMapper.Map<MongoUser, UserDto>(user);
        }

        public async Task<List<UserDto>> FindUsersAsync()
        {
            List<MongoUser> users = await _userManager.FindUsersAsync();
            return ObjectMapper.Map<List<MongoUser>, List<UserDto>>(users);
        }

        public async Task<List<UserDto>> FindUsersAsync([NotNull]string name)
        {
            List<MongoUser> users = await _userManager.FindUsersAsync(name);
            return ObjectMapper.Map<List<MongoUser>, List<UserDto>>(users);
        }
    }
}

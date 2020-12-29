using Microsoft.Extensions.DependencyInjection;
using Simple.Application.Abstractions;
using Simple.Domain.Users;
using Skywalker.Uow;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Simple.Application
{
    [UnitOfWork]
    public class SimpleUserApplicationService : SimpleApplicationService, ISimpleUserApplicationService
    {
        private IUserManager _userManager => LazyLoader.GetRequiredService<IUserManager>();

        public SimpleUserApplicationService(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public async Task<UserDto> CreateUserAsync([NotNull] string name)
        {
            User user = await _userManager.CreateUser(name);
            return ObjectMapper.Map<User, UserDto>(user);
        }

        public async Task<List<UserDto>> FindUsersAsync()
        {
            List<User> users = await _userManager.FindUsersAsync();
            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }

        public async Task<List<UserDto>> FindUsersAsync([NotNull]string name)
        {
            List<User> users = await _userManager.FindUsersAsync(name);
            return ObjectMapper.Map<List<User>, List<UserDto>>(users);
        }
    }
}

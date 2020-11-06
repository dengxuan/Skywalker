using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simple.Domain.Users
{
    public class UserManager : DomainService
    {
        private readonly IRepository<User> _users;

        public UserManager(IRepository<User> users)
        {
            _users = users;
        }

        public Task<List<User>> FindUsersAsync()
        {
            return Task.FromResult(_users.ToList());
        }

        public async Task<User> CreateUser(string name)
        {
            User user = await _users.InsertAsync(new User(GuidGenerator.Create()) { Name = name }, true);
            return user;
        }
    }
}

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
        private readonly IRepository<MongoUser> _users;

        public UserManager(IRepository<MongoUser> users)
        {
            _users = users;
        }

        public Task<List<MongoUser>> FindUsersAsync()
        {
            return Task.FromResult(_users.ToList());
        }

        public async Task<MongoUser> CreateUser(string name)
        {
            MongoUser user = await _users.InsertAsync(new MongoUser(name), true);
            return user;
        }
    }
}

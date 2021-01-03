using Microsoft.EntityFrameworkCore;
using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Simple.Domain.Users
{
    public class UserManager : DomainService, IUserManager
    {
        private readonly IRepository<User, short> _users;

        public UserManager(IRepository<User, short> users)
        {
            _users = users;
        }

        public Task<List<User>> GetUsersAsync()
        {
            return _users.ToListAsync();
        }

        public Task<List<User>> FindUsersAsync([NotNull] string name)
        {
            return Task.Run(() =>
            {
                return _users.Where(predicate => name.IsEmptyOrWhiteSpace() || predicate.Name!.Contains(name)).ToListAsync();
            });
        }

        public async Task<User> CreateUser(string name)
        {
            User user = await _users.InsertAsync(new User { Name = name });
            return user;
        }

        public async Task<User> CreateUser(User mongoUser)
        {
            User user = await _users.InsertAsync(mongoUser);
            return user;
        }
    }
}

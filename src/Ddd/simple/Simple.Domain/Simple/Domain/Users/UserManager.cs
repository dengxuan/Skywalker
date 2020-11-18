using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Simple.Domain.Users
{
    public class UserManager : DomainService
    {
        private readonly IRepository<User, short> _users;

        public UserManager(IRepository<User, short> users)
        {
            _users = users;
        }

        public Task<List<User>> FindUsersAsync()
        {
            return Task.FromResult(_users.WithDetails(propertySelector => propertySelector.UserValue).ToList());
        }

        public Task<List<User>> FindUsersAsync([NotNull] string name)
        {
            return Task.Run(() =>
            {
                return _users.WithDetails(propertySelector => propertySelector.UserValue).Where(predicate => name.IsEmptyOrWhiteSpace() || predicate.Name.Contains(name)).ToList();
            });
        }

        public async Task<User> CreateUser(string name)
        {
            User user = await _users.InsertAsync(new User { Name = name, UserValue = new UserValue { Value = name } });
            return user;
        }

        public async Task<User> CreateUser(User mongoUser)
        {
            User user = await _users.InsertAsync(mongoUser);
            return user;
        }
    }
}

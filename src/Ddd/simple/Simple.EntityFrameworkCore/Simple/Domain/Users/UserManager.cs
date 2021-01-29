using Microsoft.EntityFrameworkCore;
using Skywalker.Caching.Abstractions;
using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Simple.Domain.Users
{
    [Cache(key: "UserManager", Expiry = 10)]
    public class UserManager : DomainService, IUserManager
    {
        private readonly IRepository<User, Guid> _users;

        public UserManager(IRepository<User, Guid> users, ICachingProvider cachingProvider) : base(users,cachingProvider)
        {
            _users = users;
        }

        public List<User> GetUsersAsync()
        {
            return _users.Include(u => u.UserOrders)
                            .ThenInclude(o => o.UserValues.Where(predicate => predicate.Value != "")).AsNoTracking().AsTracking()
                         .Take(10)
                         .ToList();
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
            ICaching caching = CachingProvider.GetCaching(name);
            caching.Get(name);
            User user = await _users.InsertAsync(new User { Name = name });
            return user;
        }

        public async Task<User> CreateUser(User mongoUser)
        {
            User user = await _users.InsertAsync(mongoUser);
            return user;
        }

        public async Task<List<User>> BatchCreateUser(string name, int count)
        {
            List<User> users = new List<User>();
            for (int i = 0; i < count; i++)
            {
                users.Add(new User { Name = name });
            }
            await _users.InsertAsync(users);
            return users;
        }
    }
}

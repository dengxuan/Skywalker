using Skywalker.Domain.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Simple.Domain.Users
{
    public interface IUserManager : IDomainService
    {
        Task<List<User>> GetUsersAsync();

        Task<List<User>> FindUsersAsync([NotNull] string name);

        Task<User> CreateUser(string name);
    }
}

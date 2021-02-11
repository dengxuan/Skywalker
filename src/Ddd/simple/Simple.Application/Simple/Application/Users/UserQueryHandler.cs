using Simple.Application.Abstractions;
using Simple.Domain.Users;
using Skywalker.Ddd.ObjectMapping;
using Skywalker.Ddd.Queries.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Application.Users
{
    public class UserQueryHandler : IQueryHandler<List<UserDto>>, IQueryHandler<UserQuery, List<UserDto>>
    {
        private readonly IUserManager _userManager;

        private readonly IObjectMapper _objectMapper;

        public UserQueryHandler(IUserManager userManager, IObjectMapper objectMapper)
        {
            _userManager = userManager;
            _objectMapper = objectMapper;
        }

        public async Task<List<UserDto>> HandleAsync(CancellationToken cancellationToken)
        {
            List<User> users = await _userManager.FindUsersAsync("");

            return _objectMapper.Map<List<User>, List<UserDto>>(users);
        }

        public async Task<List<UserDto>> HandleAsync(UserQuery query, CancellationToken cancellationToken)
        {
            List<User> users = await _userManager.FindUsersAsync(query.Name);

            return _objectMapper.Map<List<User>, List<UserDto>>(users);
        }
    }
}

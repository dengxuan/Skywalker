using Simple.Application.Abstractions;
using Simple.Domain.Users;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos;
using Skywalker.Ddd.ObjectMapping;

namespace Simple.Application.Users
{
    public class UserQueryHandler : IExecuteHandler<PagedResultDto<UserOutputDto>>, IExecuteHandler<UserInputDto, PagedResultDto<UserOutputDto>>
    {
        private readonly IUserManager _userManager;

        private readonly IObjectMapper _objectMapper;

        public UserQueryHandler(IUserManager userManager, IObjectMapper objectMapper)
        {
            _userManager = userManager;
            _objectMapper = objectMapper;
        }

        public async Task<PagedResultDto<UserOutputDto>?> HandleAsync(CancellationToken cancellationToken)
        {
            List<User> users = await _userManager.FindUsersAsync("");

            List<UserOutputDto>? result = _objectMapper.Map<List<User>, List<UserOutputDto>>(users);
            if (result == null)
            {
                return new PagedResultDto<UserOutputDto>();
            }
            return new PagedResultDto<UserOutputDto>(100, result);
        }

        public async Task<PagedResultDto<UserOutputDto>?> HandleAsync(UserInputDto inputDto, CancellationToken cancellationToken)
        {
            List<User> users = await _userManager.FindUsersAsync(inputDto.Name);
            List<UserOutputDto>? result = _objectMapper.Map<List<User>, List<UserOutputDto>>(users);
            if (result == null)
            {
                return new PagedResultDto<UserOutputDto>();
            }
            return new PagedResultDto<UserOutputDto>(100, result);
        }
    }
}

using Simple.Domain.Users;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos;
using Skywalker.Ddd.ObjectMapping;
using Skywalker.Ddd.Uow;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Application.Abstractions
{
    public class UserQueryHandler : IExecuteQueryHandler<PagedResultDto<UserOutputDto>>, IExecuteQueryHandler<UserInputDto, PagedResultDto<UserOutputDto>>, IExecuteNonQueryHandler<UserInputDto>
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
            if(result == null)
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

        [UnitOfWork(IsDisabled = true)]
        async Task IExecuteNonQueryHandler<UserInputDto>.HandleAsync(UserInputDto inputDto, CancellationToken cancellationToken)
        {
            await _userManager.CreateUser(inputDto.Name);
        }
    }
}

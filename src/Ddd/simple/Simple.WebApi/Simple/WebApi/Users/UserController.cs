using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Application.Abstractions;
using Skywalker.Ddd.Queries.Abstractions;
using Skywalker.Ddd.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simple.WebApi.Users
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : SimpleController
    {
        private readonly ISimpleUserApplicationService _simpleUserApplicationService;

        public UserController(ISimpleUserApplicationService simpleUserApplicationService, ILogger<SimpleController> logger) : base(logger)
        {
            _simpleUserApplicationService = simpleUserApplicationService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _simpleUserApplicationService.FindUsersAsync();
            return users;
        }

        [HttpGet]
        [Route("all-a")]
        public async Task<UserDto> CreateUsersAsync()
        {
            var users = _simpleUserApplicationService.FindUsersAsync();
            return await _simpleUserApplicationService.CreateUserAsync("A");
        }

        [HttpGet]
        [Route("all-{name}")]
        public async Task<List<UserDto>> GetUsersAsync(string name)
        {
            await _simpleUserApplicationService.CreateUserAsync(name);
            return await _simpleUserApplicationService.FindUsersAsync(name);
        }

        [HttpGet]
        [Route("batch-crtete-{name}-{count}")]
        public Task<List<UserDto>> BatchCreateUsersAsync(string name, int count)
        {
            return _simpleUserApplicationService.BatchCreateUsersAsync(name, count);
        }
    }
}

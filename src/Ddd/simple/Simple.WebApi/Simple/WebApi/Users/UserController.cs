using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Application.Abstractions;
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
            List<UserDto> users=  await _simpleUserApplicationService.FindUsersAsync();
            return users;
        }

        [HttpGet]
        [Route("all-a")]
        public async Task<UserDto> CreateUsersAsync()
        {
            List<UserDto> users = await _simpleUserApplicationService.FindUsersAsync();
            return await _simpleUserApplicationService.CreateUserAsync("A");
        }

        [HttpGet]
        [Route("all-{name}")]
        public Task<List<UserDto>> GetUsersAsync(string name)
        {
            return _simpleUserApplicationService.FindUsersAsync(name);
        }
    }
}

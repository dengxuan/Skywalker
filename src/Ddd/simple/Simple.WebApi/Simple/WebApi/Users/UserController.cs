using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Application.Abstractions;
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
        public Task<List<UserDto>> GetUsersAsync()
        {
            return _simpleUserApplicationService.FindUsersAsync();
        }

        [HttpGet]
        [Route("all-{name}")]
        public Task<List<UserDto>> GetUsersAsync(string name)
        {
            return _simpleUserApplicationService.FindUsersAsync(name);
        }
    }
}

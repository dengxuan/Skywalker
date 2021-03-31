using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Application.Abstractions;
using Simple.Application.Users;
using Skywalker.Ddd.Queries.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simple.WebApi.Users
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : SimpleController
    {
        private readonly ISimpleUserApplicationService _simpleUserApplicationService;
        private readonly ISearcher _searcher;

        public UserController(ISimpleUserApplicationService simpleUserApplicationService, ISearcher searcher, ILogger<SimpleController> logger) : base(logger)
        {
            _simpleUserApplicationService = simpleUserApplicationService;
            _searcher = searcher;
        }

        [HttpGet]
        [Route("all")]
        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _searcher.SearchAsync<UserQuery, List<UserDto>>(new UserQuery { Name = "123" });
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

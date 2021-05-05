using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Application.Abstractions;
using Simple.Application.Users;
using Skywalker.Application.Dtos;
using Skywalker.Ddd.Application.Abstractions;
using System.Threading.Tasks;

namespace Simple.WebApi.Users
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : SimpleController
    {
        private readonly IApplication _searcher;

        public UserController(IApplication searcher, ILogger<SimpleController> logger) : base(logger)
        {
            _searcher = searcher;
        }

        [HttpGet]
        [Route("all")]
        public async Task<PagedResultDto<UserOutputDto>?> GetUsersAsync()
        {
            var users = await _searcher.ExecuteAsync<PagedResultDto<UserOutputDto>>();
            return users;
        }
    }
}

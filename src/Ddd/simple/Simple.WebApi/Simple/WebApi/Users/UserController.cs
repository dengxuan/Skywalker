using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Application.Abstractions;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos;
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
        public async Task<PagedResultDto<UserOutputDto>?> GetAsync()
        {
            var users = await _searcher.ExecuteQueryAsync<UserInputDto,PagedResultDto<UserOutputDto>>(new UserInputDto("张三"));
            return users;
        }


        [HttpPost]
        public async Task PostAsync(UserInputDto inputDto)
        {
            await _searcher.ExecuteNonQueryAsync(inputDto);
        }
    }
}

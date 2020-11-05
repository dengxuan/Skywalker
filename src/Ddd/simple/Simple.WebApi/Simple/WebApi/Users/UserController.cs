using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Simple.WebApi.Users
{
    [ApiController]
    [Route("[Controller]")]
    public class UserController : SimpleController
    {
        public UserController(ILogger<SimpleController> logger) : base(logger)
        {
        }

        [HttpGet]
        public string GetUser(int id)
        {
            Logger.LogInformation(id.ToString());
            return $"Name-{id}";
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skywalker.AspNetCore.Mvc.Abstractions.Models;

namespace Simple.WebApi
{
    [WrapResult]
    public abstract class SimpleController : ControllerBase
    {
        protected ILogger<SimpleController> Logger { get; }

        public SimpleController(ILogger<SimpleController> logger)
        {
            Logger = logger;
        }
    }
}

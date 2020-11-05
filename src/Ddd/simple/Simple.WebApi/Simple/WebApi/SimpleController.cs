using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.WebApi
{
    public abstract class SimpleController : ControllerBase
    {
        protected ILogger<SimpleController> Logger { get; }

        public SimpleController(ILogger<SimpleController> logger)
        {
            Logger = logger;
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skywalker.Aspects;
using Skywalker.AspNetCore.Mvc.Models;
using Skywalker.Uow;
using Skywalker.Uow.Abstractions;

namespace Simple.WebApi
{
    [WrapResult]
    [Aspects]
    [UnitOfWork]
    public abstract class SimpleController : ControllerBase, IAspects, IUnitOfWorkEnabled
    {
        protected ILogger<SimpleController> Logger { get; }

        public SimpleController(ILogger<SimpleController> logger)
        {
            Logger = logger;
        }
    }
}

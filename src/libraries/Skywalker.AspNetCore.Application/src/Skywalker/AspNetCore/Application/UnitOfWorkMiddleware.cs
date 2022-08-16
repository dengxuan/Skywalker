// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.AspNetCore.Application;

internal class UnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UnitOfWorkMiddleware> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly UnitOfWorkDefaultOptions _defaultOptions;

    public UnitOfWorkMiddleware(RequestDelegate next, IUnitOfWorkManager unitOfWorkManager, IOptions<UnitOfWorkDefaultOptions> options, ILogger<UnitOfWorkMiddleware> logger)
    {
        _next = next;
        _unitOfWorkManager = unitOfWorkManager;
        _defaultOptions = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Begin Invoke UnitOfWorkMiddleware.InvokeAsync");
        using var uow = _unitOfWorkManager.Begin(CreateOptions(context.Request.Method));
        _logger.LogDebug("Begin Unit of work:[{uow.Id}]", uow.Id);
        await _next(context);
        await uow.CompleteAsync();
        _logger.LogDebug("Complete Unit of work:[{uow.Id}]", uow.Id);
    }
    
    private UnitOfWorkOptions CreateOptions(string methodName)
    {
        var options = new UnitOfWorkOptions
        {
            IsTransactional = _defaultOptions.CalculateIsTransactional(autoValue: !methodName.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase))
        };
        
        return options;
    }
}

﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.UnitOfWork.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork
{
    public class UnitOfWorkInterceptor
    {
        private readonly AbpUnitOfWorkDefaultOptions _defaultOptions;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILogger<UnitOfWorkInterceptor> _logger;

        public UnitOfWorkInterceptor(IOptions<AbpUnitOfWorkDefaultOptions> options, IUnitOfWorkManager unitOfWorkManager, ILogger<UnitOfWorkInterceptor> logger)
        {
            _defaultOptions = options.Value;
            _unitOfWorkManager = unitOfWorkManager;
            _logger = logger;
        }

        public Task InvokeAsync()
        {
            //if (!UnitOfWorkHelper.IsUnitOfWorkMethod(context.Method, out var unitOfWorkAttribute))
            //{
            //    await context.ProceedAsync();
            //    return;
            //}
            //using var uow = _unitOfWorkManager.Begin(CreateOptions(context.TargetMethod, unitOfWorkAttribute!));
            //try
            //{
            //    _logger.LogInformation("开始事务:[{0}]", uow.Id);
            //    await context.ProceedAsync();
            //    await uow.CompleteAsync();
            //    _logger.LogInformation("提交事务:[{0}]", uow.Id);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, $"事务异常:[{uow.Id}] 异常信息:", uow.Id, ex.Message);
            //    await uow.RollbackAsync();
            //    ex.ReThrow();
            //}
            return Task.CompletedTask;
        }

        private AbpUnitOfWorkOptions CreateOptions(MethodInfo method, [MaybeNull] UnitOfWorkAttribute unitOfWorkAttribute)
        {
            var options = new AbpUnitOfWorkOptions();

            unitOfWorkAttribute?.SetOptions(options);

            if (unitOfWorkAttribute?.IsTransactional == null)
            {
                options.IsTransactional = _defaultOptions.CalculateIsTransactional(
                    autoValue: !method.Name.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase)
                );
            }

            return options;
        }
    }
}

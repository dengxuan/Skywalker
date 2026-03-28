// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Exceptions;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.DependencyInjection;

namespace Skywalker.Ddd.Uow;

/// <summary>
/// 工作单元管理器
/// </summary>
/// <param name="ambientUnitOfWork">环境工作单元</param>
/// <param name="serviceScopeFactory">服务范围工厂</param>
/// <param name="logger">日志记录器</param>
public class UnitOfWorkManager(
    IAmbientUnitOfWork ambientUnitOfWork,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<UnitOfWorkManager> logger) : IUnitOfWorkManager, ISingletonDependency
{
    /// <summary>
    /// 
    /// </summary>
    public IUnitOfWork? Current => GetCurrentUnitOfWork();

    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly IAmbientUnitOfWork _ambientUnitOfWork = ambientUnitOfWork;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="requiresNew"></param>
    /// <returns></returns>
    public IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false)
    {
        options.NotNull(nameof(options));

        var currentUow = Current;
        if (currentUow != null && !requiresNew)
        {
            logger.LogInformation("Begin child unit of work: {Id}", currentUow.Id);
            return new ChildUnitOfWork(currentUow);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Initialize(options);
        logger.LogInformation("Begin unit of work: {Id}", unitOfWork.Id);
        return unitOfWork;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reservationName"></param>
    /// <param name="requiresNew"></param>
    /// <returns></returns>
    public IUnitOfWork Reserve(string reservationName, bool requiresNew = false)
    {
        reservationName.NotNull(nameof(reservationName));

        if (!requiresNew
            && _ambientUnitOfWork.UnitOfWork != null
            && _ambientUnitOfWork.UnitOfWork.IsReservedFor(reservationName))
        {
            logger.LogInformation("Begin child reserved unit of work: {ReservationName}", reservationName);
            return new ChildUnitOfWork(_ambientUnitOfWork.UnitOfWork);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Reserve(reservationName);
        logger.LogInformation("Begin reserved unit of work: {ReservationName}", reservationName);
        return unitOfWork;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reservationName"></param>
    /// <param name="options"></param>
    /// <exception cref="SkywalkerException"></exception>
    public void BeginReserved(string reservationName, UnitOfWorkOptions options)
    {
        if (!TryBeginReserved(reservationName, options))
        {
            throw new SkywalkerException($"Could not find a reserved unit of work with reservation name: {reservationName}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reservationName"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public bool TryBeginReserved(string reservationName, UnitOfWorkOptions options)
    {
        reservationName.NotNull(nameof(reservationName));

        var uow = _ambientUnitOfWork.UnitOfWork;

        //Find reserved unit of work starting from current and going to outers
        while (uow != null && !uow.IsReservedFor(reservationName))
        {
            uow = uow.Outer;
        }

        if (uow == null)
        {
            return false;
        }

        uow.Initialize(options);

        return true;
    }

    private IUnitOfWork? GetCurrentUnitOfWork()
    {
        var uow = _ambientUnitOfWork.UnitOfWork;

        //Skip reserved unit of work
        while (uow != null && (uow.IsReserved || uow.IsDisposed || uow.IsCompleted))
        {
            uow = uow.Outer;
        }

        return uow;
    }

    private IUnitOfWork CreateNewUnitOfWork()
    {
        var scope = _serviceScopeFactory.CreateScope();
        try
        {
            var outerUow = _ambientUnitOfWork.UnitOfWork;

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            unitOfWork.SetOuter(outerUow);

            _ambientUnitOfWork.SetUnitOfWork(unitOfWork);

            unitOfWork!.Disposed += (sender, args) =>
            {
                _ambientUnitOfWork.SetUnitOfWork(outerUow);
                scope.Dispose();
            };

            return unitOfWork;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating a new unit of work.");
            scope.Dispose();
            throw;
        }
    }
}

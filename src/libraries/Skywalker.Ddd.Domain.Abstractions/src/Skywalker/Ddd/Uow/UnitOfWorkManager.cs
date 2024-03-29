﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.ExceptionHandler;

namespace Skywalker.Ddd.Uow;

public class UnitOfWorkManager : IUnitOfWorkManager
{
    public IUnitOfWork? Current => GetCurrentUnitOfWork();

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IAmbientUnitOfWork _ambientUnitOfWork;

    public UnitOfWorkManager(
        IAmbientUnitOfWork ambientUnitOfWork,
        IServiceScopeFactory serviceScopeFactory)
    {
        _ambientUnitOfWork = ambientUnitOfWork;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false)
    {
        options.NotNull(nameof(options));

        var currentUow = Current;
        if (currentUow != null && !requiresNew)
        {
            return new ChildUnitOfWork(currentUow);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Initialize(options);

        return unitOfWork;
    }

    public IUnitOfWork Reserve(string reservationName, bool requiresNew = false)
    {
        reservationName.NotNull(nameof(reservationName));

        if (!requiresNew &&
            _ambientUnitOfWork.UnitOfWork != null &&
            _ambientUnitOfWork.UnitOfWork.IsReservedFor(reservationName))
        {
            return new ChildUnitOfWork(_ambientUnitOfWork.UnitOfWork);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Reserve(reservationName);

        return unitOfWork;
    }

    public void BeginReserved(string reservationName, UnitOfWorkOptions options)
    {
        if (!TryBeginReserved(reservationName, options))
        {
            throw new SkywalkerException($"Could not find a reserved unit of work with reservation name: {reservationName}");
        }
    }

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
        catch
        {
            scope.Dispose();
            throw;
        }
    }
}

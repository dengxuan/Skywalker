﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Ddd.UnitOfWork.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.UnitOfWork
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        public IUnitOfWork? Current => GetCurrentUnitOfWork();

        private readonly IHybridServiceScopeFactory _serviceScopeFactory;
        private readonly IAmbientUnitOfWork _ambientUnitOfWork;

        public UnitOfWorkManager(IAmbientUnitOfWork ambientUnitOfWork, IHybridServiceScopeFactory  hybridServiceScopeFactory)
        {
            _ambientUnitOfWork = ambientUnitOfWork;
            _serviceScopeFactory = hybridServiceScopeFactory;
            System.Console.WriteLine("UnitOfWorkManager:{0}", hybridServiceScopeFactory.GetHashCode());
        }

        public IUnitOfWork Begin([NotNull] AbpUnitOfWorkOptions options, bool requiresNew = false)
        {
            Check.NotNull(options, nameof(options));

            var currentUow = Current;
            if (currentUow != null && !requiresNew)
            {
                return new ChildUnitOfWork(currentUow);
            }

            var unitOfWork = CreateNewUnitOfWork();
            unitOfWork.Initialize(options);

            return unitOfWork;
        }

        public IUnitOfWork Reserve([NotNull] string reservationName, bool requiresNew = false)
        {
            Check.NotNull(reservationName, nameof(reservationName));

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

        public void BeginReserved([NotNull] string reservationName, [NotNull] AbpUnitOfWorkOptions options)
        {
            if (!TryBeginReserved(reservationName, options))
            {
                throw new SkywalkerException($"Could not find a reserved unit of work with reservation name: {reservationName}");
            }
        }

        public bool TryBeginReserved([NotNull] string reservationName, [NotNull] AbpUnitOfWorkOptions options)
        {
            Check.NotNull(reservationName, nameof(reservationName));

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
}
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.DependencyInjection;
using Skywalker.Domain.Entities;

namespace Skywalker.Domain.Repositories
{
    public abstract class RepositoryRegistrarBase<TOptions> where TOptions : SkywalkerCommonDbContextRegistrationOptions
    {
        public TOptions Options { get; }

        protected RepositoryRegistrarBase(TOptions options)
        {
            Options = options;
        }

        public virtual void AddRepositories(IEnumerable<Type> entityTypes)
        {
            foreach (var entityType in entityTypes)
            {
                if (!ShouldRegisterDefaultRepositoryFor(entityType))
                {
                    continue;
                }

                RegisterDefaultRepository(entityType);
            }
        }

        protected virtual void RegisterDefaultRepository(Type entityType)
        {
            Options.Services.AddDefaultRepository(entityType, GetDefaultRepositoryImplementationType(entityType));
        }

        protected virtual Type GetDefaultRepositoryImplementationType(Type entityType)
        {
            var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);

            if (primaryKeyType == null)
            {
                return Options.SpecifiedDefaultRepositoryTypes
                    ? Options.DefaultRepositoryImplementationTypeWithoutKey.MakeGenericType(entityType)
                    : GetRepositoryType(entityType);
            }

            return Options.SpecifiedDefaultRepositoryTypes
                ? Options.DefaultRepositoryImplementationType.MakeGenericType(entityType, primaryKeyType)
                : GetRepositoryType(entityType, primaryKeyType);
        }

        protected virtual bool ShouldRegisterDefaultRepositoryFor(Type entityType)
        {

            if (!typeof(IAggregateRoot).IsAssignableFrom(entityType))
            {
                return false;
            }

            return true;
        }

        protected abstract Type GetRepositoryType(Type entityType);

        protected abstract Type GetRepositoryType(Type entityType, Type primaryKeyType);
    }
}
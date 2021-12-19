using Microsoft.Extensions.DependencyInjection;
using Skywalker.Domain.Entities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.DependencyInjection
{
    /// <summary>
    /// This is a base class for dbcoUse derived
    /// </summary>
    public abstract class SkywalkerCommonDbContextRegistrationOptions : ISkywalkerCommonDbContextRegistrationOptionsBuilder
    {
        public IServiceCollection Services { get; }

        public Type DefaultRepositoryDbContextType { get; protected set; }

        public Type? DefaultRepositoryImplementationType { get; private set; }

        public Type? DefaultRepositoryImplementationTypeWithoutKey { get; private set; }

        public bool SpecifiedDefaultRepositoryTypes => DefaultRepositoryImplementationType != null && DefaultRepositoryImplementationTypeWithoutKey != null;

        protected SkywalkerCommonDbContextRegistrationOptions(Type dbContextType, IServiceCollection services)
        {
            Services = services;
            DefaultRepositoryDbContextType = dbContextType;
        }

        public ISkywalkerCommonDbContextRegistrationOptionsBuilder SetDefaultRepositoryClasses(Type repositoryImplementationType, Type repositoryImplementationTypeWithoutKey)
        {
            Check.NotNull(repositoryImplementationType, nameof(repositoryImplementationType));
            Check.NotNull(repositoryImplementationTypeWithoutKey, nameof(repositoryImplementationTypeWithoutKey));

            DefaultRepositoryImplementationType = repositoryImplementationType;
            DefaultRepositoryImplementationTypeWithoutKey = repositoryImplementationTypeWithoutKey;

            return this;
        }


        public bool ShouldRegisterDefaultRepositoryFor(Type entityType)
        {

            if (!typeof(IAggregateRoot).IsAssignableFrom(entityType))
            {
                return false;
            }

            return true;
        }
    }
}
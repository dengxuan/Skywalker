﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Skywalker.Localization
{
    public class LocalizationResource
    {
        [NotNull]
        public Type ResourceType { get; }

        [NotNull]
        public string ResourceName => LocalizationResourceNameAttribute.GetName(ResourceType);

        [MaybeNull]
        public string DefaultCultureName { get; set; }

        [NotNull]
        public LocalizationResourceContributorList Contributors { get; }

        [NotNull]
        public List<Type> BaseResourceTypes { get; }

        public LocalizationResource(
            [NotNull] Type resourceType, 
            [MaybeNull] string defaultCultureName = null,
            [MaybeNull] ILocalizationResourceContributor initialContributor = null)
        {
            ResourceType = Check.NotNull(resourceType, nameof(resourceType));
            DefaultCultureName = defaultCultureName;

            BaseResourceTypes = new List<Type>();
            Contributors = new LocalizationResourceContributorList();

            if (initialContributor != null)
            {
                Contributors.Add(initialContributor);
            }

            AddBaseResourceTypes();
        }

        protected virtual void AddBaseResourceTypes()
        {
            var descriptors = ResourceType
                .GetCustomAttributes(true)
                .OfType<IInheritedResourceTypesProvider>();

            foreach (var descriptor in descriptors)
            {
                foreach (var baseResourceType in descriptor.GetInheritedResourceTypes())
                {
                    BaseResourceTypes.AddIfNotContains(baseResourceType);
                }
            }
        }
    }
}
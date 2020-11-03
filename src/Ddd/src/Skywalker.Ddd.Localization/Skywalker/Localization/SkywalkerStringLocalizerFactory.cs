using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Skywalker.Localization
{
    public class SkywalkerStringLocalizerFactory : IStringLocalizerFactory, IStringLocalizerFactoryWithDefaultResourceSupport
    {
        protected internal SkywalkerLocalizationOptions AbpLocalizationOptions { get; }
        protected ResourceManagerStringLocalizerFactory InnerFactory { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected ConcurrentDictionary<Type, StringLocalizerCacheItem> LocalizerCache { get; }

        //TODO: It's better to use decorator pattern for IStringLocalizerFactory instead of getting ResourceManagerStringLocalizerFactory as a dependency.
        public SkywalkerStringLocalizerFactory(
            ResourceManagerStringLocalizerFactory innerFactory,
            IOptions<SkywalkerLocalizationOptions> abpLocalizationOptions,
            IServiceProvider serviceProvider)
        {
            InnerFactory = innerFactory;
            ServiceProvider = serviceProvider;
            AbpLocalizationOptions = abpLocalizationOptions.Value;

            LocalizerCache = new ConcurrentDictionary<Type, StringLocalizerCacheItem>();
        }

        public virtual IStringLocalizer Create(Type resourceType)
        {
            var resource = AbpLocalizationOptions.Resources.GetOrDefault(resourceType);
            if (resource == null)
            {
                return InnerFactory.Create(resourceType);
            }

            if (LocalizerCache.TryGetValue(resourceType, out var cacheItem))
            {
                return cacheItem.Localizer;
            }

            lock (LocalizerCache)
            {
                return LocalizerCache.GetOrAdd(
                    resourceType,
                    _ => CreateStringLocalizerCacheItem(resource)
                ).Localizer;
            }
        }

        private StringLocalizerCacheItem CreateStringLocalizerCacheItem(LocalizationResource resource)
        {
            foreach (var globalContributor in AbpLocalizationOptions.GlobalContributors)
            {
                resource.Contributors.Add((ILocalizationResourceContributor) Activator.CreateInstance(globalContributor));
            }

            using (var scope = ServiceProvider.CreateScope())
            {
                var context = new LocalizationResourceInitializationContext(resource, scope.ServiceProvider);

                foreach (var contributor in resource.Contributors)
                {
                    contributor.Initialize(context);
                }
            }

            return new StringLocalizerCacheItem(
                new SkywalkerDictionaryBasedStringLocalizer(
                    resource,
                    resource.BaseResourceTypes.Select(Create).ToList()
                )
            );
        }

        public virtual IStringLocalizer Create(string baseName, string location)
        {
            //TODO: Investigate when this is called?

            return InnerFactory.Create(baseName, location);
        }

        internal static void Replace(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory, SkywalkerStringLocalizerFactory>());
            services.AddSingleton<ResourceManagerStringLocalizerFactory>();
        }

        protected class StringLocalizerCacheItem
        {
            public SkywalkerDictionaryBasedStringLocalizer Localizer { get; }

            public StringLocalizerCacheItem(SkywalkerDictionaryBasedStringLocalizer localizer)
            {
                Localizer = localizer;
            }
        }

        public IStringLocalizer CreateDefaultOrNull()
        {
            if (AbpLocalizationOptions.DefaultResourceType == null)
            {
                return null;
            }

            return Create(AbpLocalizationOptions.DefaultResourceType);
        }
    }
}
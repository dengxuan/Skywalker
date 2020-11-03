using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Skywalker.Application.Services.Abstractions;
using Skywalker.DependencyInjection;
using Skywalker.Extensions.GuidGenerator;
using Skywalker.Extensions.Linq;
using Skywalker.Ddd.ObjectMapping;
using Skywalker.Extensions.Timing;
using Skywalker.Localization;
using Skywalker.Users;
using Skywalker.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.Application.Services
{
    public abstract class ApplicationService : IApplicationService, IValidationEnabled, ITransientDependency
    {
        private Type _localizationResource = typeof(DefaultResource);
        private readonly object _locker = new object();

        protected TService LazyGetRequiredService<TService>(ref TService reference)
            => LazyGetRequiredService(typeof(TService), ref reference);

        protected TRef LazyGetRequiredService<TRef>(Type serviceType, ref TRef reference)
        {
            if (reference == null)
            {
                lock (_locker)
                {
                    if (reference == null)
                    {
                        reference = (TRef)LazyLoader.GetRequiredService(serviceType);
                    }
                }
            }

            return reference;
        }

        protected IAsyncQueryableExecuter AsyncExecuter => LazyGetRequiredService(ref _asyncExecuter);
        private IAsyncQueryableExecuter _asyncExecuter;

        private Lazy<ILogger> _lazyLogger => new Lazy<ILogger>(() => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance, true);

        private IStringLocalizer? _localizer;

        protected ILazyLoader LazyLoader { get; }

        protected ILogger Logger => _lazyLogger.Value;

        protected IClock Clock => LazyLoader.GetRequiredService<IClock>();

        protected ICurrentUser CurrentUser => LazyLoader.GetRequiredService<ICurrentUser>();

        protected IObjectMapper ObjectMapper => LazyLoader.GetRequiredService<IObjectMapper>();

        protected IGuidGenerator GuidGenerator => LazyLoader.GetRequiredService<IGuidGenerator>();

        protected ILoggerFactory LoggerFactory => LazyLoader.GetRequiredService<ILoggerFactory>();

        protected IStringLocalizerFactory StringLocalizerFactory => LazyLoader.GetRequiredService<IStringLocalizerFactory>();


        /// <summary>
        /// Checks for given <paramref name="policyName"/>.
        /// </summary>
        /// <param name="policyName">The policy name. This method does nothing if given <paramref name="policyName"/> is null or empty.</param>
        protected virtual Task CheckPolicy([MaybeNull] string policyName)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(policyName))
                {
                    return;
                }
            });
        }

        protected virtual IStringLocalizer CreateLocalizer()
        {
            if (LocalizationResource != null)
            {
                return StringLocalizerFactory.Create(LocalizationResource);
            }

            var localizer = StringLocalizerFactory.CreateDefaultOrNull();
            if (localizer == null)
            {
                throw new SkywalkerException($"Set {nameof(LocalizationResource)} or define the default localization resource type (by configuring the {nameof(SkywalkerLocalizationOptions)}.{nameof(SkywalkerLocalizationOptions.DefaultResourceType)}) to be able to use the {nameof(L)} object!");
            }

            return localizer;
        }

        protected IStringLocalizer L
        {
            get
            {
                if (_localizer == null)
                {
                    _localizer = CreateLocalizer();
                }

                return _localizer;
            }
        }

        protected Type LocalizationResource
        {
            get => _localizationResource;
            set
            {
                _localizationResource = value;
                _localizer = null;
            }
        }

        public List<string> AppliedCrossCuttingConcerns { get; } = new List<string>();

        public static string[] CommonPostfixes { get; set; } = { "AppService", "ApplicationService", "Service" };

        protected ApplicationService(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

    }
}

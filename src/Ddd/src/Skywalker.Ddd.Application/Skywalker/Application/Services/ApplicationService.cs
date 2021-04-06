using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Skywalker.Application.Services.Contracts;
using Skywalker.Ddd.ObjectMapping;
using Skywalker.Extensions.Linq;
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
        private readonly object _locker = new object();
        private readonly IServiceProvider _serviceProvider;

        private Type _localizationResource = typeof(DefaultResource);

        protected TService LazyGetRequiredService<TService>(ref TService? reference) => LazyGetRequiredService(typeof(TService), ref reference);

        protected TRef LazyGetRequiredService<TRef>(Type serviceType, ref TRef? reference)
        {
            if (reference == null)
            {
                lock (_locker)
                {
                    if (reference == null)
                    {
                        reference = (TRef)_serviceProvider.GetRequiredService(serviceType);
                    }
                }
            }

            return reference;
        }

        private IAsyncQueryableExecuter? asyncExecuter;
        protected IAsyncQueryableExecuter AsyncExecuter => LazyGetRequiredService(ref asyncExecuter);

        private IClock? clock;
        protected IClock Clock => LazyGetRequiredService(ref clock);

        private IGuidGenerator? guidGenerator;

        protected IGuidGenerator GuidGenerator => LazyGetRequiredService(ref guidGenerator);

        private ICurrentUser? currentUser;
        protected ICurrentUser CurrentUser => LazyGetRequiredService(ref currentUser);

        private IObjectMapper? objectMapper;
        protected IObjectMapper ObjectMapper => LazyGetRequiredService(ref objectMapper);

        private ILoggerFactory? loggerFactory;
        protected ILoggerFactory LoggerFactory => LazyGetRequiredService(ref loggerFactory);

        private Lazy<ILogger> _lazyLogger => new Lazy<ILogger>(() => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance, true);
        protected ILogger Logger => _lazyLogger.Value;


        private IStringLocalizerFactory? stringLocalizerFactory;
        protected IStringLocalizerFactory StringLocalizerFactory => LazyGetRequiredService(ref stringLocalizerFactory);

        private IStringLocalizer? _localizer;


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

        public List<string> AppliedCrossCuttingConcerns { get; } = new List<string>();

        public static string[] CommonPostfixes { get; set; } = { "AppService", "ApplicationService", "Service" };

        protected ApplicationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

    }
}

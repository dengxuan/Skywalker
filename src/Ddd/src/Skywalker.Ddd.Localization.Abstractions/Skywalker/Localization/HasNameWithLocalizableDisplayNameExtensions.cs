using Microsoft.Extensions.Localization;
using Skywalker.Localization.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Localization
{
    public static class HasNameWithLocalizableDisplayNameExtensions
    {
        public static string GetLocalizedDisplayName([NotNull] this IHasNameWithLocalizableDisplayName source, [NotNull] IStringLocalizerFactory stringLocalizerFactory, [MaybeNull] string localizationNamePrefix = "DisplayName:")
        {
            if (source.DisplayName != null)
            {
                return source.DisplayName.Localize(stringLocalizerFactory);
            }

            var defaultStringLocalizer = stringLocalizerFactory.CreateDefaultOrNull();
            if (defaultStringLocalizer == null)
            {
                return source.Name;
            }

            var localizedString = defaultStringLocalizer[$"{localizationNamePrefix}{source.Name}"];
            if (!localizedString.ResourceNotFound ||
                localizationNamePrefix.IsNullOrEmpty())
            {
                return localizedString;
            }

            return defaultStringLocalizer[source.Name];
        }
    }
}
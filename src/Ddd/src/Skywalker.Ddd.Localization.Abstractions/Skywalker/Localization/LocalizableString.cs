using Microsoft.Extensions.Localization;
using Skywalker.Localization.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Localization
{
    public class LocalizableString : ILocalizableString
    {
        [MaybeNull]
        public Type ResourceType { get; }

        [NotNull]
        public string Name { get; }

        public LocalizableString(Type resourceType, [NotNull] string name)
        {
            Name = Check.NotNullOrEmpty(name, nameof(name));
            ResourceType = resourceType;
        }

        public LocalizedString Localize(IStringLocalizerFactory stringLocalizerFactory)
        {
            return stringLocalizerFactory.Create(ResourceType)[Name];
        }

        public static LocalizableString Create<TResource>([NotNull] string name)
        {
            return new LocalizableString(typeof(TResource), name);
        }
    }
}
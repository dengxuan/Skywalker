using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Localization.Abstractions
{
    public interface IHasNameWithLocalizableDisplayName
    {
        [NotNull]
        public string Name { get; }

        [MaybeNull]
        public ILocalizableString DisplayName { get; }
    }
}
using Microsoft.Extensions.Localization;

namespace Skywalker.Localization.Abstractions
{
    public interface ILocalizableString
    {
        LocalizedString Localize(IStringLocalizerFactory stringLocalizerFactory);
    }
}
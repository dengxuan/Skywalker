using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Localization
{
    public interface IStringLocalizerFactoryWithDefaultResourceSupport
    {
        IStringLocalizer CreateDefaultOrNull();
    }
}
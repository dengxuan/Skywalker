using Microsoft.Extensions.Localization;

namespace Skywalker.Localization
{
    public interface ITemplateLocalizer
    {
        string Localize(IStringLocalizer localizer, string text);
    }
}
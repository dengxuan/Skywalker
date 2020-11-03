using Microsoft.Extensions.Localization;

namespace Skywalker.ExceptionHandling
{
    public interface ILocalizeErrorMessage
    {
        string LocalizeMessage(IStringLocalizer context);
    }
}
namespace System.Globalization;

public static class CultureHelper
{
    public static IDisposable Use(string culture, string? uiCulture = null)
    {
        culture.NotNull(nameof(culture));

        return Use(
            new CultureInfo(culture),
            uiCulture == null
                ? null
                : new CultureInfo(uiCulture)
        );
    }

    public static IDisposable Use(CultureInfo culture, CultureInfo? uiCulture = null)
    {
        culture.NotNull(nameof(culture));

        var currentCulture = CultureInfo.CurrentCulture;
        var currentUiCulture = CultureInfo.CurrentUICulture;

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = uiCulture ?? culture;

        return new DisposeAction(() =>
        {
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentUiCulture;
        });
    }

    public static bool IsRtl => CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

    public static bool IsValidCultureCode(string cultureCode)
    {
        if (cultureCode.IsNullOrWhiteSpace())
        {
            return false;
        }

        try
        {
            CultureInfo.GetCultureInfo(cultureCode);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }

    public static string GetBaseCultureName(string cultureName)
    {
        return cultureName.Contains('-')
            ? cultureName.Left(cultureName.IndexOf('-'))
            : cultureName;
    }
}

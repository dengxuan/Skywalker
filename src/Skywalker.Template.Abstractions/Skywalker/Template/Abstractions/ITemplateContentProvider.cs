namespace Volo.Abp.TextTemplating;

public interface ITemplateContentProvider
{
    Task<string?> GetContentOrNullAsync(string templateName, string? cultureName = null, bool tryDefaults = true, bool useCurrentCultureIfCultureNameIsNull = true);

    Task<string?> GetContentOrNullAsync(TemplateDefinition templateDefinition, string? cultureName = null, bool tryDefaults = true, bool useCurrentCultureIfCultureNameIsNull = true);
}

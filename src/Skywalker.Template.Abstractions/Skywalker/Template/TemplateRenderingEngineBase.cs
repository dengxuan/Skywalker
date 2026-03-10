using Microsoft.Extensions.Localization;
using Skywalker.Template.Abstractions;

namespace Skywalker.Template;

public abstract class TemplateRenderingEngineBase : ITemplateRenderingEngine
{
    public abstract string Name { get; }

    protected readonly ITemplateDefinitionManager TemplateDefinitionManager;
    protected readonly ITemplateContentProvider TemplateContentProvider;
    protected readonly IStringLocalizerFactory StringLocalizerFactory;

    public TemplateRenderingEngineBase(
        ITemplateDefinitionManager templateDefinitionManager,
        ITemplateContentProvider templateContentProvider,
        IStringLocalizerFactory stringLocalizerFactory)
    {
        TemplateDefinitionManager = templateDefinitionManager;
        TemplateContentProvider = templateContentProvider;
        StringLocalizerFactory = stringLocalizerFactory;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="templateName"></param>
    /// <param name="model"></param>
    /// <param name="cultureName"></param>
    /// <param name="globalContext"></param>
    /// <returns></returns>
    public abstract Task<string> RenderAsync(string templateName, object? model = null, string? cultureName = null, Dictionary<string, object>? globalContext = null);

    protected virtual async Task<string?> GetContentOrNullAsync(TemplateDefinition templateDefinition)
    {
        return await TemplateContentProvider.GetContentOrNullAsync(templateDefinition);
    }

    protected virtual IStringLocalizer GetLocalizerOrNull(TemplateDefinition templateDefinition)
    {
        if (templateDefinition.LocalizationResource != null)
        {
            return StringLocalizerFactory.Create(templateDefinition.LocalizationResource);
        }

        return StringLocalizerFactory.Create("", "");
    }
}

using System.Globalization;
using Microsoft.Extensions.Localization;
using Scriban;
using Scriban.Runtime;
using Skywalker.Template.Abstractions;

namespace Skywalker.Template.Scriban;

/// <summary>
/// Scriban template rendering engine implementation.
/// </summary>
public class ScribanTemplateRenderingEngine : TemplateRenderingEngineBase
{
    public const string EngineName = "Scriban";

    public override string Name => EngineName;

    public ScribanTemplateRenderingEngine(
        ITemplateDefinitionManager templateDefinitionManager,
        ITemplateContentProvider templateContentProvider,
        IStringLocalizerFactory stringLocalizerFactory)
        : base(templateDefinitionManager, templateContentProvider, stringLocalizerFactory)
    {
    }

    public override async Task<string> RenderAsync(
        string templateName,
        object? model = null,
        string? cultureName = null,
        Dictionary<string, object>? globalContext = null)
    {
        var templateDefinition = TemplateDefinitionManager.Get(templateName);
        var templateContent = await GetContentOrNullAsync(templateDefinition)
            ?? throw new InvalidOperationException($"Template content not found for: {templateName}");

        // Handle culture
        CultureInfo? culture = null;
        if (!string.IsNullOrEmpty(cultureName))
        {
            culture = new CultureInfo(cultureName);
        }

        var previousCulture = CultureInfo.CurrentUICulture;
        if (culture != null)
        {
            CultureInfo.CurrentUICulture = culture;
        }

        try
        {
            // Handle layout
            if (!string.IsNullOrEmpty(templateDefinition.Layout))
            {
                var layoutDefinition = TemplateDefinitionManager.Get(templateDefinition.Layout);
                var layoutContent = await GetContentOrNullAsync(layoutDefinition)
                    ?? throw new InvalidOperationException($"Layout template content not found for: {templateDefinition.Layout}");

                var bodyContent = await RenderTemplateAsync(templateContent, model, templateDefinition, globalContext);
                
                globalContext ??= new Dictionary<string, object>();
                globalContext["content"] = bodyContent;
                
                return await RenderTemplateAsync(layoutContent, model, layoutDefinition, globalContext);
            }

            return await RenderTemplateAsync(templateContent, model, templateDefinition, globalContext);
        }
        finally
        {
            if (culture != null)
            {
                CultureInfo.CurrentUICulture = previousCulture;
            }
        }
    }

    protected virtual async Task<string> RenderTemplateAsync(
        string templateContent,
        object? model,
        TemplateDefinition templateDefinition,
        Dictionary<string, object>? globalContext)
    {
        var template = global::Scriban.Template.Parse(templateContent);

        if (template.HasErrors)
        {
            throw new InvalidOperationException(
                $"Template parsing failed: {string.Join(", ", template.Messages.Select(m => m.ToString()))}");
        }

        var scriptObject = new ScriptObject();
        
        // Add model
        if (model != null)
        {
            scriptObject.Import(model, renamer: member => member.Name);
            scriptObject["model"] = model;
        }

        // Add global context
        if (globalContext != null)
        {
            foreach (var item in globalContext)
            {
                scriptObject[item.Key] = item.Value;
            }
        }

        // Add localizer
        var localizer = GetLocalizerOrNull(templateDefinition);
        scriptObject.Import("L", new Func<string, string>(key => localizer[key]));
        scriptObject.Import("L_", new Func<string, object[], string>((key, args) => localizer[key, args]));

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        return await template.RenderAsync(context);
    }
}


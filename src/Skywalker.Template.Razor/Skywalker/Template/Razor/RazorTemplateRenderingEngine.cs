using System.Dynamic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using RazorLight;
using Skywalker.DependencyInjection;
using Skywalker.Template.Abstractions;

namespace Skywalker.Template.Razor;

/// <summary>
/// Razor template rendering engine implementation using RazorLight.
/// </summary>
public class RazorTemplateRenderingEngine : TemplateRenderingEngineBase, ITransientDependency
{
    public const string EngineName = "Razor";

    public override string Name => EngineName;

    private readonly RazorLightEngine _razorEngine;

    public RazorTemplateRenderingEngine(
        ITemplateDefinitionManager templateDefinitionManager,
        ITemplateContentProvider templateContentProvider,
        IStringLocalizerFactory stringLocalizerFactory)
        : base(templateDefinitionManager, templateContentProvider, stringLocalizerFactory)
    {
        _razorEngine = new RazorLightEngineBuilder()
            .UseMemoryCachingProvider()
            .Build();
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
            var viewBag = new ExpandoObject() as IDictionary<string, object?>;
            
            // Add localizer to ViewBag
            var localizer = GetLocalizerOrNull(templateDefinition);
            viewBag["L"] = localizer;
            
            // Add global context to ViewBag
            if (globalContext != null)
            {
                foreach (var item in globalContext)
                {
                    viewBag[item.Key] = item.Value;
                }
            }

            // Handle layout
            if (!string.IsNullOrEmpty(templateDefinition.Layout))
            {
                var layoutDefinition = TemplateDefinitionManager.Get(templateDefinition.Layout);
                var layoutContent = await GetContentOrNullAsync(layoutDefinition)
                    ?? throw new InvalidOperationException($"Layout template content not found for: {templateDefinition.Layout}");

                var bodyContent = await RenderTemplateAsync(templateName, templateContent, model, viewBag);
                viewBag["RenderBody"] = bodyContent;
                
                return await RenderTemplateAsync(templateDefinition.Layout, layoutContent, model, viewBag);
            }

            return await RenderTemplateAsync(templateName, templateContent, model, viewBag);
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
        string templateKey,
        string templateContent,
        object? model,
        IDictionary<string, object?> viewBag)
    {
        var expandoViewBag = (ExpandoObject)(IDictionary<string, object?>)viewBag;

        if (model != null)
        {
            return await _razorEngine.CompileRenderStringAsync(
                templateKey,
                templateContent,
                model,
                expandoViewBag);
        }

        return await _razorEngine.CompileRenderStringAsync(
            templateKey,
            templateContent,
            new { },
            expandoViewBag);
    }
}


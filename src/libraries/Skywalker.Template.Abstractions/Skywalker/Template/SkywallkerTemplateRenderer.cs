using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.ExceptionHandler;
using Skywalker.Template.Abstractions;

namespace Skywalker.Template;

public class SkywallkerTemplateRenderer : ITemplateRenderer//, ITransientDependency
{
    protected SkywalkerTextTemplatingOptions Options { get; }

    protected IServiceScopeFactory ServiceScopeFactory { get; }

    protected ITemplateDefinitionManager TemplateDefinitionManager { get; }

    public SkywallkerTemplateRenderer(IServiceScopeFactory serviceScopeFactory, ITemplateDefinitionManager templateDefinitionManager, IOptions<SkywalkerTextTemplatingOptions> options)
    {
        ServiceScopeFactory = serviceScopeFactory;
        TemplateDefinitionManager = templateDefinitionManager;
        Options = options.Value;
    }

    public virtual async Task<string> RenderAsync(string templateName, object? model = null, string? cultureName = null, Dictionary<string, object>? globalContext = null)
    {
        var templateDefinition = TemplateDefinitionManager.Get(templateName);

        var renderEngine = templateDefinition.RenderEngine;

        if (renderEngine.IsNullOrWhiteSpace())
        {
            renderEngine = Options.DefaultRenderingEngine;
        }

        var providerType = Options.RenderingEngines.GetOrDefault(renderEngine!);

        if (providerType != null && typeof(ITemplateRenderingEngine).IsAssignableFrom(providerType))
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var templateRenderingEngine = (ITemplateRenderingEngine)scope.ServiceProvider.GetRequiredService(providerType);
                return await templateRenderingEngine.RenderAsync(templateName, model, cultureName, globalContext);
            }
        }

        throw new SkywalkerException("There is no rendering engine found with template name: " + templateName);
    }
}

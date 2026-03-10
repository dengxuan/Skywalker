using Skywalker.Extensions.Collections.Generic;
using Skywalker.Template.Abstractions;

namespace Skywalker.Template;

public class SkywalkerTextTemplatingOptions
{
    public ITypeList<ITemplateDefinitionProvider> DefinitionProviders { get; }
    public ITypeList<ITemplateContentContributor> ContentContributors { get; }
    public IDictionary<string, Type> RenderingEngines { get; }

    public string DefaultRenderingEngine { get; set; } = default!;

    public SkywalkerTextTemplatingOptions()
    {
        DefinitionProviders = new TypeList<ITemplateDefinitionProvider>();
        ContentContributors = new TypeList<ITemplateContentContributor>();
        RenderingEngines = new Dictionary<string, Type>();
    }
}

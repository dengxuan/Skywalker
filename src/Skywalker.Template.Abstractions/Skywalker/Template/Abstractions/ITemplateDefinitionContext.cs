using Skywalker.Template;

namespace Skywalker.Template.Abstractions;

public interface ITemplateDefinitionContext
{
    IReadOnlyList<TemplateDefinition> GetAll(string name);

    TemplateDefinition? GetOrNull(string name);

    void Add(params TemplateDefinition[] definitions);
}

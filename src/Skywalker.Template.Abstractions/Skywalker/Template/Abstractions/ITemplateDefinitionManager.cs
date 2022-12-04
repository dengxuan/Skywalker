namespace Skywalker.Template.Abstractions;

public interface ITemplateDefinitionManager
{
    TemplateDefinition Get(string name);

    IReadOnlyList<TemplateDefinition> GetAll();

    TemplateDefinition? GetOrNull(string name);
}

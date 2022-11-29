namespace Volo.Abp.TextTemplating;

public interface ITemplateDefinitionManager
{
    TemplateDefinition Get(string name);

    IReadOnlyList<TemplateDefinition> GetAll();

    TemplateDefinition? GetOrNull(string name);
}

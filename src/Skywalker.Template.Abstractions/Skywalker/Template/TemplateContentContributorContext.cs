namespace Skywalker.Template;

public class TemplateContentContributorContext
{
    public TemplateDefinition TemplateDefinition { get; }

    public IServiceProvider ServiceProvider { get; }

    public string? Culture { get; }

    public TemplateContentContributorContext(TemplateDefinition templateDefinition, IServiceProvider serviceProvider, string? culture)
    {
        TemplateDefinition = templateDefinition.NotNull(nameof(templateDefinition));
        ServiceProvider = serviceProvider.NotNull(nameof(serviceProvider));
        Culture = culture;
    }
}

namespace Skywalker.Template.Abstractions;

public interface ITemplateDefinitionProvider
{
    void PreDefine(ITemplateDefinitionContext context);

    void Define(ITemplateDefinitionContext context);

    void PostDefine(ITemplateDefinitionContext context);
}

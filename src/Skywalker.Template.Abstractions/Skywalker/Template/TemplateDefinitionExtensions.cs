using Skywalker.Template.VirtualFiles;

namespace Skywalker.Template;

public static class TemplateDefinitionExtensions
{
    public static TemplateDefinition WithVirtualFilePath(this TemplateDefinition templateDefinition, string virtualPath,
        bool isInlineLocalized)
    {
        templateDefinition.NotNull(nameof(templateDefinition));

        templateDefinition.IsInlineLocalized = isInlineLocalized;

        return templateDefinition.WithProperty(
            VirtualFileTemplateContentContributor.VirtualPathPropertyName,
            virtualPath
        );
    }

    public static string? GetVirtualFilePathOrNull(this TemplateDefinition templateDefinition)
    {
        templateDefinition.NotNull(nameof(templateDefinition));

        return templateDefinition
            .Properties
            .GetOrDefault(VirtualFileTemplateContentContributor.VirtualPathPropertyName) as string;
    }
}

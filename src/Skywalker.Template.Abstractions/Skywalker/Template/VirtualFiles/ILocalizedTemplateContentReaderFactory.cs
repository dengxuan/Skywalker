using Skywalker.Template;

namespace Skywalker.Template.VirtualFiles;

public interface ILocalizedTemplateContentReaderFactory
{
    Task<ILocalizedTemplateContentReader> CreateAsync(TemplateDefinition templateDefinition);
}

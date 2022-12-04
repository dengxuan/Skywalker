namespace Skywalker.Template.VirtualFiles;

public interface ILocalizedTemplateContentReader
{
    public string? GetContentOrNull(string? culture);
}

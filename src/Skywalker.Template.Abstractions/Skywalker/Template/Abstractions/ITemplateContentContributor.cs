using Skywalker.Template;

namespace Skywalker.Template.Abstractions;

public interface ITemplateContentContributor
{
    Task<string?> GetOrNullAsync(TemplateContentContributorContext context);
}

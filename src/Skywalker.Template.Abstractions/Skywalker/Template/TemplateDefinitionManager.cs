﻿using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.ExceptionHandler;
using Skywalker.Template.Abstractions;

namespace Skywalker.Template;

public class TemplateDefinitionManager : ITemplateDefinitionManager//, ISingletonDependency
{
    protected Lazy<IDictionary<string, TemplateDefinition>> TemplateDefinitions { get; }

    protected SkywalkerTextTemplatingOptions Options { get; }

    protected IServiceProvider ServiceProvider { get; }

    public TemplateDefinitionManager(
        IOptions<SkywalkerTextTemplatingOptions> options,
        IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Options = options.Value;

        TemplateDefinitions =
            new Lazy<IDictionary<string, TemplateDefinition>>(CreateTextTemplateDefinitions, true);
    }

    public virtual TemplateDefinition Get(string name)
    {
        name.NotNull(nameof(name));

        var template = GetOrNull(name);

        if (template == null)
        {
            throw new SkywalkerException("Undefined template: " + name);
        }

        return template;
    }

    public virtual IReadOnlyList<TemplateDefinition> GetAll()
    {
        return TemplateDefinitions.Value.Values.ToImmutableList();
    }

    public virtual TemplateDefinition? GetOrNull(string name)
    {
        return TemplateDefinitions.Value.GetOrDefault(name);
    }

    protected virtual IDictionary<string, TemplateDefinition> CreateTextTemplateDefinitions()
    {
        var templates = new Dictionary<string, TemplateDefinition>();

        using (var scope = ServiceProvider.CreateScope())
        {
            var providers = Options
                .DefinitionProviders
                .Select(p => scope.ServiceProvider.GetRequiredService(p) as ITemplateDefinitionProvider)
                .ToList();

            var context = new TemplateDefinitionContext(templates);

            foreach (var provider in providers)
            {
                provider.PreDefine(context);
            }

            foreach (var provider in providers)
            {
                provider.Define(context);
            }

            foreach (var provider in providers)
            {
                provider.PostDefine(context);
            }
        }

        return templates;
    }
}

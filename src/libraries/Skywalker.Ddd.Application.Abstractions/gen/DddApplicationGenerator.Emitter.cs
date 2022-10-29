// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Skywalker.Ddd.Application.Generators;

internal partial class DddApplicationGenerator
{
    private static readonly HashSet<string> s_namespaces = new()
    {
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "Skywalker.Ddd.Application",
        "Skywalker.Ddd.Application.Abstractions",
        "Skywalker.Ddd.Application.Dtos",
        "Skywalker.Ddd.Application.Dtos.Abstractions",
        "Skywalker.Ddd.Uow",
        "Skywalker.Ddd.Uow.Abstractions",
        "System.CodeDom.Compiler",
        "System.Linq",
    };

    internal class Emitter
    {
        public static void Emit(GeneratorExecutionContext context, IEnumerable<Intecepter> intecepters)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/InterceptorGenerator.sbn-cs";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);
            foreach (var intecepter in intecepters)
            {
                var namespaces = s_namespaces.Union(intecepter.Namespaces).OrderBy(keySelector => keySelector);
                var output = template.Render(new
                {
                    GeneratorVersion = generatorVersion,
                    Namespaces = namespaces,
                    Intecepter = intecepter,
                }, member => member.Name);
                context.AddSource($"Skywalker.Ddd.Application.{intecepter.Name}Intecepter.Generated.cs", SourceText.From(output, Encoding.UTF8));
            }
        }

        public static void Emit(GeneratorExecutionContext context, IEnumerable<Dependency> dependencies)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/DependencyInjection.sbn-cs";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);
            var namespaces = new List<string>();
            foreach (var dependency in dependencies)
            {
                namespaces.AddRange(dependency.Namespaces);
            }
            var output = template.Render(new
            {
                GeneratorVersion = generatorVersion,
                Namespaces = s_namespaces.Union(namespaces).OrderBy(keySelector => keySelector),
                Dependencies = dependencies.Where(predicate => !new string[] { "IApplicationService" }.Contains(predicate.Interface)).ToList(),
            }, member => member.Name);
            context.AddSource($"ApplicationServiceIServiceCollectionExtensions.Generated.cs", SourceText.From(output, Encoding.UTF8));
        }
    }
}

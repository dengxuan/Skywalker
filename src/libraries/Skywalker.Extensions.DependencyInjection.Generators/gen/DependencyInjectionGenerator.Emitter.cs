// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Skywalker.Extensions.DependencyInjection.Generators;

public partial class DependencyInjectionGenerator
{
    private static readonly HashSet<string> s_namespaces = new()
    {
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "Skywalker.Extensions.DependencyInjection",
        "Skywalker.Extensions.DependencyInjection.Abstractions",
        "Skywalker.Extensions.DependencyInjection.Interceptors",
        "Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions",
        "System.CodeDom.Compiler",
        "System.Linq",
    };

    internal class Emitter
    {
        public static void Emit(GeneratorExecutionContext context, Metadata metadata)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/InterceptorGenerator.sbn-cs";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);
            var intecepters = new HashSet<Dependency>();
            var classes = new List<string>();
            foreach (var intecepter in metadata.Intecepters)
            {
                var proxy = template.Render(new
                {
                    intecepter.Name,
                    Interfaces = string.Join(", ", intecepter.Interfaces),
                    Methods = intecepter.Methods.Select(selector => new
                    {
                        selector.Name,
                        selector.ReturnType,
                        Arguments = string.Join(", ", selector.Arguments.Select(selector => $"{selector.Value}")),
                        Parameters = string.Join(", ", selector.Arguments.Select(selector => $"{selector.Key} {selector.Value}"))
                    })
                }, member => member.Name);
                classes.Add(proxy);
                var dependency = new Dependency($"{intecepter.Name}Proxy");
                foreach (var item in intecepter.Interfaces)
                {
                    dependency.Interfaces.Add(item);
                }
                intecepters.Add(dependency);
            }
            var dependencyFile = @"Templates/DependencyInjectionGenerator.sbn-cs";
            var dependencyTemplate = Template.Parse(EmbeddedResource.GetContent(dependencyFile), dependencyFile);
            var output = dependencyTemplate.Render(new
            {
                Namespaces = s_namespaces.Union(metadata.Namespaces).OrderBy(keySelector => keySelector).ToArray(),
                Classes = classes,
                metadata.ScopedDepedency,
                metadata.SingletonDepedency,
                metadata.TransientDepedency,
                IntecepterDepedency = intecepters
            }, member => member.Name);
            context.AddSource($"DependencyInjectionIServiceCollectionExtensions.g.cs", SourceText.From(output, Encoding.UTF8));
        }
    }
}

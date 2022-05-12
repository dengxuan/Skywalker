// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Skywalker.Extensions.ObjectMapper.Generators;

public partial class ObjectMapperGenerator
{
    private static readonly HashSet<string> s_namespaces = new()
    {
        "System.CodeDom.Compiler",
        "System.Linq",
    };

    internal class Emitter
    {
        public static void Emit(GeneratorExecutionContext context, IReadOnlyList<SourceClass> sourceClasses)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/ObjectMapper.sbn-cs";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);
            foreach (var sourceClass in sourceClasses)
            {
                var targetNamespaces = sourceClass.Targets.Select(selector => selector.Namespace).ToList();
                var currentNamespaces = new HashSet<string>(s_namespaces)
                {
                    sourceClass.Namespace,
                };
                foreach (var item in sourceClass.Targets)
                {
                    currentNamespaces.Add(item.Namespace);
                }
                var orderdNamespaces = currentNamespaces.OrderBy(keySelector => keySelector).ToArray();
                var output = template.Render(new
                {
                    GeneratorVersion = generatorVersion,
                    Namespaces = orderdNamespaces,
                    sourceClass.Namespace,
                    SourceName = sourceClass.Name,
                    sourceClass.Targets
                }, member => member.Name);
                context.AddSource($"{sourceClass.Namespace}.g.cs", SourceText.From(output, Encoding.UTF8));

            }
        }
    }
}

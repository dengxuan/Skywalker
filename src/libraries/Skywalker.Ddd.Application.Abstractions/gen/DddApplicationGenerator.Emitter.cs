// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Skywalker.Ddd.Application.Generators;

public partial class DddApplicationGenerator
{
    private static readonly HashSet<string> s_namespaces = new()
    {
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "Skywalker.Ddd.Application",
        "Skywalker.Ddd.Application.Abstractions",
        "Skywalker.Ddd.Application.Dtos",
        "Skywalker.Ddd.Application.Dtos.Abstractions",
        "System.CodeDom.Compiler",
        "System.Linq",
    };

    internal class Emitter
    {
        public static void Emit(GeneratorExecutionContext context, IReadOnlyList<DbContextClass> dbContextClasses)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/DddApplication.sbn-cs";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);
            foreach (var dbContextClass in dbContextClasses)
            {
                var currentNamespaces = new HashSet<string>(s_namespaces)
                {
                    dbContextClass.Namespace,
                };
                var orderdNamespaces = currentNamespaces.OrderBy(keySelector => keySelector).ToArray();
                var entitiesIndex = dbContextClass.ReturnSymbol.Name;
                var arguments = dbContextClass.ArgumentSymbols.Select(selector => $"{selector.Type.Name} {selector.Name}").ToArray();
                var argumentNames = dbContextClass.ArgumentSymbols.Select(selector => $"{selector.Name}").ToArray();
                var handlerName = $"{dbContextClass.Name.TrimStart('I')}Handler";
                var output = template.Render(new
                {
                    GeneratorVersion = generatorVersion,
                    Namespaces = orderdNamespaces,
                    Namespace = dbContextClass.Namespace,
                    Name = dbContextClass.Name,
                    HandlerName = handlerName,
                    MethodName = dbContextClass.MethodName.TrimEnd("Async".ToArray()),
                    Arguments = string.Join(",", arguments),
                    ArgumentNames = string.Join(",", argumentNames)
                }, member => member.Name);
                context.AddSource($"{dbContextClass.Namespace}.{handlerName}.g.cs", SourceText.From(output, Encoding.UTF8));

            }
        }
    }
}

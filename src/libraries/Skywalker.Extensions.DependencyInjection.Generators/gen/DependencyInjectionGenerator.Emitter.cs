// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

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
        "Skywalker.Extensions.DependencyInjection.Interceptors",
        "Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions",
        "System.CodeDom.Compiler",
        "System.Linq",
    };

    internal class Emitter
    {
        public static void Emit(GeneratorExecutionContext context, IReadOnlyList<DbContextClass> dbContextClasses)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/DependencyInjectionGenerator.sbn-cs";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);
            foreach (var dbContextClass in dbContextClasses)
            {
                var methods = new List<Method>();
                var currentNamespaces = new HashSet<string>(s_namespaces)
                {
                    dbContextClass.Namespace,
                };
                foreach (var method in dbContextClass.Methods)
                {
                    if (method.IsStatic || method.MethodKind == MethodKind.Constructor)
                    {
                        continue;
                    }
                    currentNamespaces.Add(method.ReturnType.ContainingNamespace.ToDisplayString());
                    //foreach (var item in method.Parameters)
                    //{
                    //    currentNamespaces.Add(item.OriginalDefinition.ToDisplayString());
                    //}
                    var arguments = method.Parameters.Select(selector => selector.Name);
                    var parameters = method.Parameters.Select(selector => $"{selector.OriginalDefinition.ToDisplayString()} {selector.Name}");
                    methods.Add(new Method(method.Name, method.ReturnType.ToDisplayString(), string.Join(", ", arguments), string.Join(", ", parameters)));
                }

                var orderdNamespaces = currentNamespaces.OrderBy(keySelector => keySelector).ToArray();
                var output = template.Render(new
                {
                    GeneratorVersion = generatorVersion,
                    Namespaces = orderdNamespaces,
                    Interfaces = dbContextClass.Interfaces,
                    ClassName = dbContextClass.Name,
                    Methods = methods,
                }, member => member.Name);
                context.AddSource($"{dbContextClass.Name}Proxy.g.cs", SourceText.From(output, Encoding.UTF8));
            }
        }

        record class Method(string Name, string ReturnType, string Arguments, string Parameters);
    }
}

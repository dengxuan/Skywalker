// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

public partial class DddEntityFrameworkCoreGenerator
{
    private static readonly HashSet<string> s_namespaces = new()
    {
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "Skywalker.Ddd.Domain.Entities",
        "Skywalker.Ddd.Domain.Repositories",
        "Skywalker.Ddd.EntityFrameworkCore",
        "Skywalker.Ddd.EntityFrameworkCore.Repositories",
        "Skywalker.IdentifierGenerator.Abstractions",
        "Skywalker.Extensions.Timezone",
        "System.CodeDom.Compiler",
        "System.Linq",
    };

    internal class Emitter
    {
        public static void Emit(GeneratorExecutionContext context, IReadOnlyList<DbContextClass> dbContextClasses)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/DddEntityFrameworkCore.sbn-cs";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);
            foreach (var dbContextClass in dbContextClasses)
            {
                foreach (var dbContextProperty in dbContextClass.Properties)
                {
                    var currentNamespaces = new HashSet<string>(s_namespaces)
                    {
                        dbContextClass.Namespace,
                        dbContextProperty.Namespace
                    };
                    var orderdNamespaces = currentNamespaces.OrderBy(keySelector => keySelector).ToArray();
                    var entitiesIndex = dbContextProperty.Namespace.IndexOf(".Entities");
                    if (entitiesIndex == -1)
                    {
                        entitiesIndex = dbContextProperty.Namespace.Length;
                    }
                    var repositoriesNamespace = $"{dbContextProperty.Namespace.Substring(0, entitiesIndex)}.Repositories";
                    var output = template.Render(new { GeneratorVersion = generatorVersion, Namespaces = orderdNamespaces, Namespace = repositoriesNamespace, DbContext = dbContextClass.Name, dbContextProperty.PrimaryKey, dbContextProperty.Name }, member => member.Name);
                    context.AddSource($"{repositoriesNamespace}.{dbContextProperty.Name}.g.cs", SourceText.From(output, Encoding.UTF8));
                }
            }
        }
    }
}

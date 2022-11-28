// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

public partial class EntityFrameworkCoreGenerator
{
    internal class Emitter
    {
        private static readonly HashSet<string> s_namespaces = new()
        {
            "Microsoft.Extensions.DependencyInjection",
            "Microsoft.Extensions.DependencyInjection.Extensions",
            "Skywalker.Ddd.Domain.Entities",
            "Skywalker.Ddd.Domain.Repositories",
            "Skywalker.Ddd.Domain.Services",
            "Skywalker.Ddd.Uow",
            "Skywalker.Ddd.Uow.Abstractions",
            "Skywalker.Ddd.EntityFrameworkCore",
            "Skywalker.Ddd.EntityFrameworkCore.Repositories",
            "Skywalker.Identifier.Abstractions",
            "Skywalker.Extensions.Timezone",
            "System.CodeDom.Compiler",
            "System.Linq",
        };

        public static void Emit(GeneratorExecutionContext context, IEnumerable<Repository> repositories)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/Repository.sbn-cs";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);

            foreach (var repository in repositories)
            {
                var namespaces = s_namespaces.Union(repository.Namespaces).OrderBy(keySelector => keySelector);
                foreach (var entity in repository.Entitiess)
                {
                    var output = template.Render(new
                    {
                        GeneratorVersion = generatorVersion,
                        Namespaces = namespaces,
                        repository.Namespace,
                        repository.DbContextName,
                        entity.EntityName,
                        entity.PrimaryKeyName
                    }, member => member.Name);
                    context.AddSource($"Skywalker.Ddd.Domain.Repositories.{repository.DbContextName}.{entity.EntityName}.Generated.cs", SourceText.From(output, Encoding.UTF8));
                }
            }
        }

        public static void Emit(GeneratorExecutionContext context, IEnumerable<Intecepter> intecepters)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/DomainServiceInterceptorGenerator.sbn-cs";
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
                context.AddSource($"Skywalker.Ddd.Domain.Services.{intecepter.Name}Intecepter.Generated.cs", SourceText.From(output, Encoding.UTF8));
            }
        }

        public static void EmitDomainServiceDependencies(GeneratorExecutionContext context, IEnumerable<Dependency> dependencies)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/DomainServiceIServiceCollectionExtensions.sbn-cs";
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
                Dependencies = dependencies.Where(predicate => !new string[] { "IDomainService", "ITransientDependency", "ITransientDependency", "ITransientDependency" }.Contains(predicate.Interface)).ToList(),
            }, member => member.Name);
            context.AddSource($"DomainServiceIServiceCollectionExtensions.Generated.cs", SourceText.From(output, Encoding.UTF8));
        }

        public static void EmitRepositoryDependencies(GeneratorExecutionContext context, IEnumerable<Dependency> dependencies)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/RepositoryIServiceCollectionExtensions.sbn-cs";
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
                Dependencies = dependencies.Where(predicate => !new string[] { "IDomainService", "ITransientDependency", "ITransientDependency", "ITransientDependency" }.Contains(predicate.Interface)).ToList(),
            }, member => member.Name);
            context.AddSource($"RepositoryIServiceCollectionExtensions.Generated.cs", SourceText.From(output, Encoding.UTF8));
        }
    }
}

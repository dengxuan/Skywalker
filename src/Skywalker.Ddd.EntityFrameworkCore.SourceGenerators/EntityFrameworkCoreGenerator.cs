// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.Ddd.EntityFrameworkCore.SourceGenerators;

/// <summary>
/// EF Core 仓储与领域服务 Source Generator。
/// 扫描所有继承自 SkywalkerDbContext&lt;T&gt; 的类，为每个 DbSet&lt;TEntity&gt; 属性生成仓储和领域服务注册代码。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class EntityFrameworkCoreGenerator : IIncrementalGenerator
{
    private const string DbSetTypeName = "Microsoft.EntityFrameworkCore.DbSet`1";
    private const string IEntityTypeName = "Skywalker.Ddd.Domain.Entities.IEntity";
    private const string IEntityWithKeyTypeName = "Skywalker.Ddd.Domain.Entities.IEntity`1";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 收集所有继承自 SkywalkerDbContext<T> 的类声明
        var dbContextDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(static c => c is not null);

        // 结合编译信息
        var compilationAndClasses = context.CompilationProvider.Combine(dbContextDeclarations.Collect());

        // 生成代码
        context.RegisterSourceOutput(compilationAndClasses, (sourceContext, source) =>
        {
            var (compilation, classes) = source;
            var repositories = CollectRepositories(compilation, classes);

            if (repositories.Count == 0) return;

            var assemblyName = compilation.AssemblyName ?? "Generated";
            var sourceCode = GenerateSource(assemblyName, repositories);
            sourceContext.AddSource($"{assemblyName}.AutoRepositories.g.cs", sourceCode);
        });
    }

    /// <summary>
    /// 收集所有需要生成仓储注册的信息。
    /// </summary>
    private static List<(string DbContextName, List<RepositoryInfo> Repositories)> CollectRepositories(
        Compilation compilation,
        IEnumerable<ClassDeclarationSyntax> classDeclarations)
    {
        var result = new List<(string, List<RepositoryInfo>)>();
        var dbSetType = compilation.GetTypeByMetadataName(DbSetTypeName);
        var entityType = compilation.GetTypeByMetadataName(IEntityTypeName);
        var entityWithKeyType = compilation.GetTypeByMetadataName(IEntityWithKeyTypeName);

        if (dbSetType is null || entityType is null)
            return result;

        foreach (var classDecl in classDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(classDecl.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(classDecl);

            if (symbol is not INamedTypeSymbol classSymbol) continue;
            if (classSymbol.IsAbstract) continue;

            // 检查是否继承自 SkywalkerDbContext<T>（通过名称匹配）
            if (!InheritsFromSkywalkerDbContext(classSymbol))
                continue;

            var repositories = new List<RepositoryInfo>();
            var dbContextFullName = classSymbol.ToDisplayString();

            // 扫描所有 DbSet<TEntity> 属性
            foreach (var member in classSymbol.GetMembers())
            {
                if (member is not IPropertySymbol property) continue;
                if (property.Type is not INamedTypeSymbol propertyType) continue;

                // 检查是否是 DbSet<T>
                if (!IsDbSetType(propertyType, dbSetType)) continue;

                // 获取实体类型
                var entityTypeArg = propertyType.TypeArguments.FirstOrDefault();
                if (entityTypeArg is not INamedTypeSymbol entitySymbol) continue;

                // 检查实体是否实现 IEntity
                if (!ImplementsInterface(entitySymbol, entityType)) continue;

                // 获取主键类型
                var keyType = GetEntityKeyType(entitySymbol, entityWithKeyType);

                repositories.Add(new RepositoryInfo(
                    entitySymbol.ToDisplayString(),
                    entitySymbol.ContainingNamespace.ToDisplayString(),
                    keyType,
                    dbContextFullName));
            }

            if (repositories.Count > 0)
            {
                result.Add((dbContextFullName, repositories));
            }
        }

        return result;
    }

    /// <summary>
    /// 检查类型是否继承自 SkywalkerDbContext&lt;T&gt;（通过类型名称匹配）。
    /// </summary>
    private static bool InheritsFromSkywalkerDbContext(INamedTypeSymbol classSymbol)
    {
        var baseType = classSymbol.BaseType;
        while (baseType != null)
        {
            var baseTypeName = baseType.OriginalDefinition.ToDisplayString();
            if (baseTypeName == "Skywalker.Ddd.EntityFrameworkCore.SkywalkerDbContext<TDbContext>")
                return true;
            baseType = baseType.BaseType;
        }
        return false;
    }

    /// <summary>
    /// 检查属性类型是否是 DbSet&lt;T&gt;。
    /// </summary>
    private static bool IsDbSetType(INamedTypeSymbol propertyType, INamedTypeSymbol dbSetType)
    {
        return propertyType.OriginalDefinition.Equals(dbSetType, SymbolEqualityComparer.Default);
    }

    /// <summary>
    /// 检查类型是否实现了指定接口。
    /// </summary>
    private static bool ImplementsInterface(INamedTypeSymbol type, INamedTypeSymbol interfaceType)
    {
        return type.AllInterfaces.Any(i =>
            i.Equals(interfaceType, SymbolEqualityComparer.Default) ||
            i.OriginalDefinition.Equals(interfaceType, SymbolEqualityComparer.Default));
    }

    /// <summary>
    /// 获取实体的主键类型。
    /// </summary>
    private static string? GetEntityKeyType(INamedTypeSymbol entitySymbol, INamedTypeSymbol? entityWithKeyType)
    {
        if (entityWithKeyType is null) return null;

        foreach (var iface in entitySymbol.AllInterfaces)
        {
            if (iface.OriginalDefinition.Equals(entityWithKeyType, SymbolEqualityComparer.Default))
            {
                var keyTypeArg = iface.TypeArguments.FirstOrDefault();
                return keyTypeArg?.ToDisplayString();
            }
        }

        return null;
    }

    /// <summary>
    /// 生成仓储注册代码。
    /// </summary>
    private static string GenerateSource(string assemblyName, List<(string DbContextName, List<RepositoryInfo> Repositories)> dbContexts)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection.Extensions;");
        sb.AppendLine("using Skywalker.Ddd.Domain.Repositories;");
        sb.AppendLine("using Skywalker.Ddd.Domain.Services;");
        sb.AppendLine();
        sb.AppendLine("namespace Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// EF Core 仓储与领域服务自动注册扩展方法（自动生成）。");
        sb.AppendLine("/// </summary>");

        var className = GetSafeClassName(assemblyName);
        sb.AppendLine($"public static class {className}AutoRepositoryExtensions");
        sb.AppendLine("{");

        // 为每个 DbContext 生成扩展方法
        foreach (var (dbContextName, repositories) in dbContexts)
        {
            var dbContextShortName = GetShortTypeName(dbContextName);
            
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// 为 <see cref=\"{dbContextName}\"/> 添加自动发现的仓储和领域服务。");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    /// <param name=\"services\">服务集合。</param>");
            sb.AppendLine("    /// <returns>服务集合。</returns>");
            sb.AppendLine($"    public static IServiceCollection Add{dbContextShortName}Repositories(this IServiceCollection services)");
            sb.AppendLine("    {");

            foreach (var repo in repositories)
            {
                GenerateRepositoryRegistration(sb, repo);
                GenerateDomainServiceRegistration(sb, repo);
            }

            sb.AppendLine("        return services;");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        // 生成一个聚合方法，添加所有仓储
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 添加所有自动发现的仓储和领域服务（内部方法）。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <param name=\"services\">服务集合。</param>");
        sb.AppendLine("    /// <returns>服务集合。</returns>");
        sb.AppendLine("    internal static IServiceCollection AddAutoRepositories(this IServiceCollection services)");
        sb.AppendLine("    {");

        foreach (var (dbContextName, _) in dbContexts)
        {
            var dbContextShortName = GetShortTypeName(dbContextName);
            sb.AppendLine($"        services.Add{dbContextShortName}Repositories();");
        }

        sb.AppendLine("        return services;");
        sb.AppendLine("    }");

        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// 生成单个仓储的注册代码。
    /// </summary>
    private static void GenerateRepositoryRegistration(StringBuilder sb, RepositoryInfo repo)
    {
        var entityName = repo.EntityTypeName;
        var dbContextName = repo.DbContextTypeName;

        sb.AppendLine($"        // {GetShortTypeName(entityName)}");

        // 注册 IRepository<TEntity>
        sb.AppendLine($"        services.TryAddScoped<IRepository<{entityName}>, Repository<{dbContextName}, {entityName}>>();");
        sb.AppendLine($"        services.TryAddScoped<IReadOnlyRepository<{entityName}>, Repository<{dbContextName}, {entityName}>>();");
        sb.AppendLine($"        services.TryAddScoped<IBasicRepository<{entityName}>, Repository<{dbContextName}, {entityName}>>();");

        // 如果有主键类型，还注册 IRepository<TEntity, TKey>
        if (repo.KeyTypeName != null)
        {
            sb.AppendLine($"        services.TryAddScoped<IRepository<{entityName}, {repo.KeyTypeName}>, Repository<{dbContextName}, {entityName}, {repo.KeyTypeName}>>();");
            sb.AppendLine($"        services.TryAddScoped<IReadOnlyRepository<{entityName}, {repo.KeyTypeName}>, Repository<{dbContextName}, {entityName}, {repo.KeyTypeName}>>();");
            sb.AppendLine($"        services.TryAddScoped<IBasicRepository<{entityName}, {repo.KeyTypeName}>, Repository<{dbContextName}, {entityName}, {repo.KeyTypeName}>>();");
        }

        sb.AppendLine();
    }

    /// <summary>
    /// 生成单个领域服务的注册代码。
    /// </summary>
    private static void GenerateDomainServiceRegistration(StringBuilder sb, RepositoryInfo repo)
    {
        var entityName = repo.EntityTypeName;

        // 注册 IDomainService<TEntity>
        sb.AppendLine($"        services.TryAddScoped<IDomainService<{entityName}>, DomainService<{entityName}>>();");

        // 如果有主键类型，还注册 IDomainService<TEntity, TKey>
        if (repo.KeyTypeName != null)
        {
            sb.AppendLine($"        services.TryAddScoped<IDomainService<{entityName}, {repo.KeyTypeName}>, DomainService<{entityName}, {repo.KeyTypeName}>>();");
        }

        sb.AppendLine();
    }

    /// <summary>
    /// 将程序集名称转换为有效的类名。
    /// </summary>
    private static string GetSafeClassName(string assemblyName)
    {
        return assemblyName.Replace(".", "").Replace("-", "").Replace(" ", "");
    }

    /// <summary>
    /// 获取类型的短名称（不含命名空间）。
    /// </summary>
    private static string GetShortTypeName(string fullTypeName)
    {
        var lastDotIndex = fullTypeName.LastIndexOf('.');
        return lastDotIndex >= 0 ? fullTypeName.Substring(lastDotIndex + 1) : fullTypeName;
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Skywalker.SourceGenerators;

namespace Skywalker.Ddd.EntityFrameworkCore.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class RepositoryRegistrationGenerator : IIncrementalGenerator
{
    private const string DbContextMetadataName = "Skywalker.Ddd.EntityFrameworkCore.SkywalkerDbContext`1";
    private const string DbSetMetadataName = "Microsoft.EntityFrameworkCore.DbSet`1";
    private const string EntityMetadataName = "Skywalker.Ddd.Domain.Entities.IEntity";
    private const string EntityWithKeyMetadataName = "Skywalker.Ddd.Domain.Entities.IEntity`1";
    private const string Category = "Skywalker.Ddd.EntityFrameworkCore.SourceGenerators";

    private static readonly DiagnosticDescriptor DbSetMustBePublicInstance = new(
        id: "SKY3001",
        title: "DbSet property must be public instance",
        messageFormat: "DbSet property '{0}' on DbContext '{1}' must be a public instance property to participate in repository generation",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3001.md");

    private static readonly DiagnosticDescriptor EntityTypeMustBeAccessible = new(
        id: "SKY3002",
        title: "Entity type must be accessible to generated code",
        messageFormat: "Entity type '{0}' exposed by DbSet property '{1}' must be public or internal so generated repository registrations can reference it",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3002.md");

    private static readonly DiagnosticDescriptor EntityTypeMustImplementEntity = new(
        id: "SKY3003",
        title: "Entity type must implement IEntity",
        messageFormat: "Entity type '{0}' exposed by DbSet property '{1}' must implement Skywalker.Ddd.Domain.Entities.IEntity",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3003.md");

    private static readonly DiagnosticDescriptor EntityTypeMustBeConcreteClass = new(
        id: "SKY3004",
        title: "Entity type must be a concrete class",
        messageFormat: "Entity type '{0}' exposed by DbSet property '{1}' must be a non-abstract class",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3004.md");

    private static readonly DiagnosticDescriptor DuplicateEntityRegistration = new(
        id: "SKY3005",
        title: "Entity type is exposed by multiple DbSet properties",
        messageFormat: "Entity type '{0}' is exposed by multiple DbSet properties on DbContext '{1}': {2}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3005.md");

    private static readonly DiagnosticDescriptor ConflictingEntityKeyTypes = new(
        id: "SKY3006",
        title: "Entity key type inference is ambiguous",
        messageFormat: "Entity type '{0}' implements IEntity<TKey> with conflicting key types: {1}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3006.md");

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dbContexts = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                static (context, _) => CreateModel(context.SemanticModel, (ClassDeclarationSyntax)context.Node))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!.Value);

        context.RegisterSourceOutput(dbContexts, static (context, model) => Generate(context, model));
    }

    private static DbContextRegistrationModel? CreateModel(SemanticModel semanticModel, ClassDeclarationSyntax declaration)
    {
        if (semanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol dbContextSymbol
            || !InheritsFrom(dbContextSymbol, DbContextMetadataName))
        {
            return null;
        }

        var entityBuilder = ImmutableArray.CreateBuilder<EntityRegistrationModel>();
        var diagnosticBuilder = ImmutableArray.CreateBuilder<DiagnosticModel>();
        var entityProperties = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var member in dbContextSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (!TryGetDbSetEntityType(member, out var entityType))
            {
                continue;
            }

            if (member.DeclaredAccessibility != Accessibility.Public || member.IsStatic)
            {
                diagnosticBuilder.Add(DiagnosticModel.Create(
                    DbSetMustBePublicInstance,
                    member.Locations.FirstOrDefault(),
                    member.Name,
                    FormatType(dbContextSymbol)));
                continue;
            }

            if (!IsAccessibleToGeneratedCode(entityType))
            {
                diagnosticBuilder.Add(DiagnosticModel.Create(
                    EntityTypeMustBeAccessible,
                    member.Locations.FirstOrDefault(),
                    FormatType(entityType),
                    member.Name));
                continue;
            }

            if (!Implements(entityType, EntityMetadataName))
            {
                diagnosticBuilder.Add(DiagnosticModel.Create(
                    EntityTypeMustImplementEntity,
                    member.Locations.FirstOrDefault(),
                    FormatType(entityType),
                    member.Name));
                continue;
            }

            if (entityType.TypeKind != TypeKind.Class || entityType.IsAbstract)
            {
                diagnosticBuilder.Add(DiagnosticModel.Create(
                    EntityTypeMustBeConcreteClass,
                    member.Locations.FirstOrDefault(),
                    FormatType(entityType),
                    member.Name));
                continue;
            }

            var entityTypeName = entityType.GetFullyQualifiedName();
            if (!entityProperties.TryGetValue(entityTypeName, out var propertyNames))
            {
                propertyNames = new List<string>();
                entityProperties.Add(entityTypeName, propertyNames);
            }

            propertyNames.Add(member.Name);
            if (propertyNames.Count > 1)
            {
                diagnosticBuilder.Add(DiagnosticModel.Create(
                    DuplicateEntityRegistration,
                    member.Locations.FirstOrDefault(),
                    FormatType(entityType),
                    FormatType(dbContextSymbol),
                    string.Join(", ", propertyNames)));
                continue;
            }

            var keyResolution = ResolvePrimaryKeyType(entityType);
            if (keyResolution.ConflictingKeyTypes is not null)
            {
                diagnosticBuilder.Add(DiagnosticModel.Create(
                    ConflictingEntityKeyTypes,
                    member.Locations.FirstOrDefault(),
                    FormatType(entityType),
                    keyResolution.ConflictingKeyTypes));
                continue;
            }

            entityBuilder.Add(new EntityRegistrationModel(
                entityTypeName,
                keyResolution.PrimaryKeyType?.GetFullyQualifiedName()));
        }

        var entities = entityBuilder.ToImmutable();
        var diagnostics = diagnosticBuilder.ToImmutable();
        if (entities.Length == 0 && diagnostics.Length == 0)
        {
            return null;
        }

        return new DbContextRegistrationModel(
            dbContextSymbol.GetFullyQualifiedName(),
            CreateIdentifier(dbContextSymbol),
            new EquatableArray<EntityRegistrationModel>(entities),
            new EquatableArray<DiagnosticModel>(diagnostics));
    }

    private static void Generate(SourceProductionContext context, DbContextRegistrationModel model)
    {
        foreach (var diagnostic in model.Diagnostics)
        {
            context.ReportDiagnostic(diagnostic.Create());
        }

        if (model.Entities.Length == 0)
        {
            return;
        }

        var source = new StringBuilder();
        source.AppendLine("// <auto-generated/>");
        source.AppendLine("// This file was generated by Skywalker Source Generators.");
        source.AppendLine("// Do not modify this file manually.");
        source.AppendLine();
        source.AppendLine("#nullable enable");
        source.AppendLine("#pragma warning disable");
        source.AppendLine();
        source.Append("[assembly: global::Skywalker.Ddd.EntityFrameworkCore.SkywalkerGeneratedRepositoryRegistrationAttribute(typeof(")
            .Append(model.DbContextTypeName)
            .Append("), typeof(global::Microsoft.Extensions.DependencyInjection.")
            .Append(model.Identifier)
            .Append("SkywalkerRepositoryRegistrations), nameof(global::Microsoft.Extensions.DependencyInjection.")
            .Append(model.Identifier)
            .Append("SkywalkerRepositoryRegistrations.AddSkywalkerGeneratedRepositoriesFor")
            .Append(model.Identifier)
            .AppendLine("))]");
        source.AppendLine();
        source.AppendLine("namespace Microsoft.Extensions.DependencyInjection;");
        source.AppendLine();
        source.Append("internal static class ").Append(model.Identifier).AppendLine("SkywalkerRepositoryRegistrations");
        source.AppendLine("{");
        source.Append("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddSkywalkerGeneratedRepositoriesFor")
            .Append(model.Identifier)
            .AppendLine("(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
        source.AppendLine("    {");

        foreach (var entity in model.Entities)
        {
            if (entity.PrimaryKeyTypeName is null)
            {
                AppendRepositoryRegistration(source, entity.EntityTypeName, null, $"global::Skywalker.Ddd.Domain.Repositories.Repository<{model.DbContextTypeName}, {entity.EntityTypeName}>");
                AppendDomainServiceRegistration(source, entity.EntityTypeName, null, $"global::Skywalker.Identity.Domain.Repositories.EntityFrameworkCoreDomainService<{entity.EntityTypeName}>");
            }
            else
            {
                AppendRepositoryRegistration(source, entity.EntityTypeName, entity.PrimaryKeyTypeName, $"global::Skywalker.Ddd.Domain.Repositories.Repository<{model.DbContextTypeName}, {entity.EntityTypeName}, {entity.PrimaryKeyTypeName}>");
                AppendDomainServiceRegistration(source, entity.EntityTypeName, entity.PrimaryKeyTypeName, $"global::Skywalker.Identity.Domain.Repositories.EntityFrameworkCoreDomainService<{entity.EntityTypeName}, {entity.PrimaryKeyTypeName}>");
            }
        }

        source.AppendLine("        return services;");
        source.AppendLine("    }");
        source.AppendLine("}");

        context.AddSource($"{model.Identifier}.SkywalkerRepositoryRegistrations.g.cs", source.ToString());
    }

    private static void AppendRepositoryRegistration(StringBuilder source, string entityTypeName, string? primaryKeyTypeName, string implementationTypeName)
    {
        if (primaryKeyTypeName is not null)
        {
            AppendTransient(source, $"global::Skywalker.Ddd.Domain.Repositories.IReadOnlyRepository<{entityTypeName}, {primaryKeyTypeName}>", implementationTypeName);
            AppendTransient(source, $"global::Skywalker.Ddd.Domain.Repositories.IBasicRepository<{entityTypeName}, {primaryKeyTypeName}>", implementationTypeName);
            AppendTransient(source, $"global::Skywalker.Ddd.Domain.Repositories.IRepository<{entityTypeName}, {primaryKeyTypeName}>", implementationTypeName);
        }

        AppendTransient(source, $"global::Skywalker.Ddd.Domain.Repositories.IReadOnlyRepository<{entityTypeName}>", implementationTypeName);
        AppendTransient(source, $"global::Skywalker.Ddd.Domain.Repositories.IBasicRepository<{entityTypeName}>", implementationTypeName);
        AppendTransient(source, $"global::Skywalker.Ddd.Domain.Repositories.IRepository<{entityTypeName}>", implementationTypeName);
    }

    private static void AppendDomainServiceRegistration(StringBuilder source, string entityTypeName, string? primaryKeyTypeName, string implementationTypeName)
    {
        if (primaryKeyTypeName is not null)
        {
            AppendTransient(source, $"global::Skywalker.Ddd.Domain.Services.IDomainService<{entityTypeName}, {primaryKeyTypeName}>", implementationTypeName);
        }

        AppendTransient(source, $"global::Skywalker.Ddd.Domain.Services.IDomainService<{entityTypeName}>", implementationTypeName);
    }

    private static void AppendTransient(StringBuilder source, string serviceTypeName, string implementationTypeName)
    {
        source.AppendLine("        global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAdd(");
        source.AppendLine("            services,");
        source.Append("            global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Transient(typeof(")
            .Append(serviceTypeName)
            .Append("), typeof(")
            .Append(implementationTypeName)
            .AppendLine(")));");
    }

    private static KeyResolutionResult ResolvePrimaryKeyType(INamedTypeSymbol entityType)
    {
        var keyTypes = ImmutableArray.CreateBuilder<ITypeSymbol>();
        foreach (var interfaceType in entityType.AllInterfaces)
        {
            if (!HasMetadataName(interfaceType.ConstructedFrom, EntityWithKeyMetadataName)
                || interfaceType.TypeArguments.Length != 1)
            {
                continue;
            }

            var keyType = interfaceType.TypeArguments[0];
            if (!keyTypes.Any(existing => SymbolEqualityComparer.Default.Equals(existing, keyType)))
            {
                keyTypes.Add(keyType);
            }
        }

        return keyTypes.Count switch
        {
            0 => new KeyResolutionResult(null, null),
            1 => new KeyResolutionResult(keyTypes[0], null),
            _ => new KeyResolutionResult(null, string.Join(", ", keyTypes.Select(FormatType)))
        };
    }

    private static bool TryGetDbSetEntityType(IPropertySymbol property, out INamedTypeSymbol entityType)
    {
        entityType = null!;
        if (property.Type is not INamedTypeSymbol propertyType
            || !HasMetadataName(propertyType.ConstructedFrom, DbSetMetadataName)
            || propertyType.TypeArguments.Length != 1
            || propertyType.TypeArguments[0] is not INamedTypeSymbol dbSetEntityType)
        {
            return false;
        }

        entityType = dbSetEntityType;
        return true;
    }

    private static bool IsAccessibleToGeneratedCode(INamedTypeSymbol type)
    {
        for (INamedTypeSymbol? current = type; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal))
            {
                return false;
            }
        }

        return true;
    }

    private static string FormatType(ITypeSymbol type)
        => type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

    private static bool Implements(INamedTypeSymbol type, string metadataName)
        => type.AllInterfaces.Any(interfaceType => HasMetadataName(interfaceType.ConstructedFrom, metadataName));

    private static bool InheritsFrom(INamedTypeSymbol type, string metadataName)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (HasMetadataName(current.ConstructedFrom, metadataName))
            {
                return true;
            }
        }

        return false;
    }

    private static string CreateIdentifier(INamedTypeSymbol symbol)
        => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", string.Empty)
            .Replace('.', '_')
            .Replace('+', '_')
            .Replace('<', '_')
            .Replace('>', '_')
            .Replace(',', '_')
            .Replace(' ', '_');

    private static bool HasMetadataName(INamedTypeSymbol symbol, string metadataName)
    {
        var lastDotIndex = metadataName.LastIndexOf('.');
        var expectedNamespace = lastDotIndex < 0 ? string.Empty : metadataName.Substring(0, lastDotIndex);
        var expectedMetadataName = lastDotIndex < 0 ? metadataName : metadataName.Substring(lastDotIndex + 1);
        return symbol.MetadataName == expectedMetadataName
            && symbol.ContainingNamespace.ToDisplayString() == expectedNamespace;
    }
}

internal readonly struct DbContextRegistrationModel : IEquatable<DbContextRegistrationModel>
{
    public DbContextRegistrationModel(
        string dbContextTypeName,
        string identifier,
        EquatableArray<EntityRegistrationModel> entities,
        EquatableArray<DiagnosticModel> diagnostics)
    {
        DbContextTypeName = dbContextTypeName;
        Identifier = identifier;
        Entities = entities;
        Diagnostics = diagnostics;
    }

    public string DbContextTypeName { get; }

    public string Identifier { get; }

    public EquatableArray<EntityRegistrationModel> Entities { get; }

    public EquatableArray<DiagnosticModel> Diagnostics { get; }

    public bool Equals(DbContextRegistrationModel other)
        => DbContextTypeName == other.DbContextTypeName
            && Identifier == other.Identifier
            && Entities.Equals(other.Entities)
            && Diagnostics.Equals(other.Diagnostics);

    public override bool Equals(object? obj) => obj is DbContextRegistrationModel other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 31) + DbContextTypeName.GetHashCode();
            hash = (hash * 31) + Identifier.GetHashCode();
            hash = (hash * 31) + Entities.GetHashCode();
            hash = (hash * 31) + Diagnostics.GetHashCode();
            return hash;
        }
    }
}

internal readonly struct DiagnosticModel : IEquatable<DiagnosticModel>
{
    private DiagnosticModel(DiagnosticDescriptor descriptor, Location? location, EquatableArray<string> messageArgs)
    {
        Descriptor = descriptor;
        Location = location;
        MessageArgs = messageArgs;
    }

    public DiagnosticDescriptor Descriptor { get; }

    public Location? Location { get; }

    public EquatableArray<string> MessageArgs { get; }

    public static DiagnosticModel Create(DiagnosticDescriptor descriptor, Location? location, params string[] messageArgs)
        => new(descriptor, location, new EquatableArray<string>(messageArgs));

    public Diagnostic Create()
        => Diagnostic.Create(Descriptor, Location, MessageArgs.AsImmutableArray().Cast<object>().ToArray());

    public bool Equals(DiagnosticModel other)
        => Descriptor.Id == other.Descriptor.Id
            && MessageArgs.Equals(other.MessageArgs);

    public override bool Equals(object? obj) => obj is DiagnosticModel other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 31) + Descriptor.Id.GetHashCode();
            hash = (hash * 31) + MessageArgs.GetHashCode();
            return hash;
        }
    }
}

internal readonly struct KeyResolutionResult
{
    public KeyResolutionResult(ITypeSymbol? primaryKeyType, string? conflictingKeyTypes)
    {
        PrimaryKeyType = primaryKeyType;
        ConflictingKeyTypes = conflictingKeyTypes;
    }

    public ITypeSymbol? PrimaryKeyType { get; }

    public string? ConflictingKeyTypes { get; }
}

internal readonly struct EntityRegistrationModel : IEquatable<EntityRegistrationModel>
{
    public EntityRegistrationModel(string entityTypeName, string? primaryKeyTypeName)
    {
        EntityTypeName = entityTypeName;
        PrimaryKeyTypeName = primaryKeyTypeName;
    }

    public string EntityTypeName { get; }

    public string? PrimaryKeyTypeName { get; }

    public bool Equals(EntityRegistrationModel other)
        => EntityTypeName == other.EntityTypeName
            && PrimaryKeyTypeName == other.PrimaryKeyTypeName;

    public override bool Equals(object? obj) => obj is EntityRegistrationModel other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 31) + EntityTypeName.GetHashCode();
            hash = (hash * 31) + (PrimaryKeyTypeName?.GetHashCode() ?? 0);
            return hash;
        }
    }
}
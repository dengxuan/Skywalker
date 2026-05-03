using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.Ddd.Abstractions.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class DependencyInjectionRegistrationGenerator : IIncrementalGenerator
{
    internal const string Category = "Skywalker.Ddd.Abstractions.SourceGenerators";

    internal static readonly DiagnosticDescriptor ServiceTypeMustBeAssignable = new(
        id: "SKY1002",
        title: "Service type must be assignable from implementation type",
        messageFormat: "Service type '{0}' must be assignable from implementation type '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY1002.md");

    private static readonly ImmutableArray<string> AttributeMetadataNames = ImmutableArray.Create(
        "Skywalker.DependencyInjection.ServiceAttribute",
        "Skywalker.DependencyInjection.ApplicationServiceAttribute",
        "Skywalker.DependencyInjection.RepositoryAttribute",
        "Skywalker.DependencyInjection.EventHandlerAttribute");

    private static readonly ImmutableArray<string> ExcludedInterfaceMetadataNames = ImmutableArray.Create(
        "System.IDisposable",
        "System.IAsyncDisposable");

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource("SkywalkerDependencyInjectionAttributes.g.cs", GenerateAttributes());
        });

        var candidates = AttributeMetadataNames
            .Select(attributeName => context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: attributeName,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (context, _) => CreateModel(context)))
            .ToArray();

        var registrations = candidates[0]
            .Collect()
            .Combine(candidates[1].Collect())
            .Combine(candidates[2].Collect())
            .Combine(candidates[3].Collect())
            .Select(static (source, _) => source.Left.Left.Left
                .AddRange(source.Left.Left.Right)
                .AddRange(source.Left.Right)
                .AddRange(source.Right)
                .Distinct()
                .OrderBy(static model => model.ImplementationTypeName, StringComparer.Ordinal)
                .ToImmutableArray());

        context.RegisterSourceOutput(registrations, static (context, models) =>
        {
            foreach (var diagnostic in models.Select(static model => model.Diagnostic).Where(static diagnostic => diagnostic is not null))
            {
                context.ReportDiagnostic(diagnostic!);
            }

            if (models.Length > 0)
            {
                var source = GenerateRegistrar(models);
                if (!string.IsNullOrEmpty(source))
                {
                    context.AddSource("Skywalker.Generated.DependencyInjectionRegistrar.g.cs", source);
                }
            }
        });
    }

    private static ServiceRegistrationModel CreateModel(GeneratorAttributeSyntaxContext context)
    {
        var type = (INamedTypeSymbol)context.TargetSymbol;
        var attribute = context.Attributes[0];
        var implementationTypeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var serviceType = GetExplicitServiceType(attribute);
        var lifetime = GetLifetime(attribute);

        if (serviceType is not null)
        {
            if (!IsAssignableTo(type, serviceType))
            {
                var diagnostic = Diagnostic.Create(
                    ServiceTypeMustBeAssignable,
                    Location.None,
                    serviceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    implementationTypeName);

                return new ServiceRegistrationModel(
                    implementationTypeName,
                    lifetime,
                    serviceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    diagnostic);
            }

            return new ServiceRegistrationModel(
                implementationTypeName,
                lifetime,
                serviceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                null);
        }

        var serviceTypeNames = type.Interfaces
            .Where(static candidate => !IsExcludedInterface(candidate))
            .Select(static candidate => candidate.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Distinct()
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        if (serviceTypeNames.Length == 0)
        {
            serviceTypeNames = new[] { implementationTypeName };
        }

        return new ServiceRegistrationModel(
            implementationTypeName,
            lifetime,
            string.Join("|", serviceTypeNames),
            null);
    }

    private static INamedTypeSymbol? GetExplicitServiceType(AttributeData attribute)
    {
        foreach (var argument in attribute.NamedArguments)
        {
            if (argument.Key == "ServiceType" && argument.Value.Value is INamedTypeSymbol serviceType)
            {
                return serviceType;
            }
        }

        return null;
    }

    private static string GetLifetime(AttributeData attribute)
    {
        var attributeName = attribute.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return attributeName == "global::Skywalker.DependencyInjection.EventHandlerAttribute"
            ? "Transient"
            : "Scoped";
    }

    private static bool IsAssignableTo(INamedTypeSymbol implementationType, INamedTypeSymbol serviceType)
    {
        if (SymbolEqualityComparer.Default.Equals(implementationType, serviceType))
        {
            return true;
        }

        foreach (var candidate in implementationType.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(candidate, serviceType))
            {
                return true;
            }
        }

        var baseType = implementationType.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(baseType, serviceType))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    private static bool IsExcludedInterface(INamedTypeSymbol candidate)
    {
        var metadataName = candidate.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        return ExcludedInterfaceMetadataNames.Contains(metadataName, StringComparer.Ordinal);
    }

    private static string GenerateAttributes()
    {
        return """
            // <auto-generated/>
            // This file was generated by Skywalker Source Generators.
            // Do not modify this file manually.

            #nullable enable
            #pragma warning disable

            namespace Skywalker.DependencyInjection;

            [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
            internal sealed class ServiceAttribute : global::System.Attribute
            {
                public global::System.Type? ServiceType { get; set; }
            }

            [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
            internal sealed class ApplicationServiceAttribute : global::System.Attribute
            {
                public global::System.Type? ServiceType { get; set; }
            }

            [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
            internal sealed class RepositoryAttribute : global::System.Attribute
            {
                public global::System.Type? ServiceType { get; set; }
            }

            [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
            internal sealed class EventHandlerAttribute : global::System.Attribute
            {
                public global::System.Type? ServiceType { get; set; }
            }
            """;
    }

    private static string GenerateRegistrar(ImmutableArray<ServiceRegistrationModel> models)
    {
        var validModels = models
            .Where(static model => model.Diagnostic is null)
            .ToArray();

        if (validModels.Length == 0)
        {
            return string.Empty;
        }

        var registrations = string.Join(
            "\n",
            validModels.SelectMany(static model => model.ServiceTypeNames
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(serviceTypeName => $$"""
                        global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAdd(
                            services,
                            global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Describe(
                                typeof({{serviceTypeName}}),
                                typeof({{model.ImplementationTypeName}}),
                                global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.{{model.Lifetime}}));
                    """)));

        return $$"""
            // <auto-generated/>
            // This file was generated by Skywalker Source Generators.
            // Do not modify this file manually.

            #nullable enable
            #pragma warning disable

            [assembly: global::Skywalker.DependencyInjection.SkywalkerGeneratedDependencyInjectionRegistrationAttribute(typeof(global::Skywalker.Generated.__SkywalkerDependencyInjectionRegistrar), nameof(global::Skywalker.Generated.__SkywalkerDependencyInjectionRegistrar.AddSkywalkerGeneratedServices))]

            namespace Skywalker.Generated;

            internal static class __SkywalkerDependencyInjectionRegistrar
            {
                public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddSkywalkerGeneratedServices(global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)
                {
            {{registrations}}
                    return services;
                }
            }
            """;
    }

    private readonly record struct ServiceRegistrationModel(
        string ImplementationTypeName,
        string Lifetime,
        string ServiceTypeNames,
        Diagnostic? Diagnostic);
}
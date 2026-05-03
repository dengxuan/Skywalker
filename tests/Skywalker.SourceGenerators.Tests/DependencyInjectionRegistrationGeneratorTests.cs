using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Abstractions.SourceGenerators;

namespace Skywalker.SourceGenerators.Tests;

public sealed class DependencyInjectionRegistrationGeneratorTests
{
    private static readonly MetadataReference[] DependencyInjectionReferences =
    [
        MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
        CreateRuntimeReference("System.ComponentModel.dll"),
    ];

    private static MetadataReference CreateRuntimeReference(string assemblyFileName)
    {
        var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        return MetadataReference.CreateFromFile(Path.Combine(runtimeDirectory, assemblyFileName));
    }

    [Fact]
    public void ProducesOnlyAttributeSource_ForNoCandidateServices()
    {
        var result = GeneratorTestHelper.Run<DependencyInjectionRegistrationGenerator>("""
            namespace Demo;

            public sealed class PlainService
            {
            }
            """);

        var generatedTree = Assert.Single(result.GeneratedTrees);
        Assert.EndsWith("SkywalkerDependencyInjectionAttributes.g.cs", generatedTree.FilePath);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void GeneratesRegistrarSkeleton_ForAttributedService()
    {
        var (result, compilation) = GeneratorTestHelper.RunWithCompilation("""
            using Skywalker.DependencyInjection;

            namespace Demo;

            [Service]
            public sealed class OrderService
            {
            }
            """,
            new DependencyInjectionRegistrationGenerator(),
            DependencyInjectionReferences);

        Assert.Equal(2, result.GeneratedTrees.Length);

        var registrarTree = Assert.Single(result.GeneratedTrees, static tree => tree.FilePath.EndsWith("Skywalker.Generated.DependencyInjectionRegistrar.g.cs"));
        var registrarSource = registrarTree.GetText().ToString();

        Assert.Contains("__SkywalkerDependencyInjectionRegistrar", registrarSource);
        Assert.Contains("Register(global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)", registrarSource);
        Assert.Contains("typeof(global::Demo.OrderService)", registrarSource);
        Assert.Contains("ServiceLifetime.Scoped", registrarSource);
        Assert.Contains("ServiceCollectionDescriptorExtensions.TryAdd", registrarSource);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(compilation.GetDiagnostics().Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void GeneratesScopedRegistration_ForDiscoveredInterface()
    {
        var (result, compilation) = GeneratorTestHelper.RunWithCompilation("""
            using Skywalker.DependencyInjection;

            namespace Demo;

            public interface IOrderService
            {
            }

            [ApplicationService]
            public sealed class OrderService : IOrderService
            {
            }
            """,
            new DependencyInjectionRegistrationGenerator(),
            DependencyInjectionReferences);

        var registrarSource = Assert.Single(result.GeneratedTrees, static tree => tree.FilePath.EndsWith("Skywalker.Generated.DependencyInjectionRegistrar.g.cs"))
            .GetText()
            .ToString();

        Assert.Contains("typeof(global::Demo.IOrderService)", registrarSource);
        Assert.Contains("typeof(global::Demo.OrderService)", registrarSource);
        Assert.Contains("ServiceLifetime.Scoped", registrarSource);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(compilation.GetDiagnostics().Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void GeneratesTransientRegistration_ForEventHandler()
    {
        var (result, compilation) = GeneratorTestHelper.RunWithCompilation("""
            using Skywalker.DependencyInjection;

            namespace Demo;

            public interface IOrderPlacedHandler
            {
            }

            [EventHandler]
            public sealed class OrderPlacedHandler : IOrderPlacedHandler
            {
            }
            """,
            new DependencyInjectionRegistrationGenerator(),
            DependencyInjectionReferences);

        var registrarSource = Assert.Single(result.GeneratedTrees, static tree => tree.FilePath.EndsWith("Skywalker.Generated.DependencyInjectionRegistrar.g.cs"))
            .GetText()
            .ToString();

        Assert.Contains("typeof(global::Demo.IOrderPlacedHandler)", registrarSource);
        Assert.Contains("ServiceLifetime.Transient", registrarSource);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(compilation.GetDiagnostics().Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void GeneratesExplicitServiceTypeRegistration()
    {
        var (result, compilation) = GeneratorTestHelper.RunWithCompilation("""
            using Skywalker.DependencyInjection;

            namespace Demo;

            public interface IOrderService
            {
            }

            [Service(ServiceType = typeof(IOrderService))]
            public sealed class OrderService : IOrderService
            {
            }
            """,
            new DependencyInjectionRegistrationGenerator(),
            DependencyInjectionReferences);

        var registrarSource = Assert.Single(result.GeneratedTrees, static tree => tree.FilePath.EndsWith("Skywalker.Generated.DependencyInjectionRegistrar.g.cs"))
            .GetText()
            .ToString();

        Assert.Contains("typeof(global::Demo.IOrderService)", registrarSource);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(compilation.GetDiagnostics().Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void ReportsDiagnostic_ForUnassignableExplicitServiceType()
    {
        var (result, compilation) = GeneratorTestHelper.RunWithCompilation("""
            using Skywalker.DependencyInjection;

            namespace Demo;

            public interface IOrderService
            {
            }

            [Service(ServiceType = typeof(IOrderService))]
            public sealed class PlainService
            {
            }
            """,
            new DependencyInjectionRegistrationGenerator(),
            DependencyInjectionReferences);

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("SKY1002", diagnostic.Id);
        Assert.DoesNotContain(result.GeneratedTrees, static tree => tree.FilePath.EndsWith("Skywalker.Generated.DependencyInjectionRegistrar.g.cs"));
        Assert.Empty(compilation.GetDiagnostics().Where(static item => item.Severity == DiagnosticSeverity.Error && item.Id != "CS8019"));
    }

    [Fact]
    public void GeneratesValidRegistrations_WhenAnotherServiceReportsDiagnostic()
    {
        var (result, compilation) = GeneratorTestHelper.RunWithCompilation("""
            using Skywalker.DependencyInjection;

            namespace Demo;

            public interface IOrderService
            {
            }

            public interface ICustomerService
            {
            }

            [Service(ServiceType = typeof(IOrderService))]
            public sealed class OrderService : IOrderService
            {
            }

            [Service(ServiceType = typeof(ICustomerService))]
            public sealed class PlainService
            {
            }
            """,
            new DependencyInjectionRegistrationGenerator(),
            DependencyInjectionReferences);

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("SKY1002", diagnostic.Id);

        var registrarSource = Assert.Single(result.GeneratedTrees, static tree => tree.FilePath.EndsWith("Skywalker.Generated.DependencyInjectionRegistrar.g.cs"))
            .GetText()
            .ToString();

        Assert.Contains("typeof(global::Demo.IOrderService)", registrarSource);
        Assert.DoesNotContain("typeof(global::Demo.ICustomerService)", registrarSource);
        Assert.Empty(compilation.GetDiagnostics().Where(static item => item.Severity == DiagnosticSeverity.Error && item.Id != "CS8019"));
    }
}
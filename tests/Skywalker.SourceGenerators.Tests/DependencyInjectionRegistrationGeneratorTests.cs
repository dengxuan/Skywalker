using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Abstractions.SourceGenerators;

namespace Skywalker.SourceGenerators.Tests;

public sealed class DependencyInjectionRegistrationGeneratorTests
{
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
            [MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location)]);

        Assert.Equal(2, result.GeneratedTrees.Length);

        var registrarTree = Assert.Single(result.GeneratedTrees, static tree => tree.FilePath.EndsWith("Skywalker.Generated.DependencyInjectionRegistrar.g.cs"));
        var registrarSource = registrarTree.GetText().ToString();

        Assert.Contains("__SkywalkerDependencyInjectionRegistrar", registrarSource);
        Assert.Contains("Register(global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)", registrarSource);
        Assert.Contains("TODO #280: emit descriptor for global::Demo.OrderService", registrarSource);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(compilation.GetDiagnostics().Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }
}
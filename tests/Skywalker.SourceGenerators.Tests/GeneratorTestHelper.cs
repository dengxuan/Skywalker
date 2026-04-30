using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Skywalker.SourceGenerators.Tests;

public static class GeneratorTestHelper
{
    private static readonly CSharpParseOptions ParseOptions = CSharpParseOptions.Default
        .WithLanguageVersion(LanguageVersion.Latest);

    public static GeneratorDriverRunResult Run<TGenerator>(
        string source,
        IEnumerable<MetadataReference>? references = null,
        CSharpCompilationOptions? compilationOptions = null)
        where TGenerator : IIncrementalGenerator, new()
    {
        return Run(source, new TGenerator(), references, compilationOptions);
    }

    public static GeneratorDriverRunResult Run(
        string source,
        IIncrementalGenerator generator,
        IEnumerable<MetadataReference>? references = null,
        CSharpCompilationOptions? compilationOptions = null)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, ParseOptions);
        var compilation = CSharpCompilation.Create(
            assemblyName: "Skywalker.SourceGenerator.Tests.Target",
            syntaxTrees: [syntaxTree],
            references: CreateReferences(references),
            options: compilationOptions ?? CreateCompilationOptions());

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        return driver.GetRunResult();
    }

    public static ImmutableArray<Diagnostic> GetCompilationDiagnostics(
        string source,
        IEnumerable<MetadataReference>? references = null,
        CSharpCompilationOptions? compilationOptions = null)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, ParseOptions);
        var compilation = CSharpCompilation.Create(
            assemblyName: "Skywalker.SourceGenerator.Tests.Target",
            syntaxTrees: [syntaxTree],
            references: CreateReferences(references),
            options: compilationOptions ?? CreateCompilationOptions());

        return compilation.GetDiagnostics();
    }

    private static CSharpCompilationOptions CreateCompilationOptions()
    {
        return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithNullableContextOptions(NullableContextOptions.Enable)
            .WithOptimizationLevel(OptimizationLevel.Release);
    }

    private static ImmutableArray<MetadataReference> CreateReferences(IEnumerable<MetadataReference>? references)
    {
        var builder = ImmutableArray.CreateBuilder<MetadataReference>();
        builder.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        builder.Add(MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location));
        builder.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));

        var systemRuntime = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == "System.Runtime");
        if (systemRuntime is not null)
        {
            builder.Add(MetadataReference.CreateFromFile(systemRuntime.Location));
        }

        if (references is not null)
        {
            builder.AddRange(references);
        }

        return builder.ToImmutable();
    }
}
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Skywalker.Template.SourceGenerators.Tests;

internal static class GeneratorTestHelper
{
    private static readonly CSharpParseOptions ParseOptions = CSharpParseOptions.Default
        .WithLanguageVersion(LanguageVersion.Latest);

    public static GeneratorDriverRunResult Run(string source, IIncrementalGenerator generator)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, ParseOptions);
        var compilation = CSharpCompilation.Create(
            assemblyName: "Skywalker.Template.SourceGenerators.Tests.Target",
            syntaxTrees: [syntaxTree],
            references: CreateReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable)
                .WithOptimizationLevel(OptimizationLevel.Release));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        return driver.GetRunResult();
    }

    private static ImmutableArray<MetadataReference> CreateReferences()
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

        return builder.ToImmutable();
    }
}
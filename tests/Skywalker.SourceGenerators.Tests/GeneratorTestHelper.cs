using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Skywalker.SourceGenerators.Tests;

/// <summary>
/// One-shot driver for incremental generators in unit tests.
/// </summary>
/// <remarks>
/// All Skywalker SG test projects should depend on <c>Skywalker.SourceGenerators.Tests</c>
/// and use <see cref="Run{TGenerator}"/> / <see cref="RunDriver{TGenerator}"/> rather than
/// re-rolling their own driver setup. This keeps reference assemblies, language version,
/// and nullable settings consistent across the whole test suite.
/// </remarks>
public static class GeneratorTestHelper
{
    private static readonly CSharpParseOptions ParseOptions =
        new(LanguageVersion.Latest);

    private static readonly CSharpCompilationOptions CompilationOptions =
        new(OutputKind.DynamicallyLinkedLibrary,
            nullableContextOptions: NullableContextOptions.Enable);

    /// <summary>
    /// Runs <typeparamref name="TGenerator"/> against <paramref name="source"/> and returns
    /// the run result for assertion. The generated trees are reachable via
    /// <see cref="GeneratorDriverRunResult.GeneratedTrees"/> and diagnostics via
    /// <see cref="GeneratorDriverRunResult.Diagnostics"/>.
    /// </summary>
    public static GeneratorDriverRunResult Run<TGenerator>(
        string source,
        IEnumerable<MetadataReference>? additionalReferences = null)
        where TGenerator : IIncrementalGenerator, new()
        => RunDriver<TGenerator>(source, additionalReferences).GetRunResult();

    /// <summary>
    /// Runs <typeparamref name="TGenerator"/> and returns the post-run driver — the form
    /// <c>Verify(driver)</c> expects for snapshot assertions.
    /// </summary>
    public static GeneratorDriver RunDriver<TGenerator>(
        string source,
        IEnumerable<MetadataReference>? additionalReferences = null)
        where TGenerator : IIncrementalGenerator, new()
    {
        var compilation = CreateCompilation(source, additionalReferences);
        var driver = CSharpGeneratorDriver.Create(new TGenerator());
        return driver.RunGenerators(compilation);
    }

    /// <summary>
    /// Same as <see cref="Run{TGenerator}"/> but also returns the post-generation
    /// <see cref="Compilation"/> so callers can assert the produced code actually compiles.
    /// </summary>
    public static (GeneratorDriverRunResult Result, Compilation OutputCompilation) RunWithCompilation<TGenerator>(
        string source,
        IEnumerable<MetadataReference>? additionalReferences = null)
        where TGenerator : IIncrementalGenerator, new()
    {
        var compilation = CreateCompilation(source, additionalReferences);
        var driver = CSharpGeneratorDriver.Create(new TGenerator());
        var afterRun = driver.RunGeneratorsAndUpdateCompilation(
            compilation, out var output, out _);
        return (afterRun.GetRunResult(), output);
    }

    private static CSharpCompilation CreateCompilation(
        string source,
        IEnumerable<MetadataReference>? additionalReferences)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, ParseOptions);

        var refs = ImmutableArray.CreateBuilder<MetadataReference>();
        refs.AddRange(DefaultReferences.Value);
        if (additionalReferences is not null)
        {
            refs.AddRange(additionalReferences);
        }

        return CSharpCompilation.Create(
            assemblyName: "Skywalker.SourceGenerators.TestAssembly",
            syntaxTrees: [syntaxTree],
            references: refs.ToImmutable(),
            options: CompilationOptions);
    }

    /// <summary>
    /// .NET 8 runtime references + a small set of NuGet references that virtually every
    /// Skywalker SG needs to resolve target types (DI abstractions to start). Lazy-loaded
    /// once per process. We resolve runtime assemblies by scanning the directory of
    /// <c>typeof(object).Assembly.Location</c> rather than depending on
    /// <c>Basic.Reference.Assemblies.Net80</c>, which would force the project's Roslyn
    /// version to track its own bumps.
    /// </summary>
    private static readonly Lazy<ImmutableArray<MetadataReference>> DefaultReferences = new(
        valueFactory: LoadDefaultReferences,
        isThreadSafe: true);

    private static ImmutableArray<MetadataReference> LoadDefaultReferences()
    {
        var coreLibPath = typeof(object).Assembly.Location;
        var coreLibDir = System.IO.Path.GetDirectoryName(coreLibPath)!;

        // Hand-picked runtime assemblies covering nearly every realistic SG input.
        // Add to this list (don't remove) when a generator needs more.
        string[] runtimeNames =
        [
            "System.Runtime",
            "System.Private.CoreLib",
            "netstandard",
            "System.Collections",
            "System.Collections.Immutable",
            "System.ComponentModel",
            "System.Console",
            "System.Linq",
            "System.Linq.Expressions",
            "System.ObjectModel",
            "System.Runtime.Extensions",
            "System.Text.Encoding",
            "System.Text.Encoding.Extensions",
            "System.Threading",
            "System.Threading.Tasks",
            "mscorlib",
        ];

        var refs = ImmutableArray.CreateBuilder<MetadataReference>();
        refs.Add(MetadataReference.CreateFromFile(coreLibPath));
        foreach (var name in runtimeNames)
        {
            var path = System.IO.Path.Combine(coreLibDir, name + ".dll");
            if (System.IO.File.Exists(path))
            {
                refs.Add(MetadataReference.CreateFromFile(path));
            }
        }

        // DI abstractions — Skywalker SGs target IServiceCollection registration.
        refs.Add(MetadataReference.CreateFromFile(
            typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection).Assembly.Location));

        return refs.ToImmutable();
    }
}

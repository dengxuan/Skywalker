using Skywalker.SourceGenerators.Tests.Infrastructure;

namespace Skywalker.SourceGenerators.Tests;

public sealed class GeneratorTestHelperTests
{
    [Fact]
    public void Run_WithEmptyGenerator_ReturnsSingleGeneratorResult()
    {
        var result = GeneratorTestHelper.Run<EmptyIncrementalGenerator>("""
            namespace Demo;

            public sealed class Order
            {
            }
            """);

        Assert.Single(result.Results);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(result.GeneratedTrees);
    }

    [Fact]
    public void Run_WithPostInitializationOutput_ReturnsGeneratedTree()
    {
        var result = GeneratorTestHelper.Run<HelloWorldIncrementalGenerator>("""
            namespace Demo;

            public sealed class Order
            {
            }
            """);

        var generatedTree = Assert.Single(result.GeneratedTrees);
        Assert.EndsWith("HelloWorld.g.cs", generatedTree.FilePath);
        Assert.Contains("Hello from source generators", generatedTree.GetText().ToString());
    }
}
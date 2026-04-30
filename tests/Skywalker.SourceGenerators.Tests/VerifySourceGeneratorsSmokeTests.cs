using Skywalker.SourceGenerators.Tests.Infrastructure;

namespace Skywalker.SourceGenerators.Tests;

public sealed class VerifySourceGeneratorsSmokeTests
{
    [Fact]
    public Task Verify_GeneratedOutput()
    {
        var result = GeneratorTestHelper.Run<HelloWorldIncrementalGenerator>("""
            namespace Demo;

            public sealed class Order
            {
            }
            """);

        var generatedTree = Assert.Single(result.GeneratedTrees);
        return Verifier.Verify(generatedTree.GetText().ToString());
    }
}
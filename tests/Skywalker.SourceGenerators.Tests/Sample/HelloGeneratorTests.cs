using VerifyXunit;
using Xunit;

namespace Skywalker.SourceGenerators.Tests.Sample;

/// <summary>
/// End-to-end sanity checks for <see cref="GeneratorTestHelper"/> and <see cref="VerifySettings"/>.
/// If these go red the whole SG test infrastructure is broken and downstream SG tests will
/// fail too — keep these passing as a smoke test.
/// </summary>
[UsesVerify]
public class HelloGeneratorTests
{
    [Fact]
    public Task PartialClass_GeneratesHelloMethod()
    {
        const string source = """
            using Skywalker.SourceGenerators.Tests.Sample;

            namespace Demo;

            [Hello]
            public partial class Greeter
            {
            }
            """;

        var driver = GeneratorTestHelper.RunDriver<HelloGenerator>(
            source,
            additionalReferences: [
                Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(
                    typeof(HelloAttribute).Assembly.Location),
            ]);

        return Verifier.Verify(driver);
    }

    [Fact]
    public void NonPartialClass_ReportsTest0001()
    {
        const string source = """
            using Skywalker.SourceGenerators.Tests.Sample;

            namespace Demo;

            [Hello]
            public class Greeter
            {
            }
            """;

        var result = GeneratorTestHelper.Run<HelloGenerator>(
            source,
            additionalReferences: [
                Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(
                    typeof(HelloAttribute).Assembly.Location),
            ]);

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("TEST0001", diagnostic.Id);
        Assert.Contains("Greeter", diagnostic.GetMessage());
        Assert.Empty(result.GeneratedTrees);
    }

    [Fact]
    public void NoAttribute_GeneratesNothing()
    {
        const string source = """
            namespace Demo;

            public partial class Plain
            {
            }
            """;

        var result = GeneratorTestHelper.Run<HelloGenerator>(source);

        Assert.Empty(result.Diagnostics);
        Assert.Empty(result.GeneratedTrees);
    }
}

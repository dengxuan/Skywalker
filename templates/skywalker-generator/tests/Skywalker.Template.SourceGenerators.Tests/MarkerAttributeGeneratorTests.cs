namespace Skywalker.Template.SourceGenerators.Tests;

public sealed class MarkerAttributeGeneratorTests
{
    [Fact]
    public void Generates_Marker_For_Attributed_Class()
    {
        var result = GeneratorTestHelper.Run("""
            namespace Demo;

            [Skywalker.Template.SkywalkerGeneratorMarker]
            public sealed partial class OrderService
            {
            }
            """, new MarkerAttributeGenerator());

        var generatedSource = result.GeneratedTrees
            .Select(tree => tree.GetText().ToString())
            .Single(source => source.Contains("OrderServiceSkywalkerMarker", StringComparison.Ordinal));

        Assert.Contains("global::Demo.OrderService", generatedSource);
    }
}
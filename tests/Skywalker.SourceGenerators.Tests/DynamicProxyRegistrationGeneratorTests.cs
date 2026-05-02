using Microsoft.CodeAnalysis;
using Skywalker.Extensions.DynamicProxies;
using Skywalker.Extensions.DynamicProxies.SourceGenerators;

namespace Skywalker.SourceGenerators.Tests;

public sealed class DynamicProxyRegistrationGeneratorTests
{
    [Fact]
    public void ProducesNoSources_ForNoCandidateServices()
    {
        var result = GeneratorTestHelper.Run<DynamicProxyRegistrationGenerator>("""
            namespace Demo;

            public sealed class PlainService
            {
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void GeneratesProxy_ForInterceptableInterfaceService()
    {
        var (result, compilation) = GeneratorTestHelper.RunWithCompilation("""
            using System.Threading.Tasks;
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                void Touch(int id);
                int Count();
                Task SubmitAsync();
                Task<string> GetNameAsync(int id);
                ValueTask FlushAsync();
                ValueTask<int> GetCountAsync();
            }

            public sealed class OrderService : IOrderService
            {
                public void Touch(int id) { }
                public int Count() => 42;
                public Task SubmitAsync() => Task.CompletedTask;
                public Task<string> GetNameAsync(int id) => Task.FromResult(id.ToString());
                public ValueTask FlushAsync() => ValueTask.CompletedTask;
                public ValueTask<int> GetCountAsync() => ValueTask.FromResult(42);
            }
            """, new DynamicProxyRegistrationGenerator(), CreateReferences());

        var generatedTree = Assert.Single(result.GeneratedTrees);
        var generatedSource = generatedTree.GetText().ToString();

        Assert.EndsWith("SkywalkerProxy.SkywalkerDynamicProxy.g.cs", generatedTree.FilePath);
        Assert.Contains("internal sealed class", generatedSource);
        Assert.Contains(": global::Demo.IOrderService", generatedSource);
        Assert.Contains("Touch(", generatedSource);
        Assert.Contains("GetNameAsync(", generatedSource);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(compilation.GetDiagnostics().Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void ReportsDiagnostic_ForRefParameter()
    {
        var result = GeneratorTestHelper.Run<DynamicProxyRegistrationGenerator>("""
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                void Touch(ref int id);
            }

            public sealed class OrderService : IOrderService
            {
                public void Touch(ref int id) { }
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("SKY3101", diagnostic.Id);
        Assert.Contains("ref, out, and in parameters are not supported", diagnostic.GetMessage());
    }

    private static IEnumerable<MetadataReference> CreateReferences()
    {
        yield return MetadataReference.CreateFromFile(typeof(IInterceptable).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(Task).Assembly.Location);
    }
}
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Abstractions.SourceGenerators;
using Skywalker.DependencyInjection;

namespace Skywalker.SourceGenerators.Tests;

public sealed class DependencyInjectionRegistrationGeneratorSnapshotTests
{
    private static readonly MetadataReference[] DependencyInjectionReferences =
    [
        MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(SkywalkerGeneratedDependencyInjectionRegistrationAttribute).Assembly.Location),
        CreateRuntimeReference("System.ComponentModel.dll"),
    ];

    public static TheoryData<string, string> SnapshotCases => new()
    {
        { "SelfRegisteredService", SelfRegisteredService() },
        { "DiscoveredInterface", DiscoveredInterface() },
        { "ExplicitServiceType", ExplicitServiceType() },
        { "EventHandlerTransient", EventHandlerTransient() },
        { "MixedDiagnosticAndValidRegistration", MixedDiagnosticAndValidRegistration() },
    };

    [Theory]
    [MemberData(nameof(SnapshotCases))]
    public Task Snapshot_Cases(string name, string source)
    {
        var (result, compilation) = GeneratorTestHelper.RunWithCompilation(source, new DependencyInjectionRegistrationGenerator(), DependencyInjectionReferences);
        var errors = compilation.GetDiagnostics()
            .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error && diagnostic.Id != "CS8019");
        Assert.Empty(errors);

        var builder = new StringBuilder();
        foreach (var diagnostic in result.Diagnostics.OrderBy(static diagnostic => diagnostic.Id).ThenBy(static diagnostic => diagnostic.GetMessage()))
        {
            builder.Append("Diagnostic ")
                .Append(diagnostic.Id)
                .Append(" [")
                .Append(diagnostic.Severity)
                .Append("]: ")
                .AppendLine(diagnostic.GetMessage());
        }

        foreach (var tree in result.GeneratedTrees.OrderBy(static tree => NormalizePath(tree.FilePath), StringComparer.Ordinal))
        {
            builder.Append("// File: ").AppendLine(NormalizePath(tree.FilePath));
            builder.AppendLine(tree.GetText().ToString().TrimEnd());
            builder.AppendLine();
        }

        var settings = new VerifySettings();
        settings.UseParameters(name);
        return Verifier.Verify(builder.ToString(), settings);
    }

    private static MetadataReference CreateRuntimeReference(string assemblyFileName)
    {
        var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        return MetadataReference.CreateFromFile(Path.Combine(runtimeDirectory, assemblyFileName));
    }

    private static string NormalizePath(string path)
        => path.Replace('\\', '/');

    private static string SelfRegisteredService()
        => """
            using Skywalker.DependencyInjection;

            namespace Demo;

            [Service]
            public sealed class ClockService
            {
            }
            """;

    private static string DiscoveredInterface()
        => """
            using Skywalker.DependencyInjection;

            namespace Demo;

            public interface IOrderService
            {
            }

            [ApplicationService]
            public sealed class OrderService : IOrderService
            {
            }
            """;

    private static string ExplicitServiceType()
        => """
            using Skywalker.DependencyInjection;

            namespace Demo;

            public interface IOrderRepository
            {
            }

            [Repository(ServiceType = typeof(IOrderRepository))]
            public sealed class OrderRepository : IOrderRepository
            {
            }
            """;

    private static string EventHandlerTransient()
        => """
            using Skywalker.DependencyInjection;

            namespace Demo;

            public interface IOrderPlacedHandler
            {
            }

            [EventHandler]
            public sealed class OrderPlacedHandler : IOrderPlacedHandler
            {
            }
            """;

    private static string MixedDiagnosticAndValidRegistration()
        => """
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
            """;
}
using System.Text;
using Microsoft.CodeAnalysis;
using Skywalker.Extensions.DynamicProxies;
using Skywalker.Extensions.DynamicProxies.SourceGenerators;

namespace Skywalker.SourceGenerators.Tests;

public sealed class DynamicProxyRegistrationGeneratorSnapshotTests
{
    public static TheoryData<string, string> SnapshotCases => new()
    {
        { "VoidMethod", Service("void Touch()", "public void Touch() { }") },
        { "SyncValueMethod", Service("int Count()", "public int Count() => 42;") },
        { "TaskMethod", Service("Task SubmitAsync()", "public Task SubmitAsync() => Task.CompletedTask;") },
        { "TaskOfTMethod", Service("Task<string> GetNameAsync()", "public Task<string> GetNameAsync() => Task.FromResult(\"ok\");") },
        { "ValueTaskMethod", Service("ValueTask FlushAsync()", "public ValueTask FlushAsync() => ValueTask.CompletedTask;") },
        { "ValueTaskOfTMethod", Service("ValueTask<int> GetCountAsync()", "public ValueTask<int> GetCountAsync() => ValueTask.FromResult(42);") },
        { "SingleIntParameter", Service("void Touch(int id)", "public void Touch(int id) { }") },
        { "MultipleParameters", Service("string Format(int id, string name)", "public string Format(int id, string name) => $\"{id}:{name}\";") },
        { "NullableStringParameter", Service("string? Echo(string? value)", "public string? Echo(string? value) => value;") },
        { "StringArrayParameter", Service("Task<int> CountAsync(string[] values)", "public Task<int> CountAsync(string[] values) => Task.FromResult(values.Length);") },
        { "IntArrayReturn", Service("int[] GetIds()", "public int[] GetIds() => new[] { 1, 2 };") },
        { "ListParameter", Service("int Count(global::System.Collections.Generic.List<string> values)", "public int Count(global::System.Collections.Generic.List<string> values) => values.Count;") },
        { "DictionaryParameter", Service("int Count(global::System.Collections.Generic.Dictionary<string, int> values)", "public int Count(global::System.Collections.Generic.Dictionary<string, int> values) => values.Count;") },
        { "TupleReturn", Service("(int Id, string Name) Get()", "public (int Id, string Name) Get() => (1, \"one\");") },
        { "OverloadByParameterType", Service("string Find(int id); string Find(string name)", "public string Find(int id) => id.ToString(); public string Find(string name) => name;") },
        { "OverloadByParameterCount", Service("string Find(); string Find(int id)", "public string Find() => \"all\"; public string Find(int id) => id.ToString();") },
        { "BaseInterfaceMethod", BaseInterfaceService("void FromBase()", "public void FromBase() { }") },
        { "MultiLevelBaseInterfaceMethod", MultiLevelBaseInterfaceService() },
        { "InternalImplementation", Service("void Touch()", "internal sealed class OrderService : IOrderService { public void Touch() { } }") },
        { "InternalInterface", InternalInterfaceService() },
        { "BlockScopedNamespace", BlockScopedNamespaceService() },
        { "GlobalNamespace", GlobalNamespaceService() },
        { "TwoServiceInterfaces", TwoServiceInterfaces() },
        { "TwoImplementations", TwoImplementations() },
        { "InterceptableBaseInterface", InterceptableBaseInterface() },
        { "NestedImplementation", NestedImplementation() },
        { "NestedInterface", NestedInterface() },
        { "ParameterNamedInvocation", Service("void Touch(int invocation)", "public void Touch(int invocation) { }") },
        { "ParameterNamedTarget", Service("void Touch(int target)", "public void Touch(int target) { }") },
        { "KeywordParameterName", Service("void Touch(int @class)", "public void Touch(int @class) { }") },
        { "KeywordMethodName", Service("void @event()", "public void @event() { }") },
        { "ClosedGenericServiceInterface", ClosedGenericServiceInterface() },
        { "GenericMethodDiagnostic", Service("T Echo<T>(T value)", "public T Echo<T>(T value) => value;") },
        { "RefParameterDiagnostic", Service("void Touch(ref int value)", "public void Touch(ref int value) { }") },
        { "OutParameterDiagnostic", Service("void Touch(out int value)", "public void Touch(out int value) { value = 1; }") },
        { "InParameterDiagnostic", Service("void Touch(in int value)", "public void Touch(in int value) { }") },
        { "MixedSupportedAndUnsupportedDiagnostic", Service("void Touch(); void Update(ref int value)", "public void Touch() { } public void Update(ref int value) { }") },
        { "NonInterceptableProducesNoSources", NonInterceptableService() },
        { "OnlyInterceptableProducesNoSources", OnlyInterceptableService() },
        { "PrivateNestedImplementationProducesNoSources", PrivateNestedImplementation() },
        { "StaticInterfaceMethodProducesNoSources", StaticInterfaceMethod() },
        { "PropertyOnlyInterfaceProducesNoSources", PropertyOnlyInterface() },
        { "EventOnlyInterfaceProducesNoSources", EventOnlyInterface() },
        { "MultipleNamespaces", MultipleNamespaces() },
        { "RecordParameter", RecordParameterService() },
        { "StructParameter", StructParameterService() },
        { "EnumParameter", EnumParameterService() },
        { "NullableValueTypeReturn", Service("int? Get()", "public int? Get() => 1;") },
        { "TaskNullableReferenceReturn", Service("Task<string?> GetAsync()", "public Task<string?> GetAsync() => Task.FromResult<string?>(null);") },
        { "ValueTaskNullableValueReturn", Service("ValueTask<int?> GetAsync()", "public ValueTask<int?> GetAsync() => ValueTask.FromResult<int?>(1);") },
        { "ManyMethods", Service("void A(); int B(); Task C(); Task<string> D(); ValueTask E(); ValueTask<int> F()", "public void A() { } public int B() => 1; public Task C() => Task.CompletedTask; public Task<string> D() => Task.FromResult(\"d\"); public ValueTask E() => ValueTask.CompletedTask; public ValueTask<int> F() => ValueTask.FromResult(6);") },
        { "MethodAttributeIsIgnoredButCompiles", MethodAttributeService() },
    };

    [Theory]
    [MemberData(nameof(SnapshotCases))]
    public Task Snapshot_Cases(string name, string source)
    {
        var (result, compilation) = GeneratorTestHelper.RunWithCompilation(source, new DynamicProxyRegistrationGenerator(), CreateReferences());
        var errors = compilation.GetDiagnostics().Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        Assert.Empty(errors);

        var builder = new StringBuilder();
        foreach (var diagnostic in result.Diagnostics.OrderBy(diagnostic => diagnostic.Id).ThenBy(diagnostic => diagnostic.GetMessage()))
        {
            builder.Append("Diagnostic ")
                .Append(diagnostic.Id)
                .Append(" [")
                .Append(diagnostic.Severity)
                .Append("]: ")
                .AppendLine(diagnostic.GetMessage());
        }

        if (result.GeneratedTrees.Length == 0)
        {
            builder.AppendLine("<no generated sources>");
        }
        else
        {
            foreach (var tree in result.GeneratedTrees.OrderBy(tree => NormalizePath(tree.FilePath), StringComparer.Ordinal))
            {
                builder.Append("// File: ").AppendLine(NormalizePath(tree.FilePath));
                builder.AppendLine(tree.GetText().ToString().TrimEnd());
                builder.AppendLine();
            }
        }

        var settings = new VerifySettings();
        settings.UseParameters(name);
        return Verifier.Verify(builder.ToString(), settings);
    }

    private static string Service(string interfaceMembers, string implementationMembersOrType)
    {
        var implementation = implementationMembersOrType.Contains("OrderService", StringComparison.Ordinal)
            ? implementationMembersOrType
            : $$"""
              public sealed class OrderService : IOrderService
              {
                  {{implementationMembersOrType}}
              }
              """;

        return $$"""
            using System.Threading.Tasks;
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                {{NormalizeInterfaceMembers(interfaceMembers)}}
            }

            {{implementation}}
            """;
    }

    private static string BaseInterfaceService(string baseMembers, string implementationMembers)
        => $$"""
            using System.Threading.Tasks;
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IBaseOrderService
            {
                {{NormalizeInterfaceMembers(baseMembers)}}
            }

            public interface IOrderService : IBaseOrderService, IInterceptable
            {
            }

            public sealed class OrderService : IOrderService
            {
                {{implementationMembers}}
            }
            """;

    private static string MultiLevelBaseInterfaceService()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IRootService
            {
                void Root();
            }

            public interface IChildService : IRootService
            {
                int Child();
            }

            public interface IOrderService : IChildService, IInterceptable
            {
            }

            public sealed class OrderService : IOrderService
            {
                public void Root() { }
                public int Child() => 1;
            }
            """;

    private static string InternalInterfaceService()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            internal interface IOrderService : IInterceptable
            {
                void Touch();
            }

            internal sealed class OrderService : IOrderService
            {
                public void Touch() { }
            }
            """;

    private static string BlockScopedNamespaceService()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo
            {
                public interface IOrderService : IInterceptable
                {
                    void Touch();
                }

                public sealed class OrderService : IOrderService
                {
                    public void Touch() { }
                }
            }
            """;

    private static string GlobalNamespaceService()
        => """
            using Skywalker.Extensions.DynamicProxies;

            public interface IOrderService : IInterceptable
            {
                void Touch();
            }

            public sealed class OrderService : IOrderService
            {
                public void Touch() { }
            }
            """;

    private static string TwoServiceInterfaces()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IReadOrderService : IInterceptable
            {
                int Count();
            }

            public interface IWriteOrderService : IInterceptable
            {
                void Save();
            }

            public sealed class OrderService : IReadOrderService, IWriteOrderService
            {
                public int Count() => 1;
                public void Save() { }
            }
            """;

    private static string TwoImplementations()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                int Count();
            }

            public interface ICustomerService : IInterceptable
            {
                int Count();
            }

            public sealed class OrderService : IOrderService
            {
                public int Count() => 1;
            }

            public sealed class CustomerService : ICustomerService
            {
                public int Count() => 2;
            }
            """;

    private static string InterceptableBaseInterface()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IApplicationService : IInterceptable
            {
            }

            public interface IOrderService : IApplicationService
            {
                void Touch();
            }

            public sealed class OrderService : IOrderService
            {
                public void Touch() { }
            }
            """;

    private static string NestedImplementation()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                void Touch();
            }

            public static class Services
            {
                public sealed class OrderService : IOrderService
                {
                    public void Touch() { }
                }
            }
            """;

    private static string NestedInterface()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public static class Contracts
            {
                public interface IOrderService : IInterceptable
                {
                    void Touch();
                }
            }

            public sealed class OrderService : Contracts.IOrderService
            {
                public void Touch() { }
            }
            """;

    private static string NonInterceptableService()
        => """
            namespace Demo;

            public interface IOrderService
            {
                void Touch();
            }

            public sealed class OrderService : IOrderService
            {
                public void Touch() { }
            }
            """;

    private static string ClosedGenericServiceInterface()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IEntityService<T> : IInterceptable
            {
                T Echo(T value);
            }

            public sealed class StringEntityService : IEntityService<string>
            {
                public string Echo(string value) => value;
            }
            """;

    private static string OnlyInterceptableService()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public sealed class OrderService : IInterceptable
            {
            }
            """;

    private static string PrivateNestedImplementation()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                void Touch();
            }

            public static class Container
            {
                private sealed class OrderService : IOrderService
                {
                    public void Touch() { }
                }
            }
            """;

    private static string StaticInterfaceMethod()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                static string Name => "orders";
            }

            public sealed class OrderService : IOrderService
            {
            }
            """;

    private static string PropertyOnlyInterface()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                string Name { get; }
            }

            public sealed class OrderService : IOrderService
            {
                public string Name => "orders";
            }
            """;

    private static string EventOnlyInterface()
        => """
            using System;
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                event EventHandler? Changed;
            }

            public sealed class OrderService : IOrderService
            {
                public event EventHandler? Changed;
            }
            """;

    private static string MultipleNamespaces()
        => """
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo.Catalog
            {
                public interface IOrderService : IInterceptable
                {
                    void Touch();
                }

                public sealed class OrderService : IOrderService
                {
                    public void Touch() { }
                }
            }

            namespace Demo.Billing
            {
                public interface IInvoiceService : IInterceptable
                {
                    int Count();
                }

                public sealed class InvoiceService : IInvoiceService
                {
                    public int Count() => 1;
                }
            }
            """;

    private static string RecordParameterService()
        => CustomTypeParameterService("public sealed record OrderDto(string Number);", "string Format(OrderDto order)", "public string Format(OrderDto order) => order.Number;");

    private static string StructParameterService()
        => CustomTypeParameterService("public readonly record struct OrderId(int Value);", "int Format(OrderId id)", "public int Format(OrderId id) => id.Value;");

    private static string EnumParameterService()
        => CustomTypeParameterService("public enum OrderStatus { Draft, Submitted }", "OrderStatus Echo(OrderStatus status)", "public OrderStatus Echo(OrderStatus status) => status;");

    private static string CustomTypeParameterService(string typeDeclaration, string interfaceMembers, string implementationMembers)
        => $$"""
            using System.Threading.Tasks;
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            {{typeDeclaration}}

            public interface IOrderService : IInterceptable
            {
                {{NormalizeInterfaceMembers(interfaceMembers)}}
            }

            public sealed class OrderService : IOrderService
            {
                {{implementationMembers}}
            }
            """;

    private static string MethodAttributeService()
        => """
            using System;
            using Skywalker.Extensions.DynamicProxies;

            namespace Demo;

            public interface IOrderService : IInterceptable
            {
                [Obsolete]
                void Touch();
            }

            public sealed class OrderService : IOrderService
            {
                [Obsolete]
                public void Touch() { }
            }
            """;

    private static string NormalizeInterfaceMembers(string members)
    {
        var trimmed = members.TrimEnd();
        return trimmed.EndsWith(";", StringComparison.Ordinal) || trimmed.EndsWith("}", StringComparison.Ordinal)
            ? members
            : members + ";";
    }

    private static string NormalizePath(string path) => path.Replace('\\', '/');

    private static IEnumerable<MetadataReference> CreateReferences()
    {
        yield return MetadataReference.CreateFromFile(typeof(IInterceptable).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(Task).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location);
    }
}
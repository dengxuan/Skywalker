using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Skywalker.SourceGenerators;

namespace Skywalker.Extensions.DynamicProxies.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class DynamicProxyRegistrationGenerator : IIncrementalGenerator
{
    private const string InterceptableMetadataName = "Skywalker.Extensions.DynamicProxies.IInterceptable";
    private const string Category = "Skywalker.Extensions.DynamicProxies.SourceGenerators";

    private static readonly DiagnosticDescriptor UnsupportedMethodSignature = new(
        id: "SKY3101",
        title: "Intercepted service method signature is not supported",
        messageFormat: "Method '{0}' on intercepted service interface '{1}' cannot be proxied by the DynamicProxy source generator: {2}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3101.md");

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var services = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                static (context, _) => CreateModel(context.SemanticModel, (ClassDeclarationSyntax)context.Node))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!.Value);

        context.RegisterSourceOutput(services, static (context, model) => Generate(context, model));
    }

    private static ServiceModel? CreateModel(SemanticModel semanticModel, ClassDeclarationSyntax declaration)
    {
        if (semanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol implementation
            || implementation.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal)
            || !Implements(implementation, InterceptableMetadataName))
        {
            return null;
        }

        var proxies = ImmutableArray.CreateBuilder<ProxyModel>();
        foreach (var serviceInterface in implementation.AllInterfaces
            .Where(static type => type.OriginalDefinition.ToDisplayString() != InterceptableMetadataName)
            .OrderBy(static type => type.GetFullyQualifiedName(), StringComparer.Ordinal))
        {
            var methods = ImmutableArray.CreateBuilder<MethodModel>();
            var diagnostics = ImmutableArray.CreateBuilder<DiagnosticModel>();
            foreach (var method in GetMethods(serviceInterface))
            {
                if (TryCreateMethod(method, serviceInterface, out var methodModel, out var diagnostic))
                {
                    methods.Add(methodModel!.Value);
                }
                else if (diagnostic is not null)
                {
                    diagnostics.Add(diagnostic.Value);
                }
            }

            if (methods.Count > 0 || diagnostics.Count > 0)
            {
                proxies.Add(new ProxyModel(
                    serviceInterface.GetFullyQualifiedName(),
                    CreateProxyName(implementation, serviceInterface),
                    new EquatableArray<MethodModel>(methods.ToImmutable()),
                    new EquatableArray<DiagnosticModel>(diagnostics.ToImmutable())));
            }
        }

        var proxyModels = proxies.ToImmutable();
        return proxyModels.Length == 0
            ? null
            : new ServiceModel(implementation.GetFullyQualifiedName(), implementation.GetNamespace(), new EquatableArray<ProxyModel>(proxyModels));
    }

    private static IEnumerable<IMethodSymbol> GetMethods(INamedTypeSymbol serviceInterface)
    {
        foreach (var baseInterface in serviceInterface.Interfaces)
        {
            if (baseInterface.OriginalDefinition.ToDisplayString() == InterceptableMetadataName)
            {
                continue;
            }

            foreach (var method in GetMethods(baseInterface))
            {
                yield return method;
            }
        }

        foreach (var method in serviceInterface.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.MethodKind == MethodKind.Ordinary && !method.IsStatic)
            {
                yield return method;
            }
        }
    }

    private static bool TryCreateMethod(IMethodSymbol method, INamedTypeSymbol serviceInterface, out MethodModel? model, out DiagnosticModel? diagnostic)
    {
        model = null;
        diagnostic = null;

        if (method.TypeParameters.Length > 0)
        {
            diagnostic = CreateUnsupported(method, serviceInterface, "generic methods are deferred from preview.4 static proxy generation");
            return false;
        }

        if (method.Parameters.Any(static parameter => parameter.RefKind != RefKind.None))
        {
            diagnostic = CreateUnsupported(method, serviceInterface, "ref, out, and in parameters are not supported");
            return false;
        }

        var returnKind = GetReturnKind(method.ReturnType);
        var parameters = method.Parameters
            .Select(static parameter => new ParameterModel(parameter.Name, parameter.Type.GetFullyQualifiedName()))
            .ToImmutableArray();

        model = new MethodModel(
            method.Name,
            method.ReturnType.GetFullyQualifiedName(),
            returnKind,
            GetResultTypeName(method.ReturnType, returnKind),
            CreateMethodFieldName(method),
            new EquatableArray<ParameterModel>(parameters));
        return true;
    }

    private static DiagnosticModel CreateUnsupported(IMethodSymbol method, INamedTypeSymbol serviceInterface, string reason)
    {
        return DiagnosticModel.Create(
            UnsupportedMethodSignature,
            method.Locations.FirstOrDefault(),
            method.Name,
            serviceInterface.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
            reason);
    }

    private static ReturnKind GetReturnKind(ITypeSymbol returnType)
    {
        if (returnType.SpecialType == SpecialType.System_Void)
        {
            return ReturnKind.Void;
        }

        if (returnType is INamedTypeSymbol namedType)
        {
            return namedType.OriginalDefinition.ToDisplayString() switch
            {
                "System.Threading.Tasks.Task" => ReturnKind.Task,
                "System.Threading.Tasks.Task<TResult>" => ReturnKind.TaskOfT,
                "System.Threading.Tasks.ValueTask" => ReturnKind.ValueTask,
                "System.Threading.Tasks.ValueTask<TResult>" => ReturnKind.ValueTaskOfT,
                _ => ReturnKind.SyncValue,
            };
        }

        return ReturnKind.SyncValue;
    }

    private static string? GetResultTypeName(ITypeSymbol returnType, ReturnKind returnKind)
    {
        if (returnKind is not (ReturnKind.TaskOfT or ReturnKind.ValueTaskOfT)
            || returnType is not INamedTypeSymbol namedType
            || namedType.TypeArguments.Length != 1)
        {
            return null;
        }

        return namedType.TypeArguments[0].GetFullyQualifiedName();
    }

    private static void Generate(SourceProductionContext context, ServiceModel model)
    {
        foreach (var proxy in model.Proxies)
        {
            foreach (var diagnostic in proxy.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic.Create());
            }

            if (proxy.Diagnostics.Length == 0 && proxy.Methods.Length > 0)
            {
                context.AddSource($"{proxy.Name}.SkywalkerDynamicProxy.g.cs", GenerateProxy(model, proxy));
            }
        }
    }

    private static string GenerateProxy(ServiceModel model, ProxyModel proxy)
    {
        var source = new StringBuilder();
        source.AppendLine("// <auto-generated/>");
        source.AppendLine("// This file was generated by Skywalker Source Generators.");
        source.AppendLine("// Do not modify this file manually.");
        source.AppendLine();
        source.AppendLine("#nullable enable");
        source.AppendLine("#pragma warning disable");
        source.AppendLine();

        if (!string.IsNullOrEmpty(model.Namespace))
        {
            source.Append("namespace ").Append(model.Namespace).AppendLine(";");
            source.AppendLine();
        }

        source.Append("internal sealed class ").Append(proxy.Name).Append(" : ").AppendLine(proxy.ServiceTypeName);
        source.AppendLine("{");
        source.Append("    private readonly ").Append(model.ImplementationTypeName).AppendLine(" _target;");
        source.Append("    private readonly ").Append(proxy.ServiceTypeName).AppendLine(" _service;");
        source.AppendLine("    private readonly global::System.Collections.Generic.IReadOnlyList<global::Skywalker.Extensions.DynamicProxies.IInterceptor> _interceptors;");
        source.AppendLine();
        source.Append("    public ").Append(proxy.Name).Append('(').Append(model.ImplementationTypeName).AppendLine(" target, global::System.Collections.Generic.IEnumerable<global::Skywalker.Extensions.DynamicProxies.IInterceptor> interceptors)");
        source.AppendLine("    {");
        source.AppendLine("        _target = target ?? throw new global::System.ArgumentNullException(nameof(target));");
        source.AppendLine("        _service = target;");
        source.AppendLine("        _interceptors = global::System.Linq.Enumerable.ToArray(interceptors ?? throw new global::System.ArgumentNullException(nameof(interceptors)));");
        source.AppendLine("    }");
        source.AppendLine();

        foreach (var method in proxy.Methods)
        {
            AppendMethodInfo(source, proxy, method);
        }

        source.AppendLine();
        foreach (var method in proxy.Methods)
        {
            AppendMethod(source, method);
            source.AppendLine();
        }

        AppendInvocation(source);
        source.AppendLine("}");
        return source.ToString();
    }

    private static void AppendMethodInfo(StringBuilder source, ProxyModel proxy, MethodModel method)
    {
        source.Append("    private static readonly global::System.Reflection.MethodInfo ").Append(method.MethodFieldName)
            .Append(" = typeof(").Append(proxy.ServiceTypeName).Append(").GetMethod(\"").Append(method.Name).Append("\", global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.Public, null, ");
        AppendTypeArray(source, method);
        source.AppendLine(", null)!;");
    }

    private static void AppendTypeArray(StringBuilder source, MethodModel method)
    {
        if (method.Parameters.Length == 0)
        {
            source.Append("global::System.Type.EmptyTypes");
            return;
        }

        source.Append("new global::System.Type[] { ");
        for (var i = 0; i < method.Parameters.Length; i++)
        {
            if (i > 0)
            {
                source.Append(", ");
            }

            source.Append("typeof(").Append(method.Parameters[i].TypeName).Append(')');
        }

        source.Append(" }");
    }

    private static void AppendMethod(StringBuilder source, MethodModel method)
    {
        source.Append("    public ").Append(method.ReturnTypeName).Append(' ').Append(method.Name).Append('(');
        AppendParameters(source, method);
        source.AppendLine(")");
        source.AppendLine("    {");
        source.Append("        var invocation = new SkywalkerGeneratedMethodInvocation(_target, ").Append(method.MethodFieldName).Append(", ");
        AppendArguments(source, method);
        source.AppendLine(", _interceptors, async generatedInvocation =>");
        source.AppendLine("        {");
        AppendTargetCall(source, method);
        source.AppendLine("        });");

        switch (method.ReturnKind)
        {
            case ReturnKind.Void:
                source.AppendLine("        invocation.ProceedAsync().GetAwaiter().GetResult();");
                source.AppendLine("        return;");
                break;
            case ReturnKind.SyncValue:
                source.AppendLine("        invocation.ProceedAsync().GetAwaiter().GetResult();");
                source.Append("        return (").Append(method.ReturnTypeName).AppendLine(")invocation.ReturnValue!;");
                break;
            case ReturnKind.Task:
                source.AppendLine("        return invocation.ProceedAsync();");
                break;
            case ReturnKind.TaskOfT:
                source.Append("        return AwaitResultAsync<").Append(method.ResultTypeName).AppendLine(">(invocation);");
                break;
            case ReturnKind.ValueTask:
                source.AppendLine("        return new global::System.Threading.Tasks.ValueTask(invocation.ProceedAsync());");
                break;
            case ReturnKind.ValueTaskOfT:
                source.Append("        return new global::System.Threading.Tasks.ValueTask<").Append(method.ResultTypeName).Append(">(AwaitResultAsync<").Append(method.ResultTypeName).AppendLine(">(invocation));");
                break;
        }

        source.AppendLine("    }");
    }

    private static void AppendTargetCall(StringBuilder source, MethodModel method)
    {
        if (method.ReturnKind is ReturnKind.Void)
        {
            source.Append("            _service.").Append(method.Name).Append('(');
            AppendArgumentNames(source, method);
            source.AppendLine(");");
            source.AppendLine("            await global::System.Threading.Tasks.Task.CompletedTask.ConfigureAwait(false);");
            return;
        }

        if (method.ReturnKind is ReturnKind.SyncValue)
        {
            source.Append("            generatedInvocation.ReturnValue = _service.").Append(method.Name).Append('(');
            AppendArgumentNames(source, method);
            source.AppendLine(");");
            source.AppendLine("            await global::System.Threading.Tasks.Task.CompletedTask.ConfigureAwait(false);");
            return;
        }

        if (method.ReturnKind is ReturnKind.Task or ReturnKind.ValueTask)
        {
            source.Append("            await _service.").Append(method.Name).Append('(');
            AppendArgumentNames(source, method);
            source.AppendLine(").ConfigureAwait(false);");
            return;
        }

        source.Append("            generatedInvocation.ReturnValue = await _service.").Append(method.Name).Append('(');
        AppendArgumentNames(source, method);
        source.AppendLine(").ConfigureAwait(false);");
    }

    private static void AppendParameters(StringBuilder source, MethodModel method)
    {
        for (var i = 0; i < method.Parameters.Length; i++)
        {
            if (i > 0)
            {
                source.Append(", ");
            }

            source.Append(method.Parameters[i].TypeName).Append(' ').Append(method.Parameters[i].Name);
        }
    }

    private static void AppendArguments(StringBuilder source, MethodModel method)
    {
        if (method.Parameters.Length == 0)
        {
            source.Append("global::System.Array.Empty<object?>()");
            return;
        }

        source.Append("new object?[] { ");
        AppendArgumentNames(source, method);
        source.Append(" }");
    }

    private static void AppendArgumentNames(StringBuilder source, MethodModel method)
    {
        for (var i = 0; i < method.Parameters.Length; i++)
        {
            if (i > 0)
            {
                source.Append(", ");
            }

            source.Append(method.Parameters[i].Name);
        }
    }

    private static void AppendInvocation(StringBuilder source)
    {
        source.AppendLine("    private static async global::System.Threading.Tasks.Task<TResult> AwaitResultAsync<TResult>(SkywalkerGeneratedMethodInvocation invocation)");
        source.AppendLine("    {");
        source.AppendLine("        await invocation.ProceedAsync().ConfigureAwait(false);");
        source.AppendLine("        return (TResult)invocation.ReturnValue!;");
        source.AppendLine("    }");
        source.AppendLine();
        source.AppendLine("    private sealed class SkywalkerGeneratedMethodInvocation : global::Skywalker.Extensions.DynamicProxies.IMethodInvocation");
        source.AppendLine("    {");
        source.AppendLine("        private readonly global::System.Collections.Generic.IReadOnlyList<global::Skywalker.Extensions.DynamicProxies.IInterceptor> _interceptors;");
        source.AppendLine("        private readonly global::System.Func<SkywalkerGeneratedMethodInvocation, global::System.Threading.Tasks.Task> _target;");
        source.AppendLine("        private int _index;");
        source.AppendLine();
        source.AppendLine("        public SkywalkerGeneratedMethodInvocation(object target, global::System.Reflection.MethodInfo method, object?[] arguments, global::System.Collections.Generic.IReadOnlyList<global::Skywalker.Extensions.DynamicProxies.IInterceptor> interceptors, global::System.Func<SkywalkerGeneratedMethodInvocation, global::System.Threading.Tasks.Task> targetInvoker)");
        source.AppendLine("        {");
        source.AppendLine("            Target = target;");
        source.AppendLine("            Method = method;");
        source.AppendLine("            Arguments = arguments;");
        source.AppendLine("            ReturnType = method.ReturnType;");
        source.AppendLine("            _interceptors = interceptors;");
        source.AppendLine("            _target = targetInvoker;");
        source.AppendLine("        }");
        source.AppendLine();
        source.AppendLine("        public object Target { get; }");
        source.AppendLine("        public global::System.Reflection.MethodInfo Method { get; }");
        source.AppendLine("        public string MethodName => Method.Name;");
        source.AppendLine("        public object?[] Arguments { get; }");
        source.AppendLine("        public global::System.Type ReturnType { get; }");
        source.AppendLine("        public object? ReturnValue { get; set; }");
        source.AppendLine("        public global::System.Threading.Tasks.Task ProceedAsync()");
        source.AppendLine("        {");
        source.AppendLine("            return _index < _interceptors.Count ? _interceptors[_index++].InterceptAsync(this) : _target(this);");
        source.AppendLine("        }");
        source.AppendLine("    }");
    }

    private static string CreateProxyName(INamedTypeSymbol implementation, INamedTypeSymbol serviceInterface)
    {
        return $"{CreateIdentifier(implementation)}_{CreateIdentifier(serviceInterface)}SkywalkerProxy";
    }

    private static string CreateMethodFieldName(IMethodSymbol method)
    {
        var builder = new StringBuilder("s_").Append(method.Name);
        foreach (var parameter in method.Parameters)
        {
            builder.Append('_').Append(CreateIdentifier(parameter.Type));
        }

        builder.Append("Method");
        return builder.ToString();
    }

    private static string CreateIdentifier(ITypeSymbol symbol)
    {
        var displayName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var builder = new StringBuilder(displayName.Length);
        foreach (var character in displayName)
        {
            builder.Append(char.IsLetterOrDigit(character) ? character : '_');
        }

        return builder.ToString().Trim('_');
    }

    private static bool Implements(INamedTypeSymbol symbol, string metadataName)
    {
        return symbol.AllInterfaces.Any(type => type.OriginalDefinition.ToDisplayString() == metadataName);
    }

    private static int CombineHash(params object?[] values)
    {
        unchecked
        {
            var hash = 17;
            foreach (var value in values)
            {
                hash = (hash * 31) + (value?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }

    private enum ReturnKind
    {
        Void,
        SyncValue,
        Task,
        TaskOfT,
        ValueTask,
        ValueTaskOfT,
    }

    private readonly struct ServiceModel : IEquatable<ServiceModel>
    {
        public ServiceModel(string implementationTypeName, string @namespace, EquatableArray<ProxyModel> proxies)
        {
            ImplementationTypeName = implementationTypeName;
            Namespace = @namespace;
            Proxies = proxies;
        }

        public string ImplementationTypeName { get; }
        public string Namespace { get; }
        public EquatableArray<ProxyModel> Proxies { get; }
        public bool Equals(ServiceModel other) => ImplementationTypeName == other.ImplementationTypeName && Namespace == other.Namespace && Proxies.Equals(other.Proxies);
        public override bool Equals(object? obj) => obj is ServiceModel other && Equals(other);
        public override int GetHashCode() => CombineHash(ImplementationTypeName, Namespace, Proxies);
    }

    private readonly struct ProxyModel : IEquatable<ProxyModel>
    {
        public ProxyModel(string serviceTypeName, string name, EquatableArray<MethodModel> methods, EquatableArray<DiagnosticModel> diagnostics)
        {
            ServiceTypeName = serviceTypeName;
            Name = name;
            Methods = methods;
            Diagnostics = diagnostics;
        }

        public string ServiceTypeName { get; }
        public string Name { get; }
        public EquatableArray<MethodModel> Methods { get; }
        public EquatableArray<DiagnosticModel> Diagnostics { get; }
        public bool Equals(ProxyModel other) => ServiceTypeName == other.ServiceTypeName && Name == other.Name && Methods.Equals(other.Methods) && Diagnostics.Equals(other.Diagnostics);
        public override bool Equals(object? obj) => obj is ProxyModel other && Equals(other);
        public override int GetHashCode() => CombineHash(ServiceTypeName, Name, Methods, Diagnostics);
    }

    private readonly struct MethodModel : IEquatable<MethodModel>
    {
        public MethodModel(string name, string returnTypeName, ReturnKind returnKind, string? resultTypeName, string methodFieldName, EquatableArray<ParameterModel> parameters)
        {
            Name = name;
            ReturnTypeName = returnTypeName;
            ReturnKind = returnKind;
            ResultTypeName = resultTypeName;
            MethodFieldName = methodFieldName;
            Parameters = parameters;
        }

        public string Name { get; }
        public string ReturnTypeName { get; }
        public ReturnKind ReturnKind { get; }
        public string? ResultTypeName { get; }
        public string MethodFieldName { get; }
        public EquatableArray<ParameterModel> Parameters { get; }
        public bool Equals(MethodModel other) => Name == other.Name && ReturnTypeName == other.ReturnTypeName && ReturnKind == other.ReturnKind && ResultTypeName == other.ResultTypeName && MethodFieldName == other.MethodFieldName && Parameters.Equals(other.Parameters);
        public override bool Equals(object? obj) => obj is MethodModel other && Equals(other);
        public override int GetHashCode() => CombineHash(Name, ReturnTypeName, ReturnKind, ResultTypeName, MethodFieldName, Parameters);
    }

    private readonly struct ParameterModel : IEquatable<ParameterModel>
    {
        public ParameterModel(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }

        public string Name { get; }
        public string TypeName { get; }
        public bool Equals(ParameterModel other) => Name == other.Name && TypeName == other.TypeName;
        public override bool Equals(object? obj) => obj is ParameterModel other && Equals(other);
        public override int GetHashCode() => CombineHash(Name, TypeName);
    }

    private readonly struct DiagnosticModel : IEquatable<DiagnosticModel>
    {
        private DiagnosticModel(DiagnosticDescriptor descriptor, Location? location, EquatableArray<string> messageArgs)
        {
            Descriptor = descriptor;
            Location = location;
            MessageArgs = messageArgs;
        }

        public DiagnosticDescriptor Descriptor { get; }
        public Location? Location { get; }
        public EquatableArray<string> MessageArgs { get; }

        public static DiagnosticModel Create(DiagnosticDescriptor descriptor, Location? location, params string[] messageArgs)
        {
            return new DiagnosticModel(descriptor, location, new EquatableArray<string>(messageArgs));
        }

        public Diagnostic Create()
        {
            return Diagnostic.Create(Descriptor, Location, MessageArgs.AsImmutableArray().Cast<object>().ToArray());
        }

        public bool Equals(DiagnosticModel other)
        {
            return Descriptor.Id == other.Descriptor.Id && MessageArgs.Equals(other.MessageArgs);
        }

        public override bool Equals(object? obj) => obj is DiagnosticModel other && Equals(other);
        public override int GetHashCode() => CombineHash(Descriptor.Id, MessageArgs);
    }
}
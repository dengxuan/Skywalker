using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.Extensions.DynamicProxies.SourceGenerators;

/// <summary>
/// 拦截器代理 Source Generator。
/// 扫描所有实现了 IInterceptable 接口的类，生成静态代理类。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class InterceptorProxyGenerator : IIncrementalGenerator
{
    private const string InterceptableInterface = "Skywalker.Extensions.DynamicProxies.IInterceptable";
    private const string TransientDependencyInterface = "Skywalker.DependencyInjection.ITransientDependency";
    private const string ScopedDependencyInterface = "Skywalker.DependencyInjection.IScopedDependency";
    private const string SingletonDependencyInterface = "Skywalker.DependencyInjection.ISingletonDependency";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 收集所有非静态、非抽象的类（实现了 IInterceptable 接口）
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax cds &&
                    cds.BaseList != null,
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(static c => c is not null);

        // 结合编译信息
        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        // 生成代理类
        context.RegisterSourceOutput(compilationAndClasses, (sourceContext, source) =>
        {
            var (compilation, classes) = source;

            var proxyServices = CollectProxyServices(compilation, classes);

            foreach (var service in proxyServices)
            {
                var proxySource = GenerateProxyClass(service);
                var fileName = $"{service.ClassName}Proxy.g.cs";
                sourceContext.AddSource(fileName, proxySource);
            }

            // 生成代理注册扩展方法
            if (proxyServices.Count > 0)
            {
                var registrationSource = GenerateProxyRegistration(compilation.AssemblyName ?? "Generated", proxyServices);
                sourceContext.AddSource("InterceptorProxyRegistration.g.cs", registrationSource);
            }
        });
    }

    private static List<ProxyServiceInfo> CollectProxyServices(
        Compilation compilation,
        IEnumerable<ClassDeclarationSyntax> classDeclarations)
    {
        var services = new List<ProxyServiceInfo>();

        foreach (var classDecl in classDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(classDecl.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(classDecl);

            if (symbol is not INamedTypeSymbol classSymbol) continue;
            if (classSymbol.IsAbstract) continue;
            if (classSymbol.IsStatic) continue;

            var proxyInfo = TryCreateProxyServiceInfo(classSymbol);
            if (proxyInfo is not null)
            {
                services.Add(proxyInfo);
            }
        }

        return services;
    }

    private static ProxyServiceInfo? TryCreateProxyServiceInfo(INamedTypeSymbol classSymbol)
    {
        // 检查是否实现了 IInterceptable 接口
        var implementsInterceptable = classSymbol.AllInterfaces
            .Any(i => i.ToDisplayString() == InterceptableInterface);
        if (!implementsInterceptable) return null;

        // 确定生命周期
        var lifetime = DetermineLifetime(classSymbol);
        if (lifetime is null) return null;

        // 收集服务接口（排除 IInterceptable）
        var serviceInterfaces = CollectServiceInterfaces(classSymbol);

        // 收集需要代理的方法
        var methods = CollectProxyMethods(classSymbol);

        var implementationType = classSymbol.ToDisplayString();
        var ns = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;
        var isPublic = classSymbol.DeclaredAccessibility == Accessibility.Public;

        return new ProxyServiceInfo(
            implementationType,
            ns,
            className,
            serviceInterfaces,
            methods,
            lifetime.Value,
            isPublic);
    }

    private static ServiceLifetime? DetermineLifetime(INamedTypeSymbol classSymbol)
    {
        var interfaces = classSymbol.AllInterfaces;

        bool isTransient = interfaces.Any(i => i.ToDisplayString() == TransientDependencyInterface);
        bool isScoped = interfaces.Any(i => i.ToDisplayString() == ScopedDependencyInterface);
        bool isSingleton = interfaces.Any(i => i.ToDisplayString() == SingletonDependencyInterface);

        if (isSingleton) return ServiceLifetime.Singleton;
        if (isScoped) return ServiceLifetime.Scoped;
        if (isTransient) return ServiceLifetime.Transient;

        return null;
    }

    private static List<string> CollectServiceInterfaces(INamedTypeSymbol classSymbol)
    {
        var interfaces = new List<string>();
        foreach (var iface in classSymbol.AllInterfaces)
        {
            var ifaceName = iface.ToDisplayString();
            if (ifaceName.StartsWith("System.", StringComparison.Ordinal)) continue;
            if (ifaceName.StartsWith("Microsoft.", StringComparison.Ordinal)) continue;
            if (ifaceName.StartsWith("Skywalker.Extensions.DependencyInjection.I", StringComparison.Ordinal)) continue;
            if (ifaceName.StartsWith("Skywalker.DependencyInjection.I", StringComparison.Ordinal)) continue;
            if (ifaceName == InterceptableInterface) continue;  // 排除 IInterceptable
            interfaces.Add(ifaceName);
        }
        return interfaces;
    }

    private static List<ProxyMethodInfo> CollectProxyMethods(INamedTypeSymbol classSymbol)
    {
        var methods = new List<ProxyMethodInfo>();
        var processedMethods = new HashSet<string>();

        // 收集所有接口方法（代理类需要实现所有接口方法）
        foreach (var iface in classSymbol.AllInterfaces)
        {
            // 跳过 DI 规约接口和 IInterceptable
            var ifaceName = iface.ToDisplayString();
            if (ifaceName.StartsWith("Skywalker.DependencyInjection.I", StringComparison.Ordinal)) continue;
            if (ifaceName.StartsWith("System.", StringComparison.Ordinal)) continue;
            if (ifaceName == InterceptableInterface) continue;

            foreach (var member in iface.GetMembers())
            {
                if (member is not IMethodSymbol method) continue;
                if (method.MethodKind != MethodKind.Ordinary) continue;

                // 生成方法签名用于去重
                var signature = GetMethodSignature(method);
                if (processedMethods.Contains(signature)) continue;
                processedMethods.Add(signature);

                var returnType = method.ReturnType.ToDisplayString();
                var isAsync = returnType.StartsWith("System.Threading.Tasks.Task", StringComparison.Ordinal) ||
                             returnType.StartsWith("System.Threading.Tasks.ValueTask", StringComparison.Ordinal);

                var hasReturnValue = !method.ReturnsVoid &&
                                    returnType != "System.Threading.Tasks.Task" &&
                                    returnType != "System.Threading.Tasks.ValueTask";

                var parameters = method.Parameters
                    .Select(p => (p.Type.ToDisplayString(), p.Name))
                    .ToList();

                var typeParameters = method.TypeParameters
                    .Select(tp => tp.Name)
                    .ToList();

                var typeConstraints = method.TypeParameters
                    .Where(tp => tp.ConstraintTypes.Any() || tp.HasReferenceTypeConstraint || tp.HasValueTypeConstraint || tp.HasConstructorConstraint)
                    .Select(tp => GetTypeConstraint(tp))
                    .ToList();

                methods.Add(new ProxyMethodInfo(
                    method.Name,
                    returnType,
                    parameters,
                    isAsync,
                    hasReturnValue,
                    typeParameters,
                    typeConstraints));
            }
        }

        return methods;
    }

    private static string GetMethodSignature(IMethodSymbol method)
    {
        var parameters = string.Join(",", method.Parameters.Select(p => p.Type.ToDisplayString()));
        var typeParams = method.TypeParameters.Length > 0
            ? $"<{string.Join(",", method.TypeParameters.Select(t => t.Name))}>"
            : "";
        return $"{method.Name}{typeParams}({parameters})";
    }

    private static string GetTypeConstraint(ITypeParameterSymbol tp)
    {
        var constraints = new List<string>();

        if (tp.HasReferenceTypeConstraint) constraints.Add("class");
        if (tp.HasValueTypeConstraint) constraints.Add("struct");

        foreach (var constraint in tp.ConstraintTypes)
        {
            constraints.Add(constraint.ToDisplayString());
        }

        if (tp.HasConstructorConstraint) constraints.Add("new()");

        return $"where {tp.Name} : {string.Join(", ", constraints)}";
    }

    /// <summary>
    /// 去除类型名中的 nullable 标记（用于 typeof 操作符）
    /// </summary>
    private static string StripNullableAnnotation(string typeName)
    {
        // 处理简单的 nullable 引用类型，如 "string?" -> "string"
        if (typeName.EndsWith("?") && !typeName.Contains("<"))
        {
            return typeName.TrimEnd('?');
        }

        // 处理泛型中的 nullable 类型参数，如 "List<string?>" -> "List<string>"
        // 使用正则替换或简单的字符串处理
        return typeName.Replace("?", "");
    }

    private static string GenerateProxyClass(ProxyServiceInfo service)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Concurrent;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using System.Reflection;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using Skywalker.Extensions.DynamicProxies;");
        sb.AppendLine();
        sb.AppendLine($"namespace {service.Namespace};");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// {service.ClassName} 的代理类（自动生成）。");
        sb.AppendLine("/// </summary>");

        var interfaces = service.ServiceInterfaces.Count > 0
            ? $" : {string.Join(", ", service.ServiceInterfaces)}"
            : "";
        var accessibility = service.IsPublic ? "public" : "internal";
        sb.AppendLine($"{accessibility} sealed class {service.ClassName}Proxy{interfaces}");
        sb.AppendLine("{");

        // 实例字段
        sb.AppendLine($"    private readonly {service.ImplementationType} _target;");
        sb.AppendLine("    private readonly InterceptorChainBuilder _interceptorChainBuilder;");
        sb.AppendLine();

        // 静态缓存：非泛型方法的 MethodInfo
        foreach (var method in service.Methods)
        {
            if (method.TypeParameters.Count > 0) continue;

            var fieldName = GetMethodInfoFieldName(method);
            if (method.Parameters.Count > 0)
            {
                var paramTypes = string.Join(", ", method.Parameters.Select(p => $"typeof({StripNullableAnnotation(p.Type)})"));
                sb.AppendLine($"    private static readonly MethodInfo {fieldName} = typeof({service.ImplementationType}).GetMethod(\"{method.MethodName}\", new Type[] {{ {paramTypes} }})!;");
            }
            else
            {
                sb.AppendLine($"    private static readonly MethodInfo {fieldName} = typeof({service.ImplementationType}).GetMethod(\"{method.MethodName}\", Type.EmptyTypes)!;");
            }
        }

        // 静态缓存：泛型方法的开放 MethodInfo 和泛型实例缓存
        foreach (var method in service.Methods)
        {
            if (method.TypeParameters.Count == 0) continue;

            var baseName = GetMethodBaseName(method);
            var genericArity = method.TypeParameters.Count;
            sb.AppendLine($"    private static readonly MethodInfo s_{baseName}_OpenGeneric = typeof({service.ImplementationType}).GetMethods()");
            sb.AppendLine($"        .First(m => m.Name == \"{method.MethodName}\" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == {genericArity} && m.GetParameters().Length == {method.Parameters.Count});");
            sb.AppendLine($"    private static readonly ConcurrentDictionary<string, MethodInfo> s_{baseName}_GenericMethodCache = new();");
        }
        sb.AppendLine();

        // 实例缓存：拦截器管道委托（每个方法构建一次，懒加载）
        foreach (var method in service.Methods)
        {
            var baseName = GetMethodBaseName(method);
            if (method.TypeParameters.Count > 0)
            {
                sb.AppendLine($"    private readonly ConcurrentDictionary<string, InterceptorDelegate> _pipelineCache_{baseName} = new();");
            }
            else
            {
                sb.AppendLine($"    private InterceptorDelegate? _pipeline_{baseName};");
            }
        }
        sb.AppendLine();

        // 构造函数
        sb.AppendLine($"    public {service.ClassName}Proxy({service.ImplementationType} target, IServiceProvider serviceProvider)");
        sb.AppendLine("    {");
        sb.AppendLine("        _target = target ?? throw new ArgumentNullException(nameof(target));");
        sb.AppendLine("        _interceptorChainBuilder = new InterceptorChainBuilder(serviceProvider);");
        sb.AppendLine("        foreach (var interceptor in serviceProvider.GetServices<IInterceptor>())");
        sb.AppendLine("        {");
        sb.AppendLine("            _interceptorChainBuilder.UseInterceptor(interceptor);");
        sb.AppendLine("        }");
        sb.AppendLine("    }");

        // 生成代理方法
        foreach (var method in service.Methods)
        {
            sb.AppendLine();
            GenerateProxyMethod(sb, service, method);
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void GenerateProxyMethod(StringBuilder sb, ProxyServiceInfo service, ProxyMethodInfo method)
    {
        var typeParams = method.TypeParameters.Count > 0
            ? $"<{string.Join(", ", method.TypeParameters)}>"
            : "";
        var parameters = string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}"));
        var constraints = method.TypeConstraints.Count > 0
            ? " " + string.Join(" ", method.TypeConstraints)
            : "";

        var asyncModifier = method.IsAsync ? "async " : "";
        sb.AppendLine($"    public {asyncModifier}{method.ReturnType} {method.MethodName}{typeParams}({parameters}){constraints}");
        sb.AppendLine("    {");

        var argsArray = method.Parameters.Count > 0
            ? $"new object?[] {{ {string.Join(", ", method.Parameters.Select(p => p.Name))} }}"
            : "Array.Empty<object?>()";

        // 从 context 中读取参数并转换类型（使 target delegate 可缓存）
        var callArgsFromCtx = method.Parameters.Count > 0
            ? string.Join(", ", method.Parameters.Select((p, i) => $"({p.Type})ctx.Arguments[{i}]!"))
            : "";

        var baseName = GetMethodBaseName(method);

        if (method.TypeParameters.Count > 0)
        {
            // 泛型方法：从缓存获取或构建具体化的 MethodInfo
            var makeGenericArgs = string.Join(", ", method.TypeParameters.Select(tp => $"typeof({tp})"));
            var typeKeyParts = string.Join(" + \",\" + ", method.TypeParameters.Select(tp => $"(typeof({tp}).FullName ?? typeof({tp}).Name)"));
            sb.AppendLine($"        var typeKey = {typeKeyParts};");
            sb.AppendLine($"        var methodInfo = s_{baseName}_GenericMethodCache.GetOrAdd(typeKey, _ => s_{baseName}_OpenGeneric.MakeGenericMethod({makeGenericArgs}));");
            sb.AppendLine($"        var context = new InterceptorContext(_target, methodInfo, {argsArray});");
        }
        else
        {
            var fieldName = GetMethodInfoFieldName(method);
            sb.AppendLine($"        var context = new InterceptorContext(_target, {fieldName}, {argsArray});");
        }
        sb.AppendLine();

        // 管道构建：懒加载缓存，target delegate 从 ctx.Arguments 读取参数
        if (method.IsAsync)
        {
            if (method.TypeParameters.Count > 0)
            {
                sb.AppendLine($"        var pipeline = _pipelineCache_{baseName}.GetOrAdd(typeKey, _ => _interceptorChainBuilder.Build(async ctx =>");
            }
            else
            {
                sb.AppendLine($"        var pipeline = _pipeline_{baseName} ??= _interceptorChainBuilder.Build(async ctx =>");
            }
            sb.AppendLine("        {");
            if (method.HasReturnValue)
            {
                sb.AppendLine($"            ctx.ReturnValue = await _target.{method.MethodName}{typeParams}({callArgsFromCtx});");
            }
            else
            {
                sb.AppendLine($"            await _target.{method.MethodName}{typeParams}({callArgsFromCtx});");
            }
            sb.AppendLine(method.TypeParameters.Count > 0 ? "        }));" : "        });");
            sb.AppendLine("        await pipeline(context);");

            if (method.HasReturnValue)
            {
                var resultType = GetTaskResultType(method.ReturnType);
                sb.AppendLine($"        return ({resultType})context.ReturnValue!;");
            }
        }
        else
        {
            // 同步方法：管道仍为异步（拦截器接口为 async），通过 GetAwaiter().GetResult() 同步等待
            if (method.TypeParameters.Count > 0)
            {
                sb.AppendLine($"        var pipeline = _pipelineCache_{baseName}.GetOrAdd(typeKey, _ => _interceptorChainBuilder.Build(ctx =>");
            }
            else
            {
                sb.AppendLine($"        var pipeline = _pipeline_{baseName} ??= _interceptorChainBuilder.Build(ctx =>");
            }
            sb.AppendLine("        {");
            if (method.HasReturnValue)
            {
                sb.AppendLine($"            ctx.ReturnValue = _target.{method.MethodName}{typeParams}({callArgsFromCtx});");
            }
            else
            {
                sb.AppendLine($"            _target.{method.MethodName}{typeParams}({callArgsFromCtx});");
            }
            sb.AppendLine("            return Task.CompletedTask;");
            sb.AppendLine(method.TypeParameters.Count > 0 ? "        }));" : "        });");
            sb.AppendLine("        pipeline(context).GetAwaiter().GetResult();");

            if (method.HasReturnValue)
            {
                sb.AppendLine($"        return ({method.ReturnType})context.ReturnValue!;");
            }
        }

        sb.AppendLine("    }");
    }

    private static string GetTaskResultType(string returnType)
    {
        // 提取 Task<T> 中的 T
        if (returnType.StartsWith("System.Threading.Tasks.Task<", StringComparison.Ordinal))
        {
            return returnType.Substring(28, returnType.Length - 29);
        }
        if (returnType.StartsWith("System.Threading.Tasks.ValueTask<", StringComparison.Ordinal))
        {
            return returnType.Substring(33, returnType.Length - 34);
        }
        return "object";
    }

    private static string SanitizeIdentifier(string typeName)
    {
        // 将类型名转为合法的 C# 标识符片段
        // 需要处理的特殊字符：. < > , [ ] ? + ( ) 空格
        var sb = new StringBuilder(typeName.Length);
        foreach (var c in typeName)
        {
            switch (c)
            {
                case '[': sb.Append("Array"); break;
                case '.': case '<': case '>': case ',': case ' ':
                case ']': case '?': case '(': case ')': case '+':
                    break; // 跳过
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }

    private static string GetMethodBaseName(ProxyMethodInfo method)
    {
        var paramSuffix = method.Parameters.Count > 0
            ? "_" + string.Join("_", method.Parameters.Select(p => SanitizeIdentifier(StripNullableAnnotation(p.Type))))
            : "";
        var typeParamSuffix = method.TypeParameters.Count > 0
            ? $"_{method.TypeParameters.Count}"
            : "";
        return $"{method.MethodName}{typeParamSuffix}{paramSuffix}";
    }

    private static string GetMethodInfoFieldName(ProxyMethodInfo method)
    {
        return $"s_{GetMethodBaseName(method)}_MethodInfo";
    }

    private static string GenerateProxyRegistration(string assemblyName, List<ProxyServiceInfo> services)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection.Extensions;");
        sb.AppendLine();
        sb.AppendLine("namespace Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine();

        // 与 DI SourceGenerator 生成的类名一致，通过 partial 方法协作
        var className = assemblyName.Replace(".", "").Replace("-", "").Replace(" ", "") + "AutoServiceExtensions";
        sb.AppendLine($"public static partial class {className}");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 注册代理服务（由 DynamicProxies SourceGenerator 生成）。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    static partial void RegisterProxyServices(IServiceCollection services)");
        sb.AppendLine("    {");

        foreach (var service in services)
        {
            var lifetime = service.Lifetime.ToString();
            var proxyType = $"{service.Namespace}.{service.ClassName}Proxy";

            // 注册原始实现
            sb.AppendLine($"        services.TryAdd{lifetime}<{service.ImplementationType}>();");

            // 用代理替换接口注册
            foreach (var iface in service.ServiceInterfaces)
            {
                sb.AppendLine($"        services.Replace(ServiceDescriptor.{lifetime}<{iface}>(sp => new {proxyType}(sp.GetRequiredService<{service.ImplementationType}>(), sp)));");
            }
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.Extensions.DependencyInjection.SourceGenerators;

/// <summary>
/// 依赖注入 Source Generator。
/// 扫描所有实现了规约接口的类，生成 AddAutoServices() 扩展方法。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DependencyInjectionGenerator : IIncrementalGenerator
{
    private const string TransientDependencyInterface = "Skywalker.DependencyInjection.ITransientDependency";
    private const string ScopedDependencyInterface = "Skywalker.DependencyInjection.IScopedDependency";
    private const string SingletonDependencyInterface = "Skywalker.DependencyInjection.ISingletonDependency";
    private const string DisableAutoRegistrationAttribute = "Skywalker.DependencyInjection.DisableAutoRegistrationAttribute";
    private const string ExposeServicesAttribute = "Skywalker.DependencyInjection.ExposeServicesAttribute";
    private const string ReplaceServiceAttribute = "Skywalker.DependencyInjection.ReplaceServiceAttribute";
    private const string KeyedServiceAttribute = "Skywalker.DependencyInjection.KeyedServiceAttribute";
    private const string SharedInstanceAttribute = "Skywalker.DependencyInjection.SharedInstanceAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 收集所有类型声明
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(static c => c is not null);

        // 结合编译信息
        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        // 生成代码
        context.RegisterSourceOutput(compilationAndClasses, (sourceContext, source) =>
        {
            var (compilation, classes) = source;
            var (services, keyedServices) = CollectServices(compilation, classes);

            if (services.Count == 0 && keyedServices.Count == 0) return;

            var assemblyName = compilation.AssemblyName ?? "Generated";

            var sourceCode = GenerateSource(assemblyName, services, keyedServices);
            sourceContext.AddSource($"{assemblyName}.AutoDependencyInjection.g.cs", sourceCode);
        });
    }

    private static (List<ServiceInfo> services, List<KeyedServiceInfo> keyedServices) CollectServices(
        Compilation compilation,
        IEnumerable<ClassDeclarationSyntax> classDeclarations)
    {
        var services = new List<ServiceInfo>();
        var keyedServices = new List<KeyedServiceInfo>();

        foreach (var classDecl in classDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(classDecl.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(classDecl);

            if (symbol is not INamedTypeSymbol classSymbol) continue;
            if (classSymbol.IsAbstract) continue;
            if (classSymbol.IsStatic) continue;

            // 收集 Keyed Services
            var classKeyedServices = CollectKeyedServices(classSymbol);
            keyedServices.AddRange(classKeyedServices);

            // 获取已通过 KeyedService 注册的接口类型
            var keyedServiceTypes = new HashSet<string>(classKeyedServices.Select(k => k.ServiceType));

            // 如果类只有 KeyedService 特性，不需要常规注册
            if (classKeyedServices.Count > 0 && !HasAttribute(classSymbol, ExposeServicesAttribute))
            {
                // 检查是否还有其他需要注册的接口（排除已通过 KeyedService 注册的接口）
                var hasOtherInterfaces = classSymbol.Interfaces
                    .Any(i =>
                        !i.ToDisplayString().StartsWith("Skywalker.DependencyInjection.I", StringComparison.Ordinal) &&
                        !keyedServiceTypes.Contains(i.ToDisplayString()));

                if (!hasOtherInterfaces)
                {
                    continue; // 跳过常规注册
                }
            }

            var serviceInfo = TryCreateServiceInfo(classSymbol, keyedServiceTypes);
            if (serviceInfo is not null)
            {
                services.Add(serviceInfo);
            }
        }

        return (services, keyedServices);
    }

    private static List<KeyedServiceInfo> CollectKeyedServices(INamedTypeSymbol classSymbol)
    {
        var keyedServices = new List<KeyedServiceInfo>();

        // 确定生命周期
        var lifetime = DetermineLifetime(classSymbol);
        if (lifetime is null) return keyedServices;

        var implementationType = classSymbol.ToDisplayString();

        foreach (var attribute in classSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() != KeyedServiceAttribute)
                continue;

            if (attribute.ConstructorArguments.Length < 2)
                continue;

            // 第一个参数：服务类型
            if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol serviceTypeSymbol)
                continue;

            var serviceType = serviceTypeSymbol.ToDisplayString();

            // 第二个参数：Key
            var keyArg = attribute.ConstructorArguments[1];
            string keyValue;
            KeyType keyType;

            if (keyArg.Value is string strValue)
            {
                keyValue = $"\"{strValue}\"";
                keyType = KeyType.String;
            }
            else if (keyArg.Value is int intValue)
            {
                keyValue = intValue.ToString();
                keyType = KeyType.Int;
            }
            else
            {
                // 尝试获取常量引用
                var syntax = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is AttributeSyntax attrSyntax &&
                    attrSyntax.ArgumentList?.Arguments.Count >= 2)
                {
                    var keyArgSyntax = attrSyntax.ArgumentList.Arguments[1];
                    keyValue = keyArgSyntax.Expression.ToString();
                    keyType = KeyType.ConstantReference;
                }
                else
                {
                    continue; // 无法解析 Key
                }
            }

            keyedServices.Add(new KeyedServiceInfo(
                implementationType,
                serviceType,
                keyValue,
                keyType,
                lifetime.Value));
        }

        return keyedServices;
    }

    private static ServiceInfo? TryCreateServiceInfo(INamedTypeSymbol classSymbol, HashSet<string>? excludeServiceTypes = null)
    {
        // 检查是否有 DisableAutoRegistration 特性
        if (HasAttribute(classSymbol, DisableAutoRegistrationAttribute))
            return null;

        // 确定生命周期
        var lifetime = DetermineLifetime(classSymbol);
        if (lifetime is null) return null;

        // 检查是否有 ReplaceService 特性
        var replaceExisting = HasAttribute(classSymbol, ReplaceServiceAttribute);

        // 检查是否有 SharedInstance 特性
        var sharedInstance = HasAttribute(classSymbol, SharedInstanceAttribute);

        // 收集服务类型，排除已通过 KeyedService 注册的接口
        var serviceTypes = CollectServiceTypes(classSymbol, excludeServiceTypes);

        // 如果没有可注册的服务类型，返回 null
        if (serviceTypes.Count == 0)
            return null;

        var implementationType = classSymbol.ToDisplayString();
        var ns = classSymbol.ContainingNamespace.ToDisplayString();
        var isOpenGeneric = classSymbol.IsGenericType && classSymbol.TypeArguments.Any(t => t is ITypeParameterSymbol);

        return new ServiceInfo(
            implementationType,
            ns,
            serviceTypes,
            lifetime.Value,
            replaceExisting,
            isOpenGeneric,
            sharedInstance);
    }

    private static ServiceLifetime? DetermineLifetime(INamedTypeSymbol classSymbol)
    {
        var interfaces = classSymbol.AllInterfaces;

        bool isTransient = interfaces.Any(i => i.ToDisplayString() == TransientDependencyInterface);
        bool isScoped = interfaces.Any(i => i.ToDisplayString() == ScopedDependencyInterface);
        bool isSingleton = interfaces.Any(i => i.ToDisplayString() == SingletonDependencyInterface);

        // 优先级：Singleton > Scoped > Transient
        if (isSingleton) return ServiceLifetime.Singleton;
        if (isScoped) return ServiceLifetime.Scoped;
        if (isTransient) return ServiceLifetime.Transient;

        return null;
    }

    private static bool HasAttribute(INamedTypeSymbol classSymbol, string attributeFullName)
    {
        return classSymbol.GetAttributes()
            .Any(a => a.AttributeClass?.ToDisplayString() == attributeFullName);
    }

    private static List<string> CollectServiceTypes(INamedTypeSymbol classSymbol, HashSet<string>? excludeServiceTypes = null)
    {
        var serviceTypes = new List<string>();

        // 检查是否有 ExposeServices 特性
        var exposeAttr = classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == ExposeServicesAttribute);

        if (exposeAttr is not null)
        {
            // 使用显式指定的服务类型
            if (exposeAttr.ConstructorArguments.Length > 0 &&
                exposeAttr.ConstructorArguments[0].Kind == TypedConstantKind.Array)
            {
                foreach (var arg in exposeAttr.ConstructorArguments[0].Values)
                {
                    if (arg.Value is INamedTypeSymbol type)
                    {
                        var typeName = type.ToDisplayString();
                        // 排除已通过 KeyedService 注册的接口
                        if (excludeServiceTypes?.Contains(typeName) != true)
                        {
                            serviceTypes.Add(typeName);
                        }
                    }
                }
            }

            // 检查 IncludeSelf
            var includeSelf = exposeAttr.NamedArguments
                .FirstOrDefault(a => a.Key == "IncludeSelf").Value.Value as bool? ?? false;
            if (includeSelf)
            {
                serviceTypes.Add(classSymbol.ToDisplayString());
            }

            // 检查 IncludeDefaults
            var includeDefaults = exposeAttr.NamedArguments
                .FirstOrDefault(a => a.Key == "IncludeDefaults").Value.Value as bool? ?? false;
            if (includeDefaults)
            {
                AddDefaultInterfaces(classSymbol, serviceTypes, excludeServiceTypes);
            }

            return serviceTypes;
        }

        // 自动发现服务接口
        AddDefaultInterfaces(classSymbol, serviceTypes, excludeServiceTypes);

        // 如果没有发现任何接口，注册为自身（除非所有接口都已通过 KeyedService 注册）
        if (serviceTypes.Count == 0 && (excludeServiceTypes == null || excludeServiceTypes.Count == 0))
        {
            serviceTypes.Add(classSymbol.ToDisplayString());
        }

        return serviceTypes;
    }

    private static void AddDefaultInterfaces(INamedTypeSymbol classSymbol, List<string> serviceTypes, HashSet<string>? excludeServiceTypes = null)
    {
        // 检查是否是开放泛型类型
        var isOpenGeneric = classSymbol.IsGenericType && classSymbol.TypeArguments.Any(t => t is ITypeParameterSymbol);

        // 只收集类直接实现的接口，不包括继承链上的父接口
        // 例如：PlayerDomainService : IPlayerDomainService, IScopedDependency
        // 只注册 IPlayerDomainService，不注册 IPlayerDomainService 继承的 IDomainService<Player> 等
        foreach (var iface in classSymbol.Interfaces)
        {
            // 排除 DI 规约接口（ITransientDependency, IScopedDependency, ISingletonDependency）
            var ifaceFullName = iface.ToDisplayString();
            if (ifaceFullName.StartsWith("Skywalker.DependencyInjection.I", StringComparison.Ordinal))
            {
                continue;
            }

            // 排除已通过 KeyedService 注册的接口
            if (excludeServiceTypes?.Contains(ifaceFullName) == true)
            {
                continue;
            }

            // 对于开放泛型类型，只注册到泛型接口，不注册到非泛型接口
            if (isOpenGeneric && !iface.IsGenericType)
            {
                continue;
            }

            if (!serviceTypes.Contains(ifaceFullName))
            {
                serviceTypes.Add(ifaceFullName);
            }
        }
    }

    private static string GenerateSource(string assemblyName, List<ServiceInfo> services, List<KeyedServiceInfo> keyedServices)
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
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// 自动服务注册扩展方法（自动生成）。");
        sb.AppendLine("/// </summary>");

        // 使用程序集名称生成唯一的类名
        var className = GetSafeClassName(assemblyName) + "AutoServiceExtensions";
        sb.AppendLine($"public static partial class {className}");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 添加自动发现的服务到服务集合（内部方法）。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <param name=\"services\">服务集合。</param>");
        sb.AppendLine("    /// <returns>服务集合。</returns>");
        sb.AppendLine("    internal static IServiceCollection AddAutoServices(this IServiceCollection services)");
        sb.AppendLine("    {");

        // 按生命周期分组生成注册代码
        var transientServices = services.Where(s => s.Lifetime == ServiceLifetime.Transient).ToList();
        var scopedServices = services.Where(s => s.Lifetime == ServiceLifetime.Scoped).ToList();
        var singletonServices = services.Where(s => s.Lifetime == ServiceLifetime.Singleton).ToList();

        if (transientServices.Count > 0)
        {
            sb.AppendLine("        // Transient services");
            GenerateRegistrations(sb, transientServices, "Transient");
            sb.AppendLine();
        }

        if (scopedServices.Count > 0)
        {
            sb.AppendLine("        // Scoped services");
            GenerateRegistrations(sb, scopedServices, "Scoped");
            sb.AppendLine();
        }

        if (singletonServices.Count > 0)
        {
            sb.AppendLine("        // Singleton services");
            GenerateRegistrations(sb, singletonServices, "Singleton");
            sb.AppendLine();
        }

        // 生成 Keyed Services 注册代码
        if (keyedServices.Count > 0)
        {
            sb.AppendLine("        // Keyed services");
            GenerateKeyedRegistrations(sb, keyedServices);
            sb.AppendLine();
        }

        // 调用代理注册（由 DynamicProxies SourceGenerator 的 partial 方法实现）
        // 如果 DynamicProxies SourceGenerator 未激活，partial 方法调用为空操作
        sb.AppendLine("        RegisterProxyServices(services);");
        sb.AppendLine();

        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 注册代理服务（由 DynamicProxies SourceGenerator 实现）。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    static partial void RegisterProxyServices(IServiceCollection services);");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void GenerateRegistrations(StringBuilder sb, List<ServiceInfo> services, string lifetime)
    {
        foreach (var service in services)
        {
            if (service.SharedInstance && service.ServiceTypes.Count > 0 && !service.IsOpenGeneric)
            {
                // 共享实例模式：先注册实现类自身，再通过工厂委托注册各个接口
                if (service.ReplaceExisting)
                {
                    sb.AppendLine($"        services.Replace(ServiceDescriptor.{lifetime}<{service.ImplementationType}, {service.ImplementationType}>());");
                }
                else
                {
                    sb.AppendLine($"        services.TryAdd{lifetime}<{service.ImplementationType}>();");
                }

                foreach (var serviceType in service.ServiceTypes)
                {
                    if (service.ReplaceExisting)
                    {
                        sb.AppendLine($"        services.Replace(ServiceDescriptor.{lifetime}<{serviceType}>(sp => sp.GetRequiredService<{service.ImplementationType}>()));");
                    }
                    else
                    {
                        sb.AppendLine($"        services.TryAdd{lifetime}<{serviceType}>(sp => sp.GetRequiredService<{service.ImplementationType}>());");
                    }
                }
            }
            else
            {
                foreach (var serviceType in service.ServiceTypes)
                {
                    if (service.IsOpenGeneric)
                    {
                        // 开放泛型
                        var implTypeForGeneric = GetOpenGenericTypeName(service.ImplementationType);
                        var svcTypeForGeneric = GetOpenGenericTypeName(serviceType);

                        if (service.ReplaceExisting)
                        {
                            sb.AppendLine($"        services.Replace(ServiceDescriptor.{lifetime}(typeof({svcTypeForGeneric}), typeof({implTypeForGeneric})));");
                        }
                        else
                        {
                            sb.AppendLine($"        services.TryAdd{lifetime}(typeof({svcTypeForGeneric}), typeof({implTypeForGeneric}));");
                        }
                    }
                    else
                    {
                        // 封闭类型
                        if (service.ReplaceExisting)
                        {
                            sb.AppendLine($"        services.Replace(ServiceDescriptor.{lifetime}<{serviceType}, {service.ImplementationType}>());");
                        }
                        else
                        {
                            sb.AppendLine($"        services.TryAdd{lifetime}<{serviceType}, {service.ImplementationType}>();");
                        }
                    }
                }
            }
        }
    }

    private static void GenerateKeyedRegistrations(StringBuilder sb, List<KeyedServiceInfo> keyedServices)
    {
        foreach (var service in keyedServices)
        {
            var lifetime = service.Lifetime switch
            {
                ServiceLifetime.Transient => "Transient",
                ServiceLifetime.Scoped => "Scoped",
                ServiceLifetime.Singleton => "Singleton",
                _ => "Transient"
            };

            // 使用 TryAddKeyed{Lifetime} 来避免重复注册
            sb.AppendLine($"        services.TryAddKeyed{lifetime}<{service.ServiceType}, {service.ImplementationType}>({service.Key});");
        }
    }

    private static string GetSafeClassName(string assemblyName)
    {
        // 将程序集名称转换为有效的类名
        return assemblyName.Replace(".", "").Replace("-", "").Replace(" ", "");
    }

    private static string GetOpenGenericTypeName(string typeName)
    {
        // 将 MyClass<T> 转换为 MyClass<>
        var index = typeName.IndexOf('<');
        if (index < 0) return typeName;

        var baseName = typeName.Substring(0, index);
        var genericPart = typeName.Substring(index);
        var commaCount = genericPart.Count(c => c == ',');

        return $"{baseName}<{new string(',', commaCount)}>";
    }
}

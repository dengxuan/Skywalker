using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.DependencyInjection.SourceGenerators;

/// <summary>
/// Skywalker 模块 Source Generator。
/// 扫描所有标记了 [SkywalkerModule("ModuleName")] 的程序集，生成 AddSkywalker() 扩展方法。
/// 只为 OutputType=Exe 的项目（应用程序）生成 AddSkywalker()，Library 项目不生成。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class SkywalkerModuleGenerator : IIncrementalGenerator
{
    private const string SkywalkerModuleAttributeName = "Skywalker.SkywalkerModuleAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 获取 OutputType MSBuild 属性
        var outputTypeProvider = context.AnalyzerConfigOptionsProvider.Select((options, _) =>
        {
            options.GlobalOptions.TryGetValue("build_property.OutputType", out var outputType);
            return outputType ?? "Library"; // 默认为 Library
        });

        // 收集所有引用程序集中的模块信息
        var modulesProvider = context.CompilationProvider
            .Combine(outputTypeProvider)
            .Select((combined, cancellationToken) =>
            {
                var (compilation, outputType) = combined;

                // 只有 Exe（应用程序）才生成 AddSkywalker() 方法
                // Library 项目不生成，避免类型冲突
                if (!string.Equals(outputType, "Exe", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(outputType, "WinExe", StringComparison.OrdinalIgnoreCase))
                {
                    return (IReadOnlyList<ModuleInfo>)Array.Empty<ModuleInfo>();
                }

                var modules = new List<ModuleInfo>();

                // 扫描当前编译的程序集
                CollectModulesFromAssembly(compilation.Assembly, modules);

                // 扫描所有引用的程序集
                foreach (var reference in compilation.SourceModule.ReferencedAssemblySymbols)
                {
                    CollectModulesFromAssembly(reference, modules);
                }

                return TopologicalSort(modules);
            });

        // 生成代码
        context.RegisterSourceOutput(modulesProvider, (sourceContext, modules) =>
        {
            if (modules.Count == 0) return;

            var source = GenerateSource(modules);
            sourceContext.AddSource("SkywalkerServiceCollectionExtensions.g.cs", source);
        });
    }

    private static void CollectModulesFromAssembly(IAssemblySymbol assembly, List<ModuleInfo> modules)
    {
        foreach (var attribute in assembly.GetAttributes())
        {
            var attrClass = attribute.AttributeClass;
            if (attrClass is null) continue;

            var attrFullName = attrClass.OriginalDefinition.ToDisplayString();

            // 检查是否是 SkywalkerModuleAttribute
            if (attrFullName != SkywalkerModuleAttributeName) continue;

            // 从构造函数参数获取模块名称
            if (attribute.ConstructorArguments.Length == 0) continue;
            if (attribute.ConstructorArguments[0].Value is not string moduleName) continue;

            // 方法名为 Add{ModuleName}
            var methodName = $"Add{moduleName}";

            // 收集依赖
            var dependencies = assembly.Modules
                .SelectMany(m => m.ReferencedAssemblySymbols)
                .Select(a => a.Name)
                .Where(n => n.StartsWith("Skywalker.", StringComparison.Ordinal))
                .ToList();

            modules.Add(new ModuleInfo(
                assembly.Name,
                string.Empty, // 不再需要 ExtensionType
                methodName,
                dependencies
            ));
        }
    }

    private static IReadOnlyList<ModuleInfo> TopologicalSort(List<ModuleInfo> modules)
    {
        if (modules.Count == 0) return modules;

        var sorted = new List<ModuleInfo>();
        var visited = new HashSet<string>();
        var visiting = new HashSet<string>();
        var moduleMap = modules.ToDictionary(m => m.AssemblyName);

        foreach (var module in modules)
        {
            Visit(module, moduleMap, visited, visiting, sorted);
        }

        return sorted;
    }

    private static void Visit(
        ModuleInfo module,
        Dictionary<string, ModuleInfo> moduleMap,
        HashSet<string> visited,
        HashSet<string> visiting,
        List<ModuleInfo> sorted)
    {
        if (visited.Contains(module.AssemblyName)) return;
        if (visiting.Contains(module.AssemblyName)) return; // 循环依赖，跳过

        visiting.Add(module.AssemblyName);

        foreach (var dep in module.Dependencies)
        {
            if (moduleMap.TryGetValue(dep, out var depModule))
            {
                Visit(depModule, moduleMap, visited, visiting, sorted);
            }
        }

        visiting.Remove(module.AssemblyName);
        visited.Add(module.AssemblyName);
        sorted.Add(module);
    }

    private static string GenerateSource(IReadOnlyList<ModuleInfo> modules)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine();
        sb.AppendLine("namespace Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Skywalker 服务集合扩展方法（自动生成）。");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class SkywalkerServiceCollectionExtensions");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 添加所有 Skywalker 模块到服务集合。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <param name=\"services\">服务集合。</param>");
        sb.AppendLine("    /// <returns>服务集合。</returns>");
        sb.AppendLine("    public static IServiceCollection AddSkywalker(this IServiceCollection services)");
        sb.AppendLine("    {");

        foreach (var module in modules)
        {
            sb.AppendLine($"        services.{module.MethodName}();");
        }

        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}

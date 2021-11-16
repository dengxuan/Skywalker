﻿using Microsoft.CodeAnalysis;
using Skywalker.Aspects.CodeAnalysis.Analyzers;

namespace Skywalker.Aspects.CodeAnalysis.Analyzers.Providers;

/// <summary>
/// 表示UriAttribute诊断器
/// </summary>
sealed class UriAttributeDiagnosticProvider : HttpApiDiagnosticProvider
{
    /// <summary>   
    /// 获取诊断描述
    /// </summary>
    public override DiagnosticDescriptor Descriptor => Descriptors.UriAttributeDescriptor;

    /// <summary>
    /// UriAttribute诊断器
    /// </summary>
    /// <param name="context">上下文</param>
    public UriAttributeDiagnosticProvider(HttpApiContext context)
        : base(context)
    {
    }

    /// <summary>
    /// 返回所有的报告诊断
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<Diagnostic> CreateDiagnostics()
    {
        var attr = Context.UriAttribute;
        if (attr == null)
        {
            yield break;
        }

        foreach (var method in Context.Methods)
        {
            for (var i = 1; i < method.Parameters.Length; i++)
            {
                var parameter = method.Parameters[i];
                var uriAttribute = parameter.GetAttributes().FirstOrDefault(item => attr.Equals(item.AttributeClass));
                var appSyntax = uriAttribute?.ApplicationSyntaxReference;
                if (appSyntax == null)
                {
                    continue;
                }

                var location = appSyntax.GetSyntax().GetLocation();
                yield return CreateDiagnostic(location);
            }
        }
    }
}

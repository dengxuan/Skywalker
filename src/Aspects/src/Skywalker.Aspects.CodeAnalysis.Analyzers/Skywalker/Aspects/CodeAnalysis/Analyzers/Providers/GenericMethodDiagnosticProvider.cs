﻿using Microsoft.CodeAnalysis;

namespace Skywalker.Aspects.CodeAnalysis.Analyzers.Providers;

/// <summary>
/// 表示泛型方法诊断器
/// </summary>
sealed class GenericMethodDiagnosticProvider : HttpApiDiagnosticProvider
{
    /// <summary>
    /// 获取诊断描述
    /// </summary>
    public override DiagnosticDescriptor Descriptor => Descriptors.GenericMethodDescriptor;

    /// <summary>
    /// 泛型方法诊断器
    /// </summary>
    /// <param name="context">上下文</param>
    public GenericMethodDiagnosticProvider(HttpApiContext context)
        : base(context)
    {
    }

    /// <summary>
    /// 返回所有的报告诊断
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<Diagnostic> CreateDiagnostics()
    {
        foreach (var method in Context.Methods)
        {
            if (method.IsGenericMethod == false)
            {
                continue;
            }

            var declaringSyntax = method.DeclaringSyntaxReferences.FirstOrDefault();
            if (declaringSyntax != null)
            {
                var location = declaringSyntax.GetSyntax().GetLocation();
                yield return CreateDiagnostic(location);
            }
        }
    }
}

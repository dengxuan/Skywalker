﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Skywalker.Aspects.CodeAnalysis.Analyzers;

namespace Skywalker.Aspects.CodeAnalysis.Analyzers.Providers;

/// <summary>
/// 表示非方法声明诊断器
/// </summary>
sealed class NotMethodDefindedDiagnosticProvider : HttpApiDiagnosticProvider
{
    /// <summary>
    /// /// <summary>
    /// 获取诊断描述
    /// </summary>
    /// </summary>
    public override DiagnosticDescriptor Descriptor => Descriptors.NotMethodDefindedDescriptor;

    /// <summary>
    /// 非方法声明诊断器
    /// </summary>
    /// <param name="context">上下文</param>
    public NotMethodDefindedDiagnosticProvider(HttpApiContext context)
        : base(context)
    {
    }

    /// <summary>
    /// 返回所有的报告诊断
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<Diagnostic> CreateDiagnostics()
    {
        foreach (var member in Context.Syntax.Members)
        {
            if (member.Kind() != SyntaxKind.MethodDeclaration)
            {
                var location = member.GetLocation();
                yield return CreateDiagnostic(location);
            }
        }
    }
}

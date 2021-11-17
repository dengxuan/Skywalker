﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Skywalker.Aspects.CodeAnalysis.Analyzers;

/// <summary>
/// 表示HttpApi上下文
/// </summary>
sealed class HttpApiContext
{
    private const string IHttpApiTypeName = "WebApiClientCore.IHttpApi";
    private const string IApiAttributeTypeName = "WebApiClientCore.IApiAttribute";
    private const string UriAttributeTypeName = "WebApiClientCore.Attributes.UriAttribute";
    private const string AttributeCtorUsageTypName = "WebApiClientCore.AttributeCtorUsageAttribute";

    /// <summary>
    /// 获取接口声明语法
    /// </summary>
    public InterfaceDeclarationSyntax Syntax { get; }

    /// <summary>
    /// 获取接口
    /// </summary>
    public INamedTypeSymbol Interface { get; }

    /// <summary>
    /// 获取声明的Api方法
    /// </summary>
    public IMethodSymbol[] Methods { get; }

    /// <summary>
    /// 获取UriAttribute的类型
    /// </summary>
    public INamedTypeSymbol? UriAttribute { get; }

    /// <summary>
    /// 获取AttributeCtorUsageAttribute的类型
    /// </summary>
    public INamedTypeSymbol? AttributeCtorUsageAttribute { get; }

    /// <summary>
    /// HttpApi上下文
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="interface"></param>
    /// <param name="methods"></param>
    /// <param name="uriAttribute"></param>
    /// <param name="attributeCtorUsageAttribute"></param>
    private HttpApiContext(
        InterfaceDeclarationSyntax syntax,
        INamedTypeSymbol @interface,
        IMethodSymbol[] methods,
        INamedTypeSymbol? uriAttribute,
        INamedTypeSymbol? attributeCtorUsageAttribute)
    {
        Syntax = syntax;
        Interface = @interface;
        Methods = methods;
        UriAttribute = uriAttribute;
        AttributeCtorUsageAttribute = attributeCtorUsageAttribute;
    }

    /// <summary>
    /// 尝试解析
    /// </summary>
    /// <param name="syntaxNodeContext"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static bool TryParse(SyntaxNodeAnalysisContext syntaxNodeContext, out HttpApiContext? context)
    {
        context = null;
        var syntax = syntaxNodeContext.Node as InterfaceDeclarationSyntax;
        if (syntax == null)
        {
            return false;
        }

        var @interface = syntaxNodeContext.Compilation.GetSemanticModel(syntax.SyntaxTree).GetDeclaredSymbol(syntax);
        if (@interface == null)
        {
            return false;
        }

        var ihttpApi = syntaxNodeContext.Compilation.GetTypeByMetadataName(IHttpApiTypeName);
        if (ihttpApi == null)
        {
            return false;
        }

        var iapiAttribute = syntaxNodeContext.Compilation.GetTypeByMetadataName(IApiAttributeTypeName);
        if (IsHttpApiInterface(@interface, ihttpApi, iapiAttribute) == false)
        {
            return false;
        }

        var methods = @interface.GetMembers().OfType<IMethodSymbol>().ToArray();
        var uriAttribute = syntaxNodeContext.Compilation.GetTypeByMetadataName(UriAttributeTypeName);
        var attributeCtorUsageAttribute = syntaxNodeContext.Compilation.GetTypeByMetadataName(AttributeCtorUsageTypName);

        context = new HttpApiContext(syntax, @interface, methods, uriAttribute, attributeCtorUsageAttribute);
        return true;
    }

    /// <summary>
    /// 是否为http接口
    /// </summary>
    /// <param name="interface"></param>
    /// <param name="ihttpApi"></param>
    /// <param name="iapiAttribute"></param>
    /// <returns></returns>
    private static bool IsHttpApiInterface(INamedTypeSymbol @interface, INamedTypeSymbol ihttpApi, INamedTypeSymbol? iapiAttribute)
    {
        if (@interface.AllInterfaces.Contains(ihttpApi))
        {
            return true;
        }

        if (iapiAttribute == null)
        {
            return false;
        }

        return @interface.AllInterfaces.Append(@interface).Any(i =>
            HasAttribute(i, iapiAttribute) || i.GetMembers().OfType<IMethodSymbol>().Any(m =>
            HasAttribute(m, iapiAttribute) || m.Parameters.Any(p => HasAttribute(p, iapiAttribute))));
    }


    /// <summary>
    /// 返回是否声明指定的特性
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    private static bool HasAttribute(ISymbol symbol, INamedTypeSymbol attribute)
    {
        foreach (var attr in symbol.GetAttributes())
        {
            var attrClass = attr.AttributeClass;
            if (attrClass != null && attrClass.AllInterfaces.Contains(attribute))
            {
                return true;
            }
        }
        return false;
    }
}

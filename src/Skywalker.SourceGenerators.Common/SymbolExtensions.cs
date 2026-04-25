using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.SourceGenerators;

internal static class SymbolExtensions
{
    /// <summary>
    /// Whether the type is declared <c>partial</c> in any of its declarations.
    /// </summary>
    public static bool IsPartial(this INamedTypeSymbol symbol)
    {
        foreach (var declaration in symbol.DeclaringSyntaxReferences)
        {
            var syntax = declaration.GetSyntax();
            if (syntax is TypeDeclarationSyntax typeDecl
                && typeDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the fully-qualified type name including <c>global::</c> prefix and generic arity,
    /// suitable for emission into generated source.
    /// </summary>
    public static string GetFullyQualifiedName(this ITypeSymbol symbol)
        => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Returns the containing namespace of the type, or empty string if the type is in the global
    /// namespace.
    /// </summary>
    public static string GetNamespace(this INamedTypeSymbol symbol)
    {
        var ns = symbol.ContainingNamespace;
        return ns is null || ns.IsGlobalNamespace ? string.Empty : ns.ToDisplayString();
    }

    /// <summary>
    /// Whether the symbol is decorated with an attribute whose fully-qualified type name
    /// (without generic arity) matches <paramref name="fullyQualifiedMetadataName"/>.
    /// </summary>
    public static bool HasAttributeOfType(this ISymbol symbol, string fullyQualifiedMetadataName)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            var attrClass = attribute.AttributeClass;
            if (attrClass is not null
                && attrClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedMetadataName)
            {
                return true;
            }
        }
        return false;
    }
}

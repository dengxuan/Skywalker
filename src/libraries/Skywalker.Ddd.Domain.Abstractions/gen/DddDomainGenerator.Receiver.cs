// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Domain.Generators;

public partial class DddDomainGenerator
{
    internal class SyntaxContextReceiver : ISyntaxContextReceiver
    {
        internal MetadataClass MetadataClasses { get; } = new();

        private SyntaxContextReceiver() { }

        internal static ISyntaxContextReceiver Create() => new SyntaxContextReceiver();

        private static bool IsSyntaxTargetForGeneration(SemanticModel semanticModel, SyntaxNode node, out INamedTypeSymbol? namedTypeSymbol)
        {
            namedTypeSymbol = semanticModel.GetSymbolInfo(node).Symbol as INamedTypeSymbol;
            if (namedTypeSymbol == null)
            {
                return false;
            }
            if (namedTypeSymbol.TypeKind is not TypeKind.Class)
            {
                return false;
            }

            if (namedTypeSymbol.IsStatic || namedTypeSymbol.IsAbstract)
            {
                return false;
            }
            return true;
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (!IsSyntaxTargetForGeneration(context.SemanticModel, context.Node, out var namedTypeSymbol))
            {
                return;
            }
            var domainServiceSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(Constants.DomainServiceSymbolName);
            if (!namedTypeSymbol!.AllInterfaces.Any(predicate => s_symbolComparer.Equals(predicate, domainServiceSymbol)))
            {
                return;
            }
            if (!MetadataClasses.DomainServices.TryGetValue(namedTypeSymbol, out var implImterfaces))
            {
                implImterfaces = new HashSet<INamedTypeSymbol>(s_symbolComparer);
                MetadataClasses.DomainServices.Add(namedTypeSymbol, implImterfaces);
            }
            foreach (var item in namedTypeSymbol.AllInterfaces)
            {
                if (!s_symbolComparer.Equals(item, domainServiceSymbol))
                {
                    implImterfaces.Add(item);
                }
            }
        }

    }
}

﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Skywalker.Extensions.DependencyInjection.Generators;

public partial class DependencyInjectionGenerator
{
    internal class SyntaxContextReceiver : ISyntaxContextReceiver
    {
        internal HashSet<INamedTypeSymbol> Classes { get; } = new(s_symbolComparer);

        private SyntaxContextReceiver() { }

        internal static ISyntaxContextReceiver Create() => new SyntaxContextReceiver();

        private static bool IsSyntaxTargetForGeneration(SemanticModel semanticModel, SyntaxNode node, out INamedTypeSymbol? namedTypeSymbol)
        {
            namedTypeSymbol = semanticModel.GetSymbolInfo(node).Symbol as INamedTypeSymbol;
            if (namedTypeSymbol is null)
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
            var dependencySymbols = new[]
            {
                semanticModel.Compilation.GetTypeByMetadataName(Constants.ScopedDependencyAttributeSymbolName),
                semanticModel.Compilation.GetTypeByMetadataName(Constants.TransientDependencyAttributeSymbolName),
                semanticModel.Compilation.GetTypeByMetadataName(Constants.SingletonDependencyAttributeSymbolName)
            };
            foreach (var attributeData in namedTypeSymbol.GetAttributes())
            {
                if (dependencySymbols.Any(predicate => s_symbolComparer.Equals(predicate, attributeData.AttributeClass)))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (IsSyntaxTargetForGeneration(context.SemanticModel, context.Node, out var namedTypeSymbol))
            {
                Classes.Add(namedTypeSymbol!);
            }
        }

    }
}

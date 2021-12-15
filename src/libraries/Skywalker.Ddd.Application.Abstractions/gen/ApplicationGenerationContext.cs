using Microsoft.CodeAnalysis;

namespace Skywalker.CodeAnalyzers.Analyzers.SourceGenerator
{
    internal readonly struct ApplicationGenerationContext
    {
        public readonly IReadOnlyDictionary<INamedTypeSymbol, List<INamedTypeSymbol>> HandlerMap;
        public readonly IEnumerable<INamedTypeSymbol> HandlerTypes;
        public readonly string MediatorNamespace;

        public ApplicationGenerationContext(IReadOnlyDictionary<INamedTypeSymbol, List<INamedTypeSymbol>> handlerMap, IEnumerable<INamedTypeSymbol> handlerTypes, string mediatorNamespace)
        {
            HandlerMap = handlerMap;
            HandlerTypes = handlerTypes;
            MediatorNamespace = mediatorNamespace;
        }
    }
}

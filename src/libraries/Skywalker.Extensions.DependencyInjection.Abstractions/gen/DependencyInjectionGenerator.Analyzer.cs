using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.DependencyInjection.Generators;

public partial class DependencyInjectionGenerator
{
    protected static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.IncludeNullability;

    internal class Analyzer
    {
        private readonly INamedTypeSymbol? _modularSymbol;
        private readonly Compilation _compilation;

        public Analyzer(in GeneratorExecutionContext context)
        {
            _compilation = context.Compilation;
            _modularSymbol = _compilation.GetTypeByMetadataName(Constants.ModularSymbolName);
        }

        private List<INamedTypeSymbol> FindNamedTypeSymbol(INamespaceSymbol namespaceSymbol)
        {
            var namedTypeSymbols = new List<INamedTypeSymbol>();
            foreach (var member in namespaceSymbol.GetMembers())
            {
                switch (member)
                {
                    case INamespaceSymbol nsSymbol:
                        {
                            var result = FindNamedTypeSymbol(nsSymbol);
                            namedTypeSymbols.AddRange(result);
                            break;
                        }
                    case INamedTypeSymbol namedTypeSymbol:
                        {
                            if (namedTypeSymbol.TypeKind is not TypeKind.Class)
                            {
                                break;
                            }
                            if (namedTypeSymbol.IsStatic || namedTypeSymbol.IsAbstract)
                            {
                                break;
                            }
                            if (!s_symbolComparer.Equals(namedTypeSymbol.BaseType, _modularSymbol))
                            {
                                break;
                            }
                            namedTypeSymbols.Add(namedTypeSymbol);
                            break;
                        }
                    default: break;
                }
            }

            return namedTypeSymbols;
        }


        public INamedTypeSymbol? GetModularSymbol()
        {
            var results = FindNamedTypeSymbol(_compilation.Assembly.GlobalNamespace);
            switch (results.Count)
            {
                case 0:
                    {
                        return null;
                    }
                case 1:
                    {
                        return results.Single();
                    }
                default:
                    {
                        return null;
                    }
            }
        }

    }
}

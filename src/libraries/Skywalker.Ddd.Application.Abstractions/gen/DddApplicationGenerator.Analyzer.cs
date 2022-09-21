using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.Ddd.Application.Generators;

public partial class DddApplicationGenerator
{
    protected static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.IncludeNullability;

    internal class Analyzer : ISyntaxReceiver
    {
        /// <summary>
        /// 接口列表
        /// </summary>
        private readonly List<InterfaceDeclarationSyntax> _applicationServiceSyntaxs = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax syntax)
            {
                _applicationServiceSyntaxs.Add(syntax);
            }
        }

        /// <summary>
        /// 获取所有IAspects符号
        /// </summary>
        /// <param name="compilation"></param>
        /// <returns></returns>
        public IEnumerable<INamedTypeSymbol> GetAspectsTypes(Compilation compilation)
        {
            var aspects = compilation.GetTypeByMetadataName(Constants.ApplicationServiceSymbolName);
            if (aspects == null)
            {
                yield break;
            }

            foreach (var applicationServiceSyntax in _applicationServiceSyntaxs)
            {
                var symbol = compilation.GetSemanticModel(applicationServiceSyntax.SyntaxTree).GetDeclaredSymbol(applicationServiceSyntax);
                if(symbol is INamedTypeSymbol namedTypeSymbol)
                {
                    if (namedTypeSymbol != null && IsApplicationServiceInterface(namedTypeSymbol, aspects))
                    {
                        yield return namedTypeSymbol;
                    }
                }
            }
        }


        /// <summary>
        /// 是否为http接口
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="aspects"></param>
        /// <param name="interceptorAttribut"></param>
        /// <returns></returns>
        private static bool IsApplicationServiceInterface(INamedTypeSymbol clazz, INamedTypeSymbol aspects)
        {
            return clazz.AllInterfaces.Contains(aspects);
        }
    }

    internal readonly record struct MetadataClass
    {
        internal Dictionary<INamedTypeSymbol, Dictionary<INamedTypeSymbol, string>> DbContextClasses { get; }

        public MetadataClass()
        {
            DbContextClasses = new Dictionary<INamedTypeSymbol, Dictionary<INamedTypeSymbol, string>>(s_symbolComparer);
        }
    }
}

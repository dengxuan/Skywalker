//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace Skywalker.Extensions.ObjectMapper.Generators;

//public partial class ObjectMapperGenerator
//{
//    internal class AttributeReceiver : ISyntaxContextReceiver
//    {
//        private readonly string _attributeName;
//        private readonly HashSet<TypeDeclarationSyntax> _candidateTypes = new();

//        private INamedTypeSymbol? _attributeSymbol;

//        public AttributeReceiver(string attributeName)
//        {
//            _attributeName = attributeName;
//        }

//        internal IReadOnlyList<TypeDeclarationSyntax> CandidateTypes => _candidateTypes.ToList();

//        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
//        {
//            _attributeSymbol ??= context.SemanticModel.Compilation.GetTypeByMetadataName(_attributeName);

//            if (_attributeSymbol is null)
//            {
//                return;
//            }

//            if (context.Node is not TypeDeclarationSyntax typeDeclaration)
//            {
//                return;
//            }

//            var symbols = typeDeclaration.AttributeLists.SelectMany(syntax => syntax.Attributes, (_, attribute) =>
//            {
//                return context.SemanticModel.GetSymbolInfo(attribute).Symbol;
//            });

//            if (!symbols.Any(predicate => s_symbolComparer.Equals(predicate?.ContainingType, _attributeSymbol)))
//            {
//                return;
//            }
//            _candidateTypes.Add(typeDeclaration);
//        }
//    }

//    internal class SyntaxContextReceiver : ISyntaxContextReceiver
//    {
//        private readonly string _attributeName;
//        private INamedTypeSymbol? _attributeSymbol;

//        internal HashSet<TypeDeclarationSyntax> CandidateTypes { get; } = new();

//        private SyntaxContextReceiver(string attributeName) => _attributeName = attributeName;

//        /// <summary>
//        /// 对拥有某attribute的type生成代码
//        /// </summary>
//        /// <param name="typeDeclarationSyntax"></param>
//        /// <param name="typeSymbol"></param>
//        /// <param name="attributeEqualityComparer">判断是否是指定的attribute的比较器</param>
//        /// <returns>生成的代码</returns>
//        private delegate string? TypeWithAttribute(TypeDeclarationSyntax typeDeclarationSyntax, INamedTypeSymbol typeSymbol, Func<AttributeData, bool> attributeEqualityComparer);

//        /// <summary>
//        /// 需要生成的Attribute
//        /// </summary>
//        private static readonly Dictionary<string, TypeWithAttribute> Attributes = new()
//        {
//            { "Attributes.GenerateConstructorAttribute", TypeWithAttributeDelegates.GenerateConstructor },
//            { "Attributes.LoadSaveConfigurationAttribute", TypeWithAttributeDelegates.LoadSaveConfiguration },
//            { "Attributes.DependencyPropertyAttribute", TypeWithAttributeDelegates.DependencyProperty }
//        };

//        internal static ISyntaxContextReceiver Create() => new SyntaxContextReceiver();
//        /// <summary>
//        /// 初次快速筛选（对拥有Attribute的class和record）
//        /// </summary>
//        private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
//        {
//            return node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } and (ClassDeclarationSyntax or RecordDeclarationSyntax);
//        }

//        /// <summary>
//        /// 获取TypeDeclarationSyntax
//        /// </summary>
//        private static TypeDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
//        {
//            var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;

//            foreach (var attributeListSyntax in typeDeclarationSyntax.AttributeLists)
//            {
//                foreach (var attributeSyntax in attributeListSyntax.Attributes)
//                {
//                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
//                    {
//                        continue;
//                    }

//                    if (Attributes.ContainsKey(attributeSymbol.ContainingType.ToDisplayString()))
//                    {
//                        return typeDeclarationSyntax;
//                    }
//                }
//            }

//            return null;
//        }


//        private static bool IsSyntaxTargetForGeneration(SemanticModel semanticModel, SyntaxNode node, out INamedTypeSymbol? namedTypeSymbol)
//        {

//            namedTypeSymbol = semanticModel.GetSymbolInfo(node).Symbol as INamedTypeSymbol;
//            if (node is not RecordDeclarationSyntax recordDeclarationSyntax || recordDeclarationSyntax.AttributeLists.Count == 0)
//            {
//                return false;
//            }
//            var fromTypeArgSyntax = recordDeclarationSyntax.AttributeLists[0].Attributes[0].ArgumentList!.Arguments.First();
//            //if (node is not ClassDeclarationSyntax classDeclarationSyntax || classDeclarationSyntax.AttributeLists.Count == 0)
//            //{
//            //    return false;
//            //}

//            if (namedTypeSymbol is null)
//            {
//                return false;
//            }
//            if (namedTypeSymbol.TypeKind is not TypeKind.Class)
//            {
//                return false;
//            }

//            if (namedTypeSymbol.IsStatic || namedTypeSymbol.IsAbstract)
//            {
//                return false;
//            }
//            var autoMapSymbols = new[]
//            {
//                semanticModel.Compilation.GetTypeByMetadataName(Constants.ObjectMapperAutoMapAttribute),
//                semanticModel.Compilation.GetTypeByMetadataName(Constants.ObjectMapperAutoMapToAttribute),
//                semanticModel.Compilation.GetTypeByMetadataName(Constants.ObjectMapperAutoMapFromAttribute)
//            };
//            foreach (var attributeData in namedTypeSymbol.GetAttributes())
//            {
//                if (autoMapSymbols.Any(predicate => s_symbolComparer.Equals(predicate, attributeData.AttributeClass)))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
//        {
//            _attributeSymbol ??= context.SemanticModel.Compilation.GetTypeByMetadataName(_attributeName);
//            if (!IsSyntaxTargetForGeneration(context.Node))
//            {
//                return;
//            }
//            if (IsSyntaxTargetForGeneration(context.SemanticModel, context.Node, out var namedTypeSymbol))
//            {
//                Classes.Add(namedTypeSymbol!);
//            }
//        }

//    }
//}


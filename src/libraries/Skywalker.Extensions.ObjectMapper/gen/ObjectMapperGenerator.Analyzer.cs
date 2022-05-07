using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.ObjectMapper.Generators;

public partial class DddEntityFrameworkCoreGenerator
{
    protected static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.IncludeNullability;

    internal class Analyzer
    {

        private readonly Compilation _compilation;

        private readonly INamedTypeSymbol? _domainEntitieSymbol;

        /// <summary>
        /// Tow-way Map
        /// </summary>
        private readonly INamedTypeSymbol? _autoMapAttributeSymbol;

        /// <summary>
        /// Map To
        /// </summary>
        private readonly INamedTypeSymbol? _autoMapToAttributeSymbol;

        /// <summary>
        /// Map From
        /// </summary>
        private readonly INamedTypeSymbol? _autoMapFromAttributeSymbol;

        public Analyzer(in GeneratorExecutionContext context)
        {
            _compilation = context.Compilation;
            _domainEntitieSymbol = _compilation.GetTypeByMetadataName(Constants.EntitySymbolName);
            _autoMapAttributeSymbol= _compilation.GetTypeByMetadataName(Constants.ObjectMapperAutoMapAttribute);
            _autoMapToAttributeSymbol = _compilation.GetTypeByMetadataName(Constants.ObjectMapperAutoMapAttribute);
            _autoMapFromAttributeSymbol = _compilation.GetTypeByMetadataName(Constants.ObjectMapperAutoMapAttribute);
        }

        private Queue<INamespaceOrTypeSymbol> FindGlobalNamespaces()
        {
            var queue = new Queue<INamespaceOrTypeSymbol>();
            queue.Enqueue(_compilation.Assembly.GlobalNamespace);

            foreach (var reference in _compilation.References)
            {
                if (_compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assemblySymbol)
                {
                    continue;
                }

                if (!assemblySymbol.Modules.Any(m => m.ReferencedAssemblies.Any(ra => ra.Name is Constants.ObjectMapperAssemblyName)))
                {
                    continue;
                }
                queue.Enqueue(assemblySymbol.GlobalNamespace);
            }
            return queue;
        }

        private MetadataClass PopulateMetadata(Queue<INamespaceOrTypeSymbol> queue)
        {
            var metadataClass = new MetadataClass();
            while (queue.Count > 0)
            {
                var nsOrTypeSymbol = queue.Dequeue();

                switch (nsOrTypeSymbol)
                {
                    case INamespaceSymbol nsSymbol:
                        {
                            foreach (var member in nsSymbol.GetMembers())
                            {
                                queue.Enqueue(member);
                            }
                            break;
                        }
                    case INamedTypeSymbol namedTypeSymbol:
                        {
                            foreach (var childTypeSymbol in namedTypeSymbol.GetTypeMembers())
                            {
                                queue.Enqueue(childTypeSymbol);
                            }

                            if (namedTypeSymbol.TypeKind is not TypeKind.Class)
                            {
                                break;
                            }

                            if (namedTypeSymbol.IsStatic || namedTypeSymbol.IsAbstract)
                            {
                                break;
                            }

                            //双向Mapping
                            var attributeDatas = namedTypeSymbol.GetAttributes();
                            var autoMapAttributeData = attributeDatas.FirstOrDefault(predicate => s_symbolComparer.Equals(predicate.AttributeClass, _autoMapAttributeSymbol));
                            if (autoMapAttributeData != null)
                            {
                                var targetObjects = new HashSet<INamedTypeSymbol>(s_symbolComparer);
                                metadataClass.AutoMapperClasses.Add(namedTypeSymbol, targetObjects);
                            }

                            //MapTo
                            var autoMapToAttributeData = attributeDatas.FirstOrDefault(predicate => s_symbolComparer.Equals(predicate.AttributeClass, _autoMapToAttributeSymbol));
                            if (autoMapToAttributeData != null)
                            {
                                var targetObjects = new HashSet<INamedTypeSymbol>(s_symbolComparer);
                                metadataClass.AutoMapperClasses.Add(namedTypeSymbol, targetObjects);
                            }

                            //MapFrom
                            var autoMapFromAttributeData = attributeDatas.FirstOrDefault(predicate => s_symbolComparer.Equals(predicate.AttributeClass, _autoMapFromAttributeSymbol));
                            if (autoMapFromAttributeData != null)
                            {
                                var targetObjects = new HashSet<INamedTypeSymbol>(s_symbolComparer);
                                metadataClass.AutoMapperClasses.Add(namedTypeSymbol, targetObjects);
                            }

                            break;
                        }
                    default: break;
                }
            }
            return metadataClass;
        }

        public MetadataClass Analyze()
        {
            var queue = FindGlobalNamespaces();
            return PopulateMetadata(queue);

        }

    }

    internal readonly record struct MetadataClass
    {
        internal Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> AutoMapperClasses { get; }

        public MetadataClass()
        {
            AutoMapperClasses = new Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>>(s_symbolComparer);
        }
    }
}

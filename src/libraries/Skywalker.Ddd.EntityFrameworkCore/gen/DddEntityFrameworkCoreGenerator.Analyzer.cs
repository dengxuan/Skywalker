using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

public partial class DddEntityFrameworkCoreGenerator
{
    protected static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.IncludeNullability;

    internal class Analyzer
    {
        private readonly INamedTypeSymbol? _dbContextSymbol;
        private readonly INamedTypeSymbol? _dbSetSymbol;
        private readonly INamedTypeSymbol? _domainEntitiySymbol;
        private readonly INamedTypeSymbol? _domainEntitiyWithPrimaryKeySymbol;
        private readonly Compilation _compilation;

        public Analyzer(in GeneratorExecutionContext context)
        {
            _compilation = context.Compilation;
            _dbContextSymbol = _compilation.GetTypeByMetadataName(Constants.DbContextSymblyName);
            _dbSetSymbol = _compilation.GetTypeByMetadataName(Constants.DbSetSymblyName);
            _domainEntitiySymbol = _compilation.GetTypeByMetadataName(Constants.DomainEntitySymbolName);
            _domainEntitiyWithPrimaryKeySymbol = _compilation.GetBestTypeByMetadataName(Constants.DomainEntitySymbolNameWithPrimaryKey);
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

                if (!assemblySymbol.Modules.Any(m => m.ReferencedAssemblies.Any(ra => ra.Name is Constants.DomainAssemblyName)))
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

                            if (namedTypeSymbol.AllInterfaces.Any(predicate => s_symbolComparer.Equals(predicate, _dbContextSymbol)))
                            {
                                var dbContextProperties = new Dictionary<INamedTypeSymbol, string>(s_symbolComparer);
                                foreach (var propertyMember in namedTypeSymbol.GetMembers())
                                {
                                    if (propertyMember is not IPropertySymbol propertySymbol)
                                    {
                                        continue;
                                    }
                                    if (propertySymbol.IsStatic || propertySymbol.IsReadOnly || propertySymbol.IsWriteOnly)
                                    {
                                        continue;
                                    }
                                    if (propertySymbol.Type is not INamedTypeSymbol dbSetNamedTypeSymbol)
                                    {
                                        continue;
                                    }
                                    if (!s_symbolComparer.Equals(dbSetNamedTypeSymbol.OriginalDefinition, _dbSetSymbol))
                                    {
                                        continue;
                                    }
                                    if (dbSetNamedTypeSymbol.TypeArguments[0] is not INamedTypeSymbol entityNameTypeSymbol)
                                    {
                                        continue;
                                    }
                                    if (!entityNameTypeSymbol.AllInterfaces.Any(predicate => s_symbolComparer.Equals(predicate, _domainEntitiySymbol)))
                                    {
                                        continue;
                                    }
                                    
                                    var genericTypeEntity = entityNameTypeSymbol.AllInterfaces.FirstOrDefault(predicate => s_symbolComparer.Equals(predicate.OriginalDefinition, _domainEntitiyWithPrimaryKeySymbol));

                                    var primaryKey = string.Empty;

                                    //Todo: 判断主键泛型参数，只能是基础数据类型
                                    if (genericTypeEntity is not null && genericTypeEntity.TypeArguments.First() is INamedTypeSymbol primaryKeyTypeSymbol)
                                    {
                                        primaryKey = primaryKeyTypeSymbol.Name;
                                    }
                                    
                                    dbContextProperties.Add(entityNameTypeSymbol, primaryKey);
                                }
                                // The DbContext is not define any DbSet<Entity> properties
                                if (dbContextProperties.Count == 0)
                                {
                                    //Todo: Diagnostics report
                                    break;
                                }
                                metadataClass.DbContextClasses.Add(namedTypeSymbol, dbContextProperties);
                                break;
                            }
                            //Todo: Care about domain entity and check if all entities are add to DbContexts
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
        internal Dictionary<INamedTypeSymbol, Dictionary<INamedTypeSymbol, string>> DbContextClasses { get; }

        public MetadataClass()
        {
            DbContextClasses = new Dictionary<INamedTypeSymbol, Dictionary<INamedTypeSymbol, string>>(s_symbolComparer);
        }
    }
}

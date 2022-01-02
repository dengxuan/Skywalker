using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

internal class CompilationAnalyzer
{
    private static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.IncludeNullability;
    private readonly INamedTypeSymbol? _dbContextSymbol;
    private readonly INamedTypeSymbol? _domainServiceSymbol;
    private readonly INamedTypeSymbol? _domainEntitieSymbol;
    private readonly GeneratorExecutionContext _context;
    private readonly Compilation _compilation;
    public string GeneratorVersion { get; }

    public CompilationAnalyzer(in GeneratorExecutionContext context, string generatorVersion)
    {
        _context = context;
        _compilation = context.Compilation;
        _dbContextSymbol = _compilation.GetTypeByMetadataName(Constants.DbContextSymblyName);
        _domainServiceSymbol = _compilation.GetTypeByMetadataName(Constants.DomainServiceSymbolName);
        _domainEntitieSymbol = _compilation.GetTypeByMetadataName(Constants.DomainEntitySymbolName);
        GeneratorVersion = generatorVersion;
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
                        continue;
                    }
                case INamedTypeSymbol namedTypeSymbol:
                    {
                        foreach (var childTypeSymbol in namedTypeSymbol.GetTypeMembers())
                        {
                            queue.Enqueue(childTypeSymbol);
                        }

                        if (namedTypeSymbol.IsStatic || namedTypeSymbol.IsAbstract)
                        {
                            continue;
                        }

                        if (namedTypeSymbol.TypeKind is not TypeKind.Class)
                        {
                            continue;
                        }
                        foreach (var item in namedTypeSymbol.AllInterfaces)
                        {
                            if (s_symbolComparer.Equals(item, _domainServiceSymbol))
                            {
                                metadataClass.DomainServices.Add(namedTypeSymbol);
                                break;
                            }
                            else if (s_symbolComparer.Equals(item, _domainEntitieSymbol))
                            {
                                metadataClass.DomainEntities.Add(namedTypeSymbol);
                                break;
                            }
                            else if (s_symbolComparer.Equals(item, _dbContextSymbol))
                            {
                                metadataClass.DbContexts.Add(namedTypeSymbol);
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        continue;
                    }
                default: continue;
            }
        }
        return metadataClass;
    }

    public MetadataClass Analyze()
    {
        var queue = FindGlobalNamespaces();
        return PopulateMetadata(queue);

    }

    internal readonly record struct MetadataClass
    {
        internal HashSet<INamedTypeSymbol> DbContexts { get; }

        internal HashSet<INamedTypeSymbol> DomainEntities { get; }

        internal HashSet<INamedTypeSymbol> DomainServices { get; }

        public MetadataClass()
        {
            DbContexts = new HashSet<INamedTypeSymbol>(s_symbolComparer);
            DomainEntities = new HashSet<INamedTypeSymbol>(s_symbolComparer);
            DomainServices = new HashSet<INamedTypeSymbol>(s_symbolComparer);
        }
    }

}

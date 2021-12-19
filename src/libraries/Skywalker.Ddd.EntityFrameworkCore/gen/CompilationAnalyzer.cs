//using Microsoft.CodeAnalysis;
//using Skywalker.Ddd.EntityFrameworkCore.CodeAnalyzers;

//namespace Skywalker.CodeAnalyzers.Analyzers;

//internal class CompilationAnalyzer
//{
//    private static readonly SymbolEqualityComparer _symbolComparer = SymbolEqualityComparer.IncludeNullability;
//    private readonly INamedTypeSymbol[] _domainServicesSymbols;
//    private readonly INamedTypeSymbol[] _domainEntitiesSymbols;
//    private readonly GeneratorExecutionContext _context;
//    private readonly Compilation _compilation;
//    public string GeneratorVersion { get; }

//    public CompilationAnalyzer(in GeneratorExecutionContext context, string generatorVersion)
//    {
//        _context = context;
//        _compilation = context.Compilation;
//        _domainServicesSymbols = new INamedTypeSymbol[]
//        {
//            _compilation.GetTypeByMetadataName($"{Constants.DomainServiceNamespace}.IDomainService`1")!.OriginalDefinition,
//            _compilation.GetTypeByMetadataName($"{Constants.DomainServiceNamespace}.IDomainService`2")!.OriginalDefinition
//        };
//        _domainEntitiesSymbols = new INamedTypeSymbol[]
//        {
//            _compilation.GetTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.IEntity")!.OriginalDefinition,
//            _compilation.GetTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.IEntity`1")!.OriginalDefinition,
//            _compilation.GetTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.Entity")!.OriginalDefinition,
//            _compilation.GetTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.Entity`1")!.OriginalDefinition,
//            _compilation.GetTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.IAggregateRoot")!.OriginalDefinition,
//            _compilation.GetTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.IAggregateRoot`1")!.OriginalDefinition,
//            _compilation.GetTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.AggregateRoot")!.OriginalDefinition,
//            _compilation.GetTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.AggregateRoot`1")!.OriginalDefinition
//        };
//        GeneratorVersion = generatorVersion;
//    }

//    private void FindGlobalNamespaces(Queue<INamespaceOrTypeSymbol> queue)
//    {
//        queue.Enqueue(_compilation.Assembly.GlobalNamespace);

//        foreach (var reference in _compilation.References)
//        {
//            if (_compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assemblySymbol)
//            {
//                continue;
//            }

//            if (!assemblySymbol.Modules.Any(m => m.ReferencedAssemblies.Any(ra => ra.Name == Constants.Namespace)))
//            {
//                continue;
//            }

//            queue.Enqueue(assemblySymbol.GlobalNamespace);
//        }
//    }

//    protected virtual void ProcessMember(Queue<INamespaceOrTypeSymbol> queue, INamespaceOrTypeSymbol member, Dictionary<INamedTypeSymbol, object?> mapping)
//    {
//        if (member is INamespaceSymbol childNsSymbol)
//        {
//            queue.Enqueue(childNsSymbol);
//            return;
//        }

//        var typeSymbol = (INamedTypeSymbol)member;

//        foreach (var childTypeSymbol in typeSymbol.GetTypeMembers())
//        {
//            queue.Enqueue(childTypeSymbol);
//        }

//        if (typeSymbol.IsStatic || typeSymbol.IsAbstract)
//        {
//            return;
//        }

//        if (typeSymbol.TypeKind is not TypeKind.Class)
//        {
//            return;
//        }
//    }

//    private void PopulateMetadata(Queue<INamespaceOrTypeSymbol> queue)
//    {
//        var repostoryMappings = new Dictionary<INamedTypeSymbol, ISymbol?>(_symbolComparer);
//        while (queue.Count > 0)
//        {
//            var nsOrTypeSymbol = queue.Dequeue();

//            switch (nsOrTypeSymbol)
//            {
//                case INamespaceSymbol nsSymbol:
//                    {
//                        foreach (var member in nsSymbol.GetMembers())
//                        {
//                            queue.Enqueue(member);
//                        }
//                        continue;
//                    }
//                case INamedTypeSymbol namedTypeSymbol:
//                    {
//                        foreach (var childTypeSymbol in namedTypeSymbol.GetTypeMembers())
//                        {
//                            queue.Enqueue(childTypeSymbol);
//                        }

//                        if (namedTypeSymbol.IsStatic || namedTypeSymbol.IsAbstract)
//                        {
//                            continue;
//                        }

//                        if (namedTypeSymbol.TypeKind is not TypeKind.Class)
//                        {
//                            continue;
//                        }

//                        // If is IEntity Or IAggregateRoot, add to entities mappings.
//                        if (_domainEntitiesSymbols.Any(predicate => _symbolComparer.Equals(predicate, namedTypeSymbol.BaseType?.OriginalDefinition)))
//                        {
//                            foreach (var item in namedTypeSymbol.GetMembers())
//                            {
//                                repostoryMappings.Add(namedTypeSymbol, item);
//                            }
//                        }

//                        continue;
//                    }
//                default: continue;
//            }
//        }
//    }

//    public void Analyze(CancellationToken cancellationToken)
//    {

//        var queue = new Queue<INamespaceOrTypeSymbol>();

//        FindGlobalNamespaces(queue);
//        PopulateMetadata(queue);

//    }
//}

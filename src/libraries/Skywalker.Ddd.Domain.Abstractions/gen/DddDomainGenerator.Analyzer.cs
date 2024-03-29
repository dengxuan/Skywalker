﻿using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Domain.Generators;

public partial class DddDomainGenerator
{
    protected static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.IncludeNullability;

    internal class Analyzer
    {
        private readonly INamedTypeSymbol? _domainServiceSymbol;
        private readonly Compilation _compilation;

        public Analyzer(in GeneratorExecutionContext context)
        {
            _compilation = context.Compilation;
            _domainServiceSymbol = _compilation.GetTypeByMetadataName(Constants.DomainServiceSymbolName);
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

                            if (namedTypeSymbol.AllInterfaces.Any(predicate => s_symbolComparer.Equals(predicate, _domainServiceSymbol)))
                            {
                                if(!metadataClass.DomainServices.TryGetValue(namedTypeSymbol,out var implImterfaces))
                                {
                                    implImterfaces = new HashSet<INamedTypeSymbol>(s_symbolComparer);
                                    metadataClass.DomainServices.Add(namedTypeSymbol, implImterfaces);
                                }
                                foreach (var item in namedTypeSymbol.AllInterfaces)
                                {
                                    if (s_symbolComparer.Equals(item, _domainServiceSymbol))
                                    {
                                        continue;
                                    }
                                    implImterfaces.Add(item);
                                }
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
        internal Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> DomainServices { get; }

        public MetadataClass()
        {
            DomainServices = new Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>>(s_symbolComparer);
        }
    }
}

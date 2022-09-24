using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using static Skywalker.Extensions.DependencyInjection.Generators.DependencyInjectionGenerator;

namespace Skywalker.Extensions.DependencyInjection.Generators;

public partial class DependencyInjectionGenerator
{
    protected static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.IncludeNullability;

    internal class Analyzer
    {
        private readonly Compilation _compilation;
        private readonly INamedTypeSymbol? _interceptorSymbol;
        private readonly INamedTypeSymbol? _scopedSymbol;
        private readonly INamedTypeSymbol? _singletonSymbol;
        private readonly INamedTypeSymbol? _transientSymbol;

        public Analyzer(in GeneratorExecutionContext context)
        {
            _compilation = context.Compilation;
            _scopedSymbol = _compilation.GetTypeByMetadataName(Constants.ScopedSymbolName);
            _singletonSymbol = _compilation.GetTypeByMetadataName(Constants.SingletonSymbolName);
            _transientSymbol = _compilation.GetTypeByMetadataName(Constants.TransientSymbolName);
            _interceptorSymbol = _compilation.GetTypeByMetadataName(Constants.InterceptorSymbolName);
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

                if (!assemblySymbol.Modules.Any(m => m.ReferencedAssemblies.Any(ra => ra.Name is Constants.AssemblyName)))
                {
                    continue;
                }
                queue.Enqueue(assemblySymbol.GlobalNamespace);
            }
            return queue;
        }

        private Metadata PopulateMetadata(Queue<INamespaceOrTypeSymbol> queue)
        {
            var classes = new Metadata();
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

                            if (namedTypeSymbol.AllInterfaces.Any(predicate => s_symbolComparer.Equals(predicate, _interceptorSymbol)))
                            {
                                var intecepter = new Intecepter(namedTypeSymbol.Name);  
                                foreach (var item in namedTypeSymbol.AllInterfaces)
                                {
                                    if (s_symbolComparer.Equals(item, _interceptorSymbol))
                                    {
                                        continue;
                                    }
                                    intecepter.Interfaces.Add(item.Name);
                                    classes.Namespaces.Add(item.ContainingNamespace.ToDisplayString());
                                }
                                foreach (var item in namedTypeSymbol.GetMembers())
                                {
                                    if (item is not IMethodSymbol methodSymbol ||
                                        methodSymbol.IsStatic ||
                                        methodSymbol.MethodKind == MethodKind.Constructor ||
                                        methodSymbol.DeclaredAccessibility != Accessibility.Public)
                                    {
                                        continue;
                                    }
                                    classes.Namespaces.Add(methodSymbol.ReturnType.ContainingNamespace.ToDisplayString());
                                    var method = new Method(methodSymbol.Name, methodSymbol.ReturnType.ToDisplayString());
                                    foreach (var parameterSymbol in methodSymbol.Parameters)
                                    {
                                        classes.Namespaces.Add(parameterSymbol.Type.ContainingNamespace.ToDisplayString());
                                        method.Arguments.Add(new KeyValuePair<string, string>(parameterSymbol.Type.ToDisplayString(), parameterSymbol.Name));
                                    }
                                    intecepter.Methods.Add(method);
                                }
                                classes.Intecepters.Add(intecepter);
                                break;
                            }
                            if (namedTypeSymbol.AllInterfaces.Any(predicate => s_symbolComparer.Equals(predicate, _scopedSymbol)))
                            {
                                var dependency = new Dependency(namedTypeSymbol.Name);
                                foreach (var item in namedTypeSymbol.AllInterfaces)
                                {
                                    if (s_symbolComparer.Equals(item, _scopedSymbol))
                                    {
                                        continue;
                                    }
                                    dependency.Interfaces.Add(item.Name);
                                    classes.Namespaces.Add(item.ContainingNamespace.ToDisplayString());
                                }
                                classes.ScopedDepedency.Add(dependency);
                                break;
                            }
                            if (namedTypeSymbol.AllInterfaces.Any(predicate => s_symbolComparer.Equals(predicate, _singletonSymbol)))
                            {
                                var dependency = new Dependency(namedTypeSymbol.Name);
                                foreach (var item in namedTypeSymbol.AllInterfaces)
                                {
                                    if (s_symbolComparer.Equals(item, _singletonSymbol))
                                    {
                                        continue;
                                    }
                                    dependency.Interfaces.Add(item.Name);
                                    classes.Namespaces.Add(item.ContainingNamespace.ToDisplayString());
                                }
                                classes.SingletonDepedency.Add(dependency);
                                break;
                            }
                            if (namedTypeSymbol.AllInterfaces.Any(predicate => s_symbolComparer.Equals(predicate, _transientSymbol)))
                            {
                                var dependency = new Dependency(namedTypeSymbol.Name);
                                foreach (var item in namedTypeSymbol.AllInterfaces)
                                {
                                    if (s_symbolComparer.Equals(item, _transientSymbol))
                                    {
                                        continue;
                                    }
                                    dependency.Interfaces.Add(item.ContainingNamespace.Name);
                                    classes.Namespaces.Add(item.ToDisplayString());
                                }
                                classes.TransientDepedency.Add(dependency);
                                break;
                            }
                            break;
                        }
                    default: break;
                }
            }
            return classes;
        }

        public Metadata Analyze()
        {
            var queue = FindGlobalNamespaces();
            return PopulateMetadata(queue);
        }

    }
}

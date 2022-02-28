// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.DependencyInjection.Generators;

public partial class DependencyInjectionGenerator
{
    internal class Parser
    {

        public static IReadOnlyList<DependencyInjectionClass> DependencyInjectionClasses(HashSet<INamedTypeSymbol> classes, CancellationToken cancellationToken)
        {
            var results = new List<DependencyInjectionClass>();
            foreach (var @class in classes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var domainServiceClass = new DependencyInjectionClass
                {
                    Name = @class.Name,
                    Fullname = @class.ToDisplayString(),
                    Namespace = @class.ContainingNamespace.ToDisplayString(),
                    ImplInterfaces = new()
                    {
                        new DependencyInjectionImplInterface
                        {
                            Name = @class.Name,
                            Fullname = @class.ToDisplayString(),
                        }
                    }
                };
                foreach (var item in @class.AllInterfaces)
                {
                    var domainServiceImplInterface = new DependencyInjectionImplInterface
                    {
                        Name = item.Name,
                        Fullname = item.ToDisplayString()
                    };
                    domainServiceClass.ImplInterfaces.Add(domainServiceImplInterface);
                }
                results.Add(domainServiceClass);
            }
            return results;
        }
    }

    internal enum Lifetime
    {
        Transient,
        Scoped,
        Singleton
    }

    internal record struct DependencyInjectionClass(string Namespace, Lifetime lifetime, string Name, string Fullname, List<DependencyInjectionImplInterface> ImplInterfaces);

    internal record struct DependencyInjectionImplInterface(string Name, string Fullname);
}

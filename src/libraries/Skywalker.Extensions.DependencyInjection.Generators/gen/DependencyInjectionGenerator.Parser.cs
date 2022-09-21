// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.DependencyInjection.Generators;

public partial class DependencyInjectionGenerator
{
    internal class Parser
    {
        public static IReadOnlyList<DbContextClass> DbContextClasses(ISet<INamedTypeSymbol> dbContextClasses, CancellationToken cancellationToken)
        {
            var results = new List<DbContextClass>();
            foreach (var dbContextClass in dbContextClasses)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var dbContexterClass = new DbContextClass
                {
                    Name = dbContextClass.Name,
                    Namespace = dbContextClass.ContainingNamespace.ToDisplayString(),
                    Interfaces = string.Join(", ", dbContextClass.AllInterfaces.Select(selector => selector.ToDisplayString())),
                    Methods = new HashSet<IMethodSymbol>(s_symbolComparer)
                };
                foreach (var item in dbContextClass.GetMembers())
                {
                    if (item is IMethodSymbol method)
                    {
                        dbContexterClass.Methods.Add(method);
                    }
                }
                results.Add(dbContexterClass);
            }
            return results;
        }
    }


    /// <summary>
    /// A DbContext struct holding a bunch of DbContext properties.
    /// </summary>
    internal record struct DbContextClass(string Namespace, string Name, string Interfaces, ISet<IMethodSymbol> Methods);
}

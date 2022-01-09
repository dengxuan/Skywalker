// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Domain.Generators;

public partial class DddDomainGenerator
{
    internal class Parser
    {

        public static IReadOnlyList<DbContextClass> DbContextClasses(Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> dbContextClasses, CancellationToken cancellationToken)
        {
            var results = new List<DbContextClass>();
            foreach (var dbContextClass in dbContextClasses)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var dbContexterClass = new DbContextClass
                {
                    Properties = new(),
                    Name = dbContextClass.Key.Name,
                    Fullname = dbContextClass.Key.ToDisplayString(),
                    Namespace = dbContextClass.Key.ContainingNamespace.ToDisplayString()
                };
                foreach (var entityNamedTypeSymbol in dbContextClass.Value)
                {
                    var primaryKey = string.Empty;
                    if (entityNamedTypeSymbol.BaseType?.TypeArguments.Length > 0 && entityNamedTypeSymbol.BaseType?.TypeArguments[0] is INamedTypeSymbol primaryKeyNameTypeSymbol)
                    {
                        primaryKey = primaryKeyNameTypeSymbol.Name;
                    }
                    // The match conditions property like public DbSet<Entity> Entities {get; set;}
                    var dbContextProperty = new DbContextProperty
                    {
                        Name = entityNamedTypeSymbol.Name,
                        Fullname = entityNamedTypeSymbol.ToDisplayString(),
                        PrimaryKey = primaryKey
                    };
                    dbContexterClass.Properties.Add(dbContextProperty);
                }
                results.Add(dbContexterClass);
            }
            return results;
        }

        public static IReadOnlyList<DomainServiceClass> DomainServiceClasses(Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> domainServicesClasses, CancellationToken cancellationToken)
        {
            var results = new List<DomainServiceClass>();
            foreach (var domainService in domainServicesClasses)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var domainServiceClass = new DomainServiceClass
                {
                    Name = domainService.Key.Name,
                    Fullname = domainService.Key.ToDisplayString(),
                    Namespace = domainService.Key.ContainingNamespace.ToDisplayString(),
                    ImplInterfaces = new()
                };
                foreach (var item in domainService.Value)
                {
                    var domainServiceImplInterface = new DomainServiceImplInterface
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


    /// <summary>
    /// A DbContext struct holding a bunch of DbContext properties.
    /// </summary>
    internal record struct DbContextClass(string Namespace, string Name, string Fullname, List<DbContextProperty> Properties);

    /// <summary>
    /// A DbContext property in a DbContext class.
    /// </summary>
    internal record struct DbContextProperty(string Name, string Fullname, string PrimaryKey);

    internal record struct DomainServiceClass(string Namespace, string Name, string Fullname, List<DomainServiceImplInterface> ImplInterfaces);

    internal record struct DomainServiceImplInterface(string Name, string Fullname);
}

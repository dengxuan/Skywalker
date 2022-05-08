// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.ObjectMapper.Generators;

public partial class ObjectMapperGenerator
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
                        PrimaryKey = primaryKey,
                        Name = entityNamedTypeSymbol.Name,
                        Namespace = entityNamedTypeSymbol.ContainingNamespace.ToDisplayString()
                    };
                    dbContexterClass.Properties.Add(dbContextProperty);
                }
                results.Add(dbContexterClass);
            }
            return results;
        }
    }


    /// <summary>
    /// A DbContext struct holding a bunch of DbContext properties.
    /// </summary>
    internal record struct DbContextClass(string Namespace, string Name, List<DbContextProperty> Properties);

    /// <summary>
    /// A DbContext property in a DbContext class.
    /// </summary>
    internal record struct DbContextProperty(string Namespace, string Name, string PrimaryKey);
}

// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

public partial class DddEntityFrameworkCoreGenerator
{
    internal class Parser
    {
        public static IReadOnlyList<DbContextClass> DbContextClasses(Dictionary<INamedTypeSymbol, Dictionary<INamedTypeSymbol,string>> dbContextClasses, CancellationToken cancellationToken)
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
                foreach (var keyValuePair in dbContextClass.Value)
                {
                    var entitySymbol = keyValuePair.Key;
                    var entityPrimaryKey = keyValuePair.Value;
                    
                    // The match conditions property like public DbSet<Entity> Entities {get; set;}
                    var dbContextProperty = new DbContextProperty
                    {
                        PrimaryKey = entityPrimaryKey,
                        Name = entitySymbol.Name,
                        Namespace = entitySymbol.ContainingNamespace.ToDisplayString()
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

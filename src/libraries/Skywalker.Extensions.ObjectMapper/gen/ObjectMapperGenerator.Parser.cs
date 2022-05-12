// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.ObjectMapper.Generators;

public partial class ObjectMapperGenerator
{
    internal class Parser
    {
        public static IReadOnlyList<SourceClass> AutoMapperClasses(Dictionary<INamedTypeSymbol, HashSet<INamedTypeSymbol>> autoMapperClasses, CancellationToken cancellationToken)
        {
            var results = new List<SourceClass>();
            foreach (var autoMapperClass in autoMapperClasses)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sourceClass = new SourceClass
                {
                    Targets = new(),
                    Name = autoMapperClass.Key.Name,
                    Namespace = autoMapperClass.Key.ContainingNamespace.ToDisplayString()
                };
                foreach (var targetNamedTypeSymbol in autoMapperClass.Value)
                {
                    var constructors = targetNamedTypeSymbol.Constructors[0].Parameters;
                    var targetCLass = new TargetClass
                    {
                        Args = new(),
                        Name = targetNamedTypeSymbol.Name,
                        Namespace = targetNamedTypeSymbol.ContainingNamespace.ToDisplayString()
                    };
                    foreach (var constructor in constructors)
                    {
                        targetCLass.Args.Add(new TargetClass
                        {
                            Name = constructor.Name,
                            Namespace = constructor.ContainingNamespace.ToDisplayString(),
                        });
                    }
                    sourceClass.Targets.Add(targetCLass);
                }
                results.Add(sourceClass);
            }
            return results;
        }
    }


    /// <summary>
    /// A DbContext struct holding a bunch of DbContext properties.
    /// </summary>
    internal record struct SourceClass(string Namespace, string Name, List<TargetClass> Targets);

    /// <summary>
    /// A DbContext property in a DbContext class.
    /// </summary>
    internal record struct TargetClass(string Namespace, string Name, bool IsValueType, List<TargetClass> Args);
}

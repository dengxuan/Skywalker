// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

public partial class DddEntityFrameworkCoreGenerator
{
    internal class Parser
    {
        private const string DbContextFullname = "Skywalker.Ddd.EntityFrameworkCore.SkywalkerDbContext<TDbContext>";
        private static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.Default;
        private readonly Compilation _compilation;
        private readonly INamedTypeSymbol? _dbSetSymbol;
        private readonly INamedTypeSymbol? _dbContextSymbol;
        private readonly INamedTypeSymbol?[] _entitiesSymbols;
        private readonly CancellationToken _cancellationToken;
        private readonly Action<Diagnostic> _reportDiagnostic;

        public Parser(Compilation compilation, Action<Diagnostic> reportDiagnostic, CancellationToken cancellationToken)
        {
            _compilation = compilation;
            _dbSetSymbol = _compilation.GetBestTypeByMetadataName(Constants.DbSetSymblyName);
            _dbContextSymbol = _compilation.GetBestTypeByMetadataName(Constants.DbContextSymblyName);
            _entitiesSymbols = new INamedTypeSymbol?[]
            {
                _compilation.GetBestTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.Entity"),
                _compilation.GetBestTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.Entity`1"),
                _compilation.GetBestTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.IEntity"),
                _compilation.GetBestTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.IEntity`1"),
                _compilation.GetBestTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.AggregateRoot"),
                _compilation.GetBestTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.AggregateRoot`1"),
                _compilation.GetBestTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.IAggregateRoot"),
                _compilation.GetBestTypeByMetadataName($"{Constants.DomainEntitiesNamespace}.IAggregateRoot`1"),
            };
            _cancellationToken = cancellationToken;
            _reportDiagnostic = reportDiagnostic;
        }

        private static bool IsOpenGeneric(INamedTypeSymbol symbol) => symbol.TypeArguments.Length > 0 && symbol.TypeArguments[0] is ITypeParameterSymbol;

        internal static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax m && m.Members.Count > 0;

        internal static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
            if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol namedTypeSymbol)
            {
                return null;
            }

            if (namedTypeSymbol.IsAbstract || namedTypeSymbol.IsStatic)
            {
                return null;
            }

            var baseTypeSymbol = namedTypeSymbol.OriginalDefinition.BaseType;
            if (baseTypeSymbol == null)
            {
                return null;
            }
            var dbContextFullname = baseTypeSymbol.OriginalDefinition.ToDisplayString();

            if (dbContextFullname != DbContextFullname)
            {
                return null;
            }
            return classDeclarationSyntax;
        }

        public IReadOnlyList<DbContextClass> DbContextClasses(IEnumerable<ClassDeclarationSyntax> classes)
        {
            var results = new List<DbContextClass>();
            foreach (var @class in classes)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                var sm = _compilation.GetSemanticModel(@class.SyntaxTree);
                if (sm.GetDeclaredSymbol(@class) is not INamedTypeSymbol classNamedTypeSymbol)
                {
                    continue;
                }
                // we only care about SkywalkerDbContext<TDbContext>
                if (s_symbolComparer.Equals(_dbContextSymbol, classNamedTypeSymbol.OriginalDefinition))
                {
                    continue;
                }
                var dbContexterClass = new DbContextClass
                {
                    Properties = new(),
                    Name = classNamedTypeSymbol.Name,
                    Fullname = classNamedTypeSymbol.ToDisplayString(),
                    Namespace = classNamedTypeSymbol.ContainingNamespace.ToDisplayString()
                };
                foreach (var member in @class.Members)
                {
                    if (member is not PropertyDeclarationSyntax property)
                    {
                        continue;
                    }
                    if (sm.GetDeclaredSymbol(property, _cancellationToken) is not IPropertySymbol propertySymbol)
                    {
                        continue;
                    }
                    if (propertySymbol.IsStatic || propertySymbol.IsReadOnly)
                    {
                        continue;
                    }
                    // The property is DbSet
                    if (!s_symbolComparer.Equals(_dbSetSymbol, propertySymbol.Type.OriginalDefinition))
                    {
                        continue;
                    }
                    if (propertySymbol.Type is not INamedTypeSymbol propertyNamedTypeSymbol)
                    {
                        continue;
                    }
                    if (propertyNamedTypeSymbol.TypeArguments[0] is not INamedTypeSymbol entityNamedTypeSymbol)
                    {
                        continue;
                    }
                    // The property type arguments is domain Entity or AggregateRoot
                    if (!_entitiesSymbols.Any(predicate => s_symbolComparer.Equals(predicate, entityNamedTypeSymbol.BaseType?.OriginalDefinition)))
                    {
                        continue;
                    }
                    var primaryKey = string.Empty;
                    if (entityNamedTypeSymbol.BaseType!.TypeArguments.Length > 0 && entityNamedTypeSymbol.BaseType?.TypeArguments[0] is INamedTypeSymbol primaryKeyNameTypeSymbol)
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
                    results.Add(dbContexterClass);
                }
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
}

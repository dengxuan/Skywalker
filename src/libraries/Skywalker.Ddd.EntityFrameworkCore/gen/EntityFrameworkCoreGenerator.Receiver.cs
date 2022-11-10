// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

public partial class EntityFrameworkCoreGenerator
{

    private static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.IncludeNullability;

    internal class Receiver : ISyntaxContextReceiver
    {
        private const string DbSetSymbolName = "Microsoft.EntityFrameworkCore.DbSet`1";
        private const string DbContextSymbolName = "Skywalker.Ddd.EntityFrameworkCore.ISkywalkerDbContext";
        private const string DomainEntitySymbolName = "Skywalker.Ddd.Domain.Entities.IEntity";
        private const string DomainEntitySymbolNameWithPrimaryKey = "Skywalker.Ddd.Domain.Entities.IEntity`1";
        private const string DomainServiceSymblyName = "Skywalker.Ddd.Domain.Services.IDomainService";

        public IList<Intecepter> Intecepters = new List<Intecepter>();
        public IList<Repository> Repositories = new List<Repository>();
        public ISet<Dependency> Dependencies = new HashSet<Dependency>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not ClassDeclarationSyntax)
            {
                return;
            }
            if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol namedTypeSymbol)
            {
                return;
            }
            var dbContextSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(DbContextSymbolName);
            if (namedTypeSymbol.AllInterfaces.Any(x => s_symbolComparer.Equals(dbContextSymbol, x)))
            {
                var dbSetSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(DbSetSymbolName);
                if (dbSetSymbol == null)
                {
                    return;
                }
                var domainEntitiySymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(DomainEntitySymbolName);
                if (domainEntitiySymbol == null)
                {
                    return;
                }
                var domainEntitiyWithPrimaryKeySymbol = context.SemanticModel.Compilation.GetBestTypeByMetadataName(DomainEntitySymbolNameWithPrimaryKey);
                if (domainEntitiyWithPrimaryKeySymbol == null)
                {
                    return;
                }
                var repository = GetRepository(namedTypeSymbol, dbSetSymbol, domainEntitiySymbol, domainEntitiyWithPrimaryKeySymbol);
                Repositories.Add(repository);

                return;
            }
            var domainServiceSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(DomainServiceSymblyName);
            if (namedTypeSymbol.AllInterfaces.Any(x => s_symbolComparer.Equals(domainServiceSymbol, x)))
            {
                var intecepter = GetIntecepter(namedTypeSymbol);
                Intecepters.Add(intecepter);
                return;
            }
        }


        /// <summary>
        /// 获取所有IAspects符号
        /// </summary>
        /// <returns></returns>
        public Intecepter GetIntecepter(INamedTypeSymbol domainServiceSymbol)
        {
            var intecepter = new Intecepter(domainServiceSymbol.Name);
            intecepter.Namespaces.Add(domainServiceSymbol.ContainingNamespace.ToDisplayString());
            foreach (var @interface in domainServiceSymbol.AllInterfaces)
            {
                intecepter.Interfaces.Add(@interface.Name);
                intecepter.Namespaces.Add(@interface.ContainingNamespace.ToDisplayString());
                var dependency = new Dependency(@interface.Name, $"{intecepter.Name}Intecepter", new HashSet<string>
                {
                    intecepter.Namespace,
                    @interface.ContainingNamespace.ToDisplayString(),
                });
                Dependencies.Add(dependency);
                foreach (var item in @interface.GetMembers())
                {
                    if (item is not IMethodSymbol methodSymbol ||
                        methodSymbol.IsStatic ||
                        methodSymbol.MethodKind == MethodKind.Constructor ||
                        methodSymbol.DeclaredAccessibility != Accessibility.Public)
                    {
                        continue;
                    }
                    intecepter.Namespaces.Add(methodSymbol.ReturnType.ContainingNamespace.ToDisplayString());
                    var method = new Method(methodSymbol.Name, true, methodSymbol.ReturnType);
                    foreach (var parameterSymbol in methodSymbol.Parameters)
                    {
                        if ((parameterSymbol.Type is IArrayTypeSymbol arrayTypeSymbol))
                        {
                            intecepter.Namespaces.Add(arrayTypeSymbol.ElementType.ContainingNamespace.ToDisplayString());
                        }
                        else
                        {
                            intecepter.Namespaces.Add(parameterSymbol.Type.ContainingNamespace.ToDisplayString());
                        }
                        method.TypedParameters.Add(new KeyValuePair<string, string>(parameterSymbol.Type.ToDisplayString(), parameterSymbol.Name));
                    }
                    intecepter.Methods.Add(method);
                }
            }

            foreach (var item in domainServiceSymbol.GetMembers())
            {
                if (item is not IMethodSymbol methodSymbol ||
                    methodSymbol.IsStatic ||
                    methodSymbol.MethodKind == MethodKind.Constructor ||
                    methodSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    continue;
                }
                intecepter.Namespaces.Add(methodSymbol.ReturnType.ContainingNamespace.ToDisplayString());
                var method = new Method(methodSymbol.Name, true, methodSymbol.ReturnType);
                foreach (var parameterSymbol in methodSymbol.Parameters)
                {
                    if ((parameterSymbol.Type is IArrayTypeSymbol arrayTypeSymbol))
                    {
                        intecepter.Namespaces.Add(arrayTypeSymbol.ElementType.ContainingNamespace.ToDisplayString());
                    }
                    else
                    {
                        intecepter.Namespaces.Add(parameterSymbol.Type.ContainingNamespace.ToDisplayString());
                    }
                    method.TypedParameters.Add(new KeyValuePair<string, string>(parameterSymbol.Type.ToDisplayString(), parameterSymbol.Name));
                }
                intecepter.Methods.Add(method);
            }
            if (domainServiceSymbol.BaseType == null)
            {
                return intecepter;
            }
            foreach (var item in domainServiceSymbol.BaseType.GetMembers())
            {
                if (item is not IMethodSymbol methodSymbol ||
                    methodSymbol.IsStatic ||
                    methodSymbol.MethodKind == MethodKind.Constructor ||
                    methodSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    continue;
                }
                intecepter.Namespaces.Add(methodSymbol.ReturnType.ContainingNamespace.ToDisplayString());
                var method = new Method(methodSymbol.Name, true, methodSymbol.ReturnType);
                foreach (var parameterSymbol in methodSymbol.Parameters)
                {
                    if ((parameterSymbol.Type is IArrayTypeSymbol arrayTypeSymbol))
                    {
                        intecepter.Namespaces.Add(arrayTypeSymbol.ElementType.ContainingNamespace.ToDisplayString());
                    }
                    else
                    {
                        intecepter.Namespaces.Add(parameterSymbol.Type.ContainingNamespace.ToDisplayString());
                    }
                    method.TypedParameters.Add(new KeyValuePair<string, string>(parameterSymbol.Type.ToDisplayString(), parameterSymbol.Name));
                }
                intecepter.Methods.Add(method);
            }
            return intecepter;
        }

        public Repository GetRepository(INamedTypeSymbol dbContextSymbol, INamedTypeSymbol dbSetSymbol, INamedTypeSymbol domainEntitiySymbol, INamedTypeSymbol domainEntitiyWithPrimaryKeySymbol)
        {
            var repository = new Repository(dbContextSymbol.Name);
            repository.Namespaces.Add(dbContextSymbol.ContainingNamespace.ToDisplayString());
            foreach (var propertyMember in dbContextSymbol.GetMembers())
            {
                if (propertyMember is not IPropertySymbol propertySymbol)
                {
                    continue;
                }
                if (propertySymbol.IsStatic || propertySymbol.IsReadOnly || propertySymbol.IsWriteOnly)
                {
                    continue;
                }
                if (propertySymbol.Type is not INamedTypeSymbol dbSetNamedTypeSymbol)
                {
                    continue;
                }
                if (!s_symbolComparer.Equals(dbSetNamedTypeSymbol.OriginalDefinition, dbSetSymbol))
                {
                    continue;
                }
                if (dbSetNamedTypeSymbol.TypeArguments[0] is not INamedTypeSymbol entityNameTypeSymbol)
                {
                    continue;
                }
                if (!entityNameTypeSymbol.AllInterfaces.Any(predicate => s_symbolComparer.Equals(predicate, domainEntitiySymbol)))
                {
                    continue;
                }
                repository.Namespaces.Add(entityNameTypeSymbol!.ContainingNamespace.ToDisplayString());
                var primaryKeyName = string.Empty;
                var primaryKeyNamespace = string.Empty;
                var genericTypeEntity = entityNameTypeSymbol.AllInterfaces.FirstOrDefault(predicate => s_symbolComparer.Equals(predicate.OriginalDefinition, domainEntitiyWithPrimaryKeySymbol));
                if (genericTypeEntity is not null && genericTypeEntity.TypeArguments.First() is INamedTypeSymbol primaryKeyTypeSymbol)
                {
                    primaryKeyNamespace = primaryKeyTypeSymbol.ContainingNamespace.ToDisplayString();
                    repository.Namespaces.Add(primaryKeyNamespace);
                    primaryKeyName = primaryKeyTypeSymbol.ToDisplayString();
                }
                var entity = new Entity(entityNameTypeSymbol.Name, primaryKeyName);
                repository.Entitiess.Add(entity);
                var dependencyNamespaces = new HashSet<string>
                {
                    entityNameTypeSymbol!.ContainingNamespace.ToDisplayString(),
                };
                if (!string.IsNullOrEmpty(primaryKeyNamespace))
                {
                    dependencyNamespaces.Add(primaryKeyNamespace!);
                }
                Dependencies.Add(new Dependency($"IRepository<{entity.EntityName}>", $"{entity.EntityName}Repository", dependencyNamespaces));
                Dependencies.Add(new Dependency($"IBasicRepository<{entity.EntityName}>", $"{entity.EntityName}Repository", dependencyNamespaces));
                Dependencies.Add(new Dependency($"IReadOnlyRepository<{entity.EntityName}>", $"{entity.EntityName}Repository", dependencyNamespaces));
                if (!string.IsNullOrEmpty(entity.PrimaryKeyName))
                {
                    Dependencies.Add(new Dependency($"IRepository<{entity.EntityName}, {entity.PrimaryKeyName}>", $"{entity.EntityName}Repository", dependencyNamespaces));
                    Dependencies.Add(new Dependency($"IBasicRepository<{entity.EntityName}, {entity.PrimaryKeyName}>", $"{entity.EntityName}Repository", dependencyNamespaces));
                    Dependencies.Add(new Dependency($"IReadOnlyRepository<{entity.EntityName}, {entity.PrimaryKeyName}>", $"{entity.EntityName}Repository", dependencyNamespaces));
                }
            }
            return repository;
        }


    }

    internal readonly record struct Dependency(string Interface, string ImplementationName, ISet<string> Namespaces);

    internal readonly record struct Entity(string EntityName, string PrimaryKeyName);

    internal readonly record struct Repository(string DbContextName)
    {
        public string Namespace { get; } = "Skywalker.Ddd.EntityFrameworkCore.Generators";

        public ISet<Entity> Entitiess { get; } = new HashSet<Entity>();

        public ISet<string> Namespaces { get; } = new HashSet<string>();
    }


    internal record struct Method(string Name, bool IsTransactional, ITypeSymbol ReturnType)
    {
        public bool IsAsync
        {
            get
            {
                return ReturnType.ContainingSymbol.ToDisplayString() == "System.Threading.Tasks" && (ReturnType.Name == "Task" || ReturnType.Name == "ValueTask");
            }
        }

        public bool IsVoid
        {
            get
            {
                if (IsAsync)
                {
                    return !((INamedTypeSymbol)ReturnType).IsGenericType;
                }
                return ReturnType.Name == "Void";
            }
        }

        public string ReturnTypeName => ReturnType.ToDisplayString();

        public List<KeyValuePair<string, string>> TypedParameters { get; } = new();

        public string Arguments => string.Join(", ", TypedParameters.Select(selector => selector.Value));

        public string Parameters => string.Join(", ", TypedParameters.Select(selector => $"{selector.Key} {selector.Value}"));

        public override int GetHashCode()
        {
            var hashCode = Name.GetHashCode();
            foreach (var item in TypedParameters)
            {
                hashCode ^= item.Key.GetHashCode();
            }
            return hashCode;
        }
    }

    internal readonly record struct Intecepter(string Name)
    {
        public string Namespace { get; } = "Skywalker.Ddd.EntityFrameworkCore.Generators";

        public ISet<string> Namespaces { get; } = new HashSet<string>();

        public ISet<string> Interfaces { get; } = new HashSet<string>();

        public ISet<Method> Methods { get; } = new HashSet<Method>(new MethodEqualityComparer());
    }

    internal class MethodEqualityComparer : EqualityComparer<Method>
    {
        public override bool Equals(Method x, Method y)
        {
            if (x.GetHashCode() != y.GetHashCode())
            {
                return false;
            }
            if (x.Name != y.Name ||
                x.TypedParameters.Count != y.TypedParameters.Count)
            {
                return false;
            }
            for (var i = 0; i < x.TypedParameters.Count; i++)
            {
                if (x.TypedParameters[i].Key != y.TypedParameters[i].Key)
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode(Method obj)
        {
            return obj.GetHashCode();
        }
    }

}

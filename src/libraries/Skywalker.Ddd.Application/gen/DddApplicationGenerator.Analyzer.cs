using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.Ddd.Application.Generators;

internal partial class DddApplicationGenerator
{
    protected static readonly SymbolEqualityComparer s_symbolComparer = SymbolEqualityComparer.IncludeNullability;

    internal class Analyzer : ISyntaxContextReceiver
    {
        private const string ApplicationServiceSymbolName = "Skywalker.Ddd.Application.Abstractions.IApplicationService";

        public ISet<Dependency> Dependencies = new HashSet<Dependency>();
        public IList<Intecepter> Intecepters = new List<Intecepter>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not ClassDeclarationSyntax)
            {
                return;
            }
            if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol namedTypeSymbol || namedTypeSymbol.IsAbstract)
            {
                return;
            }

            var applicationServiceSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(ApplicationServiceSymbolName);
            if (namedTypeSymbol.AllInterfaces.Any(x => s_symbolComparer.Equals(applicationServiceSymbol, x)))
            {
                var intecepter = GetIntecepter(applicationServiceSymbol!, namedTypeSymbol);
                Intecepters.Add(intecepter);
                return;
            }
        }

        /// <summary>
        /// 获取所有IApplicationService符号
        /// </summary>
        /// <returns></returns>
        public Intecepter GetIntecepter(INamedTypeSymbol applicationServiceSymbol, INamedTypeSymbol domainServiceSymbol)
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

            for (var baseSymbol = domainServiceSymbol; baseSymbol is not null && baseSymbol.AllInterfaces.Any(x => s_symbolComparer.Equals(applicationServiceSymbol, x)); baseSymbol = baseSymbol.BaseType)
            {
                foreach (var item in baseSymbol.GetMembers())
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
            return intecepter;
        }
    }

    internal readonly record struct Dependency(string Interface, string ImplementationName, ISet<string> Namespaces);

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

    internal readonly record struct Intecepter(string Name)
    {
        public string Namespace { get; } = "Skywalker.Ddd.Application.Generators";

        public ISet<string> Namespaces { get; } = new HashSet<string>();

        public ISet<string> Interfaces { get; } = new HashSet<string>();

        public ISet<Method> Methods { get; } = new HashSet<Method>(new MethodEqualityComparer());
    }

    internal readonly record struct MetadataClass
    {
        internal Dictionary<INamedTypeSymbol, Dictionary<INamedTypeSymbol, string>> DbContextClasses { get; }

        public MetadataClass()
        {
            DbContextClasses = new Dictionary<INamedTypeSymbol, Dictionary<INamedTypeSymbol, string>>(s_symbolComparer);
        }
    }
}

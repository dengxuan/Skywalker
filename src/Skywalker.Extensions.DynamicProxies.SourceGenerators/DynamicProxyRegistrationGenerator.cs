using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Skywalker.SourceGenerators;

namespace Skywalker.Extensions.DynamicProxies.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class DynamicProxyRegistrationGenerator : IIncrementalGenerator
{
    private const string InterceptableMetadataName = "Skywalker.Extensions.DynamicProxies.IInterceptable";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                static (context, _) => CreateModel(context.SemanticModel, (ClassDeclarationSyntax)context.Node))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!.Value)
            .Collect();

        context.RegisterSourceOutput(candidates, static (_, _) => { });
    }

    private static InterceptableServiceModel? CreateModel(SemanticModel semanticModel, ClassDeclarationSyntax declaration)
    {
        if (semanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol typeSymbol
            || !Implements(typeSymbol, InterceptableMetadataName))
        {
            return null;
        }

        return new InterceptableServiceModel(typeSymbol.GetFullyQualifiedName());
    }

    private static bool Implements(INamedTypeSymbol symbol, string metadataName)
    {
        return symbol.AllInterfaces.Any(type => type.OriginalDefinition.ToDisplayString() == metadataName);
    }

    private readonly struct InterceptableServiceModel : System.IEquatable<InterceptableServiceModel>
    {
        public InterceptableServiceModel(string serviceTypeName)
        {
            ServiceTypeName = serviceTypeName;
        }

        public string ServiceTypeName { get; }

        public bool Equals(InterceptableServiceModel other)
        {
            return ServiceTypeName == other.ServiceTypeName;
        }

        public override bool Equals(object? obj)
        {
            return obj is InterceptableServiceModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ServiceTypeName.GetHashCode();
        }
    }
}
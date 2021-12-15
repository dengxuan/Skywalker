using Microsoft.CodeAnalysis;

namespace Skywalker.CodeAnalyzers.Analyzers;

internal sealed class RequestMessageHandlerWrapper
{
    public readonly string MessageType;
    private readonly CompilationAnalyzer _analyzer;

    public RequestMessageHandlerWrapper(string messageType, CompilationAnalyzer analyzer)
    {
        MessageType = messageType;
        _analyzer = analyzer;
    }

    public string FullNamespace => $"global::{_analyzer.ApplicationNamespace}";

    public string HandlerWrapperTypeName(TypeKind type) => $"{MessageType}{(type == TypeKind.Struct ? "Struct" : "Class")}HandlerWrapper";

    public string HandlerWrapperTypeFullName(TypeKind type) => $"{FullNamespace}.{HandlerWrapperTypeName(type)}";

    public string HandlerWrapperTypeNameWithGenericTypeArguments(TypeKind type) => $"{HandlerWrapperTypeName(type)}<TRequest, TResponse>";

    public string HandlerWrapperTypeNameWithGenericTypeArguments(INamedTypeSymbol requestSymbol, INamedTypeSymbol responseSymbol) => $"{HandlerWrapperTypeFullName(requestSymbol.TypeKind)}<{requestSymbol.GetTypeSymbolFullName()}, {responseSymbol.GetTypeSymbolFullName()}>";

    public string HandlerWrapperTypeOfExpression(TypeKind type) => $"typeof({HandlerWrapperTypeFullName(type)}<,>)";

    public string ClassHandlerWrapperTypeNameWithGenericTypeArguments => HandlerWrapperTypeNameWithGenericTypeArguments(TypeKind.Class);

    public string StructHandlerWrapperTypeNameWithGenericTypeArguments => HandlerWrapperTypeNameWithGenericTypeArguments(TypeKind.Struct);

    public string ClassHandlerWrapperTypeName => HandlerWrapperTypeName(TypeKind.Class);

    public string StructHandlerWrapperTypeName => HandlerWrapperTypeName(TypeKind.Struct);
}

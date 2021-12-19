﻿using Microsoft.CodeAnalysis;

namespace Skywalker.CodeAnalyzers.Analyzers;

internal sealed class RequestMessage : SymbolMetadata<RequestMessage>
{
    public RequestMessageHandler? Handler { get; private set; }

    public readonly INamedTypeSymbol ResponseSymbol;

    public readonly RequestMessageHandlerWrapper WrapperType;

    public readonly string MessageType;

    public RequestMessage(INamedTypeSymbol symbol, INamedTypeSymbol responseSymbol, string messageType, CompilationAnalyzer analyzer)
        : base(symbol, analyzer)
    {
        ResponseSymbol = responseSymbol;
        WrapperType = analyzer.RequestMessageHandlerWrappers.Single(w => w.MessageType == messageType);
        MessageType = messageType;
    }

    public string RequestFullName => Symbol.GetTypeSymbolFullName();
    public string ResponseFullName => ResponseSymbol!.GetTypeSymbolFullName();

    public void SetHandler(RequestMessageHandler handler) => Handler = handler;

    public string HandlerWrapperTypeNameWithGenericTypeArguments =>
        WrapperType.HandlerWrapperTypeNameWithGenericTypeArguments(Symbol, ResponseSymbol);

    public string PipelineHandlerType =>
        $"global::Skywalker.Ddd.Application.Pipeline.IPipelineBehavior<{RequestFullName}, {ResponseFullName}>";


    public string HandlerWrapperPropertyName =>
       $"Wrapper_For_{Symbol.GetTypeSymbolFullName(withGlobalPrefix: false, includeTypeParameters: false).Replace("global::", "").Replace('.', '_')}";

    public string SyncMethodName => "QueryAsync";
    public string AsyncMethodName => "QueryAsync";

    public string SyncReturnType => ResponseSymbol.GetTypeSymbolFullName();
    public string AsyncReturnType => Analyzer
        .Compilation
        .GetTypeByMetadataName(typeof(ValueTask<>).FullName!)!
        .Construct(ResponseSymbol)
        .GetTypeSymbolFullName();
}
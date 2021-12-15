using Microsoft.CodeAnalysis;

namespace Skywalker.CodeAnalyzers.Analyzers;

internal sealed class RequestMessageHandler : MessageHandler<RequestMessageHandler>
{
    public readonly string MessageType;
    public readonly RequestMessageHandlerWrapper WrapperType;

    public RequestMessageHandler(INamedTypeSymbol symbol, string messageType, CompilationAnalyzer analyzer)
        : base(symbol, analyzer)
    {
        MessageType = messageType;
        WrapperType = analyzer.RequestMessageHandlerWrappers.Single(w => w.MessageType == messageType);
    }
}

using Microsoft.CodeAnalysis;

namespace Skywalker.CodeAnalyzers.Analyzers;

internal sealed class CommandMessage : SymbolMetadata<CommandMessage>
{
    private readonly HashSet<CommandMessageHandler> _handlers;

    public CommandMessage(INamedTypeSymbol symbol, CompilationAnalyzer analyzer)
        : base(symbol, analyzer)
    {
        _handlers = new();
    }

    internal void AddHandlers(CommandMessageHandler handler) => _handlers.Add(handler);

    public string FullName => Symbol.GetTypeSymbolFullName();

    public int HandlerCount => _handlers.Count;

    public string ServiceLifetime => Analyzer.ServiceLifetime;

    public string HandlerTypeOfExpression => $"typeof(global::Skywalker.Ddd.Application.ICommandHandler<{Symbol.GetTypeSymbolFullName()}>)";

    public IEnumerable<string> HandlerServicesRegistrationBlock
    {
        get
        {
            foreach (var handler in _handlers)
            {
                var getExpression = $"sp => sp.GetRequiredService<{handler.FullName}>()";
                yield return $"services.Add(new SD({HandlerTypeOfExpression}, {getExpression}, {ServiceLifetime}));";
            }
        }
    }
}

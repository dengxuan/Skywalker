using Microsoft.CodeAnalysis;

namespace Skywalker.CodeAnalyzers.Analyzers;

internal sealed class CommandMessageHandler : MessageHandler<CommandMessageHandler>
{
    public CommandMessageHandler(INamedTypeSymbol symbol, CompilationAnalyzer analyzer)
        : base(symbol, analyzer)
    {
    }

    public string OpenGenericTypeOfExpression =>
        $"typeof(global::Skywalker.Ddd.Application.ICommandHandler<>)";

    public string OpenGenericServiceRegistrationBlock =>
        $"services.Add(new SD({OpenGenericTypeOfExpression}, {TypeOfExpression(false)}, {ServiceLifetime}));";
}

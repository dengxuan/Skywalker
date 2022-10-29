using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Application.Generators;

[Generator]
internal partial class DddApplicationGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is Analyzer receiver)
        {
            Emitter.Emit(context, receiver.Intecepters);
            Emitter.Emit(context, receiver.Dependencies);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new Analyzer());
    }
}

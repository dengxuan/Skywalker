using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.DependencyInjection.Generators;

[Generator]
public partial class DependencyInjectionGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver || receiver.Classes.Count == 0)
        {
            return;
        }
        var classes = Parser.DependencyInjectionClasses(receiver.Classes, context.CancellationToken);
        Emitter.Emit(context, classes);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(SyntaxContextReceiver.Create);
    }
}

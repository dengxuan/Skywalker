using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.ObjectMapper.Generators;

[Generator]
public partial class ObjectMapperGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var compilationAnalyzer = new Analyzer(in context);
        var metadataClass = compilationAnalyzer.Analyze();
        var dbContextClasses = Parser.AutoMapperClasses(metadataClass.AutoMapperClasses, context.CancellationToken);
        Emitter.Emit(context, dbContextClasses);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        //context.RegisterForSyntaxNotifications(SyntaxContextReceiver.Create);
    }
}

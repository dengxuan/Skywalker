using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Application.Generators;

[Generator]
public partial class DddApplicationGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var compilationAnalyzer = new Analyzer(in context);
        var metadataClass = compilationAnalyzer.Analyze();
        var dbContextClasses = Builder.DbContextClasses(metadataClass.DbContextClasses, context.CancellationToken);
        Emitter.Emit(context, dbContextClasses);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}

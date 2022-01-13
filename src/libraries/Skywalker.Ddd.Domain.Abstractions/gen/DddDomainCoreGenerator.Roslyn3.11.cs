using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Domain.Generators;

[Generator]
public partial class DddDomainGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var compilationAnalyzer = new Analyzer(in context);
        var metadataClass = compilationAnalyzer.Analyze();
        var domainServiceClasses = Parser.DomainServiceClasses(metadataClass.DomainServices, context.CancellationToken);
        Emitter.Emit(context, domainServiceClasses);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }
}

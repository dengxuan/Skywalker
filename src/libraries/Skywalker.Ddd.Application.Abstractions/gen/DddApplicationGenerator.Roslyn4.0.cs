using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Skywalker.Ddd.Application;
using System.Text;

namespace Skywalker.CodeAnalyzers.Analyzers.SourceGenerator;

[Generator]
public sealed partial class ApplicationGenerator : IIncrementalGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            ExecuteInternal(in context);
        }
        catch (Exception exception)
        {
            context.ReportGenericError(exception);

            throw;
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context) => throw new NotImplementedException();

    private void ExecuteInternal(in GeneratorExecutionContext context)
    {
        var generatorAssembly = GetType().Assembly;
        var generatorVersion = generatorAssembly.GetName().Version!.ToString();

        GenerateOptionsAttribute(in context, generatorVersion);

        var compilationAnalyzer = new CompilationAnalyzer(in context, generatorVersion);
        compilationAnalyzer.Analyze(context.CancellationToken);

        if (compilationAnalyzer.HasErrors)
        {
            return;
        }

        var mediatorImplementationGenerator = new ApplicationImplementationGenerator();
        mediatorImplementationGenerator.Generate(in context, compilationAnalyzer);
    }

    private void GenerateOptionsAttribute(in GeneratorExecutionContext context, string generatorVersion)
    {
        var model = new { GeneratorVersion = generatorVersion };

    }

}

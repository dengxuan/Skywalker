using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Skywalker.Ddd.Application;
using System.Text;

namespace Skywalker.CodeAnalyzers.Analyzers.SourceGenerator;

[Generator]
public sealed partial class ApplicationGenerator : ISourceGenerator
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
        //var model = new { GeneratorVersion = generatorVersion };

        //var file = @"Templates/ApplicationOptionsAttribute.sbn-cs";
        //var template = Template.Parse(EmbeddedResource.GetContent(file), file);
        //var output = template.Render(model, member => member.Name);
        //context.AddSource("ApplicationOptionsAttribute.g.cs", SourceText.From(output, Encoding.UTF8));
    }
    
    public void Initialize(GeneratorInitializationContext context)
    {
    }
}

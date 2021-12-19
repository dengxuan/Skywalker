using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Skywalker.Ddd.Application;
using System.Text;

namespace Skywalker.CodeAnalyzers.Analyzers.SourceGenerator;

internal sealed partial class ApplicationImplementationGenerator
{
    internal void Generate(in GeneratorExecutionContext context, CompilationAnalyzer compilationAnalyzer)
    {
        //var compilation = context.Compilation;

        //var file = @"Templates/Application.sbn-cs";

        //var template = Template.Parse(EmbeddedResource.GetContent(file), file);
        //var output = template.Render(compilationAnalyzer, member => member.Name);

        //context.AddSource("Application.g.cs", SourceText.From(output, Encoding.UTF8));
    }
}

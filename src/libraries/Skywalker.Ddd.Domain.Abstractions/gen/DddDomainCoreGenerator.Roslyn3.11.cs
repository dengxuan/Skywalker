using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Skywalker.Ddd.Domain.Generators;

[Generator]
public class DomainServiceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var _generator_assembly = GetType().Assembly;
        var generatorVersion = _generator_assembly.GetName().Version!.ToString();
        var compilationAnalyzer = new CompilationAnalyzer(in context, generatorVersion);
        compilationAnalyzer.Analyze();
        context.AddSource("Skywalker.g.cs", SourceText.From("class A {}", Encoding.UTF8));
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }
}

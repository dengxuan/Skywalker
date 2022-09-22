// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Extensions.DependencyInjection.Generators;

[Generator]
public partial class DependencyInjectionGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var compilationAnalyzer = new Analyzer(in context);
        var matadata = compilationAnalyzer.Analyze();
        Emitter.Emit(context, matadata);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

}

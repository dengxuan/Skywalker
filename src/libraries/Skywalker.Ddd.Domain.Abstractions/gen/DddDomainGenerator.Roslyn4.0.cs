// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Domain.Generators;

[Generator]
public partial class DddDomainGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var compilationAnalyzer = new Analyzer(in context);
        var metadataClass = compilationAnalyzer.Analyze();
        System.Diagnostics.Debugger.Launch();
        var parser = new Parser(context.CancellationToken);
        var dbContextClasses = parser.DbContextClasses(metadataClass.DbContextClasses);
        Emitter.Emit(context, dbContextClasses);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

}

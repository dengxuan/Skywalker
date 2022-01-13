// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

[Generator]
public partial class DddEntityFrameworkCoreGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var compilationAnalyzer = new Analyzer(in context);
        var metadataClass = compilationAnalyzer.Analyze();
        var dbContextClasses = Parser.DbContextClasses(metadataClass.DbContextClasses, context.CancellationToken);
        Emitter.Emit(context, dbContextClasses);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

}

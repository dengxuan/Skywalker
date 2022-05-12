// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Skywalker.Extensions.ObjectMapper.Generators;

[Generator]
public partial class ObjectMapperGenerator : ISourceGenerator
{

    public void Execute(GeneratorExecutionContext context)
    {
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
        var compilationAnalyzer = new Analyzer(in context);
        var metadataClass = compilationAnalyzer.Analyze();
        var sourceClasses = Parser.AutoMapperClasses(metadataClass.AutoMapperClasses, context.CancellationToken);
        Emitter.Emit(context, sourceClasses);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        //context.RegisterForSyntaxNotifications(SyntaxContextReceiver.Create);
    }

}

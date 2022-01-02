// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

[Generator]
public partial class DddEntityFrameworkCoreGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var generatorAssembly = GetType().Assembly;
        var generatorVersion = generatorAssembly.GetName().Version!.ToString();
        var compilationAnalyzer = new CompilationAnalyzer(in context, generatorVersion);
        var metadataClass = compilationAnalyzer.Analyze();

        //if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver || receiver.ClassDeclarations.Count == 0)
        //{
        //    // nothing to do yet
        //    return;
        //}
        //var parser = new Parser(context.Compilation, context.ReportDiagnostic, context.CancellationToken);
        //var dbContextClasses = parser.DbContextClasses(receiver.ClassDeclarations);
        //if (dbContextClasses.Count > 0)
        //{
        //    Emitter.Emit(context, dbContextClasses);
        //}
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        //context.RegisterForSyntaxNotifications(SyntaxContextReceiver.Create);
    }

    private sealed class SyntaxContextReceiver : ISyntaxContextReceiver
    {
        internal static ISyntaxContextReceiver Create()
        {
            return new SyntaxContextReceiver();
        }

        public HashSet<ClassDeclarationSyntax> ClassDeclarations { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (Parser.IsSyntaxTargetForGeneration(context.Node))
            {
                var classSyntax = Parser.GetSemanticTargetForGeneration(context);
                if (classSyntax != null)
                {
                    ClassDeclarations.Add(classSyntax);
                }
            }
        }
    }
}

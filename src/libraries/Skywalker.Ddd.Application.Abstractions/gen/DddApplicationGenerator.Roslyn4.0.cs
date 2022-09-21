// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Application.Generators;

[Generator]
public partial class DddApplicationGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        Debugger.Launch();
        if (context.SyntaxReceiver is Analyzer receiver)
        {
            var builders = receiver
                .GetAspectsTypes(context.Compilation)
                .Select(i => new Builder(i))
                .Distinct();
            foreach (var builder in builders)
            {
                var build =  builder.Build();
                Emitter.Emit(context, build);
                //context.AddSource($"{builder.AspectsTypeName}Proxy", builder.ToSourceText());
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new Analyzer());
    }

}

// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Domain.Generators;

[Generator]
public partial class DddDomainGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver || receiver.MetadataClasses.DomainServices.Count > 0)
        {
            return;
        }
        var domainServiceClasses = Parser.DomainServiceClasses(receiver.MetadataClasses.DomainServices, context.CancellationToken);
        Emitter.Emit(context, domainServiceClasses);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(SyntaxContextReceiver.Create);
    }

}

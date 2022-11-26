using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.Generators;


/// <summary>
/// 
/// </summary>
[Generator]
public partial class EntityFrameworkCoreGenerator : ISourceGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is Receiver receiver)
        {
            Emitter.Emit(context, receiver.Intecepters);
            Emitter.Emit(context, receiver.Repositories);
            Emitter.EmitDomainServiceDependencies(context, receiver.DomainServiceDependencies);
            Emitter.EmitRepositoryDependencies(context, receiver.RepositoryDependencies);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new Receiver());
    }

}

using Microsoft.CodeAnalysis;
using Skywalker.Aspects.CodeAnalysis.Analyzers;

namespace Skywalker.Aspects.CodeAnalysis.Analyzers.Providers;

/// <summary>
/// public修饰符号诊断
/// </summary>
sealed class ModifierDiagnosticProvider : HttpApiDiagnosticProvider
{
    public override DiagnosticDescriptor Descriptor => Descriptors.ModifierDescriptor;

    /// <summary>
    /// public修饰符号诊断
    /// </summary>
    /// <param name="context"></param>
    public ModifierDiagnosticProvider(HttpApiContext context)
        : base(context)
    {
    }

    public override IEnumerable<Diagnostic> CreateDiagnostics()
    {
        var syntax = Context.Syntax;
        var isVisiable = syntax.Modifiers.Any(item => "public".Equals(item.ValueText));
        if (isVisiable == false)
        {
            var location = syntax.Identifier.GetLocation();
            yield return CreateDiagnostic(location);
        }
    }
}

using Microsoft.CodeAnalysis;

namespace Skywalker.SourceGenerators;

/// <summary>
/// Common diagnostics shared by all Skywalker source generators. Generator-specific diagnostics
/// (SKY1xxx-SKY5xxx) live in their own generator project.
/// </summary>
/// <remarks>
/// Diagnostic ID segment allocation is documented in <c>docs/architecture/source-generators-spec.md §1.3</c>.
/// Human-readable docs for each ID live under <c>docs/diagnostics/SKYxxxx.md</c>.
/// </remarks>
internal static class DiagnosticDescriptors
{
    private const string Category = "Skywalker.SourceGenerators";
    private const string HelpLinkBase = "https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/";

    /// <summary>
    /// SKY9001 — class marked for SG processing must be declared <c>partial</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor MustBePartial = new(
        id: "SKY9001",
        title: "Service class must be partial",
        messageFormat: "Class '{0}' marked with [{1}] must be declared as partial",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Skywalker source generators emit additional members into your class. The class must be declared as 'partial' so the compiler can merge them.",
        helpLinkUri: HelpLinkBase + "SKY9001.md");
}

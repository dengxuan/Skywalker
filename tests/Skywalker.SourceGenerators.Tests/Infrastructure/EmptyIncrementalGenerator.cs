using Microsoft.CodeAnalysis;

namespace Skywalker.SourceGenerators.Tests.Infrastructure;

internal sealed class EmptyIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
    }
}
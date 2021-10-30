using Skywalker;
using Skywalker.Ddd.Tracing;

namespace Microsoft.Extensions.DependencyInjection;

internal static class TracingSkywalkerBuilderExtensions
{
    public static SkywalkerDddBuilder AddTracing(this SkywalkerDddBuilder builder)
    {
        builder.Services.AddSingleton<ICorrelationIdProvider, DefaultCorrelationIdProvider>();
        return builder;
    }
}

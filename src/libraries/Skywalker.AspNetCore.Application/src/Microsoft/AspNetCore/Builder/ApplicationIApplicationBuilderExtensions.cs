using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Pipeline.Abstractions;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationIApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplication(this IApplicationBuilder builder, Action<IPipelineChainBuilder> options)
    {
        var chainBuilder = builder.ApplicationServices.GetRequiredService<IPipelineChainBuilder>();
        options?.Invoke(chainBuilder);
        return builder;
    }
}

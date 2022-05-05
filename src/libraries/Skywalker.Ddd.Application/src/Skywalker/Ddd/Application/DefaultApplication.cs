using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Application.Pipeline;
using Skywalker.Ddd.Application.Pipeline.Abstractions;

namespace Skywalker.Ddd.Application;

internal class DefaultApplication : IApplication
{
    private readonly InterceptDelegate _pipeline;
    private readonly IServiceProvider _serviceProvider;

    public DefaultApplication(IServiceProvider serviceProvider, IPipelineChainBuilder pipelineChainBuilder)
    {
        _serviceProvider = serviceProvider;
        _pipeline = pipelineChainBuilder.Build();
    }

    public async ValueTask ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        var handler = _serviceProvider.GetRequiredService<IApplicationHandler<TRequest>>();
        await _pipeline(new PipelineContext(handler, async (PipelineContext context) =>
        {
            await handler!.HandleAsync(request, cancellationToken);
        }, request, cancellationToken));
    }

    public async ValueTask<TResponse?> ExecuteAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto where TResponse : IResponseDto
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        var handler = _serviceProvider.GetRequiredService<IApplicationHandler<TRequest, TResponse>>();
        var context = new PipelineContext(handler, async (PipelineContext context) =>
        {
            context.ReturnValue = await handler.HandleAsync(request, cancellationToken);
        }, request, cancellationToken);
        await _pipeline(context);
        return (TResponse?)context.ReturnValue!;
    }
}

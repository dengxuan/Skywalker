using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.EventBus.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Pipeline;
using Skywalker.Spider.Pipeline.Abstractions;
using Skywalker.Spider.Scheduler.Abstractions;
using System;
using System.Threading.Tasks;

namespace Skywalker.Spider;

public class ResponseEventHandler : IEventHandler<Response>
{
    private readonly IScheduler _scheduler;

    private readonly InProgressRequests _inProgressRequests;

    private readonly ILogger<ResponseEventHandler> _logger;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ResponseEventHandler(IScheduler scheduler, InProgressRequests inProgressRequests, ILogger<ResponseEventHandler> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _scheduler = scheduler;
        _inProgressRequests = inProgressRequests;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleEventAsync(Response response)
    {
        Request? request = _inProgressRequests.Dequeue(response.RequestHash);
        if (request == null)
        {
            return;
        }
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            SpiderOptions options = scope.ServiceProvider.GetRequiredService<IOptions<SpiderOptions>>().Value;
            using var context = new PipelineContext(scope.ServiceProvider, options, request, response);
            var pipelines = scope.ServiceProvider.GetServices<IPipeline>();
            foreach (var pipeline in pipelines)
            {
                await pipeline.HandleAsync(context);
            }

            var count = await _scheduler.EnqueueAsync(context.FollowRequests);
            await _scheduler.SuccessAsync(request);
        }
        //catch (ExitException ee)
        //{
        //    Logger.LogError($"Exit: {ee}");
        //    await ExitAsync();
        //}
        catch (Exception e)
        {
            await _scheduler.FailAsync(request);
            _logger.LogError($" handle {System.Text.Json.JsonSerializer.Serialize(request)} failed: {e}");
        }
    }
}

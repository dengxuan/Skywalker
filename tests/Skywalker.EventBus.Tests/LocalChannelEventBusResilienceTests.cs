using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus.Abstractions;
using Skywalker.EventBus.Local;

namespace Skywalker.EventBus.Tests;

/// <summary>
/// #309：单个处理器抛异常不能杀死消费循环，后续事件必须继续投递。
/// </summary>
public class LocalChannelEventBusResilienceTests
{
    public class ResilienceEvent
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ThrowingEventHandler : IEventHandler<ResilienceEvent>
    {
        public Task HandleEventAsync(ResilienceEvent eventData)
        {
            throw new InvalidOperationException("poisoned handler");
        }
    }

    public class RecordingEventHandler : IEventHandler<ResilienceEvent>
    {
        public static List<ResilienceEvent> ReceivedEvents { get; } = [];

        public Task HandleEventAsync(ResilienceEvent eventData)
        {
            ReceivedEvents.Add(eventData);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ProcessingLoop_SurvivesHandlerFailure_AndKeepsDispatchingSubsequentEvents()
    {
        // Arrange：同一事件先经过抛异常的处理器，再经过记录处理器
        var services = new ServiceCollection();
        services.AddEventBusLocal(options =>
        {
            options.AddEventHandler<ThrowingEventHandler>();
            options.AddEventHandler<RecordingEventHandler>();
        });

        await using var provider = services.BuildServiceProvider();
        var eventBus = provider.GetRequiredService<ILocalEventBus>();
        RecordingEventHandler.ReceivedEvents.Clear();

        // Act：第一条事件毒化 ThrowingEventHandler；第二条事件验证循环仍存活
        await eventBus.PublishAsync(new ResilienceEvent { Message = "first" });
        await eventBus.PublishAsync(new ResilienceEvent { Message = "second" });

        for (var i = 0; i < 20 && RecordingEventHandler.ReceivedEvents.Count < 2; i++)
        {
            await Task.Delay(100);
        }

        // Assert：两条事件都送达了记录处理器（抛异常的处理器没有杀死循环）
        Assert.Equal(2, RecordingEventHandler.ReceivedEvents.Count);
        Assert.Equal("first", RecordingEventHandler.ReceivedEvents[0].Message);
        Assert.Equal("second", RecordingEventHandler.ReceivedEvents[1].Message);
    }
}

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Skywalker.EventBus.MemoryChannels;

internal class ConnectionFactoryFactory : IConnectionFactory
{

    private readonly ILogger<ConnectionFactoryFactory> _logger;

    private readonly ConcurrentDictionary<string, IChannelModel> _channelModels = new();

    private readonly IChannelMessageConsumer _messageConsumer;

    public ConnectionFactoryFactory(IChannelMessageConsumer messageConsumer, ILogger<ConnectionFactoryFactory> logger)
    {
        _messageConsumer = messageConsumer;
        _logger = logger;
    }

    private IChannelModel CreateChannelModel(string routingKey)
    {
        Channel<Message> channel = Channel.CreateUnbounded<Message>();
        return new MemoryChannel(routingKey, channel);
    }

    public IConnection CreateChannel()
    {
        IChannelModel channelModel = _channelModels.GetOrAdd("memory-channel", CreateChannelModel);
        channelModel.OnReceived += _messageConsumer.InvokeAsync;
        IConnection connection = (IConnection)channelModel;
        connection.OnClosed += Connection_OnClosed;
        connection.Open();
        return connection;
    }

    private Task Connection_OnClosed(string routingKey)
    {
        _channelModels.TryRemove(routingKey, out _);
        return Task.CompletedTask;
    }
}

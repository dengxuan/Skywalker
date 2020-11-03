using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Terminal.Abstractions;
using Skywalker.Lightning.Messaging;
using Skywalker.Lightning.Serializer;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Lightning
{
    public class LightningTerminal : ILightningTerminal
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<LightningResponse>> _resultCallbackTask = new ConcurrentDictionary<string, TaskCompletionSource<LightningResponse>>();
        private readonly IChannel _channel;
        private readonly IEventLoopGroup _loopGroup;
        private readonly IMessageListener _messageListener;
        private readonly ILogger<LightningTerminal> _logger;
        private readonly string _node;
        private readonly ILightningSerializer _LightningSerializer;

        internal LightningTerminal(IChannel channel, IEventLoopGroup loopGroup, IMessageListener messageListener, ILogger<LightningTerminal> logger, string node, ILightningSerializer LightningSerializer)
        {
            _channel = channel;
            _loopGroup = loopGroup;
            _messageListener = messageListener;
            _logger = logger;
            _node = node;
            _LightningSerializer = LightningSerializer;
            _messageListener.OnReceived += OnReceived;
        }

        private void OnReceived(LightningMessage<LightningResponse> message)
        {
            if (_resultCallbackTask.TryGetValue(message.Id, out var task))
            {
                task.TrySetResult(message.Body);
            }
            else
            {
                _logger.LogWarning($"The message callback wait task was not found, probably because the timeout has been canceled waiting.[message id:{message.Id}]");

            }
        }

        public async Task<LightningResponse> SendAsync(LightningRequest message)
        {
            var lightningMessage = new LightningMessage<LightningRequest>(Guid.NewGuid().ToString("N"), message);
            _logger.LogTrace($"Sending message to node {_node}:\nMessage id:{lightningMessage.Id}\nArgs:{_LightningSerializer.Serialize(message)}\n\n");
            var tcs = new TaskCompletionSource<LightningResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var ct = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // Todo: Configuration
            ct.Token.Register(() =>
            {
                tcs.TrySetResult(new LightningResponse("Remoting invoke timeout!"));
                _logger.LogWarning("Remoting invoke timeout,You can set the wait time with the Remoting_Invoke_CancellationTokenSource_Timeout option.Send to node:{_node} Message id:{transportMessage.Id} ", _node, lightningMessage.Id);
            }, false);

            if (!_resultCallbackTask.TryAdd(lightningMessage.Id, tcs))
            {
                throw new Exception("Failed to send.");
            }
            try
            {
                await _channel.WriteAndFlushAsync(lightningMessage);
                _logger.LogTrace($"Send completed, waiting for node {_node} to return results:Message id:{lightningMessage.Id}");
                var result = await tcs.Task;
                _logger.LogTrace($"The Terminal received the return result of node {_node}:Message id:{lightningMessage.Id} Body:{result}");
                return result;
            }
            finally
            {
                _resultCallbackTask.TryRemove(lightningMessage.Id, out var value);
                value?.TrySetCanceled();
            }
        }

        public async Task DisconnectAsync()
        {
            _logger.LogTrace($"Stopping Terminal.[{_channel.LocalAddress}]");
            foreach (var task in _resultCallbackTask.Values)
            {
                task.TrySetCanceled();
            }

            _resultCallbackTask.Clear();
            if (_channel.Open)
            {
                await _channel.CloseAsync();
                await _loopGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }

            _logger.LogTrace($"The Terminal[{_channel.LocalAddress}] has stopped.");
        }
    }
}

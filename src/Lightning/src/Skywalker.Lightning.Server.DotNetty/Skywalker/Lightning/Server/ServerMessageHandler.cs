using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Skywalker.Lightning;
using Skywalker.Lightning.Messaging;
using Skywalker.Lightning.Server;
using Skywalker.Lightning.Server.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Server
{
    public class ServerMessageHandler : ChannelHandlerAdapter
    {
        private readonly ILogger<ServerMessageHandler> _logger;
        private readonly ILightningServiceFactory _lightningServiceFactory;

        public ServerMessageHandler(ILightningServiceFactory LightningServiceFactory, ILogger<ServerMessageHandler> logger)
        {
            _lightningServiceFactory = LightningServiceFactory;
            _logger = logger;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Task.Run(async () =>
            {
                if (message is LightningMessage<LightningRequest> lightningMessage)
                {
                    try
                    {
                        _logger.LogTrace($"The server received the message:\nCurrent node:{0}\nRoute:{lightningMessage.Id}\nMessage id:{lightningMessage.Id}\nArgs:{lightningMessage.Body.Parameters?.JoinAsString("|")}");
                        LightningServiceDescriptor descriptor = _lightningServiceFactory.GetLightningServiceDescriptor(lightningMessage.Body.ServiceName);
                        var result = await descriptor.InvokeHandler(lightningMessage.Body.Parameters!);
                        await context.WriteAndFlushAsync(new LightningMessage<LightningResponse>(lightningMessage.Id, new LightningResponse(result)));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"The server message processing failed:{ex.Message}. Current node:{0} Route:{lightningMessage.Body.ServiceName}.{lightningMessage.Body.Parameters} Message id:{lightningMessage.Id}");
                        await context.WriteAndFlushAsync(new LightningMessage<LightningResponse>(lightningMessage.Id, new LightningResponse()));
                    }
                }
            });
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
        }
    }
}

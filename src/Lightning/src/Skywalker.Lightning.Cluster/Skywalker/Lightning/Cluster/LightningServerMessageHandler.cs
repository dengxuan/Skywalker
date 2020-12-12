using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Cluster
{
    public class LightningServerMessageHandler : ChannelHandlerAdapter
    {
        private static volatile IChannelGroup _groups;

        private int _lossConnectCount = 0;

        private readonly ILightningDescriptorResolver _descriptorResolver;
        private readonly ILogger<LightningServerMessageHandler> _logger;

        public LightningServerMessageHandler(ILightningDescriptorResolver descriptorResolver, ILogger<LightningServerMessageHandler> logger)
        {
            _descriptorResolver = descriptorResolver;
            _logger = logger;
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            Console.WriteLine("已经15秒未收到客户端的消息了！");
            if (evt is IdleStateEvent eventState)
            {
                if (eventState.State == IdleState.ReaderIdle)
                {
                    _lossConnectCount++;
                    if (_lossConnectCount > 2)
                    {
                        Console.WriteLine("关闭这个不活跃通道！");
                        context.CloseAsync();
                    }
                }
            }
            else
            {
                base.UserEventTriggered(context, evt);
            }
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
                        //LightningServiceDescriptor descriptor = _lightningServiceFactory.GetLightningServiceDescriptor(lightningMessage.Body.ServiceName);
                        //var result = await descriptor.InvokeHandler(lightningMessage.Body.Parameters!);
                        //await context.WriteAndFlushAsync(new LightningMessage<LightningResponse>(lightningMessage.Id, new LightningResponse(result)));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"The server message processing failed:{ex.Message}. Current node:{0} Route:{lightningMessage.Body.ServiceName}.{lightningMessage.Body.Parameters} Message id:{lightningMessage.Id}");
                        await context.WriteAndFlushAsync(new LightningMessage<LightningResponse>(lightningMessage.Id, new LightningResponse()));
                    }
                }
            });
        }

        public override void HandlerAdded(IChannelHandlerContext context)
        {
            Console.WriteLine($"客户端{context}上线.");
            base.HandlerAdded(context);

            IChannelGroup g = _groups;
            if (g == null)
            {
                lock (this)
                {
                    if (_groups == null)
                    {
                        g = _groups = new DefaultChannelGroup(context.Executor);
                    }
                }
            }

            g.Add(context.Channel);
            _groups.WriteAndFlushAsync($"欢迎{context.Channel.RemoteAddress}加入.");
        }
        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            Console.WriteLine($"客户端{context}下线.");
            base.HandlerRemoved(context);

            _groups.Remove(context.Channel);
            _groups.WriteAndFlushAsync($"恭送{context.Channel.RemoteAddress}离开.");
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

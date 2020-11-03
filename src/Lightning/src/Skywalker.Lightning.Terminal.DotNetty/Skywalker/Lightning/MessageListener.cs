using Skywalker.Lightning.Terminal.Abstractions;
using Skywalker.Lightning.Messaging;
using System.Threading.Tasks;

namespace Skywalker.Lightning
{
    internal class MessageListener : IMessageListener
    {
        public event MessageEventHandler? OnReceived;

        public Task Received(LightningMessage<LightningResponse> message)
        {
            return Task.Run(() =>
            {
                OnReceived?.Invoke(message);
            });
        }
    }
}

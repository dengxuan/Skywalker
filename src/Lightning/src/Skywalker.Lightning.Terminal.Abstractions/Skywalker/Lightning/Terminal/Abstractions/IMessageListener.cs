using Skywalker.Lightning.Messaging;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Terminal.Abstractions
{
    public interface IMessageListener
    {
        event MessageEventHandler OnReceived;

        Task Received(LightningMessage<LightningResponse> message);
    }
}

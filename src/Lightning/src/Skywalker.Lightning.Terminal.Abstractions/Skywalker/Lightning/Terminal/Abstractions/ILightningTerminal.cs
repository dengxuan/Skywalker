using Skywalker.Lightning.Messaging;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Terminal.Abstractions
{
    public interface ILightningTerminal
    {
        Task<LightningResponse> SendAsync(LightningRequest message);

        Task DisconnectAsync();
    }
}

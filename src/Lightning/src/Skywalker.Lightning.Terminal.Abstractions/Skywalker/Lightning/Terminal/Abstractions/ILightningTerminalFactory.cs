using DotNetty.Common.Utilities;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Terminal.Abstractions
{
    public interface ILightningTerminalFactory
    {
        public AttributeKey<IMessageListener> MessageListenerAttributeKey { get; }

        Task<ILightningTerminal> CreateTerminalAsync();

        Task RemoveTerminal(string clusterId);

        Task RemoveAllTerminal();
    }
}

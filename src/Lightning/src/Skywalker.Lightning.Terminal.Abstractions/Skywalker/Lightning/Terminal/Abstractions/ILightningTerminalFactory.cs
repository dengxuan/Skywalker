using DotNetty.Common.Utilities;
using Skywalker.Lightning.Cluster.Abstractions;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Terminal.Abstractions
{
    public interface ILightningTerminalFactory
    {
        public AttributeKey<IMessageListener> MessageListenerAttributeKey { get; }

        Task<ILightningTerminal> CreateTerminalAsync(ILightningCluster cluster);

        Task RemoveTerminal(string clusterId);

        Task RemoveAllTerminal();
    }
}

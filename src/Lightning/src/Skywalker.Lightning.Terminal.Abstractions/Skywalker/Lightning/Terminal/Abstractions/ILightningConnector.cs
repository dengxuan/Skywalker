using System.Threading.Tasks;

namespace Skywalker.Lightning.Terminal.Abstractions
{
    public interface ILightningConnector
    {
        Task Connect(string host, int port);
    }
}

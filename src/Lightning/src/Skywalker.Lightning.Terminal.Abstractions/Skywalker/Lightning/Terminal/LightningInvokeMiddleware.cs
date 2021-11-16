using Skywalker.Lightning.Messaging;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Terminal
{
    public delegate Task<LightningResponse> LightningInvokeMiddleware(LightningInvokeContext context);
}

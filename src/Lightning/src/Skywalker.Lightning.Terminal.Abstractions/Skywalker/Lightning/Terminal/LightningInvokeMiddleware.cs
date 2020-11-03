using Skywalker.Lightning.Messaging;
using System.Threading.Tasks;

namespace Skywalker.Lightning
{
    public delegate Task<LightningResponse> LightningInvokeMiddleware(LightningInvokeContext context);
}

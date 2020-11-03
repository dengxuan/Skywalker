using System.Threading.Tasks;

namespace Skywalker.Lightning.Server
{
    public delegate object FastInvokeHandler(object target, object[] paramters);

    public delegate Task<object> FastInvokeAsyncHandler(object target, object[] paramters);
}

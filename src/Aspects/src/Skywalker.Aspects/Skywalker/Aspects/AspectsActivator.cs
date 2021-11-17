using Skywalker.Aspects.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Aspects;

public class AspectsActivator<TProxy> : IAspectsActivator<TProxy>
{
    public TProxy CreateInstance(IAspectsInterceptor interceptor)
    {
        throw new NotImplementedException();
    }
}

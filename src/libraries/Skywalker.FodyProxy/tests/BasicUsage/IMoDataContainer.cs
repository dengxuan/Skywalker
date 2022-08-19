using Skywalker.FodyProxy;
using Skywalker.FodyProxy.Context;

namespace BasicUsage;

public interface IMoDataContainer
{
    IInterceptor Mo { get; set; }

    MethodContext Context { get; set; }
}

public class MoDataContainer : IMoDataContainer
{
    public IInterceptor Mo { get; set; }

    public MethodContext Context { get; set; }
}

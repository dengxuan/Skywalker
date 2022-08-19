using Skywalker.FodyProxy;
using Skywalker.FodyProxy.Context;

namespace BasicUsage.Attributes;

public class SyncAttribute : InterceptorAttribute
{
    public override void OnEntry(MethodContext context)
    {
        var datas = (List<string>)context.Arguments[0];
        datas.Add(nameof(OnEntry));
    }

    public override void OnException(MethodContext context)
    {
        var datas = (List<string>)context.Arguments[0];
        datas.Add(nameof(OnException));
    }

    public override void OnSuccess(MethodContext context)
    {
        var datas = (List<string>)context.Arguments[0];
        datas.Add(nameof(OnSuccess));
    }

    public override void OnExit(MethodContext context)
    {
        var datas = (List<string>)context.Arguments[0];
        datas.Add(nameof(OnExit));
    }
}

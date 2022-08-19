﻿using Skywalker.FodyProxy.Context;

namespace BasicUsage.Attributes;

public class OnExceptionAttribute : ContainerAttribute
{
    public override void OnException(MethodContext context)
    {
        base.OnException(context);
        context.Exception.Data.Add(nameof(OnExceptionAttribute), Guid.NewGuid());
    }
}

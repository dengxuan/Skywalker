﻿namespace Skywalker.Extensions.Linq.Parser;

internal class WrappedValue<TValue>
{
    public TValue Value { get; private set; }

    public WrappedValue(TValue value)
    {
        Value = value;
    }
}

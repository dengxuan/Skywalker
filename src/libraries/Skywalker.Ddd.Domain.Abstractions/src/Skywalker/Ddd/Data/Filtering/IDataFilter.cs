// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Data.Filtering;

public interface IDataFilter<TFilter>
    where TFilter : class
{
    IDisposable Enable();

    IDisposable Disable();

    bool IsEnabled { get; }
}

public interface IDataFilter
{
    IDisposable Enable<TFilter>()
        where TFilter : class;

    IDisposable Disable<TFilter>()
        where TFilter : class;

    bool IsEnabled<TFilter>()
        where TFilter : class;
}

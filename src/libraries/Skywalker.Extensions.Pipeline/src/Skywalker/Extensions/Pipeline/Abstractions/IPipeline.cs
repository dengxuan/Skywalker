// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Pipeline.Abstractions;

/// <summary>
/// Pipeline to surround the inner handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
public interface IPipeline
{
    ValueTask InvokeAsync(PipelineContext context);
}

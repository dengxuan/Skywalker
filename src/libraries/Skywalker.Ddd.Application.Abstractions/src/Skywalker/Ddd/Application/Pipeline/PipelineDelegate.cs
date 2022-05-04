namespace Skywalker.Ddd.Application.Pipeline;

/// <summary>
/// Represents an interception operation.
/// </summary>
/// <param name="context">The context for the invocation to the proxy.</param>
/// <returns>The task to perform interception operation.</returns>
public delegate Task InterceptDelegate(PipelineContext context);

/// <summary>
/// Represents an pipeline.
/// </summary>
/// <param name="next">A <see cref="InterceptDelegate"/> used to invoke the next pipeline or target application handler.</param>
/// <returns>A <see cref="InterceptDelegate"/> representing the interception operation.</returns>
public delegate InterceptDelegate PipelineDelegate(InterceptDelegate next);

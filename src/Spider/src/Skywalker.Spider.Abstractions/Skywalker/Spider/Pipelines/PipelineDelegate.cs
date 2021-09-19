using System.Threading.Tasks;

namespace Skywalker.Spider.Pipelines;

/// <summary>
/// Represents an interception operation.
/// </summary>
/// <param name="context">The context for the pipeline.</param>
/// <returns>The task to perform interception operation.</returns>
public delegate Task PipelineDelegate(PipelineContext context);

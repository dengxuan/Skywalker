namespace Skywalker.Ddd.Application.Pipeline;


/// <summary>
/// Represents an pipeline.
/// </summary>
/// <param name="next">A <see cref="ApplicationDelegate"/> used to invoke the next pipeline or target application handler.</param>
/// <returns>A <see cref="ApplicationDelegate"/> representing the interception operation.</returns>
public delegate ApplicationDelegate ApplicationDelegate(ApplicationDelegate next);

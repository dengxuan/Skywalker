namespace Skywalker.ObjectMapping;

/// <summary>
/// Object mapper interface.
/// </summary>
public interface IObjectMapper
{
    /// <summary>
    /// Maps the source object to a new destination object.
    /// </summary>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source object.</param>
    /// <returns>The mapped destination object.</returns>
    TDestination Map<TDestination>(object source);

    /// <summary>
    /// Maps the source object to a new destination object.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source object.</param>
    /// <returns>The mapped destination object.</returns>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    /// Maps the source object to an existing destination object.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="destination">The destination object.</param>
    /// <returns>The mapped destination object.</returns>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
}

/// <summary>
/// Typed object mapper interface.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TDestination">The destination type.</typeparam>
public interface IObjectMapper<in TSource, TDestination>
{
    /// <summary>
    /// Maps the source object to a new destination object.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <returns>The mapped destination object.</returns>
    TDestination Map(TSource source);

    /// <summary>
    /// Maps the source object to an existing destination object.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="destination">The destination object.</param>
    /// <returns>The mapped destination object.</returns>
    TDestination Map(TSource source, TDestination destination);
}


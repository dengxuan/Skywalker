namespace Skywalker.ObjectMapping;

/// <summary>
/// Extension methods for object mapping.
/// </summary>
public static class ObjectMapperExtensions
{
    /// <summary>
    /// Maps a collection of source objects to destination objects.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="mapper">The object mapper.</param>
    /// <param name="sources">The source objects.</param>
    /// <returns>The mapped destination objects.</returns>
    public static IEnumerable<TDestination> MapCollection<TSource, TDestination>(
        this IObjectMapper mapper,
        IEnumerable<TSource> sources)
    {
        return sources.Select(mapper.Map<TSource, TDestination>);
    }

    /// <summary>
    /// Maps a list of source objects to destination objects.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="mapper">The object mapper.</param>
    /// <param name="sources">The source objects.</param>
    /// <returns>The mapped destination objects.</returns>
    public static List<TDestination> MapList<TSource, TDestination>(
        this IObjectMapper mapper,
        IEnumerable<TSource> sources)
    {
        return sources.Select(mapper.Map<TSource, TDestination>).ToList();
    }

    /// <summary>
    /// Maps an array of source objects to destination objects.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="mapper">The object mapper.</param>
    /// <param name="sources">The source objects.</param>
    /// <returns>The mapped destination objects.</returns>
    public static TDestination[] MapArray<TSource, TDestination>(
        this IObjectMapper mapper,
        IEnumerable<TSource> sources)
    {
        return sources.Select(mapper.Map<TSource, TDestination>).ToArray();
    }
}


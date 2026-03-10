namespace Skywalker.ObjectMapping;

/// <summary>
/// Default object mapper that uses reflection for simple property mapping.
/// </summary>
public class DefaultObjectMapper : IObjectMapper
{
    /// <inheritdoc/>
    public TDestination Map<TDestination>(object source)
    {
        if (source == null)
        {
            return default!;
        }

        var destination = Activator.CreateInstance<TDestination>();
        return MapPropertiesFromObject(source, destination);
    }

    /// <inheritdoc/>
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
        {
            return default!;
        }

        var destination = Activator.CreateInstance<TDestination>();
        return MapProperties(source, destination);
    }

    /// <inheritdoc/>
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
        {
            return destination;
        }

        return MapProperties(source, destination);
    }

    private static TDestination MapProperties<TSource, TDestination>(TSource source, TDestination destination)
    {
        var sourceType = typeof(TSource);
        var destinationType = typeof(TDestination);

        var sourceProperties = sourceType.GetProperties()
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, p => p);

        var destinationProperties = destinationType.GetProperties()
            .Where(p => p.CanWrite);

        foreach (var destProp in destinationProperties)
        {
            if (sourceProperties.TryGetValue(destProp.Name, out var sourceProp) &&
                destProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
            {
                var value = sourceProp.GetValue(source);
                destProp.SetValue(destination, value);
            }
        }

        return destination;
    }

    private static TDestination MapPropertiesFromObject<TDestination>(object source, TDestination destination)
    {
        var sourceType = source.GetType();
        var destinationType = typeof(TDestination);

        var sourceProperties = sourceType.GetProperties()
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, p => p);

        var destinationProperties = destinationType.GetProperties()
            .Where(p => p.CanWrite);

        foreach (var destProp in destinationProperties)
        {
            if (sourceProperties.TryGetValue(destProp.Name, out var sourceProp) &&
                destProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
            {
                var value = sourceProp.GetValue(source);
                destProp.SetValue(destination, value);
            }
        }

        return destination;
    }
}


using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Localization;

/// <summary>
/// A dictionary that holds localization resources by their resource type.
/// </summary>
public class LocalizationResourceDictionary : IDictionary<Type, LocalizationResource>
{
    private readonly Dictionary<Type, LocalizationResource> _resources = new();

    /// <inheritdoc/>
    public LocalizationResource this[Type key]
    {
        get => _resources[key];
        set => _resources[key] = value;
    }

    /// <inheritdoc/>
    public ICollection<Type> Keys => _resources.Keys;

    /// <inheritdoc/>
    public ICollection<LocalizationResource> Values => _resources.Values;

    /// <inheritdoc/>
    public int Count => _resources.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds a new localization resource.
    /// </summary>
    /// <typeparam name="TResource">The resource type.</typeparam>
    /// <param name="defaultCultureName">The default culture name.</param>
    /// <returns>The created resource.</returns>
    public LocalizationResource Add<TResource>(string? defaultCultureName = null)
    {
        return Add(typeof(TResource), defaultCultureName);
    }

    /// <summary>
    /// Adds a new localization resource.
    /// </summary>
    /// <param name="resourceType">The resource type.</param>
    /// <param name="defaultCultureName">The default culture name.</param>
    /// <returns>The created resource.</returns>
    public LocalizationResource Add(Type resourceType, string? defaultCultureName = null)
    {
        if (_resources.ContainsKey(resourceType))
        {
            throw new InvalidOperationException($"Localization resource '{resourceType.FullName}' already exists.");
        }

        var resource = new LocalizationResource(resourceType, defaultCultureName);
        _resources[resourceType] = resource;
        return resource;
    }

    /// <summary>
    /// Gets a resource by its type, or null if not found.
    /// </summary>
    /// <typeparam name="TResource">The resource type.</typeparam>
    /// <returns>The resource, or null if not found.</returns>
    public LocalizationResource? GetOrNull<TResource>()
    {
        return GetOrNull(typeof(TResource));
    }

    /// <summary>
    /// Gets a resource by its type, or null if not found.
    /// </summary>
    /// <param name="resourceType">The resource type.</param>
    /// <returns>The resource, or null if not found.</returns>
    public LocalizationResource? GetOrNull(Type resourceType)
    {
        return _resources.TryGetValue(resourceType, out var resource) ? resource : null;
    }

    /// <inheritdoc/>
    public void Add(Type key, LocalizationResource value) => _resources.Add(key, value);

    /// <inheritdoc/>
    public void Add(KeyValuePair<Type, LocalizationResource> item) => _resources.Add(item.Key, item.Value);

    /// <inheritdoc/>
    public void Clear() => _resources.Clear();

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<Type, LocalizationResource> item) => _resources.ContainsKey(item.Key);

    /// <inheritdoc/>
    public bool ContainsKey(Type key) => _resources.ContainsKey(key);

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<Type, LocalizationResource>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<Type, LocalizationResource>>)_resources).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<Type, LocalizationResource>> GetEnumerator() => _resources.GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(Type key) => _resources.Remove(key);

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<Type, LocalizationResource> item) => _resources.Remove(item.Key);

    /// <inheritdoc/>
    public bool TryGetValue(Type key, [MaybeNullWhen(false)] out LocalizationResource value)
        => _resources.TryGetValue(key, out value);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


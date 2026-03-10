namespace Skywalker.Localization;

/// <summary>
/// Represents a localized string with its name and value.
/// </summary>
public class LocalizedString
{
    /// <summary>
    /// Gets the name of the localized string.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value of the localized string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets a value indicating whether the string was not found in the resource.
    /// </summary>
    public bool ResourceNotFound { get; }

    /// <summary>
    /// Gets the location where the string was found (e.g., resource file path).
    /// </summary>
    public string? SearchedLocation { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LocalizedString"/>.
    /// </summary>
    /// <param name="name">The name of the localized string.</param>
    /// <param name="value">The value of the localized string.</param>
    public LocalizedString(string name, string value)
        : this(name, value, resourceNotFound: false, searchedLocation: null)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="LocalizedString"/>.
    /// </summary>
    /// <param name="name">The name of the localized string.</param>
    /// <param name="value">The value of the localized string.</param>
    /// <param name="resourceNotFound">Whether the string was not found in the resource.</param>
    /// <param name="searchedLocation">The location where the string was searched.</param>
    public LocalizedString(string name, string value, bool resourceNotFound, string? searchedLocation = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        ResourceNotFound = resourceNotFound;
        SearchedLocation = searchedLocation;
    }

    /// <summary>
    /// Implicitly converts the <see cref="LocalizedString"/> to its string value.
    /// </summary>
    public static implicit operator string(LocalizedString localizedString) => localizedString.Value;

    /// <inheritdoc/>
    public override string ToString() => Value;
}


using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Localization.EntityFrameworkCore.Entities;

/// <summary>
/// Represents a localized text entry in the database.
/// </summary>
public class LocalizationText : AggregateRoot<string>
{
    /// <summary>
    /// Gets or sets the resource name (group) this text belongs to.
    /// </summary>
    public virtual string ResourceName { get; protected set; } = default!;

    /// <summary>
    /// Gets or sets the culture name (e.g., "en-US", "zh-CN").
    /// </summary>
    public virtual string CultureName { get; protected set; } = default!;

    /// <summary>
    /// Gets or sets the localization key.
    /// </summary>
    public virtual string Key { get; protected set; } = default!;

    /// <summary>
    /// Gets or sets the localized value.
    /// </summary>
    public virtual string Value { get; set; } = default!;

    /// <summary>
    /// Protected constructor for ORM.
    /// </summary>
    protected LocalizationText()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="LocalizationText"/>.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="key">The localization key.</param>
    /// <param name="value">The localized value.</param>
    public LocalizationText(string id, string resourceName, string cultureName, string key, string value)
    {
        Check.NotNullOrWhiteSpace(id, nameof(id));
        Check.NotNullOrWhiteSpace(resourceName, nameof(resourceName));
        Check.NotNullOrWhiteSpace(cultureName, nameof(cultureName));
        Check.NotNullOrWhiteSpace(key, nameof(key));
        Check.NotNull(value, nameof(value));

        Id = id;
        ResourceName = resourceName;
        CultureName = cultureName;
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Updates the localized value.
    /// </summary>
    /// <param name="value">The new value.</param>
    public void SetValue(string value)
    {
        Check.NotNull(value, nameof(value));
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{ResourceName}/{CultureName}/{Key} = {Value}";
    }
}


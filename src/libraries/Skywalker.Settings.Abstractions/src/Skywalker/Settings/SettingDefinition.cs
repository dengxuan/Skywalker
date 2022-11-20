namespace Volo.Abp.Settings;

public class SettingDefinition
{
    /// <summary>
    /// Unique name of the setting.
    /// </summary>
    public string Name { get; }

    public string DisplayName { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Default value of the setting.
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Can clients see this setting and it's value.
    /// It maybe dangerous for some settings to be visible to clients (such as an email server password).
    /// Default: false.
    /// </summary>
    public bool IsVisibleToClients { get; set; }

    /// <summary>
    /// A list of allowed providers to get/set value of this setting.
    /// An empty list indicates that all providers are allowed.
    /// </summary>
    public List<string> Providers { get; }

    /// <summary>
    /// Is this setting inherited from parent scopes.
    /// Default: True.
    /// </summary>
    public bool IsInherited { get; set; }

    /// <summary>
    /// Can be used to get/set custom properties for this setting definition.
    /// </summary>
    public Dictionary<string, object> Properties { get; }

    /// <summary>
    /// Is this setting stored as encrypted in the data source.
    /// Default: False.
    /// </summary>
    public bool IsEncrypted { get; set; }

    public SettingDefinition(
        string name,
        string? defaultValue = null,
        string? displayName = null,
        string? description = null,
        bool isVisibleToClients = false,
        bool isInherited = true,
        bool isEncrypted = false)
    {
        Name = name;
        DefaultValue = defaultValue;
        IsVisibleToClients = isVisibleToClients;
        DisplayName = displayName ?? name;
        Description = description;
        IsInherited = isInherited;
        IsEncrypted = isEncrypted;

        Properties = new Dictionary<string, object>();
        Providers = new List<string>();
    }

    /// <summary>
    /// Sets a property in the <see cref="Properties"/> dictionary.
    /// This is a shortcut for nested calls on this object.
    /// </summary>
    public virtual SettingDefinition WithProperty(string key, object value)
    {
        Properties[key] = value;
        return this;
    }

    /// <summary>
    /// Adds one or more providers to the <see cref="Providers"/> list.
    /// This is a shortcut for nested calls on this object.
    /// </summary>
    public virtual SettingDefinition WithProviders(params string[] providers)
    {
        if (!providers.IsNullOrEmpty())
        {
            Providers.AddIfNotContains(providers);
        }

        return this;
    }
}

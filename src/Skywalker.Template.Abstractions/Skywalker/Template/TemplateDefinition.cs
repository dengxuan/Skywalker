using Microsoft.Extensions.Localization;

namespace Volo.Abp.TextTemplating;

public class TemplateDefinition //: IHasNameWithLocalizableDisplayName
{
    public const int MaxNameLength = 128;

    public string Name { get; }

    public LocalizedString DisplayName { get; set; }

    public bool IsLayout { get; }

    public string? Layout { get; set; }

    public Type? LocalizationResource { get; set; }

    public bool IsInlineLocalized { get; set; }

    public string? DefaultCultureName { get; }

    public string? RenderEngine { get; set; }

    /// <summary>
    /// Gets/sets a key-value on the <see cref="Properties"/>.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <returns>
    /// Returns the value in the <see cref="Properties"/> dictionary by given <see cref="name"/>.
    /// Returns null if given <see cref="name"/> is not present in the <see cref="Properties"/> dictionary.
    /// </returns>
    public object? this[string name]
    {
        get => Properties.GetOrDefault(name);
        set => Properties[name] = value;
    }

    /// <summary>
    /// Can be used to get/set custom properties for this feature.
    /// </summary>
    public Dictionary<string, object?> Properties { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="localizationResource"></param>
    /// <param name="displayName"></param>
    /// <param name="isLayout"></param>
    /// <param name="layout"></param>
    /// <param name="defaultCultureName"></param>
    public TemplateDefinition(string name, Type? localizationResource = null,
        LocalizedString? displayName = null,
        bool isLayout = false,
        string? layout = null,
        string? defaultCultureName = null)
    {
        Name = name;
        LocalizationResource = localizationResource;
        DisplayName = displayName ?? new LocalizedString(Name, Name);
        IsLayout = isLayout;
        Layout = layout;
        DefaultCultureName = defaultCultureName;
        Properties = new Dictionary<string, object?>();
    }

    /// <summary>
    /// Sets a property in the <see cref="Properties"/> dictionary.
    /// This is a shortcut for nested calls on this object.
    /// </summary>
    public virtual TemplateDefinition WithProperty(string key, object value)
    {
        Properties[key] = value;
        return this;
    }

    /// <summary>
    /// Sets the Render Engine of <see cref="TemplateDefinition"/>.
    /// </summary>
    public virtual TemplateDefinition WithRenderEngine(string renderEngine)
    {
        RenderEngine = renderEngine;
        return this;
    }
}

using Skywalker.Extensions.Collections.Generic;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

/// <summary>
/// Options for configuring the Skywalker Settings module.
/// </summary>
public class SkywalkerSettingOptions
{
    /// <summary>
    /// List of value provider types in priority order (highest priority first).
    /// Providers are evaluated from first to last, and the first non-null value is returned.
    /// Default order: User > Global > Configuration > Default
    /// </summary>
    public ITypeList<ISettingValueProvider> ValueProviders { get; } = new TypeList<ISettingValueProvider>();
}

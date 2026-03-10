// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Settings.AspNetCore;

/// <summary>
/// DTO for updating a setting.
/// </summary>
/// <param name="Value">Setting value.</param>
/// <param name="ProviderName">Provider name.</param>
/// <param name="ProviderKey">Provider key (optional).</param>
public record UpdateSettingDto(string Value, string ProviderName, string? ProviderKey = null);

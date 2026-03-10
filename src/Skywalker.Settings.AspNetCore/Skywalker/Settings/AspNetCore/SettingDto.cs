// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Settings.AspNetCore;

/// <summary>
/// Setting DTO for API responses.
/// </summary>
/// <param name="Name">Setting name.</param>
/// <param name="Value">Setting value.</param>
public record SettingDto(string Name, string? Value);

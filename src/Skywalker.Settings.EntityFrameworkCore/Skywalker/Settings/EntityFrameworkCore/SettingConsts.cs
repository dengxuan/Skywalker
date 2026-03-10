// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Settings.EntityFrameworkCore;

/// <summary>
/// Constants for Setting entity constraints.
/// </summary>
public static class SettingConsts
{
    /// <summary>
    /// Maximum length of Setting name.
    /// Default value: 128
    /// </summary>
    public static int MaxNameLength { get; set; } = 128;

    /// <summary>
    /// Maximum length of Setting value.
    /// Default value: 2048
    /// </summary>
    public static int MaxValueLength { get; set; } = 2048;

    /// <summary>
    /// Maximum length of provider name.
    /// Default value: 8
    /// </summary>
    public static int MaxProviderNameLength { get; set; } = 8;

    /// <summary>
    /// Maximum length of provider key.
    /// Default value: 64
    /// </summary>
    public static int MaxProviderKeyLength { get; set; } = 64;
}

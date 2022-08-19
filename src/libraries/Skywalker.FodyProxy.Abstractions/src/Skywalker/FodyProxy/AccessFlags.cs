﻿namespace Skywalker.FodyProxy;

/// <summary>
/// accessable flags
/// </summary>
[Flags]
#if FODY
internal
#else
public
#endif
 enum AccessFlags
{
    /// <summary>
    /// Static
    /// </summary>
    Static = 0b1001,
    /// <summary>
    /// Instance
    /// </summary>
    Instance = 0b0110,
    /// <summary>
    /// Public
    /// </summary>
    Public = 0b0011,
    /// <summary>
    /// NonPublic
    /// </summary>
    NonPublic = 0b1100,
    /// <summary>
    /// Static and Public
    /// </summary>
    StaticPublic = Static & Public,
    /// <summary>
    /// Static and NonPublic
    /// </summary>
    StaticNonPublic = Static & NonPublic,
    /// <summary>
    /// Instance and Public (default)
    /// </summary>
    InstancePublic = Instance & Public,
    /// <summary>
    /// Instance and NonPublic
    /// </summary>
    InstanceNonPublic = Instance & NonPublic,
    /// <summary>
    /// All
    /// </summary>
    All = Static | Instance
}

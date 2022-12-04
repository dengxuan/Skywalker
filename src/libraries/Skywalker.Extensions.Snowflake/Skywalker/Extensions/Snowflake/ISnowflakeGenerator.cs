// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.Snowflake;

/// <summary>
/// Used to generate unique id.
/// </summary>
public interface ISnowflakeGenerator
{
    /// <summary>
    /// Creates a new unique id.
    /// </summary>
    long Create();
}

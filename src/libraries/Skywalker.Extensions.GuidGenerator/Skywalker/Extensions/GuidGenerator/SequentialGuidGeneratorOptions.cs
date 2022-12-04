// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.GuidGenerator;

public class SequentialGuidGeneratorOptions
{
    /// <summary>
    /// Default value: null (unspecified).
    /// Use <see cref="GetDefaultSequentialGuidType"/> method
    /// to get the value on use, since it fall backs to a default value.
    /// </summary>
    public SequentialGuidType? DefaultSequentialGuidType { get; set; }

    /// <summary>
    /// Get the <see cref="DefaultSequentialGuidType"/> value
    /// or returns <see cref="SequentialGuidType.SequentialAsString"/>
    /// if <see cref="DefaultSequentialGuidType"/> was null.
    /// </summary>
    public SequentialGuidType GetDefaultSequentialGuidType()
    {
        return DefaultSequentialGuidType ?? SequentialGuidType.SequentialAsString;
    }
}

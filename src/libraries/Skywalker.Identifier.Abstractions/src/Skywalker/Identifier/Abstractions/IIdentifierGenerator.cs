// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Identifier.Abstractions;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentifier"></typeparam>
public interface IIdentifierGenerator<out TIdentifier>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    TIdentifier? Generate();
}

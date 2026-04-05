// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.ApplicationParts;

/// <summary>
/// Skywalker 应用程序的一个组成部分。
/// </summary>
public abstract class ApplicationPart
{
    /// <summary>
    /// 获取此部分的名称。
    /// </summary>
    public abstract string Name { get; }
}

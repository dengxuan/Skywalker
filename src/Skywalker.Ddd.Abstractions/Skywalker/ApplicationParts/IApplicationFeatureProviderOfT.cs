// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.ApplicationParts;

/// <summary>
/// 指定 <typeparamref name="TFeature"/> 功能的提供者。
/// </summary>
/// <typeparam name="TFeature">功能类型。</typeparam>
public interface IApplicationFeatureProvider<TFeature> : IApplicationFeatureProvider
{
    /// <summary>
    /// 使用 <paramref name="parts"/> 中的信息填充 <paramref name="feature"/> 实例。
    /// </summary>
    /// <param name="parts"><see cref="ApplicationPart"/> 列表，按照 <see cref="SkywalkerPartManager.ApplicationParts"/> 中的顺序排列。</param>
    /// <param name="feature">要填充的功能实例。</param>
    void PopulateFeature(IEnumerable<ApplicationPart> parts, TFeature feature);
}

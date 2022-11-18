// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// 实体Dto，包含实体主键定义
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IEntityDto<TKey> : IDto
{
    /// <summary>
    /// 
    /// </summary>
    TKey Id { get; init; }
}

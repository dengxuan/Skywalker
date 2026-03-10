// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.EntityFrameworkCore.SourceGenerators;

/// <summary>
/// 仓储信息，用于代码生成。
/// </summary>
/// <param name="EntityTypeName">实体类型的完整名称</param>
/// <param name="EntityTypeNamespace">实体类型的命名空间</param>
/// <param name="KeyTypeName">主键类型名称，null 表示无主键或复合主键</param>
/// <param name="DbContextTypeName">DbContext 类型的完整名称</param>
internal sealed record RepositoryInfo(
    string EntityTypeName,
    string EntityTypeNamespace,
    string? KeyTypeName,
    string DbContextTypeName);

// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Skywalker.Ddd.EntityFrameworkCore.ValueGeneration;

public class StringIdValueGenerator() : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry)
    {
        // 如果属性已经有值，直接返回现有值
        var currentValue = entry.Property("Id").CurrentValue as string;
        if (!string.IsNullOrEmpty(currentValue))
        {
            return currentValue;
        }
        return $"{DateTime.Now:yyyyMMddHHmmssfffff}-{CorrelationIdGenerator.Instance.Next}";
    }
}

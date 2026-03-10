// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.GuidGenerator;

namespace Skywalker.Ddd.EntityFrameworkCore.ValueGeneration;

public class GuidIdValueGenerator : ValueGenerator<Guid>
{
    public override bool GeneratesTemporaryValues => false;

    public override Guid Next(EntityEntry entry)
    {
        // 如果属性已经有值，直接返回现有值
        var currentValue = entry.Property("Id").CurrentValue;
        if (currentValue is Guid guid && guid != Guid.Empty)
        {
            return guid;
        }
        return Guid.NewGuid();
    }
}

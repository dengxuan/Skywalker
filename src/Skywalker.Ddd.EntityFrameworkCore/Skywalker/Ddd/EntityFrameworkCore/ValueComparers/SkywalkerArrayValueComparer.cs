// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Skywalker.Ddd.EntityFrameworkCore.ValueComparers;

public class SkywalkerArrayValueComparer : ValueComparer<string[]>
{
    public SkywalkerArrayValueComparer() : base((lift, right) => lift!.SequenceEqual(right!), d => d.Aggregate(0, (k, v) => HashCode.Combine(k, v.GetHashCode())))
    {
    }
}

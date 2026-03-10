// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Skywalker.Ddd.EntityFrameworkCore.ValueComparers;

public class SkywalkerJsonValueComparer<TPropertyType> : ValueComparer<TPropertyType>
{
    public SkywalkerJsonValueComparer() : base((lift, right) => lift!.Equals(right!), d => d!.GetHashCode())
    {
    }
}

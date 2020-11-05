﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Skywalker.EntityFrameworkCore.ValueComparers
{
    public class SkywalkerDictionaryValueComparer<TKey, TValue> : ValueComparer<Dictionary<TKey, TValue>>
    {
        public SkywalkerDictionaryValueComparer()
            : base(
                  (d1, d2) => d1.SequenceEqual(d2),
                  d => d.Aggregate(0, (k, v) => HashCode.Combine(k, v.GetHashCode())),
                  d => d.ToDictionary(k => k.Key, v => v.Value))
        {
        }
    }
}

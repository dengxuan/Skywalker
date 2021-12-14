using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Skywalker.Ddd.EntityFrameworkCore.ValueComparers
{
    public class SkywalkerDictionaryValueComparer<TKey, TValue> : ValueComparer<Dictionary<TKey, TValue>> where TKey : notnull
    {
        public SkywalkerDictionaryValueComparer() : base((d1, d2) => d1!.SequenceEqual(d2!), d => d.Aggregate(0, (k, v) => HashCode.Combine(k, v.GetHashCode())), d => d.ToDictionary(k => k.Key, v => v.Value))
        {
        }
    }
}

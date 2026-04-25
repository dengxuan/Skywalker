using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Skywalker.SourceGenerators;

/// <summary>
/// Value-equal wrapper around <see cref="ImmutableArray{T}"/>, suitable for use as a field of
/// records that flow through the incremental generator pipeline. Using a raw
/// <see cref="ImmutableArray{T}"/> defeats the per-step caching because its default equality is
/// reference-based.
/// </summary>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    public static readonly EquatableArray<T> Empty = new(ImmutableArray<T>.Empty);

    private readonly ImmutableArray<T> _array;

    public EquatableArray(ImmutableArray<T> array) => _array = array;

    public EquatableArray(T[] array) => _array = ImmutableArray.Create(array);

    public int Length => _array.IsDefault ? 0 : _array.Length;

    public bool IsEmpty => Length == 0;

    public T this[int index] => _array[index];

    public ImmutableArray<T> AsImmutableArray() => _array.IsDefault ? ImmutableArray<T>.Empty : _array;

    public bool Equals(EquatableArray<T> other)
    {
        var len = Length;
        if (len != other.Length) return false;
        for (var i = 0; i < len; i++)
        {
            if (!_array[i].Equals(other._array[i])) return false;
        }
        return true;
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> array && Equals(array);

    public override int GetHashCode()
    {
        if (_array.IsDefault) return 0;
        unchecked
        {
            var hash = 17;
            foreach (var item in _array)
            {
                hash = (hash * 31) + (item?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_array.IsDefault) yield break;
        foreach (var item in _array) yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);

    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);

    public static implicit operator EquatableArray<T>(ImmutableArray<T> array) => new(array);

    public static implicit operator EquatableArray<T>(T[] array) => new(array);
}

﻿using System.Collections;
#if NETCOREAPP || NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Skywalker.Extensions.Collections.Generic;

public class CopyOnWriteDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
{
    private readonly IDictionary<TKey, TValue> _sourceDictionary;
    private readonly IEqualityComparer<TKey> _comparer;
    private IDictionary<TKey, TValue>? _innerDictionary;

    public CopyOnWriteDictionary(
        IDictionary<TKey, TValue> sourceDictionary,
        IEqualityComparer<TKey> comparer)
    {
        _sourceDictionary = sourceDictionary ?? throw new ArgumentNullException(nameof(sourceDictionary));
        _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    private IDictionary<TKey, TValue> ReadDictionary
    {
        get
        {
            return _innerDictionary ?? _sourceDictionary;
        }
    }

    private IDictionary<TKey, TValue> WriteDictionary
    {
        get
        {
            if (_innerDictionary == null)
            {
                _innerDictionary = new Dictionary<TKey, TValue>(_sourceDictionary,
                                                                _comparer);
            }

            return _innerDictionary;
        }
    }

    public virtual ICollection<TKey> Keys
    {
        get
        {
            return ReadDictionary.Keys;
        }
    }

    public virtual ICollection<TValue> Values
    {
        get
        {
            return ReadDictionary.Values;
        }
    }

    public virtual int Count
    {
        get
        {
            return ReadDictionary.Count;
        }
    }

    public virtual bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public virtual TValue this[TKey key]
    {
        get
        {
            return ReadDictionary[key];
        }
        set
        {
            WriteDictionary[key] = value;
        }
    }

    public virtual bool ContainsKey(TKey key)
    {
        return ReadDictionary.ContainsKey(key);
    }

    public virtual void Add(TKey key, TValue value)
    {
        WriteDictionary.Add(key, value);
    }

    public virtual bool Remove(TKey key)
    {
        return WriteDictionary.Remove(key);
    }

#if NETCOREAPP || NET5_0_OR_GREATER
    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return ReadDictionary.TryGetValue(key, out value);
    }
#else
    public virtual bool TryGetValue(TKey key, out TValue value)
    {
        return ReadDictionary.TryGetValue(key, out value);
    }
#endif

    public virtual void Add(KeyValuePair<TKey, TValue> item)
    {
        WriteDictionary.Add(item);
    }

    public virtual void Clear()
    {
        WriteDictionary.Clear();
    }

    public virtual bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ReadDictionary.Contains(item);
    }

    public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ReadDictionary.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return WriteDictionary.Remove(item);
    }

    public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ReadDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

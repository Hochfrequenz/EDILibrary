// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections;
using System.Collections.Generic;

namespace EDILibrary.Helper
{
    /// <summary>
    /// A minimal <see cref="IDictionary{TKey,TValue}"/> that enumerates its entries in insertion
    /// order.
    /// </summary>
    /// <remarks>
    /// Used instead of <see cref="System.Dynamic.ExpandoObject"/> for the property bags that
    /// <see cref="EdiJsonMapper"/> builds and then serializes to JSON. ExpandoObject describes its
    /// member set with an internal "class" object and transitions to a new one on every added
    /// member, which makes building the deeply nested bags of a segment-group-heavy message
    /// noticeably more expensive than a plain dictionary.
    ///
    /// <see cref="Dictionary{TKey,TValue}"/> would be just as fast, but its enumeration order is
    /// explicitly documented as unspecified, and the enumeration order is what determines the
    /// property order of the emitted JSON. This type keeps that order an explicit guarantee so the
    /// serialized output stays byte-for-byte what ExpandoObject produced.
    ///
    /// Only the behaviour <see cref="EdiJsonMapper"/> relies on is intended to match ExpandoObject:
    /// insertion-ordered enumeration, <see cref="Add(string, object)"/> throwing on a duplicate key,
    /// and the indexer overwriting in place while keeping the position the key was first inserted
    /// at. Known, deliberate divergences, all of them unreachable from the mapper and all covered by
    /// InsertionOrderedDictionaryTests:
    /// <list type="bullet">
    /// <item>removing a key and adding it again appends it here, where ExpandoObject restores it at
    /// the slot it originally occupied (the mapper never removes a key);</item>
    /// <item>overwriting a value while enumerating is tolerated here and throws on ExpandoObject,
    /// i.e. this type is the more permissive of the two (the mapper never mutates a bag it is
    /// enumerating);</item>
    /// <item><see cref="Keys"/> and <see cref="Values"/> are snapshots, where ExpandoObject returns
    /// live read-only views;</item>
    /// <item>exception messages differ, though the exception types used by the mapper do not.</item>
    /// </list>
    /// </remarks>
    internal sealed class InsertionOrderedDictionary : IDictionary<string, object>
    {
        private readonly List<string> _order = new List<string>();
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public int Count => _order.Count;

        public bool IsReadOnly => false;

        public ICollection<string> Keys => new List<string>(_order);

        public ICollection<object> Values => _order.ConvertAll(key => _values[key]);

        public object this[string key]
        {
            get => _values[key];
            set
            {
                if (!_values.ContainsKey(key))
                {
                    _order.Add(key);
                }

                // overwriting an existing key keeps the position it was first inserted at, and
                // deliberately does not touch _order, so enumerators stay valid
                _values[key] = value;
            }
        }

        public void Add(string key, object value)
        {
            // Dictionary<,> and ExpandoObject both throw ArgumentException for a duplicate key
            _values.Add(key, value);
            _order.Add(key);
        }

        public void Add(KeyValuePair<string, object> item) => Add(item.Key, item.Value);

        public bool ContainsKey(string key) => _values.ContainsKey(key);

        public bool TryGetValue(string key, out object value) =>
            _values.TryGetValue(key, out value);

        public bool Remove(string key)
        {
            if (!_values.Remove(key))
            {
                return false;
            }

            _order.Remove(key);
            return true;
        }

        public bool Remove(KeyValuePair<string, object> item) => Contains(item) && Remove(item.Key);

        public void Clear()
        {
            _order.Clear();
            _values.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item) =>
            TryGetValue(item.Key, out object value)
            && EqualityComparer<object>.Default.Equals(value, item.Value);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            foreach (var entry in this)
            {
                array[arrayIndex++] = entry;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (string key in _order)
            {
                yield return new KeyValuePair<string, object>(key, _values[key]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

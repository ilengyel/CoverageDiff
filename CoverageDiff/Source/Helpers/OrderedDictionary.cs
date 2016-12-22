namespace CoverageDiff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// A dictionary object that allows rapid hash lookups using keys, but also
    /// maintains the key insertion order so that values can be retrieved by
    /// key index.
    /// Kudos: http://stackoverflow.com/a/9844528/279098
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="CoverageDiff.IOrderedDictionary{TKey, TValue}" />
    public sealed class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
    {
        private KeyedCollection2<TKey, KeyValuePair<TKey, TValue>> keyedCollection;

        public OrderedDictionary()
        {
            Initialize();
        }

        public OrderedDictionary(IEqualityComparer<TKey> comparer)
        {
            Initialize(comparer);
        }

        public OrderedDictionary(IOrderedDictionary<TKey, TValue> dictionary)
        {
            Initialize();
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                keyedCollection.Add(pair);
            }
        }

        public OrderedDictionary(IOrderedDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            Initialize(comparer);
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                keyedCollection.Add(pair);
            }
        }

        public int Count
        {
            get { return keyedCollection.Count; }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return keyedCollection.Select(x => x.Key).ToList();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return keyedCollection.Select(x => x.Value).ToList();
            }
        }

        public IEqualityComparer<TKey> Comparer { get; private set; }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return Keys; }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return Values; }
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return keyedCollection.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        ICollection IDictionary.Values
        {
            get { return (ICollection)this.Values; }
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get { return (ICollection)this.Keys; }
        }

        int ICollection.Count
        {
            get { return ((ICollection)keyedCollection).Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)keyedCollection).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)keyedCollection).SyncRoot; }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return this[key];
            }

            set
            {
                this[key] = value;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key associated with the value to get or set.</param>
        public TValue this[TKey key]
        {
            get
            {
                return GetValue(key);
            }

            set
            {
                SetValue(key, value);
            }
        }

        /// <summary>
        /// Gets or sets the value at the specified index.
        /// </summary>
        /// <param name="index">The index of the value to get or set.</param>
        public TValue this[int index]
        {
            get
            {
                return GetItem(index).Value;
            }

            set
            {
                SetItem(index, value);
            }
        }

        object IOrderedDictionary.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (TValue)value;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }

            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            keyedCollection.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Clear()
        {
            keyedCollection.Clear();
        }

        public void Insert(int index, TKey key, TValue value)
        {
            keyedCollection.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
        }

        public int IndexOf(TKey key)
        {
            if (keyedCollection.Contains(key))
            {
                return keyedCollection.IndexOf(keyedCollection[key]);
            }
            else
            {
                return -1;
            }
        }

        public bool ContainsValue(TValue value)
        {
            return this.Values.Contains(value);
        }

        public bool ContainsValue(TValue value, IEqualityComparer<TValue> comparer)
        {
            return this.Values.Contains(value, comparer);
        }

        public bool ContainsKey(TKey key)
        {
            return keyedCollection.Contains(key);
        }

        public KeyValuePair<TKey, TValue> GetItem(int index)
        {
            if (index < 0 || index >= keyedCollection.Count)
            {
                throw new ArgumentException(string.Format("The index was outside the bounds of the dictionary: {0}", index));
            }

            return keyedCollection[index];
        }

        /// <summary>
        /// Sets the value at the index specified.
        /// </summary>
        /// <param name="index">The index of the value desired</param>
        /// <param name="value">The value to set</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the index specified does not refer to a KeyValuePair in this object
        /// </exception>
        public void SetItem(int index, TValue value)
        {
            if (index < 0 || index >= keyedCollection.Count)
            {
                throw new ArgumentException($"The index is outside the bounds of the dictionary: {index}");
            }

            var kvp = new KeyValuePair<TKey, TValue>(keyedCollection[index].Key, value);
            keyedCollection[index] = kvp;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return keyedCollection.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            return keyedCollection.Remove(key);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= keyedCollection.Count)
            {
                throw new ArgumentException(string.Format("The index was outside the bounds of the dictionary: {0}", index));
            }

            keyedCollection.RemoveAt(index);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key associated with the value to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentException">When key does not exist.</exception>
        public TValue GetValue(TKey key)
        {
            if (keyedCollection.Contains(key) == false)
            {
                throw new ArgumentException($"The given key is not present in the dictionary: {key}");
            }

            var kvp = keyedCollection[key];
            return kvp.Value;
        }

        /// <summary>
        /// Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key associated with the value to set.</param>
        /// <param name="value">The the value to set.</param>
        public void SetValue(TKey key, TValue value)
        {
            var kvp = new KeyValuePair<TKey, TValue>(key, value);
            var idx = IndexOf(key);
            if (idx > -1)
            {
                keyedCollection[idx] = kvp;
            }
            else
            {
                keyedCollection.Add(kvp);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (keyedCollection.Contains(key))
            {
                value = keyedCollection[key].Value;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public void SortKeys()
        {
            keyedCollection.SortByKeys();
        }

        public void SortKeys(IComparer<TKey> comparer)
        {
            keyedCollection.SortByKeys(comparer);
        }

        public void SortKeys(Comparison<TKey> comparison)
        {
            keyedCollection.SortByKeys(comparison);
        }

        public void SortValues()
        {
            var comparer = Comparer<TValue>.Default;
            SortValues(comparer);
        }

        public void SortValues(IComparer<TValue> comparer)
        {
            keyedCollection.Sort((x, y) => comparer.Compare(x.Value, y.Value));
        }

        public void SortValues(Comparison<TValue> comparison)
        {
            keyedCollection.Sort((x, y) => comparison(x.Value, y.Value));
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Add(key, value);
        }

        bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return ContainsKey(key);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return Remove(key);
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            keyedCollection.Add(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            keyedCollection.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return keyedCollection.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            keyedCollection.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return keyedCollection.Remove(item);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator<TKey, TValue>(this);
        }

        void IOrderedDictionary.Insert(int index, object key, object value)
        {
            Insert(index, (TKey)key, (TValue)value);
        }

        void IOrderedDictionary.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        void IDictionary.Clear()
        {
            Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return keyedCollection.Contains((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator<TKey, TValue>(this);
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)keyedCollection).CopyTo(array, index);
        }

        private void Initialize(IEqualityComparer<TKey> comparer = null)
        {
            this.Comparer = comparer;
            if (comparer != null)
            {
                keyedCollection = new KeyedCollection2<TKey, KeyValuePair<TKey, TValue>>(x => x.Key, comparer);
            }
            else
            {
                keyedCollection = new KeyedCollection2<TKey, KeyValuePair<TKey, TValue>>(x => x.Key);
            }
        }
    }
}

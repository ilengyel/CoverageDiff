namespace CoverageDiff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator, IDisposable
    {
        private readonly IEnumerator<KeyValuePair<TKey, TValue>> impl;

        public DictionaryEnumerator(IDictionary<TKey, TValue> value)
        {
            this.impl = value.GetEnumerator();
        }

        public DictionaryEntry Entry
        {
            get
            {
                var pair = impl.Current;
                return new DictionaryEntry(pair.Key, pair.Value);
            }
        }

        public object Key => impl.Current.Key;

        public object Value => impl.Current.Value;

        public object Current => Entry;

        public void Dispose() => impl.Dispose();

        public void Reset() => impl.Reset();

        public bool MoveNext() => impl.MoveNext();
    }
}

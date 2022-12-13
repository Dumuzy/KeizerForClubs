using System.Collections.Generic;

namespace AwiUtils
{
    /// <summary> A Generic that is like a Dictionary, but it can hold multiple items per key. </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class Multimap<K, V> where V : class
    {
        public ICollection<K> Keys => dict.Keys;

        public ICollection<Li<V>> Values => dict.Values;

        /// <summary> Adds the specified value for the specified key. </summary>
        public void Add(K key, V value)
        {
            if (dict.TryGetValue(key, out Li<V> values))
                values.Add(value);
            else
            {
                var li = new Li<V>();
                li.Add(value);
                dict.Add(key, li);
            }
        }

        /// <param name="value"> Enthält nach dem Beenden die Li, die dem angegebenen Schlüssel zugeordnet ist, 
        /// falls der Schlüssel gefunden wurde; null sonst. </param>
        /// <returns> True, falls der Key existiert, false sonst. </returns>
        public bool TryGetValues(K key, out Li<V> value) => dict.TryGetValue(key, out value);

        /// <summary> Gets the flattened collection of values, for which the predicate is true. </summary>
        public Li<V> ValuesWhere(System.Func<V, bool> predicate)
        {
            var values = new Li<V>();
            foreach (var vals in dict.Values)
                foreach (var val in vals)
                    if (predicate(val))
                        values.Add(val);
            return values;
        }

        /// <summary> Returns the value for key, if there is an unique one. Null otherwise. </summary>
        public V TryGetUniqueValue(K key)
        {
            dict.TryGetValue(key, out Li<V> ee);
            V e = ee?.Count == 1 ? ee[0] : null;
            return e;
        }

        /// <summary> Removes the specified value for the specified key. </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>True</c> if such a value existed and was removed; otherwise <c>false</c>.</returns>
        public bool Remove(K key, V value)
        {
            if (!dict.ContainsKey(key))
                return false;

            return dict[key].Remove(value);
        }

        /// <summary> Removes all values for the specified key. </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>True</c> if any such values existed; otherwise <c>false</c>.</returns>
        public bool RemoveAll(K key) => dict.Remove(key);

        public void Clear() => dict.Clear();

        /// <summary> True if the the multimap contains any values for the specified key. </summary>
        public bool ContainsKey(K key) => dict.ContainsKey(key);

        private readonly Dictionary<K, Li<V>> dict = new Dictionary<K, Li<V>>();
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AwiUtils
{
    /// <summary> A class that is like a set in this regard that it can't hold elements twice; and that is like a list 
    /// in the regard of keeping insertion order. From .NET 4.0 on there is SortedSet for this purpose. </summary>
    /// <typeparam name="T"></typeparam>
    public class OrderedSet<T>
    {
        public OrderedSet() { set = new HashSet<T>(); }
        public OrderedSet(IEnumerable<T> items) { set = new HashSet<T>(); AddRange(items); }
        public OrderedSet(IEqualityComparer<T> comparer) { set = new HashSet<T>(comparer); }

        public void Add(T item)
        {
            if (!set.Contains(item))
            {
                set.Add(item);
                items.Add(item);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public ReadOnlyCollection<T> Items { get { return items.AsReadOnly(); } }

        /// <remarks> Contains(item) is O(1). </remarks>
        public bool Contains(T t) { return set.Contains(t); }

        public int Count => items.Count;

        public override string ToString() => items.ToString();

        Li<T> items = new Li<T>();
        HashSet<T> set;
    }
}

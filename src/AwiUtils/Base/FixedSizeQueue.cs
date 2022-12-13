using System.Collections.Generic;

namespace AwiUtils
{
    /// <summary> A queue which cannot grow longer than a certain size. </summary>
    public class FixedSizedQueue<T> : Queue<T>
    {
        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public int Size { get; private set; }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            while (base.Count > Size)
                base.Dequeue();
        }
    }
}

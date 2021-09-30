using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.XelNaga.Models
{
    public class SafeQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly object loc = new object();

        public void Enqueue(T item)
        {
            lock (loc)
            {
                queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            T item;
            lock (loc)
            {
                item = queue.Dequeue();
            }
            return item;
        }

        public bool Any()
        {
            bool any;
            lock (loc)
            {
                any = queue.Any();
            }
            return any;
        }

        public int Count()
        {
            int count = 0;
            lock (loc)
            {
                count = queue.Count();
            }
            return count;
        }
    }
}

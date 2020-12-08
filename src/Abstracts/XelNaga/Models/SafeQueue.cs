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
            T item = default;
            lock (loc)
            {
                item = queue.Dequeue();
            }
            return item;
        }

        public bool Any()
        {
            bool any = false;
            lock (loc)
            {
                any = queue.Any();
            }
            return any;
        }
    }
}

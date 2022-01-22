using System.Collections.Concurrent;

namespace Yggdrasil.Utility
{
    // Trying to avoid having to use locks. The pool size can still go above maximum
    // from race conditions.

    public class ConcurrentPool<T> where T : class, new()
    {
        public volatile int MaxPoolCount = int.MaxValue - 100;

        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public int Count => _queue.Count;

        public void Clear()
        {
            while (_queue.TryDequeue(out _)) { }
        }

        public void PrePool(int count)
        {
            while (count-- > 0 && _queue.Count < MaxPoolCount)
            {
                _queue.Enqueue(new T());
            }
        }

        public T Get()
        {
            if (_queue.TryDequeue(out var item)) { return item; }

            return new T();
        }

        public void Recycle(T item)
        {
            if (_queue.Count >= MaxPoolCount) { return; }
            _queue.Enqueue(item);
        }
    }
}
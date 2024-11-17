using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Mmogf.Servers
{
    public class ConcurrentHashSet<T>
    {
        private ConcurrentDictionary<T, byte> _internalDictionary;

        public ConcurrentHashSet()
        {
            _internalDictionary = new ConcurrentDictionary<T, byte>();
        }

        public bool TryAdd(T item)
        {
            return _internalDictionary.TryAdd(item, 1);
        }

        public bool TryRemove(T item)
        {
            return _internalDictionary.TryRemove(item, out _);
        }

        public bool Contains(T item)
        {
            return _internalDictionary.ContainsKey(item);
        }

        public ICollection<T> AsCollection()
        {
            return _internalDictionary.Keys;
        }
    }
}

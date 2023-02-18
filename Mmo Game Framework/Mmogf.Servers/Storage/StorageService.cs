using ServiceStack.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Storage
{
    public class StorageService : IStorageService
    {
        private ICacheClient _cacheClient;

        public StorageService(ICacheClient cacheClient)
        {
            _cacheClient = cacheClient;
        }

        public T Get<T>(string key)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return _cacheClient.Get<T>(key);
        }

        public bool Set<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return _cacheClient.Set(key, data);
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return _cacheClient.Remove(key);
        }

        public long Increment(string key, uint data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return _cacheClient.Increment(key, data);
        }

        public long Decrement(string key, uint data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return _cacheClient.Decrement(key, data);
        }
    }
}

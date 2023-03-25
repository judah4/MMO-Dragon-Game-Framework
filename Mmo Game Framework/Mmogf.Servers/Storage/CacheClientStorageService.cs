using ServiceStack.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Storage
{
    public sealed class CacheClientStorageService : IStorageService
    {
        private readonly ICacheClient _cacheClient;

        private string _storageType;

        public CacheClientStorageService(ICacheClient cacheClient, string storageType)
        {
            if (cacheClient == null)
                throw new ArgumentNullException(nameof(cacheClient));
            if (storageType == null)
                throw new ArgumentNullException(nameof(storageType));

            _cacheClient = cacheClient;
            _storageType = storageType;
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

        public override string ToString()
        {
            return _storageType;
        }
    }
}

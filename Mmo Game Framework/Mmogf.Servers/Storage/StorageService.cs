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

        public bool Add<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return _cacheClient.Add(key, data);
        }
    }
}

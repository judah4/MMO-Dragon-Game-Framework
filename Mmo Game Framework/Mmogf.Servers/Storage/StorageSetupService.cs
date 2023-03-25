using Microsoft.Extensions.Configuration;
using ServiceStack.Caching;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Storage
{
    public class StorageSetupService
    {
        private readonly IConfiguration _configuration;
        private ICacheClient _cacheClient;
        private IStorageService _storageService;
        private RedisManagerPool _redisManagerPool;

        public string StorageType {  get; private set; }

        public StorageSetupService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Init()
        {
            if(_cacheClient != null)
                return;

            //https://docs.servicestack.net/caching

            var host = _configuration["Storage:Host"];
            StorageType = _configuration["Storage:Type"];

            switch (StorageType)
            {
                default: //memory
                    _cacheClient = new MemoryCacheClient();
                    StorageType = "in-memory";
                    _storageService = new CacheClientStorageService(_cacheClient, StorageType);
                    break;
                case "memcache":
                    //package is dotnet framework for some reason. Unsupported for linux at the moment.
                    //_cacheClient = new MemcachedClientCache(new[] { "127.0.0.0" });
                    throw new Exception("memcache is not supported yet.");
                    //break;
                case "redis":
                    //"gateway.docker.internal:6379"
                    _redisManagerPool = new RedisManagerPool(host);
                    _cacheClient = _redisManagerPool.GetCacheClient();
                    _storageService = new CacheClientStorageService(_cacheClient, StorageType);
                    break;
            }


        }

        public IStorageService GetStorage()
        {
            Init();

            return _storageService;
        }

    }
}

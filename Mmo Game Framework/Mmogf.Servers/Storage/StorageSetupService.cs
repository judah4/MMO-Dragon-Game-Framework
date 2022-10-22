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
        readonly IConfiguration _configuration;
        ICacheClient _cacheClient;
        RedisManagerPool _redisManagerPool;

        public StorageSetupService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Init()
        {
            if(_cacheClient != null)
                return;

            //https://docs.servicestack.net/caching

            var type = _configuration["Storage:Type"];
            var host = _configuration["Storage:Host"];

            switch(type)
            {
                default: //memory
                    _cacheClient = new MemoryCacheClient();
                    break;
                //case "memcahce":
                //    package is dotnet framework for some reason. Unsupported for linux at the moment.
                //    _cacheClient = new MemcachedClientCache(new[] { "127.0.0.0" });
                //    break;
                case "redis":
                    //"gateway.docker.internal:6379"
                    _redisManagerPool = new RedisManagerPool(host);
                    _cacheClient = _redisManagerPool.GetCacheClient();
                    break;
            }


        }

        public ICacheClient GetStorage()
        {
            Init();

            return _cacheClient;
        }

    }
}

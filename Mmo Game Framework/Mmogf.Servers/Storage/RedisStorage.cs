using ServiceStack.Caching;
using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Storage
{
    public class RedisStorage
    {
        ICacheClient _redis;

        public async Task ConnectTest()
        {
            if(_redis == null)
            {
                _redis = new MemoryCacheClient();
            }
            
            var result = _redis.Add<string>("Foo", "bar");
            Console.WriteLine(result);
            //container.Register<IRedisClientsManager>(c =>new RedisManagerPool("gateway.docker.internal:6379"));
            //container.Register(c => c.Resolve<IRedisClientsManager>().GetCacheClient());
        }

    }

}

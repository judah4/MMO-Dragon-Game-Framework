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
        ConnectionMultiplexer _redis;

        public async Task ConnectTest()
        {
            if(_redis == null)
            {
                _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    EndPoints = { "localhost:6379" },// change this to the docker ip
                });
            }
            
            var db = _redis.GetDatabase();
            var pong = await db.PingAsync();
            Console.WriteLine(pong);
        }
    }
}

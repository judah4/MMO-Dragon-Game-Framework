using ServiceStack.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmogf.Servers.Storage
{
    public interface IStorageService
    {
        T Get<T>(string key);

        bool Set<T>(string key, T data);
        bool Remove(string key);
        long Increment(string key, uint data);
        long Decrement(string key, uint data);

    }
}

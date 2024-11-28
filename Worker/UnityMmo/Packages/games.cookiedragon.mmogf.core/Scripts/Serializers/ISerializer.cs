using System;

namespace Mmogf.Servers.Serializers
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T obj);

        T Deserialize<T>(byte[] data);

        object Deserialize(Type type, byte[] data);
    }
}

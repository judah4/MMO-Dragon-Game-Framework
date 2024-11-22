using ProtoBuf;
using System.IO;

namespace Mmogf.Servers.Serializers
{
    public class ProtobufSerializer : ISerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}

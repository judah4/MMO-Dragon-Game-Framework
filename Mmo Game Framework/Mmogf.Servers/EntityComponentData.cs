namespace Mmogf.Servers
{
    public sealed class EntityComponentData : IComponentData
    {
        private readonly byte[] _data;

        public EntityComponentData(byte[] bytes)
        {
            _data = bytes;
        }

        public byte[] AsBytes()
        {
            return _data;
        }
    }
}

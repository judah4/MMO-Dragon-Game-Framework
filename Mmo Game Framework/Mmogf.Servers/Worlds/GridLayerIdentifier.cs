namespace Mmogf.Servers.Shared
{
    public struct GridLayerIdentifier
    {

        public GridLayerIdentifier(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public static bool operator ==(GridLayerIdentifier a, GridLayerIdentifier b) => a.Equals(b);
        public static bool operator !=(GridLayerIdentifier a, GridLayerIdentifier b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is GridLayerIdentifier other && Equals(other);
        }

        public bool Equals(GridLayerIdentifier other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}
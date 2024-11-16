namespace Mmogf.Servers.Shared
{
    public struct RemoteWorkerIdentifier
    {
        public RemoteWorkerIdentifier(long id)
        {
            Id = id;
        }

        public long Id { get; }

        public bool IsValid()
        {
            return Id != 0;
        }

        public static bool operator ==(RemoteWorkerIdentifier a, RemoteWorkerIdentifier b) => a.Equals(b);
        public static bool operator !=(RemoteWorkerIdentifier a, RemoteWorkerIdentifier b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is RemoteWorkerIdentifier other && Equals(other);
        }

        public bool Equals(RemoteWorkerIdentifier other)
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
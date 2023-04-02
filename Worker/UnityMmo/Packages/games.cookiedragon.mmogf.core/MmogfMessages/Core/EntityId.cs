using MessagePack;
using System;

namespace Mmogf.Core
{
    [MessagePackObject]
    public struct EntityId
    {

        public EntityId(int id)
        {
           Id = id;
        }

        [Key(0)]
        public int Id { get; }

        public bool IsValid()
        {
            return Id != 0;
        }

        public static bool operator ==(EntityId a, EntityId b) => a.Equals(b);
        public static bool operator !=(EntityId a, EntityId b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is EntityId other && Equals(other);
        }

        public bool Equals(EntityId other)
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
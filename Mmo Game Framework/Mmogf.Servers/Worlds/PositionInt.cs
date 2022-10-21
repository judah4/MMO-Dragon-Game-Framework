using System;

namespace Mmogf.Servers.Worlds
{

    public struct PositionInt
    {

        public PositionInt(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }


        public static bool operator ==(PositionInt a, PositionInt b) => a.Equals(b);
        public static bool operator !=(PositionInt a, PositionInt b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PositionInt other && Equals(other);
        }

        public bool Equals(PositionInt other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

    }
}
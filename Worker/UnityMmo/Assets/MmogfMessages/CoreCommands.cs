using System;
using System.Runtime.Serialization;

namespace Mmogf
{

    [DataContract]
    public struct Nothing
    {
    }

    [DataContract]
    public partial struct Vector3d : System.IEquatable<Vector3d>
    {
        [DataMember(Order = 1)]
        public double X { get; set; }
        [DataMember(Order = 2)]
        public double Y { get; set; }
        [DataMember(Order = 2)]
        public double Z { get; set; }

        public bool Equals(Vector3d other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3d)
            {
                return Equals((Vector3d)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            ////https://stackoverflow.com/questions/1646807/quick-and-simple-hash-code-combinations
            //unchecked
            //{
            //    int hash = 17;
            //    hash = hash * 31 + X.GetHashCode();
            //    hash = hash * 31 + Y.GetHashCode();
            //    hash = hash * 31 + Z.GetHashCode();
            //    return hash;
            //}

            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(Vector3d first, Vector3d second)
        {
            return first.Equals(second);
        }
        public static bool operator !=(Vector3d first, Vector3d second)
        {
            // or !Equals(first, second), but we want to reuse the existing comparison 
            return !(first == second);
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

    }

    [DataContract]
    public partial struct Vector3f
    {
        [DataMember(Order = 1)]
        public float X { get; set; }
        [DataMember(Order = 2)]
        public float Y { get; set; }
        [DataMember(Order = 3)]
        public float Z { get; set; }

    }

}

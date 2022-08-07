using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{

    [MessagePackObject]
    public struct Nothing
    {
    }

    [MessagePackObject]
    public partial struct Vector3d : System.IEquatable<Vector3d>
    {
        [Key(0)]
        public double X { get; set; }
        [Key(1)]
        public double Y { get; set; }
        [Key(2)]
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

    [MessagePackObject]
    public partial struct Vector3f
    {
        [Key(0)]
        public float X { get; set; }
        [Key(1)]
        public float Y { get; set; }
        [Key(2)]
        public float Z { get; set; }

    }

}

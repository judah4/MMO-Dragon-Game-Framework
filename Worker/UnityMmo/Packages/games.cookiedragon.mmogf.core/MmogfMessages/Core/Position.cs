using System;

namespace Mmogf.Core
{

    public struct Position
    {

        public Position(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static double DistanceSquared(Position position1, Position position2)
        {
            var xsq = Math.Pow(position1.X - position2.X, 2);
            var ysq = Math.Pow(position1.Y - position2.Y, 2);
            var zsq = Math.Pow(position1.Z - position2.Z, 2);

            return xsq + ysq + zsq;
        }
        public static double Distance(Position position1, Position position2)
        {
            return Math.Sqrt(DistanceSquared(position1, position2));
        }


        public static bool WithinArea(Position position, Position point, float radius)
        {
            var dist = Distance(position, point);

            return dist < radius;

        }

        public static readonly Position Zero = new Position(0, 0, 0);

        public static bool operator ==(Position a, Position b) => a.Equals(b);
        public static bool operator !=(Position a, Position b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Position other && Equals(other);
        }

        public bool Equals(Position other)
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

        public FixedVector3 ToFixedVector3()
        {
            return new FixedVector3
            {
                X = DoubleToFixed(X),
                Y = DoubleToFixed(Y),
                Z = DoubleToFixed(Z)
            };
        }

        public static Position FromFixedVector3(FixedVector3 v)
        {
            return new Position
            {
                X = FixedToDouble(v.X),
                Y = FixedToDouble(v.Y),
                Z = FixedToDouble(v.Z)
            };
        }


        // 2^-10 => 0.0009765625 precision
        private const int FixedPointOne = (int)(1u << 10);

        private static int DoubleToFixed(double a)
        {
            return (int)(a * FixedPointOne);
        }

        private static double FixedToDouble(int a)
        {
            return (double)a / FixedPointOne;
        }

    }
}
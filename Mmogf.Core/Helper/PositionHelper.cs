using System;
using System.Collections.Generic;
using System.Text;

namespace Mmogf.Core
{
    public partial class Position
    {
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
    }
}

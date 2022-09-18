using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public static class VectorHelpers
    {
        public static Mmogf.Vector3d ToVector3d(this Vector3 vector3, CommonHandler server)
        {
            var pos = server.transform.position;


            return new Mmogf.Vector3d()
            {
                X = (double)vector3.x - pos.x,
                Y = (double)vector3.y - pos.x,
                Z = (double)vector3.z - pos.x,
            };
        }

        public static Vector3 ToVector3(this Vector3d vector3, CommonHandler server)
        {
            var pos = server.transform.position;
            vector3.X += pos.x;
            vector3.Y += pos.y;
            vector3.Z += pos.z;

            return new Vector3()
            {
                x = (float)vector3.X,
                y = (float)vector3.Y,
                z = (float)vector3.Z,
            };
        }
    }
}

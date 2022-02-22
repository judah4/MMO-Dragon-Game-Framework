using Mmogf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class RotationHelpers
    {
        public static Quaternion ToQuaternion(this Rotation rotation)
        {
            return new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
        }

        public static Rotation ToRotation(this Quaternion rotation)
        {
            return new Rotation() 
            {
                X = rotation.x,
                Y = rotation.y,
                Z = rotation.z,
                W = rotation.w
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mmogf.Core
{
    public static class RotationHelpers
    {
        public static Quaternion ToQuaternion(this Rotation rotation)
        {
            return Quaternion.Euler(0, rotation.Heading, 0);
        }

        public static Rotation ToRotation(this Quaternion rotation)
        {
            return new Rotation() 
            {
                Heading = rotation.eulerAngles.y,
            };
        }
    }
}

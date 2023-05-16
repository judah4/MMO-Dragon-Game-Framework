using UnityEngine;

namespace Mmogf.Core
{
    public static class RotationHelpers
    {


        public static Quaternion ToQuaternion(this Rotation rotation)
        {
            var heading = rotation.Heading;
            float yNormed = (float)heading / short.MaxValue;
            float y = yNormed * 360f;

            return Quaternion.Euler(0, y, 0);
        }

        public static Rotation ToRotation(this Quaternion rotation)
        {
            float y = rotation.eulerAngles.y;
            float yNormed = y / 360f;
            short heading = (short)(yNormed * short.MaxValue);

            return new Rotation() 
            {
                Heading = heading,
            };
        }
    }
}

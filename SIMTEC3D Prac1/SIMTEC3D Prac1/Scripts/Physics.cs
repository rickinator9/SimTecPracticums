using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SIMTEC3D_Prac1.Scripts
{
    class Physics
    {
        public static Vector3 dotProductCalculation(Vector3 direction1, Vector3 direction2)
        {
            if (direction1.Length() == 0 || direction2.Length() == 0)
            {
                return Vector3.Zero;
            }
            else
            {
                Vector3 normalized1 = new Vector3(direction1.X, direction1.Y, direction1.Z);
                normalized1.Normalize();
                Vector3 normalized2 = new Vector3(direction2.X, direction2.Y, direction2.Z);
                normalized2.Normalize();
                float dotProduct = normalized1.X * normalized2.X + normalized1.Y * normalized2.Y + normalized1.Z * normalized2.Z;

                return dotProduct * normalized2 * direction1.Length();
            }
        }

        public static Vector3 getDistanceBetweenPoint(Vector3 point1, Vector3 point2, Vector3 direction)
        {
            if (point1.Equals(point2))
            {
                return Vector3.Zero;
            }
            else
            {
                Vector3 distance = point2 - point1;
                Vector3 normalizedDistance = distance;
                normalizedDistance.Normalize();
                float dotProduct = normalizedDistance.X * direction.X + normalizedDistance.Y * direction.Y + normalizedDistance.Z * direction.Z;

                return dotProduct * direction * distance.Length();
            }
        }
    }
}

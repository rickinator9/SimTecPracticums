using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SIMTEC3D_Prac1.Scripts
{
    public class CollisionInfo
    {
        public Vector3 pivot;
        public Vector2 distanceTillCollision;
        public float planeRotation;

        public CollisionInfo(Vector3 pivot, Vector2 distanceTillCollision, float planeRotation)
        {
            this.pivot = pivot;
            this.distanceTillCollision = distanceTillCollision;
            this.planeRotation = planeRotation;
        }

        public CollisionInfo duplicate()
        {
            return new CollisionInfo(pivot, distanceTillCollision, planeRotation);
        }
    }
}

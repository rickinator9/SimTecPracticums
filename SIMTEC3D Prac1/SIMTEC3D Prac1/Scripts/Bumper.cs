using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SIMTEC3D_Prac1.Scripts
{
    class Bumper : Box
    {
        public Bumper(Vector3 position, Vector3 rotation, float scale, GraphicsDevice device)
            : base(position, rotation, scale, device)
        {
            surfaces = new Plane[10];

            Vector3 frontPosition = new Vector3(position.X, position.Y - 1f * scale, position.Z + 1f * scale);
            Vector3 frontRotation = new Vector3(rotation.X + (float)(0.5f * Math.PI), rotation.Y, rotation.Z);
            surfaces[0] = new Plane(frontPosition, frontRotation, scale, device);
            surfaces[1] = new Plane(new Vector3(frontPosition.X, frontPosition.Y + 2f * scale, frontPosition.Z), frontRotation, scale, device);

            Vector3 leftPosition = new Vector3(position.X - 1f * scale, position.Y - 1f * scale, position.Z);
            Vector3 leftRotation = new Vector3(rotation.X, rotation.Y, rotation.Z + (float)(0.5f * Math.PI));
            surfaces[2] = new Plane(leftPosition, leftRotation, scale, device);
            surfaces[3] = new Plane(new Vector3(leftPosition.X, leftPosition.Y + 2f * scale, leftPosition.Z), leftRotation, scale, device);

            Vector3 rightPosition = new Vector3(position.X + 1f * scale, position.Y - 1f * scale, position.Z);
            Vector3 rightRotation = new Vector3(rotation.X, rotation.Y, rotation.Z + (float)(1.5f * Math.PI));
            surfaces[4] = new Plane(rightPosition, rightRotation, scale, device);
            surfaces[5] = new Plane(new Vector3(rightPosition.X, rightPosition.Y + 2f * scale, rightPosition.Z), rightRotation, scale, device);

            Vector3 backPosition = new Vector3(position.X, position.Y - 1f * scale, position.Z - 1f * scale);
            Vector3 backRotation = new Vector3(rotation.X + (float)(1.5f * Math.PI), rotation.Y, rotation.Z);
            surfaces[6] = new Plane(backPosition, backRotation, scale, device);
            surfaces[7] = new Plane(new Vector3(backPosition.X, backPosition.Y + 2f * scale, backPosition.Z), backRotation, scale, device);

            Vector3 topPosition = new Vector3(position.X, position.Y + 2f * scale, position.Z);
            Vector3 topRotation = rotation;
            surfaces[8] = new Plane(topPosition, topRotation, scale, device);

            Vector3 bottomPosition = new Vector3(position.X, position.Y - 2f * scale, position.Z);
            Vector3 bottomRotation = new Vector3(rotation.X, rotation.Y, rotation.Z + (float)(1f * Math.PI));
            surfaces[9] = new Plane(bottomPosition, bottomRotation, scale, device);
        } 
    }
}

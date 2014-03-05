using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SIMTEC3D_Prac1.Scripts
{
    class Box: GameObject
    {
        private Plane[] surfaces;

        public Box(Vector3 position, Vector3 rotation, float scale, GraphicsDevice device): base() {
            surfaces = new Plane[6];

            Vector3 botPosition = new Vector3(position.X, position.Y - scale, position.Z);
            surfaces[0] = new Plane(botPosition, rotation, scale, device);

            Vector3 topPosition = new Vector3(position.X, position.Y + scale, position.Z);
            Vector3 topRotation = new Vector3(rotation.X + (float)Math.PI, rotation.Y, rotation.Z);
            surfaces[1] = new Plane(topPosition, topRotation, scale, device);

            Vector3 rightPosition = new Vector3(position.X + scale, position.Y, position.Z);
            Vector3 rightRotation = new Vector3(rotation.X, rotation.Y, rotation.Z + (float)(0.5*Math.PI));
            surfaces[2] = new Plane(rightPosition, rightRotation, scale, device);

            Vector3 leftPosition = new Vector3(position.X - scale, position.Y, position.Z);
            Vector3 leftRotation = new Vector3(rotation.X, rotation.Y, rotation.Z + (float)(1.5 * Math.PI));
            surfaces[3] = new Plane(leftPosition, leftRotation, scale, device);

            Vector3 backPosition = new Vector3(position.X, position.Y, position.Z + scale);
            Vector3 backRotation = new Vector3(rotation.X + (float)(1.5 * Math.PI), rotation.Y, rotation.Z);
            surfaces[4] = new Plane(backPosition, backRotation, scale, device);

            Vector3 frontPosition = new Vector3(position.X, position.Y, position.Z - scale);
            Vector3 frontRotation = new Vector3(rotation.X + (float)(0.5 * Math.PI), rotation.Y, rotation.Z);
            surfaces[5] = new Plane(frontPosition, frontRotation, scale, device);
        }

        public override void LoadContent(ContentManager content)
        {
            foreach (Plane plane in surfaces)
            {
                plane.LoadContent(content);
            }
        }

        public Plane[] planes
        {
            get
            {
                return surfaces;
            }
        }

        public override void update(float deltaTime)
        {
            foreach (Plane plane in surfaces)
            {
                plane.update(deltaTime);
            }
        }

        public override void draw(Matrix viewMatrix)
        {
            foreach (Plane plane in surfaces)
            {
                plane.draw(viewMatrix);
            }
        }
    }
}

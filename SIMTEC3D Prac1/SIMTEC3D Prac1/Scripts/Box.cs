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
        protected Plane[] surfaces;

        public Box(Vector3 position, Vector3 rotation, float scale, GraphicsDevice device): base() {
            surfaces = new Plane[13];

            //A bottom plane with hole
            float tild = (float)(0.01 * Math.PI);
            Vector3 leftTilted = new Vector3(rotation.X, rotation.Y, rotation.Z + tild);
            Vector3 rightTilted = new Vector3(rotation.X, rotation.Y, rotation.Z - tild);
            float tildHeightOffSet = (float)Math.Sin(tild)*scale;

            Vector3 botPosition1 = new Vector3(position.X + 2f / 3f * scale, position.Y - scale + tildHeightOffSet/3, position.Z - 2f / 3f * scale);
            surfaces[0] = new Plane(botPosition1, leftTilted, scale / 3f, device);
            Vector3 botPosition2 = new Vector3(position.X + 2f / 3f * scale, position.Y - scale + tildHeightOffSet / 3, position.Z);
            surfaces[1] = new Plane(botPosition2, leftTilted, scale / 3f, device);
            Vector3 botPosition3 = new Vector3(position.X + 2f / 3f * scale, position.Y - scale + tildHeightOffSet / 3, position.Z + 2f / 3f * scale);
            surfaces[2] = new Plane(botPosition3, leftTilted, scale / 3f, device);
            Vector3 botPosition4 = new Vector3(position.X, position.Y - scale, position.Z);
            surfaces[3] = new Plane(botPosition4, rotation, scale / 3f, device);
            Vector3 botPosition5 = new Vector3(position.X - 2f / 3f * scale, position.Y - scale + tildHeightOffSet / 3, position.Z + 2f / 3f * scale);
            surfaces[4] = new Plane(botPosition5, rightTilted, scale / 3f, device);
            Vector3 botPosition6 = new Vector3(position.X - 2f / 3f * scale, position.Y - scale + tildHeightOffSet / 3, position.Z);
            surfaces[5] = new Plane(botPosition6, rightTilted, scale / 3f, device);
            Vector3 botPosition7 = new Vector3(position.X - 2f / 3f * scale, position.Y - scale + tildHeightOffSet / 3, position.Z - 2f / 3f * scale);
            surfaces[6] = new Plane(botPosition7, rightTilted, scale / 3f, device);
            Vector3 botPosition8 = new Vector3(position.X, position.Y - scale, position.Z - 2f / 3f * scale);
            surfaces[7] = new Plane(botPosition8, rotation, scale / 3f, device);

            //The box
            Vector3 topPosition = new Vector3(position.X, position.Y + scale, position.Z);
            Vector3 topRotation = new Vector3(rotation.X + (float)Math.PI, rotation.Y, rotation.Z);
            surfaces[8] = new Plane(topPosition, topRotation, scale, device);

            Vector3 rightPosition = new Vector3(position.X + scale, position.Y, position.Z);
            Vector3 rightRotation = new Vector3(rotation.X, rotation.Y, rotation.Z + (float)(0.5 * Math.PI));
            surfaces[9] = new Plane(rightPosition, rightRotation, scale, device);

            Vector3 leftPosition = new Vector3(position.X - scale, position.Y, position.Z);
            Vector3 leftRotation = new Vector3(rotation.X, rotation.Y, rotation.Z + (float)(1.5 * Math.PI));
            surfaces[10] = new Plane(leftPosition, leftRotation, scale, device);

            Vector3 backPosition = new Vector3(position.X, position.Y, position.Z + scale);
            Vector3 backRotation = new Vector3(rotation.X + (float)(1.5 * Math.PI), rotation.Y, rotation.Z);
            surfaces[11] = new Plane(backPosition, backRotation, scale, device);

            Vector3 frontPosition = new Vector3(position.X, position.Y, position.Z - scale);
            Vector3 frontRotation = new Vector3(rotation.X + (float)(0.5 * Math.PI), rotation.Y, rotation.Z);
            surfaces[12] = new Plane(frontPosition, frontRotation, scale, device);
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

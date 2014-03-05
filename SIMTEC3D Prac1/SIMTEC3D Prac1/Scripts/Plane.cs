using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SIMTEC3D_Prac1.Scripts
{
    class Plane: ModelObject
    {
        private Vector4 equation;

        public Plane(Vector3 position, Vector3 rotation, float scale, GraphicsDevice device) : base(position, rotation, scale, device) 
        {
            //normal direction calculation is not added yet!
            Vector3 normalDirection = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z));
            float d = position.X * normalDirection.X + position.Y * normalDirection.Y + position.Z * normalDirection.Z;
            equation = new Vector4(normalDirection, d);
        }

        protected override Model loadModel(ContentManager content)
        {
            return content.Load<Model>("Models\\plane");
        }

        public Vector4 planeEquation
        {
            get
            {
                return equation;
            }
        }
    }
}

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
        private Vector4 _equation;

        public Plane(Vector3 position, Vector3 rotation, float scale, GraphicsDevice device) : base(position, rotation, scale, device) 
        {
            //normal direction calculation is not added yet!
            Vector3 normalDirection = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z));
            float d = position.X * normalDirection.X + position.Y * normalDirection.Y + position.Z * normalDirection.Z;
            _equation = new Vector4(normalDirection, d);
        }

        protected override Model loadModel(ContentManager content)
        {
            return content.Load<Model>("Models\\plane");
        }

        public Vector3 getDistanceTillPoint(Vector3 point)
        {
            Vector3 pivot = getPivotWithLine(point, normal);
            return (pivot - point);
        }

        //Need to be determend yet
        public bool inBondry(Vector3 point)
        {
            return true;
        }

        public Vector3 getPivotWithLine(Vector3 point, Vector3 direction)
        {
            float t = (_equation.W - (_equation.X * point.X + _equation.Y * point.Y + _equation.Z * point.Z))
                / (_equation.X * direction.X + _equation.Y * direction.Y + _equation.Z * direction.Z);

           Vector3 pivot = new Vector3(point.X + t * direction.X, point.Y + t * direction.Y, point.Z + t * direction.Z);

           return inBondry(pivot)? pivot: new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        public Vector4 equation
        {
            get
            {
                return _equation;
            }
        }

        public Vector3 normal
        {
            get
            {
                return new Vector3(equation.X, equation.Y, equation.Z);
            }
        }
    }
}

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

        private float getDistanceTillBoundry(Vector3 point, Vector3 ax)
        {
            ax = Vector3.Transform(ax, Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z));

            Vector3 min = position - scale * ax;
            Vector3 max = position + scale * ax;

            Vector3 distanceTillMinVector = Physics.getDistanceBetweenPoint(min, point, ax);
            Vector3 distanceTillMaxVector = Physics.getDistanceBetweenPoint(max, point, ax);

            float distanceTillMin = distanceTillMinVector.X + distanceTillMinVector.Y + distanceTillMinVector.Z;
            float distanceTillMax = distanceTillMaxVector.X + distanceTillMaxVector.Y + distanceTillMaxVector.Z;

            float distanceTillBoundry;
            if (Math.Sign(distanceTillMin) != Math.Sign(distanceTillMax) || distanceTillMin.Equals(Vector3.Zero) || distanceTillMax.Equals(Vector3.Zero))
            {
                distanceTillBoundry = 0;
            }
            else
            {
                distanceTillBoundry = Math.Min(Math.Abs(distanceTillMin), Math.Abs(distanceTillMax));
            }

            return distanceTillBoundry;
        }

        public Vector2 getDistanceTillBoundry(Vector3 point)
        {
            return new Vector2(getDistanceTillBoundry(point, Vector3.Right), getDistanceTillBoundry(point, Vector3.Forward));
        }

        public CollisionInfo getPivotWithLineAndDistanceTillBoundries(Vector3 point, Vector3 direction, float radius)
        {
            Vector2 distance = getDistanceTillBoundry(point);
            if (distance.X <= radius && distance.Y <= radius)
            {
                return new CollisionInfo(getPivotWithLine(point, direction), distance, 0);
            }
            else
            {
                return null;
            }
        }

        public Vector3 getPivotWithLine(Vector3 point, Vector3 direction)
        {
            float t = (_equation.W - (_equation.X * point.X + _equation.Y * point.Y + _equation.Z * point.Z))
                / (_equation.X * direction.X + _equation.Y * direction.Y + _equation.Z * direction.Z);

            Vector3 pivot = new Vector3(point.X + t * direction.X, point.Y + t * direction.Y, point.Z + t * direction.Z);

            return pivot;
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

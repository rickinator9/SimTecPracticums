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

        //Calculate how far a pivot is away from a plane in direction ax
        private float getDistanceTillBoundry(Vector3 pivot, Vector3 ax)
        {
            //Apply the rotation of the plane on the vector ax
            ax = Vector3.Transform(ax, Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z));

            //Determen the limits of the plane
            Vector3 min = position - scale * ax;
            Vector3 max = position + scale * ax;

            //Determen the distance from point to plane limit
            Vector3 distanceTillMinVector = Physics.getDistanceBetweenPoint(min, pivot, ax);
            Vector3 distanceTillMaxVector = Physics.getDistanceBetweenPoint(max, pivot, ax);
            float distanceTillMin = distanceTillMinVector.X + distanceTillMinVector.Y + distanceTillMinVector.Z;
            float distanceTillMax = distanceTillMaxVector.X + distanceTillMaxVector.Y + distanceTillMaxVector.Z;

            float distanceTillBoundry;
            if (Math.Sign(distanceTillMin) != Math.Sign(distanceTillMax) || distanceTillMin.Equals(Vector3.Zero) || distanceTillMax.Equals(Vector3.Zero)) //Determen if a plane is between or on the plane limits
            {
                distanceTillBoundry = 0;
            }
            else
            {
                if (Math.Abs(distanceTillMin) < Math.Abs(distanceTillMax))
                {
                    distanceTillBoundry = -distanceTillMin;
                }
                else
                {
                    distanceTillBoundry = -distanceTillMax;
                }
            }

            return distanceTillBoundry;
        }

        //Determen how far a pivot is away square plane
        public Vector2 getDistanceTillBoundry(Vector3 pivot)
        {
            return new Vector2(getDistanceTillBoundry(pivot, Vector3.Right), getDistanceTillBoundry(pivot, Vector3.Forward));
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

        public static Vector3 vectorMatrixMultiplication(Vector3 vector, Matrix matrix) 
        {
            float x = vector.X * matrix.M11 + vector.Y * matrix.M12 + vector.Z * matrix.M13 + matrix.M41;
            float y = vector.X * matrix.M21 + vector.Y * matrix.M22 + vector.Z * matrix.M23 + matrix.M42;
            float z = vector.X * matrix.M31 + vector.Y * matrix.M32 + vector.Z * matrix.M33 + matrix.M43;
            return new Vector3(x, y, z);
        }

        public void rotate(Matrix matrix, Vector3 rotation)
        {
            this.rotation -= rotation;
            position = vectorMatrixMultiplication(position, matrix);

            Vector3 normalDirection = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(this.rotation.X) * Matrix.CreateRotationY(this.rotation.Y) * Matrix.CreateRotationZ(this.rotation.Z));
            float d = position.X * normalDirection.X + position.Y * normalDirection.Y + position.Z * normalDirection.Z;
            _equation = new Vector4(normalDirection, d);
        }

        public void translate(Matrix matrix)
        {
            position = vectorMatrixMultiplication(position, matrix);

            Vector3 normalDirection = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z));
            float d = position.X * normalDirection.X + position.Y * normalDirection.Y + position.Z * normalDirection.Z;
            _equation = new Vector4(normalDirection, d);
        }
    }
}

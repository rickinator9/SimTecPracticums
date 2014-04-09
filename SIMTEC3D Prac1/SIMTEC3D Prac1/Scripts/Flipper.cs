using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SIMTEC3D_Prac1.Scripts
{
    class Flipper : GameObject
    {
        protected Plane[] surfaces;
        private Vector3 pivot;
        private float rotation;
        private float maxAngle;
        private float rotationASecond;
        private Ball ball;

        public Flipper(Vector3 position, Vector3 pivot, float maxAngle, float rotationASecond, Vector3 rotation, float scale, Ball ball, GraphicsDevice device): base() {
            this.pivot = position + pivot * scale;
            this.rotation = 0;
            this.maxAngle = maxAngle;
            this.rotationASecond = rotationASecond;
            this.ball = ball;

            surfaces = new Plane[10];

            //The box
            Vector3 botPosition = new Vector3(position.X, position.Y - scale, position.Z);
            Vector3 botRotation = new Vector3(rotation.X + (float)Math.PI, rotation.Y, rotation.Z);
            surfaces[0] = new Plane(botPosition, botRotation, scale, device);
            Vector3 botPosition2 = new Vector3(position.X + 2 * scale, position.Y - scale, position.Z);
            surfaces[1] = new Plane(botPosition2, botRotation, scale, device);

            Vector3 topPosition = new Vector3(position.X, position.Y + scale, position.Z);
            Vector3 topRotation = rotation;
            surfaces[2] = new Plane(topPosition, topRotation, scale, device);
            Vector3 topPosition2 = new Vector3(position.X + 2 * scale, position.Y + scale, position.Z);
            surfaces[3] = new Plane(topPosition2, topRotation, scale, device);

            Vector3 rightPosition = new Vector3(position.X + 3 * scale, position.Y, position.Z);
            Vector3 rightRotation = new Vector3(rotation.X, rotation.Y, rotation.Z - 0.5f*(float)Math.PI);
            surfaces[4] = new Plane(rightPosition, rightRotation, scale, device);

            Vector3 leftPosition = new Vector3(position.X - scale, position.Y, position.Z);
            Vector3 leftRotation = new Vector3(rotation.X, rotation.Y, rotation.Z + 0.5f * (float)Math.PI);
            surfaces[5] = new Plane(leftPosition, leftRotation, scale, device);

            Vector3 backPosition = new Vector3(position.X, position.Y, position.Z + scale);
            Vector3 backRotation = new Vector3(rotation.X + 0.5f * (float)Math.PI, rotation.Y, rotation.Z);
            surfaces[6] = new Plane(backPosition, backRotation, scale, device);
            Vector3 backPosition2 = new Vector3(position.X + 2 * scale, position.Y, position.Z + scale);
            surfaces[7] = new Plane(backPosition2, backRotation, scale, device);

            Vector3 frontPosition = new Vector3(position.X, position.Y, position.Z - scale);
            Vector3 frontRotation = new Vector3(rotation.X - 0.5f * (float)Math.PI, rotation.Y, rotation.Z);
            surfaces[8] = new Plane(frontPosition, frontRotation, scale, device);
            Vector3 front2Position = new Vector3(position.X + 2 * scale, position.Y, position.Z - scale); ;
            surfaces[9] = new Plane(front2Position, frontRotation, scale, device);
        }

        private int sign(float value)
        {
            return value < 0 ? -1 : 1;
        }

        private void rotate(float deltaTime)
        {
            //Calculate the rotation in this frame
            float rotation = sign(rotationASecond) * Math.Min(Math.Abs(rotationASecond) * deltaTime, maxAngle - Math.Abs(this.rotation));
            Matrix rotationMatrix = Matrix.CreateRotationY(rotation);

            this.rotation += rotation;

            foreach (Plane plane in surfaces)
            {
                Vector3 oldDistanceVector = plane.getDistanceTillPoint(ball.getPivotWithPlane(plane.normal));
                float oldDistance = oldDistanceVector.X + oldDistanceVector.Y + oldDistanceVector.Z;

                //Rotate a plane
                plane.translate(Matrix.CreateTranslation(-pivot));
                plane.rotate(rotationMatrix, new Vector3(0, rotation, 0));
                plane.translate(Matrix.CreateTranslation(pivot));

                Vector3 newDistanceVector = plane.getDistanceTillPoint(ball.getPivotWithPlane(plane.normal));
                float newDistance = newDistanceVector.X + newDistanceVector.Y + newDistanceVector.Z;

                if (Math.Sign(oldDistance) != Math.Sign(newDistance))       //Check if the ball is on the other side of the frame after this frame
                {
                    Vector3 pivotWithPlane = plane.getPivotWithLine(ball.position, -plane.normal);
                    Vector2 distanceTillBoundry = plane.getDistanceTillBoundry(pivotWithPlane);
                    if (distanceTillBoundry.Length() == 0)      //Check if the pivot with the plane is inside the boundries of the plane
                    {
                        Vector3 ballPosition = ball.position;

                        //Rotate the bal around the turning point of the flipper
                        ballPosition = Plane.vectorMatrixMultiplication(ballPosition, Matrix.CreateTranslation(-pivot));
                        ballPosition = Plane.vectorMatrixMultiplication(ballPosition, Matrix.CreateRotationY(rotation));
                        ballPosition = Plane.vectorMatrixMultiplication(ballPosition, Matrix.CreateTranslation(pivot));

                        //Lift the bal from teh ground, to prefent it the move past the ground plane
                        ballPosition.Y += 0.2f;

                        //Move the bal away from the plane
                        ball.accelerate(plane.normal, ballPosition);
                    }
                }
            }
        }

        private void rotateBack(float deltaTime)
        {
            float rotation = sign(rotationASecond) * Math.Max(-Math.Abs(rotationASecond) * deltaTime, -Math.Abs(this.rotation))/2;
            this.rotation += rotation;
            Matrix rotationMatrix = Matrix.CreateRotationY(rotation);
            foreach (Plane plane in surfaces)
            {
                plane.translate(Matrix.CreateTranslation(-pivot));
                plane.rotate(rotationMatrix, new Vector3(0, rotation, 0));
                plane.translate(Matrix.CreateTranslation(pivot));
            }
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
            KeyboardState keyboardState = Keyboard.GetState();
            if (rotationASecond < 0 && keyboardState.IsKeyDown(Keys.Left) ||
                rotationASecond > 0 && keyboardState.IsKeyDown(Keys.Right))
            {
                rotate(deltaTime);
            }
            else
            {
                rotateBack(deltaTime);
            }
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SIMTEC3D_Prac1.Scripts
{
    class Ball : ModelObject
    {
        private GameObject[] gameobjects;
        private Vector3 velocity;

        public Ball(Vector3 position, float scale, GraphicsDevice device, GameObject[] gameobjects) : base(position, Vector3.Zero, scale, device)
        {
            this.gameobjects = gameobjects;
            this.velocity = new Vector3(0.5f, 3f, 0.5f);
        }

        protected override Model loadModel(ContentManager content)
        {
            return content.Load<Model>("Models\\ball");
        }

        public override void update(float deltaTime)
        {
            move(deltaTime);
            base.update(deltaTime);
        }

        //Move the ball and check for collision
        private void move(float deltaTime)
        {
            if (velocity.Length() != 0)
            {
                Vector3 speed = velocity * deltaTime;
                Vector3 speedBeforeCollision = speed;
                speed = applyPhysics(speed);
                if (speed.Equals(speedBeforeCollision))
                {
                    position += speed;
                }
                else
                {
                    velocity = speed / deltaTime;
                }
            }
        }

        //Calculate the position of the end of the ball that is pointing toward the plane
        public Vector3 getPivotWithPlane(Vector3 planeNormal)
        {
            return new Vector3(position.X - planeNormal.X * scale, position.Y - planeNormal.Y * scale, position.Z - planeNormal.Z * scale);
        }

        //Check for collision with all other gameobjects
        private Vector3 applyPhysics(Vector3 speed)
        {
            foreach (GameObject gameObject in gameobjects)
            {
                if (gameObject is Box)
                {
                    foreach (Plane plane in ((Box)gameObject).planes)
                    {
                        speed = applyCollisionPhysics(plane, speed);
                    }
                }
                else if (gameObject is Plane)
                {
                    speed = applyCollisionPhysics((Plane)gameObject, speed);
                }
            }
            return speed;
        }

        //Check for collision with a plane
        private Vector3 applyCollisionPhysics(Plane plane, Vector3 speed)
        {
            //Calculate the position of the end of the ball that is pointing toward the plane
            Vector3 ballCollisionPoint = getPivotWithPlane(plane.normal);
            Vector3 newBallCollisionPoint = ballCollisionPoint + speed;     //The new position, ignoring collisions

            Vector3 speedInNormalDirection = Physics.dotProductCalculation(speed, plane.normal);

            Vector3 distanceVector = plane.getDistanceTillPoint(ballCollisionPoint);         //The distance from bal to plane in the next frame
            Vector3 newDistanceVector = plane.getDistanceTillPoint(newBallCollisionPoint);    //The distance from bal to plane in the previous frame
            if (!distanceVector.X.Equals(float.NaN) && !newDistanceVector.X.Equals(float.NaN))
            {
                float distance = distanceVector.X + distanceVector.Y + distanceVector.Z;
                float newDistance = newDistanceVector.X + newDistanceVector.Y + newDistanceVector.Z;

                if (Math.Sign(distance) != Math.Sign(newDistance))     //Is the bal past the plane
                {
                    Vector3 pivot = plane.getPivotWithLine(ballCollisionPoint, speed);
                    if (!pivot.X.Equals(float.NaN))
                    {
                        speed = bounce(ballCollisionPoint, speed, speedInNormalDirection, plane.normal, pivot);
                    }
                }
            }

            return speed;
        }

        //Bounce the ball away from a plane
        private Vector3 bounce(Vector3 ballCollisionPoint, Vector3 speed, Vector3 speedInNormalDirection, Vector3 planeNormal, Vector3 pivot)
        {
            //Dotproduct calculation
            Vector3 normalizedNormal = new Vector3(planeNormal.X, planeNormal.Y, planeNormal.Z);
            normalizedNormal.Normalize();

            //Set the bal just next to the plane
            position = pivot + normalizedNormal * 1.0001f * scale;

            //Reverse the velocity in the dircection of the plane
            speed = speed - 2 * speedInNormalDirection;

            return speed;
        }
    }
}

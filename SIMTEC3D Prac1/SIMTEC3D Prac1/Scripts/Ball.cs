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
            this.velocity = new Vector3(2, 3, 1);
        }

        protected override Model loadModel(ContentManager content)
        {
            return content.Load<Model>("Models\\ball");
        }

        public override void update(float deltaTime)
        {
            move(velocity * deltaTime);
            base.update(deltaTime);
        }

        //Move the ball and check for collision
        private void move(Vector3 speed)
        {
            Vector3 speedBeforeCollision = speed;
            speed = applyPhysics(speed);
            if (speed.Equals(speedBeforeCollision))
            {
                position += speed;
            }
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
                        speed = applyPhysics(plane, speed);
                    }
                }
                else if (gameObject is Plane)
                {
                    speed = applyPhysics((Plane)gameObject, speed);
                }
            }
            return speed;
        }

        //Check for collision with a plane
        private Vector3 applyPhysics(Plane plane, Vector3 speed)
        {
            Vector3 normal = new Vector3(plane.planeEquation.X, plane.planeEquation.Y, plane.planeEquation.Z);

            //Calculate the position of the end of the ball that is pointing toward the plane
            Vector3 ballCollisionPoint = new Vector3(position.X-normal.X*scale, position.Y-normal.Y*scale, position.Z-normal.Z*scale);
            Vector3 newBallCollisionPoint = ballCollisionPoint + speed;     //The new position, ignoring collisions

            //Calculating the pivot, on the plane that is closest to the ball
            float t = (plane.planeEquation.W - (plane.planeEquation.X*newBallCollisionPoint.X + plane.planeEquation.Y*newBallCollisionPoint.Y + plane.planeEquation.Z*newBallCollisionPoint.Z)) 
                / (plane.planeEquation.X*normal.X + plane.planeEquation.Y*normal.Y + plane.planeEquation.Z*normal.Z);
            Vector3 pivot = new Vector3(newBallCollisionPoint.X + t * normal.X, newBallCollisionPoint.Y + t * normal.Y, newBallCollisionPoint.Z + t * normal.Z);


            Vector3 distance = pivot - newBallCollisionPoint;         //The distance from bal to plane in the next frame
            Vector3 previousDistance = pivot - ballCollisionPoint;    //The distance from bal to plane in the previous frame
            if (Math.Sign(distance.X * normal.X+distance.Y*normal.Y+distance.Z*normal.Z) 
                    != Math.Sign(previousDistance.X * normal.X+previousDistance.Y*normal.Y+previousDistance.Z*normal.Z)) //Is the bal past the plane
            {
                //Calculate the pivot where the ball first touched the plane
                t = (plane.planeEquation.W - (plane.planeEquation.X * ballCollisionPoint.X + plane.planeEquation.Y * ballCollisionPoint.Y + plane.planeEquation.Z * ballCollisionPoint.Z))
                   / (plane.planeEquation.X * velocity.X + plane.planeEquation.Y * velocity.Y + plane.planeEquation.Z * velocity.Z);
                pivot = new Vector3(ballCollisionPoint.X + t * velocity.X, ballCollisionPoint.Y + t * velocity.Y, ballCollisionPoint.Z + t * velocity.Z);

                speed = bounce(normal, pivot, speed);
            }

            return speed;
        }

        //Bounce the ball away from a plane
        private Vector3 bounce(Vector3 normal, Vector3 pivot, Vector3 speed)
        {
            //Dotproduct calculation
            Vector3 normalizedNormal = new Vector3(normal.X, normal.Y, normal.Z);
            normalizedNormal.Normalize();
            Vector3 normalizedVelocity = new Vector3(this.velocity.X, this.velocity.Y, this.velocity.Z);
            normalizedVelocity.Normalize();
            float dotProduct = normalizedVelocity.X * normalizedNormal.X + normalizedVelocity.Y * normalizedNormal.Y + normalizedVelocity.Z * normalizedNormal.Z;

            //Set the bal just next to the plane
            position = pivot + normalizedNormal * 1.01f * scale;

            //Reverse the velocity in the dircection of the plane
            this.velocity -= 2 * dotProduct * normalizedNormal * this.velocity.Length();
            speed = speed - 2 * dotProduct * normalizedNormal * speed.Length();

            return speed;
        }
    }
}

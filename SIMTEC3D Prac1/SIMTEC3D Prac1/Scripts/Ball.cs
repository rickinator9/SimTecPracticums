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
            this.velocity = new Vector3(4, 3, 2);
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

        private void move(Vector3 speed)
        {
            Vector3 speedBeforeCollision = speed;
            speed = collisionCheck(speed);
            if (speed.Equals(speedBeforeCollision))
            {
                position += speed;
            }
        }

        private Vector3 collisionCheck(Vector3 speed)
        {
            foreach (GameObject gameObject in gameobjects)
            {
                if (gameObject is Box)
                {
                    foreach (Plane plane in ((Box)gameObject).planes)
                    {
                        speed = collisionCheck(plane, speed);
                    }
                }
                else if (gameObject is Plane)
                {
                    speed = collisionCheck((Plane)gameObject, speed);
                }
            }
            return speed;
        }

        private Vector3 collisionCheck(Plane plane, Vector3 speed)
        {
            Vector3 normal = new Vector3(plane.planeEquation.X, plane.planeEquation.Y, plane.planeEquation.Z);
            Vector3 newPosition = position + speed;
            float t = (plane.planeEquation.W - (plane.planeEquation.X*newPosition.X + plane.planeEquation.Y*newPosition.Y + plane.planeEquation.Z*newPosition.Z)) 
                / (plane.planeEquation.X*normal.X + plane.planeEquation.Y*normal.Y + plane.planeEquation.Z*normal.Z);
            Vector3 pivot = new Vector3(newPosition.X + t * normal.X, newPosition.Y + t * normal.Y, newPosition.Z + t * normal.Z);


            Vector3 distance = pivot - newPosition;
            Vector3 previousDistance = pivot - position;
            if (distance.Length() <= scale || Math.Sign(distance.X * normal.X+distance.Y*normal.Y+distance.Z*normal.Z) != Math.Sign(previousDistance.X * normal.X+previousDistance.Y*normal.Y+previousDistance.Z*normal.Z))
            {
                t = (plane.planeEquation.W - (plane.planeEquation.X * position.X + plane.planeEquation.Y * position.Y + plane.planeEquation.Z * position.Z))
                   / (plane.planeEquation.X * normal.X + plane.planeEquation.Y * normal.Y + plane.planeEquation.Z * normal.Z);
                pivot = new Vector3(position.X + t * normal.X, position.Y + t * normal.Y, position.Z + t * normal.Z);
                speed = collide(normal, pivot, speed);
            }

            return speed;
        }

        private Vector3 collide(Vector3 normal, Vector3 pivot, Vector3 speed)
        {
            Vector3 normalizedNormal = new Vector3(normal.X, normal.Y, normal.Z);
            normalizedNormal.Normalize();
            Vector3 normalizedVelocity = new Vector3(this.velocity.X, this.velocity.Y, this.velocity.Z);
            normalizedVelocity.Normalize();

            position = pivot + normalizedNormal * 1.01f;
            float dotProduct = normalizedVelocity.X * normalizedNormal.X+normalizedVelocity.Y*normalizedNormal.Y+normalizedVelocity.Z*normalizedNormal.Z;

            this.velocity -= 2 * dotProduct * normalizedNormal * this.velocity.Length();
            speed = speed - 2 * dotProduct * normalizedNormal * speed.Length();

            return speed;
        }
    }
}

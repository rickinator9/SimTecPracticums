﻿using System;
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
        public struct SpeedInfo
        {
            public Vector3 speed;
            public Vector3 speedThisFrame;
        }

        private GameObject[] gameobjects;
        private Vector3 velocity, acceleration;
        private float airFriction, bounceFriction;

        public Ball(Vector3 position, float scale, GraphicsDevice device, GameObject[] gameobjects) : base(position, Vector3.Zero, scale, device)
        {
            this.gameobjects = gameobjects;
            this.velocity = new Vector3(0.12f, -0.5f, 0.1f);
            this.acceleration = new Vector3(0f, -0.10f, 0f);
            this.airFriction = 0.999f;
            this.bounceFriction = 0.96f;
        }

        protected override Model loadModel(ContentManager content)
        {
            return content.Load<Model>("Models\\ball");
        }

        public override void update(float deltaTime)
        {
            this.velocity += this.acceleration;
            this.velocity *= airFriction;
            move(deltaTime);
            base.update(deltaTime);
        }

        //Move the ball and check for collision
        private void move(float deltaTime)
        {
            if (velocity.Length() != 0)
            {
                Vector3 speed = velocity * deltaTime;
                SpeedInfo speedInfo = applyPhysics(speed);
                position += speedInfo.speedThisFrame;
                if (!speed.Equals(speedInfo.speed))
                {
                    velocity = speedInfo.speed / deltaTime;
                }
            }
        }

        //Calculate the position of the end of the ball that is pointing toward the plane
        public Vector3 getPivotWithPlane(Vector3 planeNormal)
        {
            return new Vector3(position.X - planeNormal.X * radius, position.Y - planeNormal.Y * radius, position.Z - planeNormal.Z * radius);
        }

        //Check for collision with all other gameobjects
        private SpeedInfo applyPhysics(Vector3 speed)
        {
            SpeedInfo speedInfo;
            speedInfo.speed = speed;
            speedInfo.speedThisFrame = speed;

            foreach (GameObject gameObject in gameobjects)
            {
                if (gameObject is Box)
                {
                    foreach (Plane plane in ((Box)gameObject).planes)
                    {
                        SpeedInfo info = applyCollisionPhysics(plane, speedInfo.speed);
                        speedInfo.speed = info.speed;
                        if (info.speedThisFrame.Length() < speedInfo.speedThisFrame.Length())
                        {
                            speedInfo.speedThisFrame = info.speedThisFrame;
                        }
                    }
                }
                else if (gameObject is Plane)
                {
                    SpeedInfo info = applyCollisionPhysics((Plane)gameObject, speedInfo.speed);
                    speedInfo.speed = info.speed;
                    if (info.speedThisFrame.Length() < speedInfo.speedThisFrame.Length())
                    {
                        speedInfo.speedThisFrame = info.speedThisFrame;
                    }
                }
            }

            return speedInfo;
        }

        //Check for collision with a plane
        private SpeedInfo applyCollisionPhysics(Plane plane, Vector3 speed)
        {
            SpeedInfo speedInfo;
            speedInfo.speed = speed;
            speedInfo.speedThisFrame = speed;

            Vector3 speedInNormalDirection = Physics.dotProductCalculation(speed, plane.normal);

            //Calculate the position of the end of the ball that is pointing toward the plane
            Vector3 ballCollisionPoint = getPivotWithPlane(plane.normal);

            CollisionInfo collisionInfo = applyCollisionPhysics(plane, speed, ballCollisionPoint, speedInNormalDirection);

            if (collisionInfo != null)
            {
                float maxDistance = Math.Max(collisionInfo.distanceTillCollision.X, collisionInfo.distanceTillCollision.Y);
                if (collisionInfo.distanceTillCollision.Length() == 0)
                {
                    speedInfo = bounce(speed, speedInNormalDirection, plane.normal);
                }
                else if (maxDistance <= radius)
                {
                    //Calculate the position of the end of the ball that is pointing toward the plane
                    Vector3 newPosition = ballCollisionPoint + speed;     //The new position, ignoring collisions

                    CollisionInfo centerCollisionInfo = applyCollisionPhysics(plane, speed, position, speedInNormalDirection, true);
                    if (centerCollisionInfo != null)
                    {
                        Vector3 startingNormal = new Vector3(0, 1, 0);
                        Vector3 planeNormal = plane.normal;
                        planeNormal.Normalize();
                        Vector3 crossProduct = Vector3.Cross(startingNormal, planeNormal);
                        crossProduct.Normalize();
                        Vector3 distanceTillCollision = new Vector3(collisionInfo.distanceTillCollision.X, -1+(float) Math.Sqrt(collisionInfo.distanceTillCollision.X * collisionInfo.distanceTillCollision.X + collisionInfo.distanceTillCollision.Y * collisionInfo.distanceTillCollision.Y), collisionInfo.distanceTillCollision.Y);
                        if (!crossProduct.X.Equals(float.NaN) && !crossProduct.Y.Equals(float.NaN) && !crossProduct.Z.Equals(float.NaN))
                        {
                            Quaternion q = new Quaternion(crossProduct.X, crossProduct.Y, crossProduct.Z, 1 + Vector3.Dot(startingNormal, planeNormal));
                            q.Normalize();
                            distanceTillCollision = Vector3.Transform(distanceTillCollision, q);
                        }
                        Vector3 offSet = distanceTillCollision;
                        offSet = offSet * scale;
                        Vector3 recalculatedBallCollisionPoint = position + offSet;
                        Vector3 speedInCollisionDirection = Physics.dotProductCalculation(speed, offSet / scale);
                        if (Math.Sign(speedInCollisionDirection.X) != Math.Sign(offSet.X) || Math.Sign(speedInCollisionDirection.Y) != Math.Sign(offSet.Y) || Math.Sign(speedInCollisionDirection.Y) != Math.Sign(offSet.Y))
                        {
                            speedInCollisionDirection *= -1;
                        }
                        speedInfo = bounce(speed, speedInCollisionDirection, plane.normal);
                    }
                }
            }

            return speedInfo;
        }
        //Check for collision with a plane
        private CollisionInfo applyCollisionPhysics(Plane plane, Vector3 speed, Vector3 ballCollisionPoint, Vector3 speedInCollisionDirection, bool ignoreLength = false)
        { 
            CollisionInfo collisionInfo = null;
            Vector3 newBallCollisionPoint = ballCollisionPoint + speed;     //The new position, ignoring collisions

            Vector3 distanceVector = plane.getDistanceTillPoint(ballCollisionPoint);         //The distance from bal to plane in the next frame
            Vector3 newDistanceVector = plane.getDistanceTillPoint(newBallCollisionPoint);    //The distance from bal to plane in the previous frame
            if (!distanceVector.X.Equals(float.NaN) && !newDistanceVector.X.Equals(float.NaN))
            {
                float distance = distanceVector.X + distanceVector.Y + distanceVector.Z;
                float newDistance = newDistanceVector.X + newDistanceVector.Y + newDistanceVector.Z;
                bool passesPlane = Math.Sign(distance) != Math.Sign(newDistance);
                if (ignoreLength || passesPlane) 
                {
                    Vector3 distanceTowardPlane = Physics.dotProductCalculation(distanceVector, plane.normal);
                    if (ignoreLength || (Math.Sign(distanceTowardPlane.X) == Math.Sign(-plane.normal.X) || Math.Sign(distanceTowardPlane.Y) == Math.Sign(-plane.normal.Y) || Math.Sign(distanceTowardPlane.Z) == Math.Sign(-plane.normal.Z)))
                    {
                        collisionInfo = plane.getPivotWithLineAndDistanceTillBoundries(ballCollisionPoint, speed, radius);
                    }
                }
            }

            return collisionInfo;
        }

        //Bounce the ball away from a plane
        private SpeedInfo bounce(Vector3 speed, Vector3 speedInCollisionDirection, Vector3 planeNormal)
        {            
            //Dotproduct calculation
            Vector3 normalizedNormal = new Vector3(planeNormal.X, planeNormal.Y, planeNormal.Z);
            normalizedNormal.Normalize();

            //Reverse the velocity in the dirrection of the plane
            SpeedInfo speedInfo;
            speedInfo.speed = speed - 2 * speedInCollisionDirection;
            speedInfo.speed = speedInfo.speed / speedInfo.speed.Length() * speed.Length();
            speedInfo.speedThisFrame = Vector3.Zero;
            speedInfo.speed *= bounceFriction; 
            return speedInfo;
        }

        public float radius
        {
            get
            {
                return scale;
            }
        }
    }
}

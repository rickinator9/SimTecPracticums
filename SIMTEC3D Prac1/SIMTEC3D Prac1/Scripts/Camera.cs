using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SIMTEC3D_Prac1.Scripts {
    class Camera
    {
        private Vector3 position;
        private Vector3 target;
        private Matrix matrix;
        private float rotateSpeed;
        private float zoomSpeed;
        private float maxDistToYAxis;
        private int scrollWheelValue;

        public Camera(Vector3 position, Vector3 target)
        {
            this.position = position;
            this.target = target;
            rotateSpeed = 0.08f;
            zoomSpeed = 5f;
            maxDistToYAxis = 1;
            scrollWheelValue = Mouse.GetState().ScrollWheelValue;
        }

        public Matrix viewMatrix
        {
            get
            {
                return matrix;
            }
        }

        private Vector3 direction
        {
            get
            {
                Vector3 direction = target - position;
                direction.Normalize();
                return direction;
            }
        }

        private float distance
        {
            get
            {
                Vector3 direction = target - position;
                return direction.Length();
            }
        }

        private Vector3 Right
        {
            get
            {
                return new Vector3(direction.Y * Vector3.Up.Z - direction.Z * Vector3.Up.Y,
                    direction.Z * Vector3.Up.X - direction.X * Vector3.Up.Z, direction.X * Vector3.Up.Y - direction.Y * Vector3.Up.X);
            }
        }

        private Vector3 Up
        {
            get
            {
                Vector3 right = Right;
                return new Vector3(direction.Y * right.Z - direction.Z * right.Y, direction.Z * right.X - direction.X * right.Z,
                    direction.X * right.Y - direction.Y * right.X);
            }
        }

        public void update(float deltaTime, GraphicsDevice gfxDevice)
        {
            Rectangle windowInfo = gfxDevice.PresentationParameters.Bounds;

            if (Mouse.GetState().X < windowInfo.X + windowInfo.Width * 0.1 && Mouse.GetState().X > windowInfo.X) // Left
            {
                position += -Right * rotateSpeed * deltaTime * distance;
            }
            else if (Mouse.GetState().X > windowInfo.X + windowInfo.Width * 0.9 && Mouse.GetState().X < windowInfo.X + windowInfo.Width) // Right
            {
                position += Right * rotateSpeed * deltaTime * distance;
            }

            if (Mouse.GetState().Y > windowInfo.Y + windowInfo.Height * 0.9 && Mouse.GetState().Y < windowInfo.Y + windowInfo.Height) // Down
            {
                Vector3 rotation = Up * rotateSpeed * deltaTime * distance;
                Vector3 normalisedPos = direction + rotation;
                normalisedPos.Normalize();
                if ((Vector3.Up - normalisedPos).Length() >= maxDistToYAxis)
                {
                    position += rotation;
                }
            }
            else if (Mouse.GetState().Y < windowInfo.Y + windowInfo.Height * 0.1 && Mouse.GetState().Y > windowInfo.Y) // Up
            {
                Vector3 rotation = -Up * rotateSpeed * deltaTime * distance;
                Vector3 normalisedPos = direction + rotation;
                normalisedPos.Normalize();
                if ((Vector3.Down - normalisedPos).Length() >= maxDistToYAxis)
                {
                    position += rotation;
                }
            }
            if (Mouse.GetState().ScrollWheelValue != scrollWheelValue)
            {
                position += direction * Math.Min(distance - 0.01f, zoomSpeed * (Mouse.GetState().ScrollWheelValue - scrollWheelValue) * 0.003f);
                scrollWheelValue = Mouse.GetState().ScrollWheelValue;
            }

            matrix = Matrix.CreateLookAt(position, target, Vector3.Up);
        }
    }
}

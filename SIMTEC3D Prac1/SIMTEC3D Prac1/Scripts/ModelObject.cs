using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SIMTEC3D_Prac1.Scripts
{
    abstract class ModelObject: GameObject
    {
        public Vector3 position;
        protected float scale;
        protected Vector3 rotation;
        private Model model;
        private Matrix worldMatrix, projectionMatrix;

        public ModelObject(Vector3 position, Vector3 rotation, float scale, GraphicsDevice device): base()
        {
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;

            calculateWorldMatrix();
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.1f, 200.0f);
        }

        protected abstract Model loadModel(ContentManager content);

        public override void LoadContent(ContentManager content)
        {
            model = loadModel(content);
        }

        private void calculateWorldMatrix()
        {
            worldMatrix = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
        }

        public override void update(float deltaTime)
        {
            calculateWorldMatrix();
        }

        public override void draw(Matrix viewMatrix)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                }
            }
            model.Draw(worldMatrix, viewMatrix, projectionMatrix);
        }
    }
}

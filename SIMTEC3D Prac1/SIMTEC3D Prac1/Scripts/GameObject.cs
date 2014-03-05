using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SIMTEC3D_Prac1.Scripts
{
    abstract class GameObject
    {
        public abstract void update(float deltaTime);
        public abstract void draw(Matrix viewMatrix);
        public abstract void LoadContent(ContentManager content);
    }
}

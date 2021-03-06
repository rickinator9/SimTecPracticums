using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace SIMTEC3D_Prac1.Scripts
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameObject[] gameObjects;
        private Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameObjects = new GameObject[5];
            gameObjects[0] = new Ball(new Vector3(-9, -9, -5), 0.3f, GraphicsDevice, gameObjects);
            gameObjects[1] = new Box(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 10, GraphicsDevice);
            gameObjects[2] = new Bumper(new Vector3(0, -8f, -7f), new Vector3(0, 0, 0), 1, GraphicsDevice);
            gameObjects[3] = new Flipper(new Vector3(-8.5f, -8, 5), new Vector3(-0.75f, 0, 0), 0.2f * (float)Math.PI, -0.12f * (float)Math.PI, new Vector3(0, 0, 0), 2, (Ball)gameObjects[0], GraphicsDevice);
            gameObjects[4] = new Flipper(new Vector3(4f, -8, 5), new Vector3(2.5f, 0, 0), 0.2f * (float)Math.PI, 0.12f * (float)Math.PI, new Vector3(0, 0, 0), 2, (Ball)gameObjects[0], GraphicsDevice);
            camera = new Camera(new Vector3(0, 25, 10), new Vector3(0, 0, -1));
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.LoadContent(Content);
            }
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float deltaTime = 0.01f * gameTime.ElapsedGameTime.Milliseconds;
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.update(deltaTime);
            }
            camera.update(deltaTime, graphics.GraphicsDevice);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.draw(camera.viewMatrix);
            }
            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411.SimpleEngine;

namespace Lab05
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab05 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // LAB 05
        Skybox skybox;
        //string[] skyboxTextures; //names of images
        
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 1.33f, 0.1f, 100f);

        //MouseState previousMouse;
        float angle = 0f;
        float angle2 = 0f;
        Vector3 cameraPosition = new Vector3(0, 0, 10);

        public Lab05()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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

            string[] skyboxTextures =
            {
                "skybox/SunsetPNG2", "skybox/SunsetPNG1",
                "skybox/SunsetPNG4", "skybox/SunsetPNG3",
                "skybox/SunsetPNG6", "skybox/SunsetPNG5"
            };

            skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //-----------------------------------------------------------------------------------------------------------------------------
            //  Rotate Camera Angle
            //-----------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) // If Left Arrow Key is down
            {
                // *** ADD COMMENTS ***
                angle -= 0.02f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) // If Right Arrow Key is down
            {
                // *** ADD COMMENTS ***
                angle += 0.02f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) // If Up Arrow Key is down
            {
                // *** ADD COMMENTS ***
                angle2 -= 0.02f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) // If Down Arrow Key is down
            {
                // *** ADD COMMENTS ***
                angle2 += 0.02f;
            }

            // *** ADD COMMENTS ***
            view = (Matrix.CreateRotationY(angle)
                * Matrix.CreateRotationX(angle2)
                * Matrix.CreateTranslation(cameraPosition));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState(); // Use this in assignment 1 if weird output

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(view, projection, cameraPosition);

            base.Draw(gameTime);
        }
    }
}
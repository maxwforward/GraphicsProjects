using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


//########################################################################################################################################
//  LAB10
//########################################################################################################################################
using CPI411.SimpleEngine;


namespace Lab10
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab10 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        //********************************************************************************************************************************
        //	TEMPLATE - Variables
        //********************************************************************************************************************************
        SpriteFont font;
        Effect effect;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        Matrix lightView, lightProjection;
        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 10;
        MouseState preMouse;
        Model model;
        Texture2D texture;


        //################################################################################################################################
        //  LAB10 - Variables
        //################################################################################################################################
        Matrix invertCamera = Matrix.Identity;
        ParticleManager particleManager;
        Vector3 particlePosition;
        System.Random random;


        public Lab10()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //****************************************************************************************************************************
            //	TEMPLATE
            //****************************************************************************************************************************
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

            //############################################################################################################################
            //  LAB10
            //############################################################################################################################
            effect = Content.Load<Effect>("ParticleShader");
            texture = Content.Load<Texture2D>("fire");
            model = Content.Load<Model>("torus");
            random = new System.Random();
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particlePosition = new Vector3(0, 0, 0);
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

            //****************************************************************************************************************************
            //	TEMPLATE
            //****************************************************************************************************************************
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {             
                angle = angle2 = angleL = angleL2 = 0;
                distance = 10;
                cameraTarget = Vector3.Zero;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                distance += (Mouse.GetState().X - preMouse.X) / 100f;
            }
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }
            preMouse = Mouse.GetState();
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(
                cameraPosition,
                cameraTarget,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            lightPosition = Vector3.Transform(
                new Vector3(0, 0, 10),
                Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            lightView = Matrix.CreateLookAt(lightPosition, Vector3.Zero,
            Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL)));
            lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 1f, 50f);

            //############################################################################################################################
            //  LAB10 - (From Yoshi's Lecture)
            //############################################################################################################################
            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                Particle particle = particleManager.getNext();
                particle.Position = particlePosition;
                particle.Velocity = new Vector3(5, 0, 0);
                particle.Acceleration = new Vector3(0, 0, 0);
                particle.MaxAge = 1;
                particle.Init();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                Particle particle = particleManager.getNext();
                particle.Position = particlePosition;
                particle.Velocity = new Vector3(5, 5, 0);
                particle.Acceleration = new Vector3(0, -10, 0);
                particle.MaxAge = 1;
                particle.Init();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                for (int i=0; i<60; i++)
                {
                    double angle = System.Math.PI * (i * 6) / 180.0;
                    Particle particle = particleManager.getNext();
                    particle.Position = particlePosition;
                    particle.Velocity = new Vector3(
                        10.0f * (float)System.Math.Sin(angle),
                        0,
                        10.0f * (float)System.Math.Cos(angle));
                    particle.Acceleration = new Vector3(0, 0, 0);
                    particle.MaxAge = 1;
                    particle.Init();
                }
            }

            //############################################################################################################################
            //  LAB10
            //############################################################################################################################
            if (Keyboard.GetState().IsKeyDown(Keys.P)) // P is the key to generate particles
            { 
                Particle particle = particleManager.getNext(); 
                particle.Position = particlePosition;
                // particle.Velocity = new Vector3(0, 0, 0);
                particle.Velocity = new Vector3(random.Next(-5, 5), random.Next(-5, 5), random.Next(-5, 5));
                particle.Acceleration = new Vector3(0, 0, 0); 
                particle.MaxAge = 1; 
                particle.Init(); 
            }
            particleManager.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);

            //############################################################################################################################
            //  LAB10 - Task 1
            //############################################################################################################################
            invertCamera = Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle);

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //############################################################################################################################
            //  LAB10
            //############################################################################################################################
            GraphicsDevice.BlendState = BlendState.AlphaBlend; 
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //############################################################################################################################
            //  LAB10 - Step 1 shows Model
            //############################################################################################################################
            model.Draw(world, view, projection);

            //############################################################################################################################
            //  LAB10 - Step 2 Culling OFF
            //############################################################################################################################
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //############################################################################################################################
            //  LAB10 - Step 3 Apply Particle Shader
            //############################################################################################################################
            effect.CurrentTechnique = effect.Techniques[0]; 
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["InverseCamera"].SetValue(invertCamera);
            effect.Parameters["Texture"].SetValue(texture);
            particleManager.Draw(GraphicsDevice);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            base.Draw(gameTime);
        }
    }
}
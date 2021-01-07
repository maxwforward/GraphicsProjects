using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411.SimpleEngine;

namespace Lab06
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab06 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        //##############################################################################################################
        // Global Variables         ********** From Lab05 **********
        //##############################################################################################################
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 1.33f, 0.01f, 1000f);
        Vector3 cameraPosition = new Vector3(0, 0, 3);
        Skybox skybox;
        float angle = 0f;
        Texture2D texture; // PART C

        
        //##############################################################################################################
        // Global Variables         ********** From Lab06 **********
        //##############################################################################################################
        Model model;
        Effect effect;


        //##############################################################################################################
        // Global Variables         ********** From Tutorial **********
        //##############################################################################################################
        float distance = 3;
        TextureCube skyboxTexture;


        public Lab06()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //*****************************************************
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //*****************************************************
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

            // TODO: use this.Content to load your game content here

            //----------------------------------------------------------------------------------------------------------
            // From Lab05
            //----------------------------------------------------------------------------------------------------------
            string[] skyboxTextures =
            {
                "skybox/SunsetPNG2", "skybox/SunsetPNG1",
                "skybox/SunsetPNG4", "skybox/SunsetPNG3",
                "skybox/SunsetPNG6", "skybox/SunsetPNG5"
            };
            skybox = new Skybox(skyboxTextures, Content, graphics.GraphicsDevice);

            //----------------------------------------------------------------------------------------------------------
            // From Lab06
            //----------------------------------------------------------------------------------------------------------
            model = Content.Load<Model>("Helicopter");
            effect = Content.Load<Effect>("Reflection");
            texture = Content.Load<Texture2D>("HelicopterTexture"); // PART C

            //----------------------------------------------------------------------------------------------------------
            // From Tutorial
            //----------------------------------------------------------------------------------------------------------
            skyboxTexture = skybox.skyBoxTexture;
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

            //----------------------------------------------------------------------------------------------------------
            //  Use Arrow Keys to rotate camera angle         ********** From Lab05 **********
            //----------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) // If Left Arrow Key is down
            {
                angle -= 0.02f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) // If Right Arrow Key is down
            {
                angle += 0.02f;
            }

            //----------------------------------------------------------------------------------------------------------
            // From Tutorial
            //----------------------------------------------------------------------------------------------------------
            cameraPosition = distance * new Vector3((float)System.Math.Sin(angle), 0, (float)System.Math.Cos(angle));
            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.UnitY);

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

            //----------------------------------------------------------------------------------------------------------
            // From Lab06
            //----------------------------------------------------------------------------------------------------------
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState; 
            RasterizerState rasterizerState = new RasterizerState(); 
            rasterizerState.CullMode = CullMode.None; 
            graphics.GraphicsDevice.RasterizerState = rasterizerState; 
            skybox.Draw(view, projection, cameraPosition); 
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState; 
            DrawModelWithEffect();

            base.Draw(gameTime);
        }


        //==============================================================================================================
        // From Lab06
        //==============================================================================================================
        private void DrawModelWithEffect()
        {
            effect.CurrentTechnique = effect.Techniques[0];
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        //----------------------------------------------------------------------------------------------
                        // From Lab06
                        //----------------------------------------------------------------------------------------------
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform); 
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["decalMap"].SetValue(texture); // PART C

                        //----------------------------------------------------------------------------------------------
                        // From Tutorial
                        //----------------------------------------------------------------------------------------------
                        effect.Parameters["environmentMap"].SetValue(skyboxTexture);

                        //----------------------------------------------------------------------------------------------
                        // From Lab06
                        //----------------------------------------------------------------------------------------------
                        pass.Apply(); 
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer); 
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, 
                            part.VertexOffset, 
                            part.StartIndex, 
                            part.PrimitiveCount);
                    }
                }
            }
        }
    }
}
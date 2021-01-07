using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//#############################################################################################################################################
//  NEEDED FOR SKYBOX                                                                                                          (FROM LAB06)
//#############################################################################################################################################
using CPI411.SimpleEngine;


namespace Assignment2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        //#####################################################################################################################################
        //  GLOBAL VARIABLES                                                                                                   (FROM LAB06)
        //#####################################################################################################################################
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (800f/600f), 0.01f, 1000f);
        Vector3 cameraPosition = new Vector3(0, 0, 5);


        //#####################################################################################################################################
        //  PART A                                                                                                       (FROM ASSIGNMENT1)
        //#####################################################################################################################################
        float horizontalCameraAngle = 0f;
        float verticalCameraAngle = 0f;
        float distance = 5f;
        MouseState previousMouseState;


        //#####################################################################################################################################
        //  PART B                                                                                                             (FROM LAB06)
        //#####################################################################################################################################
        Skybox currentSkybox; // Stores the selected skybox
        Skybox testColors;  // B-7
        Skybox officeRoom;  // B-8
        Skybox daytimeSky;  // B-9
        Skybox mySkybox;    // B-0


        //#####################################################################################################################################
        //  PART B-6                                                                                                           (FROM LAB06)
        //#####################################################################################################################################
        Model helicopterModel;
        Texture2D helicopterTexture;


        //#####################################################################################################################################
        //  PART C                                                                                                       (FROM ASSIGNMENT1)
        //#####################################################################################################################################
        int technique = 0;
        KeyboardState previousKeyboardState;
        SpriteFont font;
        bool showMenu = false;
        bool showValues = false;


        //#####################################################################################################################################
        //  PART C                                                                                                             (FROM LAB06)
        //#####################################################################################################################################
        Effect effect;


        //#####################################################################################################################################
        //  PART C                                                                                                             
        //#####################################################################################################################################
        float reflectivity = 0.75f;
        Vector3 eta_Ratio = new Vector3(0.65f, 0.67f, 0.69f);
        float fresnelPower = 2.0f;
        float fresnelScale = 15f;
        float fresnelBias = 0.5f;


        public Assignment2()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //*********************************************************************************************************************************
            //  ALWAYS ADD                                                                                                       (FROM ALL)
            //*********************************************************************************************************************************
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

            // TODO: use this.Content to load your game content here

            //*********************************************************************************************************************************
            //  PART B-6                                                                                                       (FROM LAB06)
            //*********************************************************************************************************************************
            helicopterModel = Content.Load<Model>("Helicopter");
            helicopterTexture = Content.Load<Texture2D>("HelicopterTexture");

            //*********************************************************************************************************************************
            //  PART B-7                                                                                                       (FROM LAB06)
            //*********************************************************************************************************************************
            string[] testColorsSkyboxTextures = 
            {
                "Environment Maps/debug_negx", "Environment Maps/debug_posx",
                "Environment Maps/debug_negy", "Environment Maps/debug_posy",
                "Environment Maps/debug_negz", "Environment Maps/debug_posz"
            };
            testColors = new Skybox(testColorsSkyboxTextures, Content, GraphicsDevice, 256);
            currentSkybox = testColors; // Initialize as current skybox

            //*********************************************************************************************************************************
            //  PART B-8                                                                                                       (FROM LAB06)
            //*********************************************************************************************************************************
            string[] officeRoomSkyboxTextures =
            {
                "Environment Maps/nvlobby_new_negx", "Environment Maps/nvlobby_new_posx",
                "Environment Maps/nvlobby_new_negy", "Environment Maps/nvlobby_new_posy",
                "Environment Maps/nvlobby_new_negz", "Environment Maps/nvlobby_new_posz"
            };
            officeRoom = new Skybox(officeRoomSkyboxTextures, Content, GraphicsDevice, 512);

            //*********************************************************************************************************************************
            //  PART B-9                                                                                                       (FROM LAB06)
            //*********************************************************************************************************************************
            string[] daytimeSkySkyboxTextures =
            {
                "Environment Maps/grandcanyon_negx", "Environment Maps/grandcanyon_posx",
                "Environment Maps/grandcanyon_negy", "Environment Maps/grandcanyon_posy",
                "Environment Maps/grandcanyon_negz", "Environment Maps/grandcanyon_posz"
            };
            daytimeSky = new Skybox(daytimeSkySkyboxTextures, Content, GraphicsDevice, 512);

            //*********************************************************************************************************************************
            //  PART B-0                                                                                                       (FROM LAB06)
            //*********************************************************************************************************************************
            string[] spaceSkyboxTextures =
            {
                "Environment Maps/space_negx", "Environment Maps/space_posx",
                "Environment Maps/space_negy", "Environment Maps/space_posy",
                "Environment Maps/space_negz", "Environment Maps/space_posz"
            };
            mySkybox = new Skybox(spaceSkyboxTextures, Content, GraphicsDevice, 1024);

            //*********************************************************************************************************************************
            //  PART C                                                                                                   (FROM ASSIGNMENT1)
            //*********************************************************************************************************************************
            effect = Content.Load<Effect>("EnvironmentMaps");
            font = Content.Load<SpriteFont>("Text");
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

            //*********************************************************************************************************************************
            //  PART A                                                                                                   (FROM ASSIGNMENT1)
            //*********************************************************************************************************************************
            MouseState currentMouseState = Mouse.GetState();

            //---------------------------------------------------------------------------------------------------------------------------------
            //  PART A - Rotate the camera with Mouse Left Drag                                                          (FROM ASSIGNMENT1)
            //---------------------------------------------------------------------------------------------------------------------------------
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)            
            {
                horizontalCameraAngle -= (previousMouseState.X - currentMouseState.X) / 100f;
                verticalCameraAngle -= (previousMouseState.Y - currentMouseState.Y) / 100f;            
            }

            //---------------------------------------------------------------------------------------------------------------------------------
            //  PART A - Change the distance of camera to the center with Mouse Right Drag                               (FROM ASSIGNMENT1)
            //---------------------------------------------------------------------------------------------------------------------------------
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                distance -= (previousMouseState.Y - currentMouseState.Y) / 100f;
            }

            //*********************************************************************************************************************************
            //  PART A                                                                                                         (FROM LAB06)
            //*********************************************************************************************************************************
            cameraPosition = Vector3.Transform(Vector3.Zero, 
                Matrix.CreateTranslation(new Vector3(0, 0, distance)) 
                * Matrix.CreateRotationX(verticalCameraAngle) 
                * Matrix.CreateRotationY(horizontalCameraAngle));                      
            view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

            //*********************************************************************************************************************************
            //  PART A                                                                                                   (FROM ASSIGNMENT1)
            //*********************************************************************************************************************************            
            previousMouseState = currentMouseState;

            //---------------------------------------------------------------------------------------------------------------------------------
            //  PART B - Switch the environment map being displayed                                                      (FROM ASSIGNMENT1)
            //---------------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.D7)) currentSkybox = testColors; // B-7
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) currentSkybox = officeRoom; // B-8
            if (Keyboard.GetState().IsKeyDown(Keys.D9)) currentSkybox = daytimeSky; // B-9
            if (Keyboard.GetState().IsKeyDown(Keys.D0)) currentSkybox = mySkybox;   // B-0

            //---------------------------------------------------------------------------------------------------------------------------------
            //  PART C - Change the shader effect                                                                        (FROM ASSIGNMENT1)
            //---------------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.F7)) technique = 0;  // Reflection Shader
            if (Keyboard.GetState().IsKeyDown(Keys.F8)) technique = 1;  // Refraction Shader
            if (Keyboard.GetState().IsKeyDown(Keys.F9)) technique = 2;  // Refraction + Dispersion Shader
            if (Keyboard.GetState().IsKeyDown(Keys.F10)) technique = 3; // Fresnel Shader

            //---------------------------------------------------------------------------------------------------------------------------------
            //  PART C - Change the reflectivity                                                                         (FROM ASSIGNMENT1)
            //---------------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus)) reflectivity = MathHelper.Clamp(reflectivity + 0.01f, 0f, 1.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) reflectivity = MathHelper.Clamp(reflectivity - 0.01f, 0f, 1.0f);

            //*********************************************************************************************************************************
            //  PART C - Check for shift key being pressd                                                                (FROM ASSIGNMENT1)
            //*********************************************************************************************************************************
            bool shift = false;
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in pressedKeys) if (key == Keys.LeftShift || key == Keys.RightShift) shift = true;

            //---------------------------------------------------------------------------------------------------------------------------------
            //  PART C - Change the etaRatio                                                                             (FROM ASSIGNMENT1)
            //---------------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.R) && !shift) eta_Ratio.X = MathHelper.Clamp(eta_Ratio.X + 0.001f, 0f, 1.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.R) && shift) eta_Ratio.X = MathHelper.Clamp(eta_Ratio.X - 0.001f, 0f, 1.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.G) && !shift) eta_Ratio.Y = MathHelper.Clamp(eta_Ratio.Y + 0.001f, 0f, 10.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.G) && shift) eta_Ratio.Y = MathHelper.Clamp(eta_Ratio.Y - 0.001f, 0f, 10.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.B) && !shift) eta_Ratio.Z = MathHelper.Clamp(eta_Ratio.Z + 0.001f, 0f, 10.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.B) && shift) eta_Ratio.Z = MathHelper.Clamp(eta_Ratio.Z - 0.001f, 0f, 10.0f);

            //---------------------------------------------------------------------------------------------------------------------------------
            //  PART C - Change the fresnel                                                                              (FROM ASSIGNMENT1)
            //---------------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.Q) && !shift) fresnelPower += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.Q) && shift) fresnelPower -= 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && !shift) fresnelScale += 0.5f;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && shift) fresnelScale -= 0.5f;
            if (Keyboard.GetState().IsKeyDown(Keys.E) && !shift) fresnelBias += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.E) && shift) fresnelBias -= 0.01f;

            //---------------------------------------------------------------------------------------------------------------------------------
            //  PART C - Display Text Information                                                                        (FROM ASSIGNMENT1)
            //---------------------------------------------------------------------------------------------------------------------------------
            KeyboardState currentKeyboardState = Keyboard.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H)) showValues = !showValues;
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKeyboardState.IsKeyDown(Keys.OemQuestion)) showMenu = !showMenu;
            previousKeyboardState = currentKeyboardState;

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

            // *** Draw Skybox First            
            GraphicsDevice.BlendState = BlendState.Opaque;            
            GraphicsDevice.DepthStencilState = new DepthStencilState();            
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;            
            RasterizerState ras = new RasterizerState();            
            ras.CullMode = CullMode.None;            
            GraphicsDevice.RasterizerState = ras;            
            currentSkybox.Draw(view, projection, cameraPosition);            
            
            // *** Draw 3D helicopter            
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;            
            DrawModelWithEffect();

            //*********************************************************************************************************************************
            spriteBatch.Begin();
            if (showMenu) ShowControls();
            if (showValues) ShowInfo();
            spriteBatch.End();
            //*********************************************************************************************************************************

            base.Draw(gameTime);
        }


        //=====================================================================================================================================
        //  FUNCTION TO DRAW MODEL WITH EFFECT                                                                                 (FROM LAB06)
        //=====================================================================================================================================
        private void DrawModelWithEffect()
        {
            //*********************************************************************************************************************************
            //  PART C                                                                                                   (FROM ASSIGNMENT1)
            //*********************************************************************************************************************************
            effect.CurrentTechnique = effect.Techniques[technique]; 
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in helicopterModel.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform); 
                        effect.Parameters["View"].SetValue(view); 
                        effect.Parameters["Projection"].SetValue(projection); 
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition); 
                        Matrix worldInverseTranspose = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform)); 
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose); 
                        effect.Parameters["decalMap"].SetValue(helicopterTexture);
                        effect.Parameters["environmentMap"].SetValue(currentSkybox.skyBoxTexture);

                        //*******************************************************************************
                        effect.Parameters["Reflectivity"].SetValue(reflectivity);
                        effect.Parameters["etaRatio"].SetValue(eta_Ratio);
                        effect.Parameters["FresnelBias"].SetValue(fresnelBias);
                        effect.Parameters["FresnelScale"].SetValue(fresnelScale);
                        effect.Parameters["FresnelPower"].SetValue(fresnelPower);
                        //*******************************************************************************

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

        private void ShowControls()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Rotate camera: Mouse Left Drag", 
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change distance of camera: Mouse Right Drag", 
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Set skybox textures as test colors: 7 Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Set skybox textures as office room: 8 Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Set skybox textures as daytime sky: 9 Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Set skybox textures as outer space: 0 Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Reflection Shader: F7 Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Refraction Shader: F8 Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Refraction + Dispersion Shader: F9 Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Fresnel Shader: F10 Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Increase Reflectivity: + Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Decrease Reflectivity: - Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Increase Fresnel Power: Q Key (+SHIFT Key: decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Increase Fresnel Scale: W Key (+SHIFT Key: decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Increase Fresnel Bias: E Key (+SHIFT Key: decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Increase eta Ratio of red: R Key (+SHIFT Key: decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Increase eta Ratio of green: G Key (+SHIFT Key: decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Increase eta Ratio of blue: B Key (+SHIFT Key: decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
        }

        private void ShowInfo()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Camera Angle X: " + horizontalCameraAngle,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Camera Angle Y: " + verticalCameraAngle,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Camera Distance: " + distance,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Reflectivity: " + reflectivity,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Fresnel Power: " + fresnelPower,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Fresnel Scale: " + fresnelScale,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Fresnel Bias: " + fresnelBias,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "eta Ratio of red: " + eta_Ratio.X,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "eta Ratio of green: " + eta_Ratio.Y,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "eta Ratio of blue: " + eta_Ratio.Z,
                (Vector2.UnitY * 16 * (i++)), Color.White);
        }
    }
}
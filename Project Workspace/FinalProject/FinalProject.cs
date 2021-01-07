//############################################################################################################################################
//  Final Project - Volumetric Light Scattering as a Post-Process
//  FinalProject.cs - Game Class
//  CPI 411 - Graphics for Games
//  Max Forward
//############################################################################################################################################


//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//  FROM ONLINE TUTORIAL - Use outside code
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
using PostProcess;
using System;


// Use outside code
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


//********************************************************************************************************************************************
//  FinalProject Namespace
//********************************************************************************************************************************************
namespace FinalProject // Declare namespace for project
{
    //****************************************************************************************************************************************
    //  FinalProject Main Class
    //****************************************************************************************************************************************
    public class FinalProject : Game // "FinalProject" inherits from base class "Game"
    {
        // Declare Instance Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Model model; // 3D object
        // Effect effect; // Shader effect
        MouseState previousMouseState; // Previous state of the mouse
        Matrix world = Matrix.Identity; // World Matrix
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 100), Vector3.Zero, Vector3.UnitY); // View Matrix
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 1.0f, 1000.0f); // Projection Matrix
        Vector3 cameraPosition; // Position where camera is located
        Vector3 cameraTarget; // Where camera is looking at
        float cameraAngleX = 0; // Horizontal angle of camera
        float cameraAngleY = 0; // Vertical angle of camera
        float cameraDistance = 100f; // Distance of camera to the center
        Effect effect;
        Texture2D chapelTexture;
        Texture2D lensFlareTexture;
        Texture2D mosaicTexture;
        Texture2D glassTexture;
        Model gear1Model;
        Model gear2Model;
        Model gear3Model;
        KeyboardState oldState;
        bool rotate = false;
        SpriteFont font;
        KeyboardState previousKeyboardState;
        bool showMenu = false;
        bool showValues = false;
        int numberOfGears = 3;


        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        //  FROM ONLINE TUTORIAL - Instance Variables
        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        Vector3 lightPosition = new Vector3(0.2f, 0.26f, 0);
        Vector2 lightOffset = new Vector2(0f, 0f);
        float lightTextureSizeFactor = 1.5f;
        float lightShaftExposure = 0.3f;
        float lightShaftDecay = 0.98f;
        float lightShaftDensity = 1.0f;
        float lightShaftWeight = 0.8f;
        float modelExposure = 0.4f;
        int windowWidth = 800;
        int windowHeight = 600;
        Effect occlusionEffect;
        BasicEffect basicEffect;
        RenderTarget2D renderTargetColor;
        RenderTarget2D renderTargetOcclusion; // Contains Occlusion Mask
        RenderTarget2D renderTargetBackgroundLight; // Draw background light texture to Occlusion Mask
        RenderTarget2D renderTargetLinearFilter;
        RenderTarget2D renderTargetShafts;
        RenderTarget2D renderTargetShaftsFull;
        RenderTarget2D renderTargetFinal; // Combination of all render targets
        Texture2D texture; // Background light source texture
        Viewport viewport;
        PostProcessEffects postProcess;
        Vector3[] modelPosition = { new Vector3(0f, 0f, 0f), new Vector3(-22f, -10f, 5f), new Vector3(-20f, 10f, -40f) };
        float[] modelRotationAngle = { 0.0f, 0.0f, 0.0f };
        float[] modelRotationSpeed = { 0.002f, 0.003f, 0.001f };


        //====================================================================================================================================
        //  FinalProject Constructor
        //====================================================================================================================================
        public FinalProject()
        {
            // Initialize starting variables
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Set graphics profile to HiDef
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //  FROM ONLINE TUTORIAL
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }


        //====================================================================================================================================
        //  Initialize Method - Perform initialization prior to running and load any non-graphic related content
        //====================================================================================================================================
        protected override void Initialize()
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //  FROM ONLINE TUTORIAL - Create render targets
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            // Model with lighting and shader
            renderTargetColor = new RenderTarget2D(
                GraphicsDevice, 
                windowWidth, 
                windowHeight, 
                false, 
                SurfaceFormat.HalfVector4, 
                DepthFormat.Depth24);
            // Occlusion mask
            renderTargetOcclusion = new RenderTarget2D(
                GraphicsDevice, 
                windowWidth, 
                windowHeight, 
                false, 
                SurfaceFormat.HalfVector4, 
                DepthFormat.Depth24);
            // Background light texture + Occlusion Mask
            renderTargetBackgroundLight = new RenderTarget2D(
                GraphicsDevice, 
                windowWidth, 
                windowHeight, 
                false, 
                SurfaceFormat.HalfVector4, 
                DepthFormat.Depth24);
            // Linear Filter
            renderTargetLinearFilter = new RenderTarget2D(
                GraphicsDevice, 
                windowWidth, 
                windowHeight, 
                false, 
                SurfaceFormat.HalfVector4, 
                DepthFormat.Depth24);
            // Light Shafts
            renderTargetShafts = new RenderTarget2D(
                GraphicsDevice, 
                windowWidth / 2, 
                windowHeight / 2, 
                false, 
                SurfaceFormat.HalfVector4, 
                DepthFormat.Depth24);
            // Scaled to full resolution
            renderTargetShaftsFull = new RenderTarget2D(
                GraphicsDevice, 
                windowWidth, 
                windowHeight, 
                false, 
                SurfaceFormat.HalfVector4, 
                DepthFormat.Depth24);
            // Combination of all render targets
            renderTargetFinal = new RenderTarget2D(
                GraphicsDevice, 
                windowWidth, 
                windowHeight, 
                false, 
                SurfaceFormat.HalfVector4, 
                DepthFormat.Depth24);
            // Create instance of PostProcessEffects
            postProcess = new PostProcessEffects(windowWidth, windowHeight, GraphicsDevice, Content);
            // Create basic effect
            basicEffect = new BasicEffect(GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
            };

            // Create the GraphicsDevice
            base.Initialize();
        }


        //====================================================================================================================================
        //  LoadContent Method - Load game content
        //====================================================================================================================================
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load content
            model = Content.Load<Model>("Gear1");
            gear1Model = Content.Load<Model>("Gear1");
            gear2Model = Content.Load<Model>("Gear2");
            gear3Model = Content.Load<Model>("Gear3");
            effect = Content.Load<Effect>("ModelShader");
            occlusionEffect = Content.Load<Effect>("Occlusion");
            texture = Content.Load<Texture2D>("chapel");
            chapelTexture = Content.Load<Texture2D>("chapel");
            lensFlareTexture = Content.Load<Texture2D>("lensFlare");
            mosaicTexture = Content.Load<Texture2D>("mosaic");
            glassTexture = Content.Load<Texture2D>("glass");
            font = Content.Load<SpriteFont>("Text");
        }


        //====================================================================================================================================
        //  UnloadContent Method - Unload game content
        //====================================================================================================================================
        protected override void UnloadContent() { }


        //====================================================================================================================================
        //  Update Method - Called multiple times per second to update the game
        //====================================================================================================================================
        protected override void Update(GameTime gameTime)
        {
            // Exit the game if 'Back' button or 'Esc' key is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            //--------------------------------------------------------------------------------------------------------------------------------
            // Rotate the camera with Mouse Left Click + Drag
            //--------------------------------------------------------------------------------------------------------------------------------
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                cameraAngleX -= (Mouse.GetState().X - previousMouseState.X) / 100f;
                cameraAngleY -= (Mouse.GetState().Y - previousMouseState.Y) / 100f;
            }

            //--------------------------------------------------------------------------------------------------------------------------------
            // Change the distance of camera to the center with Mouse Right Click + Drag
            //--------------------------------------------------------------------------------------------------------------------------------
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                cameraDistance += (Mouse.GetState().Y - previousMouseState.Y) / 100f;
            }

            //--------------------------------------------------------------------------------------------------------------------------------
            // Translate the camera with Mouse Middle Click + Drag
            //--------------------------------------------------------------------------------------------------------------------------------
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 viewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(cameraAngleY) * Matrix.CreateRotationY(cameraAngleX));
                Vector3 viewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(cameraAngleY) * Matrix.CreateRotationY(cameraAngleX));
                cameraTarget -= viewRight * (Mouse.GetState().X - previousMouseState.X) / 10f;
                cameraTarget += viewUp * (Mouse.GetState().Y - previousMouseState.Y) / 10f;
            }

            // Save the current state of the mouse
            previousMouseState = Mouse.GetState();

            // Update the camera position
            cameraPosition = Vector3.Transform(new Vector3(0, 10, cameraDistance),
                Matrix.CreateRotationX(cameraAngleY) * Matrix.CreateRotationY(cameraAngleX) * Matrix.CreateTranslation(cameraTarget));

            // Update the View Matrix
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(cameraAngleY) * Matrix.CreateRotationY(cameraAngleX)));

            bool shift = false;
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in pressedKeys) if (key == Keys.LeftShift || key == Keys.RightShift) shift = true;
            KeyboardState currentKeyboardState = Keyboard.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H)) showValues = !showValues;
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKeyboardState.IsKeyDown(Keys.OemQuestion)) showMenu = !showMenu;
            previousKeyboardState = currentKeyboardState;
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) texture = chapelTexture;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) texture = lensFlareTexture;
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) texture = mosaicTexture;
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) texture = glassTexture;
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) model = gear1Model;
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) model = gear2Model;
            if (Keyboard.GetState().IsKeyDown(Keys.D7)) model = gear3Model;
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && !shift) lightPosition.X -= 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && !shift) lightPosition.X += 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !shift) lightPosition.Y -= 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !shift) lightPosition.Y += 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                modelRotationSpeed[0] += 0.0005f;
                modelRotationSpeed[1] += 0.0005f;
                modelRotationSpeed[2] += 0.0005f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                modelRotationSpeed[0] -= 0.0005f;
                modelRotationSpeed[1] -= 0.0005f;
                modelRotationSpeed[2] -= 0.0005f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E) && !shift) lightShaftExposure += 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.E) && shift) lightShaftExposure -= 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.C) && !shift) lightShaftDecay += 0.0005f;
            if (Keyboard.GetState().IsKeyDown(Keys.C) && shift) lightShaftDecay -= 0.0005f;
            if (Keyboard.GetState().IsKeyDown(Keys.D) && !shift) lightShaftDensity += 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.D) && shift) lightShaftDensity -= 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && !shift) lightShaftWeight += 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.W) && shift) lightShaftWeight -= 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && shift) lightOffset.X -= 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && shift) lightOffset.X += 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && shift) lightOffset.Y -= 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && shift) lightOffset.Y += 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !shift) lightTextureSizeFactor += 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.T) && shift) lightTextureSizeFactor -= 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.M) && !shift) modelExposure += 0.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.M) && shift) modelExposure -= 0.005f;

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //  FROM ONLINE TUTORIAL
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            // Animate the models
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space)) 
            {
                if (rotate == false) rotate = true;
                else rotate = false;
            }
            oldState = newState;
            if (rotate == true)
            {
                for (int i = 0; i < numberOfGears; ++i)
                {
                    if (modelRotationAngle[i] < Math.PI * 2) modelRotationAngle[i] += modelRotationSpeed[i];
                    else modelRotationAngle[i] = 0f;
                }
            }
            // Basic effect for background light texture
            viewport = GraphicsDevice.Viewport;
            basicEffect.Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) *
                          Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);

            // Update the game
            base.Update(gameTime);
        }


        //====================================================================================================================================
        //  Draw Method - Called multiple times per second to draw the game
        //====================================================================================================================================
        protected override void Draw(GameTime gameTime)
        {
            // Clear background and fill with color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //  FROM ONLINE TUTORIAL
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            // Draw the same scene with two different materials (effects) into two different render targets using the RenderScene method
            // Render scene with black Occlusion Mask
            RenderScene(renderTargetOcclusion, occlusionEffect, view, projection);
            // Render the scene with model shader effect and lighting
            RenderScene(renderTargetColor, effect, view, projection);
            // Render the Occlusion mask over the background light texture
            GraphicsDevice.SetRenderTarget(renderTargetBackgroundLight);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(
                SpriteSortMode.Immediate, 
                BlendState.AlphaBlend, 
                null, 
                null, 
                null,
                basicEffect, 
                Matrix.CreateScale(1f / lightTextureSizeFactor));
            spriteBatch.Draw(
                texture,
                new Rectangle(
                    (int)((2*lightPosition.X + lightOffset.X) * GraphicsDevice.Viewport.Width - texture.Width / lightTextureSizeFactor / 2),
                    (int)((2*lightPosition.Y + lightOffset.Y) * GraphicsDevice.Viewport.Height - texture.Height / lightTextureSizeFactor / 2),
                    (int)(texture.Width / lightTextureSizeFactor),
                    (int)(texture.Height / lightTextureSizeFactor)), 
                Color.White);
            spriteBatch.Draw(renderTargetOcclusion, Vector2.Zero, Color.Black);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            // Apply linear filter effect
            postProcess.linearFilter(renderTargetBackgroundLight, renderTargetLinearFilter);
            // Apply light shafts on half resolution
            postProcess.LightShafts(
                renderTargetLinearFilter, 
                renderTargetShafts, 
                lightPosition,
                lightShaftDensity,
                lightShaftDecay,
                lightShaftWeight,
                lightShaftExposure);
            // Up-scale the light shafts effect
            postProcess.HalfToFullscreen(renderTargetShafts, renderTargetShaftsFull);
            // Combine all render targets
            GraphicsDevice.SetRenderTarget(renderTargetFinal);
            GraphicsDevice.Clear(ClearOptions.Target, Vector4.Zero, 1, 0);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(renderTargetShaftsFull, Vector2.Zero, Color.White);
            spriteBatch.Draw(renderTargetColor, Vector2.Zero, Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            // Draw the render targets
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(renderTargetFinal, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Show Info/Controls
            spriteBatch.Begin();
            if (showMenu) ShowControls();
            if (showValues) ShowInfo();
            spriteBatch.End();

            // Draw the game
            base.Draw(gameTime);
        }


        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        //  FROM ONLINE TUTORIAL
        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        private void RenderScene(RenderTarget2D RenderTarget, Effect Effect, Matrix View, Matrix Projection)
        {
            Matrix worldInverseTranspose, viewInverse;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts) part.Effect = Effect;
            }
            bool OldDepthBufferEnable = GraphicsDevice.DepthStencilState.DepthBufferEnable;
            CullMode OldCullMode = GraphicsDevice.RasterizerState.CullMode;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target, new Vector4(0f, 0f, 0f, 0f), 1, 0); // Clear render target
            for (int i = 0; i < numberOfGears; ++i) // Draw Model
            {
                world = Matrix.CreateScale(0.2f, 0.2f, 0.2f) 
                    * Matrix.CreateRotationZ(modelRotationAngle[i]) 
                    * Matrix.CreateTranslation(modelPosition[i]);
                worldInverseTranspose = Matrix.Invert(Matrix.Transpose(world));
                viewInverse = Matrix.Invert(View);
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (Effect currentEffect in mesh.Effects)
                    {
                        currentEffect.CurrentTechnique = currentEffect.Techniques[0];
                        currentEffect.Parameters["World"].SetValue(world);
                        currentEffect.Parameters["View"].SetValue(View);
                        currentEffect.Parameters["Projection"].SetValue(Projection);
                        currentEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                        currentEffect.Parameters["ViewInverse"].SetValue(viewInverse);
                        currentEffect.Parameters["Exposure"].SetValue(modelExposure);
                    }
                    mesh.Draw();
                }
            }
            GraphicsDevice.Textures[0] = null;
            GraphicsDevice.Textures[1] = null;
            GraphicsDevice.SetRenderTarget(null);
            DepthStencilState depthStencilState = new DepthStencilState
            {
                DepthBufferEnable = OldDepthBufferEnable
            };
            GraphicsDevice.DepthStencilState = depthStencilState;
            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = OldCullMode;
            GraphicsDevice.RasterizerState = rasterizerState;
        }


        //====================================================================================================================================
        //  ShowControls Method
        //====================================================================================================================================
        private void ShowControls()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Rotate camera: Mouse Left Drag",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change distance of camera: Mouse Right Drag",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Move Camera: Middle Mouse Drag Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Rotate Model: SPACEBAR",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Background Texture: 1-4 Keys",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Model: 5-7 Keys Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change light position: Arrow Keys",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change the light offset: Arrow Keys + SHIFT Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Increase/Decrease model rotation speed: +/- Keys",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change light shaft exposure: E Key (+ SHIFT Key for decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change light shaft decay: C Key (+ SHIFT Key for decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change light shaft density: D Key (+ SHIFT Key for decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change light shaft weight: W Key (+ SHIFT Key for decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change texture factor: T Key (+ SHIFT Key for decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change model exposure: M Key (+ SHIFT Key for decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
        }


        //====================================================================================================================================
        //  ShowInfo Method
        //====================================================================================================================================
        private void ShowInfo()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Camera Angle X: " + cameraAngleX,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Camera Angle Y: " + cameraAngleY,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Camera Distance: " + cameraDistance,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Light position X: " + lightPosition.X,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Light position Y: " + lightPosition.Y,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Light offset X: " + lightOffset.X,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Light offset Y: " + lightOffset.Y, 
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Light shaft exposure: " + lightShaftExposure,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Light shaft decay: " + lightShaftDecay,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Light shaft density: " + lightShaftDensity,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Light shaft weight: " + lightShaftWeight,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Light Texture Size Factor: " + lightTextureSizeFactor,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Model Exposure: " + modelExposure,
                (Vector2.UnitY * 16 * (i++)), Color.White);
        }
    }
}
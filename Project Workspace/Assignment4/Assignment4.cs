using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//=========================================================================================================================================
//  LAB10
//=========================================================================================================================================
using CPI411.SimpleEngine;



namespace Assignment4
{
    public class Assignment4 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        //*********************************************************************************************************************************
        //	TEMPLATE - Variables
        //*********************************************************************************************************************************
        Model model;
        Effect effect;
        Texture2D texture;
        SpriteFont font;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 30), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 30;
        MouseState preMouse;


        //=================================================================================================================================
        //  LAB10 - Variables
        //=================================================================================================================================
        Matrix invertCamera = Matrix.Identity;
        ParticleManager particleManager;
        Vector3 particlePosition;
        System.Random random;


        //#################################################################################################################################
        //  ASSIGNMENT4 - Variables
        //#################################################################################################################################
        Texture2D smoke;
        Texture2D water;
        Texture2D fire;
        int functionKey = 1;    // 1 = Fountain basic       2 = Fountain medium     3 = Fountain advanced
        int emitterShape = 1;   // 1 = Square               2 = Curve               3 = Ring
        KeyboardState oldState;
        int curveShapeCounter = 0;
        KeyboardState previousKeyboardState;
        bool showMenu = false;
        bool showValues = false;
        float friction = 0.5f;
        float resilience = 1.0f;
        int lifespan = 4;
        float windParamX = 1.0f;
        float windParamZ = 1.0f;


        public Assignment4()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //*****************************************************************************************************************************
            //	TEMPLATE
            //*****************************************************************************************************************************
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }


        protected override void Initialize()
        {
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //#############################################################################################################################
            //  ASSIGNMENT4
            //#############################################################################################################################
            smoke = Content.Load<Texture2D>("smoke");
            water = Content.Load<Texture2D>("water");
            fire = Content.Load<Texture2D>("fire");
            texture = null;
            font = Content.Load<SpriteFont>("Text");

            //=============================================================================================================================
            //  LAB10
            //=============================================================================================================================
            model = Content.Load<Model>("Plane");
            effect = Content.Load<Effect>("ParticleSystemShader");
            particleManager = new ParticleManager(GraphicsDevice, 10000);
            particlePosition = new Vector3(0, 5, 0);
            random = new System.Random();
        }


        protected override void UnloadContent(){}


        protected override void Update(GameTime gameTime)
        {
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            //*****************************************************************************************************************************
            //	TEMPLATE - Update camera and light
            //*****************************************************************************************************************************
            if (Mouse.GetState().LeftButton == ButtonState.Pressed) // Rotate the camera: Mouse Left Drag
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed) // Change the distance of camera to the center: Mouse Right Drag
            {
                distance += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed) // Translate the camera: Mouse Middle Drag
            { 
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)); 
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)); 
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f; 
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f; 
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f; // Rotate the light: Arrow Keys
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f; // Rotate the light: Arrow Keys
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f; // Rotate the light: Arrow Keys
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f; // Rotate the light: Arrow Keys
            if (Keyboard.GetState().IsKeyDown(Keys.S)) // Reset camera and light: 'S' Key
            {
                angle = angle2 = angleL = angleL2 = 0;
                distance = 30;
                cameraTarget = Vector3.Zero;
            }
            preMouse = Mouse.GetState();
            cameraPosition = Vector3.Transform(new Vector3(0, 10, distance), 
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, 
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            lightPosition = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));

            //#############################################################################################################################
            //  ASSIGNMENT4
            //#############################################################################################################################
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) texture = null;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) texture = smoke;
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) texture = water;
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) texture = fire;
            if (Keyboard.GetState().IsKeyDown(Keys.F1)) functionKey = 1; // Fountain Basic
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) functionKey = 2; // Fountain Medium
            if (Keyboard.GetState().IsKeyDown(Keys.F3)) functionKey = 3; // Fountain Advanced

            bool shift = false;
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in pressedKeys) if (key == Keys.LeftShift || key == Keys.RightShift) shift = true;
            KeyboardState currentKeyboardState = Keyboard.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H)) showValues = !showValues;
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKeyboardState.IsKeyDown(Keys.OemQuestion)) showMenu = !showMenu;
            previousKeyboardState = currentKeyboardState;
            if (Keyboard.GetState().IsKeyDown(Keys.R) && !shift) resilience += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.R) && shift) resilience -= 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.F) && !shift) friction += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.F) && shift) friction -= 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.L) && !shift) lifespan += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.L) && shift) lifespan -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.X) && !shift) windParamX += 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.X) && shift) windParamX -= 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && !shift) windParamZ += 0.1f;
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && shift) windParamZ -= 0.1f;

            //#############################################################################################################################
            //  ASSIGNMENT4 - Change emitter shape
            //#############################################################################################################################
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.F4) && oldState.IsKeyUp(Keys.F4)) // Change Emitter Shape: 'F4' Key
            {
                if (emitterShape==3)
                {
                    emitterShape = 1;
                }
                else emitterShape += 1;
            }
            oldState = newState;

            //=============================================================================================================================
            //  LAB10
            //=============================================================================================================================
            if (Keyboard.GetState().IsKeyDown(Keys.P)) // Generate particles: 'P' Key
            {
                //#########################################################################################################################
                //  ASSIGNMENT4 - Fountain Basic
                //#########################################################################################################################
                if (functionKey == 1) // Fountain basic: 'F1' Key
                {
                    //---------------------------------------------------------------------------------------------------------------------
                    //  ASSIGNMENT4 - Square emitter shape
                    //---------------------------------------------------------------------------------------------------------------------
                    if (emitterShape == 1) // Square emitter shape
                    {
                        for (int i = 0; i < 60; i++)
                        {
                            double angle = System.Math.PI * (i * 6) / 180.0;
                            Particle particle = particleManager.getNext();
                            if (i >= 0 && i < 7)
                            {
                                int j = i + 1;
                                particle.Position = new Vector3(j, 5, 7);
                            }
                            if (i >= 7 && i < 15)
                            {
                                int j = i - 7;
                                particle.Position = new Vector3(7, 5, (7 - j));
                            }
                            if (i >= 15 && i < 22)
                            {
                                int j = i - 14;
                                particle.Position = new Vector3(7, 5, -j);
                            }
                            if (i >= 22 && i < 30)
                            {
                                int j = i - 22;
                                particle.Position = new Vector3((7 - j), 5, -7);
                            }
                            if (i >= 30 && i < 37)
                            {
                                int j = i - 29;
                                particle.Position = new Vector3(-j, 5, -7);
                            }
                            if (i >= 37 && i < 45)
                            {
                                int j = i - 37;
                                particle.Position = new Vector3(-7, 5, (-7 + j));
                            }
                            if (i >= 45 && i < 52)
                            {
                                int j = i - 44;
                                particle.Position = new Vector3(-7, 5, j);
                            }
                            if (i >= 52 && i < 60)
                            {
                                int j = i - 52;
                                particle.Position = new Vector3((-7 + j), 5, 7);
                            }
                            particle.Velocity = new Vector3(
                                10.0f * (float)System.Math.Sin(angle), 
                                5, 
                                10.0f * (float)System.Math.Cos(angle));
                            particle.Acceleration = new Vector3(0, 0, 0);
                            particle.MaxAge = lifespan;
                            particle.Init();
                        }
                    }
                    //---------------------------------------------------------------------------------------------------------------------
                    //  ASSIGNMENT4 - Curve emitter shape
                    //---------------------------------------------------------------------------------------------------------------------
                    if (emitterShape == 2) // Curve emitter shape
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = new Vector3((float)System.Math.Sin(curveShapeCounter), 5, 0);
                        particle.Velocity = new Vector3(0, 5, 0);
                        particle.Acceleration = new Vector3(0, 0, 0);
                        particle.MaxAge = lifespan;
                        particle.Init();
                        curveShapeCounter += 50;
                    }
                    //---------------------------------------------------------------------------------------------------------------------
                    //  ASSIGNMENT4 - Ring emitter shape
                    //---------------------------------------------------------------------------------------------------------------------
                    if (emitterShape == 3) // Ring emitter shape
                    {
                        for (int i = 0; i < 60; i++)
                        {
                            double angle = System.Math.PI * (i * 6) / 180.0;
                            Particle particle = particleManager.getNext();
                            particle.Position = particlePosition;
                            particle.Velocity = new Vector3(
                                10.0f * (float)System.Math.Sin(angle), 
                                5, 
                                10.0f * (float)System.Math.Cos(angle));
                            particle.Acceleration = new Vector3(0, 0, 0);
                            particle.MaxAge = lifespan;
                            particle.Init();
                        }
                    }
                }
                //#########################################################################################################################
                //  ASSIGNMENT4 - Fountain Medium
                //#########################################################################################################################
                if (functionKey == 2) // Fountain medium: 'F2' Key
                {
                    //---------------------------------------------------------------------------------------------------------------------
                    //  ASSIGNMENT4 - Square emitter shape
                    //---------------------------------------------------------------------------------------------------------------------
                    if (emitterShape == 1) // Square emitter shape
                    {
                        int i = (random.Next(0, 60));
                        double angle = System.Math.PI * (i * 6) / 180.0;
                        Particle particle = particleManager.getNext();
                        if (i >= 0 && i < 7)
                        {
                            int j = i + 1;
                            particle.Position = new Vector3(j, 5, 7);
                        }
                        if (i >= 7 && i < 15)
                        {
                            int j = i - 7;
                            particle.Position = new Vector3(7, 5, (7 - j));
                        }
                        if (i >= 15 && i < 22)
                        {
                            int j = i - 14;
                            particle.Position = new Vector3(7, 5, -j);
                        }
                        if (i >= 22 && i < 30)
                        {
                            int j = i - 22;
                            particle.Position = new Vector3((7 - j), 5, -7);
                        }
                        if (i >= 30 && i < 37)
                        {
                            int j = i - 29;
                            particle.Position = new Vector3(-j, 5, -7);
                        }
                        if (i >= 37 && i < 45)
                        {
                            int j = i - 37;
                            particle.Position = new Vector3(-7, 5, (-7 + j));
                        }
                        if (i >= 45 && i < 52)
                        {
                            int j = i - 44;
                            particle.Position = new Vector3(-7, 5, j);
                        }
                        if (i >= 52 && i < 60)
                        {
                            int j = i - 52;
                            particle.Position = new Vector3((-7 + j), 5, 7);
                        }
                        particle.Velocity = new Vector3(
                            10.0f * (float)System.Math.Sin(angle), 
                            random.Next(0, 5), 
                            10.0f * (float)System.Math.Cos(angle));
                        particle.Acceleration = new Vector3(0, 0, 0);
                        particle.MaxAge = lifespan;
                        particle.Init();
                    }
                    //---------------------------------------------------------------------------------------------------------------------
                    //  ASSIGNMENT4 - Curve emitter shape
                    //---------------------------------------------------------------------------------------------------------------------
                    if (emitterShape == 2)  // Curve emitter shape
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = new Vector3((float)System.Math.Sin(curveShapeCounter), 5, 0);
                        particle.Velocity = new Vector3(random.Next(-5, 5), random.Next(0, 5), random.Next(-5, 5));
                        particle.Acceleration = new Vector3(0, 0, 0);
                        particle.MaxAge = lifespan;
                        particle.Init();
                        curveShapeCounter += 50;
                    }
                    //---------------------------------------------------------------------------------------------------------------------
                    //  ASSIGNMENT4 - Ring emitter shape
                    //---------------------------------------------------------------------------------------------------------------------
                    if (emitterShape == 3) // Curve emitter shape
                    {
                        double angle = System.Math.PI * (random.Next(0, 60) * 6) / 180.0;
                        Particle particle = particleManager.getNext();
                        particle.Position = particlePosition;
                        particle.Velocity = new Vector3(
                            10.0f * (float)System.Math.Sin(angle),
                            random.Next(0, 5),
                            10.0f * (float)System.Math.Cos(angle));
                        particle.Acceleration = new Vector3(0, 0, 0);
                        particle.MaxAge = lifespan;
                        particle.Init();
                    }
                }
                //#########################################################################################################################
                //  ASSIGNMENT4 - Fountain Advanced
                //#########################################################################################################################
                if (functionKey == 3) // Fountain advanced: 'F3' Key
                {
                    //---------------------------------------------------------------------------------------------------------------------
                    //  ASSIGNMENT4 - Square emitter shape
                    //---------------------------------------------------------------------------------------------------------------------
                    if (emitterShape == 1) // Square emitter shape
                    {
                        int i = (random.Next(0, 60));
                        double angle = System.Math.PI * (i * 6) / 180.0;
                        Particle particle = particleManager.getNext();
                        if (i >= 0 && i < 7)
                        {
                            int j = i + 1;
                            particle.Position = new Vector3(j, 5, 7);
                        }
                        if (i >= 7 && i < 15)
                        {
                            int j = i - 7;
                            particle.Position = new Vector3(7, 5, (7 - j));
                        }
                        if (i >= 15 && i < 22)
                        {
                            int j = i - 14;
                            particle.Position = new Vector3(7, 5, -j);
                        }
                        if (i >= 22 && i < 30)
                        {
                            int j = i - 22;
                            particle.Position = new Vector3((7 - j), 5, -7);
                        }
                        if (i >= 30 && i < 37)
                        {
                            int j = i - 29;
                            particle.Position = new Vector3(-j, 5, -7);
                        }
                        if (i >= 37 && i < 45)
                        {
                            int j = i - 37;
                            particle.Position = new Vector3(-7, 5, (-7 + j));
                        }
                        if (i >= 45 && i < 52)
                        {
                            int j = i - 44;
                            particle.Position = new Vector3(-7, 5, j);
                        }
                        if (i >= 52 && i < 60)
                        {
                            int j = i - 52;
                            particle.Position = new Vector3((-7 + j), 5, 7);
                        }
                        particle.Velocity = new Vector3(
                            10.0f * (float)System.Math.Sin(angle) + windParamX,
                            random.Next(0, 5),
                            10.0f * (float)System.Math.Cos(angle) + windParamZ);
                        particle.Acceleration = new Vector3(0, 0, 0);
                        particle.MaxAge = lifespan;
                        particle.Init();
                    }
                    //---------------------------------------------------------------------------------------------------------------------
                    //  ASSIGNMENT4 - Curve emitter shape
                    //---------------------------------------------------------------------------------------------------------------------
                    if (emitterShape == 2)  // Curve emitter shape
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = new Vector3((float)System.Math.Sin(curveShapeCounter), 5, 0);
                        particle.Velocity = new Vector3(random.Next(-5, 5) + windParamX, random.Next(0, 5), random.Next(-5, 5) + windParamZ);
                        particle.Acceleration = new Vector3(0, 0, 0);
                        particle.MaxAge = lifespan;
                        particle.Init();
                        curveShapeCounter += 50;
                    }
                    //---------------------------------------------------------------------------------------------------------------------
                    //  ASSIGNMENT4 - Ring emitter shape
                    //---------------------------------------------------------------------------------------------------------------------
                    if (emitterShape == 3) // Curve emitter shape
                    {
                        double angle = System.Math.PI * (random.Next(0, 60) * 6) / 180.0;
                        Particle particle = particleManager.getNext();
                        particle.Position = particlePosition;
                        particle.Velocity = new Vector3(
                            10.0f * (float)System.Math.Sin(angle) + windParamX,
                            random.Next(0, 5),
                            10.0f * (float)System.Math.Cos(angle) + windParamZ);
                        particle.Acceleration = new Vector3(0, 0, 0);
                        particle.MaxAge = lifespan;
                        particle.Init();
                    }
                }
            }

            //#########################################################################################################################
            //  ASSIGNMENT4 - Update Particles
            //#########################################################################################################################
            if (functionKey == 2)
            {
                particleManager.UpdateMedium(gameTime.ElapsedGameTime.Milliseconds * 0.001f);
            }
            if (functionKey == 3)
            {
                particleManager.UpdateAdvanced(gameTime.ElapsedGameTime.Milliseconds * 0.001f, resilience, friction);
            }
            else particleManager.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);
            invertCamera = Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //=============================================================================================================================
            //  LAB10
            //=============================================================================================================================
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            model.Draw(world, view, projection);
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //#############################################################################################################################
            // ASSIGNMENT4 - Change the technique if no texture
            //#############################################################################################################################
            if (texture == null)
            {
                effect.CurrentTechnique = effect.Techniques[1];
            }
            else effect.CurrentTechnique = effect.Techniques[0];

            //=============================================================================================================================
            //  LAB10
            //=============================================================================================================================
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["InverseCamera"].SetValue(invertCamera);
            effect.Parameters["Texture"].SetValue(texture);

            //#######################################################################################
            // ASSIGNMENT4 - Extra parameters for Phong Shading
            //#######################################################################################
            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(world));
            effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
            effect.Parameters["CameraPosition"].SetValue(cameraPosition);
            effect.Parameters["LightPosition"].SetValue(lightPosition);

            //=============================================================================================================================
            //  LAB10
            //=============================================================================================================================
            particleManager.Draw(GraphicsDevice);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            spriteBatch.Begin();
            if (showMenu) ShowControls();
            if (showValues) ShowInfo();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ShowControls()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Rotate camera: Mouse Left Drag",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change distance of camera: Mouse Right Drag",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Move Camera: Middle Mouse Drag Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Resilience Camera: R Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Friction: F Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Lifespan: L Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Wind Force (X-Direction): X Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Wind Force (Z-Direction): Z Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
        }

        private void ShowInfo()
        {
            int i = 0;
            spriteBatch.DrawString(font, "Camera Angle X: " + angle,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Camera Angle Y: " + angle2,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Camera Distance: " + distance,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Resilience: " + resilience,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Friction: " + friction,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Lifespan: " + lifespan,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Wind Force (X): " + windParamX,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Wind Force (Z): " + windParamZ,
                (Vector2.UnitY * 16 * (i++)), Color.White);
        }
    }
}
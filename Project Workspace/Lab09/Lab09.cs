using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Lab09
{
    public class Lab09 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        //**********************************************************************************************************//
        //	TEMPLATE - Global Variables
        //**********************************************************************************************************//
        SpriteFont font;
        Effect effect;
        Texture2D texture;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 30), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Matrix lightView = Matrix.CreateLookAt(new Vector3(0, 0, 10), -Vector3.UnitZ, Vector3.UnitY);
        Matrix lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 1f, 100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        float angle, angle2, angleL, angleL2, distance;
        MouseState preMouse;


        //==========================================================================================================//
        //	LAB09 - Global Variables
        //==========================================================================================================//
        Model[] models;
        RenderTarget2D renderTarget; // Keep depth map
        Texture2D shadowMap; // Converted to Texture2D


        public Lab09()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //**********************************************************************************************************//
            //	TEMPLATE
            //**********************************************************************************************************//
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

            //==========================================================================================================//
            //	LAB09 - Load models in array
            //==========================================================================================================//
            models = new Model[2];
            models[0] = Content.Load<Model>("Plane");
            models[1] = Content.Load<Model>("torus");
            effect = Content.Load<Effect>("ShadowShader");
            texture = Content.Load<Texture2D>("Checker");
            resetView();

            //==========================================================================================================//
            // *** Lab9: Step1     
            //==========================================================================================================//
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            //renderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth,pp.BackBufferHeight, 
            //true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);            
            renderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single,
                DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
        }


        protected override void UnloadContent() { }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //**********************************************************************************************************//
            //	TEMPLATE
            //**********************************************************************************************************//
            if (Keyboard.GetState().IsKeyDown(Keys.S)) resetView();
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed) distance += (Mouse.GetState().X - preMouse.X) / 100f;
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }
            preMouse = Mouse.GetState();
            // Update Camera            
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Transform(Vector3.UnitY,
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            // Update Light            
            lightPosition = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            // Update LightMatrix            
            lightView = Matrix.CreateLookAt(lightPosition, -Vector3.Normalize(lightPosition), Vector3.Up);
            //lightView = Matrix.CreateLookAt(lightPosition, Vector3.Zero,
            //Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL)));
            //lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 1f, 50f);
            //if (Keyboard.GetState().IsKeyDown(Keys.S)) {angle = angle2 = angleL = angleL2 = 0; distance = 30; cameraTarget = Vector3.Zero;}

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(Color.CornflowerBlue);

            //**********************************************************************************************************//
            //	TEMPLATE
            //**********************************************************************************************************//
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //==========================================================================================================//
            //  LAB09           
            //==========================================================================================================//
            // *** Lab9 : Step2, Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            // *** Lab 9 : Step3, Render a shadow map            
            DrawShadowMap();
            // *** Lab 9 : Step4: Clear the render target
            GraphicsDevice.SetRenderTarget(null);
            shadowMap = (Texture2D)renderTarget;
            // *** Lab 9 : Step5: Clear the render target            
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            // *** Lab 9 : Step6: Draw a scene
            DrawShadowScene();
            // *** Draw Depth map  
            //---------------------------------------------------------------------Commented out on mine
            /*using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin();
                sprite.Draw(shadowMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
                sprite.End();
            }*/
            //---------------------------------------------------------------------Commented out on mine
            /*spriteBatch.Begin();
            int i = 0;
            spriteBatch.DrawString(font, "Light ("
                + lightPosition.X.ToString("0.00")
                + "," + lightPosition.Y.ToString("0.00")
                + "," + lightPosition.Z.ToString("0.00")
                + ")", Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Camera("
                + cameraPosition.X.ToString("0.00")
                + "," + cameraPosition.Y.ToString("0.00")
                + "," + cameraPosition.Z.ToString("0.00")
                + ")", Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.DrawString(font, "Angles for Light("
                + angleL.ToString("0.00")
                + "," + angleL2.ToString("0.00")
                + ")", Vector2.UnitY * 15 * (i++), Color.White);
            spriteBatch.End();*/
            //*****************            
            shadowMap = null;

            base.Draw(gameTime);
        }


        private void resetView()
        {
            angle = angle2 = 0; 
            angleL = angleL2 = 0; // MathHelper.ToRadians(45);            
            distance = 30f;            
            cameraTarget = Vector3.Zero;
        }


        //==========================================================================================================//
        //	LAB09
        //==========================================================================================================//
        void DrawShadowMap()
        {
            effect.CurrentTechnique = effect.Techniques["ShadowMapShader"];
            foreach (Model model in models)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                            effect.Parameters["LightViewMatrix"].SetValue(lightView);
                            effect.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
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


        //==========================================================================================================//
        //	LAB09
        //==========================================================================================================//
        void DrawShadowScene()
        {
            effect.CurrentTechnique = effect.Techniques["ShadowedScene"];
            foreach (Model model in models)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                            effect.Parameters["View"].SetValue(view);
                            effect.Parameters["Projection"].SetValue(projection);
                            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                            effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                            effect.Parameters["LightViewMatrix"].SetValue(lightView);
                            effect.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                            effect.Parameters["LightPosition"].SetValue(lightPosition);
                            effect.Parameters["ShadowMap"].SetValue(shadowMap);
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
}
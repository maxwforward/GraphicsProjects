using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace Lab12
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab12 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        //**********************************************************************************************************//
        //	TEMPLATE - Global Variables
        //**********************************************************************************************************//
        SpriteFont font;
        Effect effect;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 20), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        Matrix lightView, lightProjection;
        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 20;
        MouseState preMouse;
        Model model;
        Texture2D texture;


        //**********************************************************************************************************//
        // LAB12
        //**********************************************************************************************************//
        RenderTarget2D renderTarget;
        Texture2D depthAndNormalMap, randomNormalMap;
        float offset = 800 / 256f;
        float SSAORad = 0.001f;
        VertexPositionTexture[] vertices = 
        { 
            new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)), 
            new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)), 
            new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)), 
            new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)), 
            new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)), 
            new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)) 
        };


        public Lab12()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //**********************************************************************************************************//
            //	TEMPLATE
            //**********************************************************************************************************//
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

            //**********************************************************************************************************//
            //	TEMPLATE
            //**********************************************************************************************************//
            model = Content.Load<Model>("objects");
            effect = Content.Load<Effect>("DepthAndNormal");
            randomNormalMap = Content.Load<Texture2D>("noise");

            //**********************************************************************************************************//
            //	LAB12
            //**********************************************************************************************************//
            PresentationParameters pp = GraphicsDevice.PresentationParameters; 
            renderTarget = new RenderTarget2D(
                GraphicsDevice, 
                pp.BackBufferWidth, 
                pp.BackBufferHeight, 
                false, 
                SurfaceFormat.Color, 
                DepthFormat.Depth24);
            font = Content.Load<SpriteFont>("Font");
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

            //**********************************************************************************************************//
            //	LAB12
            //**********************************************************************************************************//
            if (Keyboard.GetState().IsKeyDown(Keys.O) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) offset += 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.O) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) offset -= 0.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.R) && !Keyboard.GetState().IsKeyDown(Keys.LeftShift)) SSAORad += 0.001f;
            if (Keyboard.GetState().IsKeyDown(Keys.R) && Keyboard.GetState().IsKeyDown(Keys.LeftShift)) SSAORad -= 0.001f;

            //**********************************************************************************************************//
            //	TEMPLATE
            //**********************************************************************************************************//
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { angle = angle2 = angleL = angleL2 = 0; distance = 30; cameraTarget = Vector3.Zero; }
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

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //**********************
            //  LAB12
            //**********************
            GraphicsDevice.BlendState = BlendState.Opaque; GraphicsDevice.DepthStencilState = new DepthStencilState(); 
            GraphicsDevice.SetRenderTarget(renderTarget); 
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0); 
            DrawDepthAndNormalMap(); 
            GraphicsDevice.SetRenderTarget(null); 
            depthAndNormalMap = (Texture2D)renderTarget;
            //********* This block will be used later for Deferred Shading (SSAO) 
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0); 
            DrawSSAO();
            //*********
            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice)) 
            { 
                sprite.Begin(); 
                //sprite.Draw(depthAndNormalMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0); 
                sprite.End(); 
            }
            spriteBatch.Begin();
            int i = 0;
            spriteBatch.DrawString(font, "Offset: " + offset.ToString("0.00"), Vector2.UnitY * 15 * (i++), Color.Black);
            spriteBatch.DrawString(font, "Rad: " + SSAORad.ToString("0.000"), Vector2.UnitY * 15 * (i++), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }


        //***************************************************************
        //  LAB12 Private Method
        //***************************************************************
        private void DrawDepthAndNormalMap() 
        { 
            effect = Content.Load<Effect>("DepthAndNormal"); 
            effect.CurrentTechnique = effect.Techniques[0]; 
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
                        pass.Apply(); 
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer); 
                        GraphicsDevice.Indices = part.IndexBuffer; 
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount); 
                    } 
                } 
            } 
        }


        //***************************************************************
        //  LAB12 Private Method
        //***************************************************************
        private void DrawSSAO() 
        { 
            effect = Content.Load<Effect>("SSAO"); 
            effect.CurrentTechnique = effect.Techniques[0]; 
            effect.CurrentTechnique.Passes[0].Apply(); 
            effect.Parameters["RandomNormalTexture"].SetValue(randomNormalMap); 
            effect.Parameters["DepthAndNormalTexture"].SetValue(depthAndNormalMap); 
            effect.Parameters["offset"].SetValue(offset); 
            effect.Parameters["rad"].SetValue(SSAORad);
            // *** Draw the output into the 2 triangles
            GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3); 
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Lab08
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab08 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        //************************************************ TEMPLATE ************************************************//
        SpriteFont font;
        Effect effect;  
        Matrix world = Matrix.Identity;  
        Matrix view = Matrix.CreateLookAt(
                new Vector3(0, 0, 20),
                new Vector3(0, 0, 0),
                Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),
                800f / 600f,
                0.1f,
                100f);   
        Vector3 cameraPosition, cameraTarget, lightPosition;  
        Matrix lightView;
        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 20;
        MouseState preMouse;
        Model model;
        Texture2D texture;
        //**********************************************************************************************************//


        //------------------------------------------------- LAB08 --------------------------------------------------//
        Effect projectiveTexture;
        Matrix lightProjection;
        int currentTechnique = 0;
        Color currentColor = Color.CornflowerBlue;
        //----------------------------------------------------------------------------------------------------------//


        public Lab08()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //************************************************ TEMPLATE ************************************************//
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //**********************************************************************************************************//
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


        protected override void LoadContent()
        {
            //************************************************ TEMPLATE ************************************************//
            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("Plane");
            effect = Content.Load<Effect>("ProjectiveTexture");
            texture = Content.Load<Texture2D>("nvlobby_new_negz");
            //**********************************************************************************************************//

            //------------------------------------------------- LAB08 --------------------------------------------------//
            projectiveTexture = Content.Load<Effect>("ProjectiveTexture");
            //----------------------------------------------------------------------------------------------------------//
        }


        protected override void Update(GameTime gameTime)
        {
            //************************************************ TEMPLATE ************************************************//
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
            //lightView = Matrix.CreateLookAt(lightPosition, Vector3.Zero, Vector3.UnitY);
            //**********************************************************************************************************//

            //------------------------------------------------- LAB08 --------------------------------------------------//
            lightView = Matrix.CreateLookAt(lightPosition, Vector3.Zero,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL)));
            lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 1f, 50f);
            if (Keyboard.GetState().IsKeyDown(Keys.F1)) { currentTechnique = 0; currentColor = Color.CornflowerBlue; }
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) { currentTechnique = 1; currentColor = Color.Black; } 
            //----------------------------------------------------------------------------------------------------------//

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(currentColor);

            //************************************************ TEMPLATE ************************************************//
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            effect.CurrentTechnique = effect.Techniques[currentTechnique];
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

                        //------------------------------------------------- LAB08 --------------------------------------------------//
                        effect.Parameters["ProjectiveTexture"].SetValue(texture);
                        effect.Parameters["LightViewMatrix"].SetValue(lightView);
                        effect.Parameters["LightProjectionMatrix"].SetValue(lightProjection);
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        //----------------------------------------------------------------------------------------------------------//

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
            //**********************************************************************************************************//
            
            base.Draw(gameTime);
        }
    }
}
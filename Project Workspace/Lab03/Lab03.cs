using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab03
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab03 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model;
        Effect effect;

        Matrix world, view, projection;

        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        float diffuseIntensity = 1.0f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 diffuseLightDirection = new Vector3(1, 1, 1);

        // PART D
        //float distance = 15f;

        // PART D
        float angle, angle2;
        MouseState previousMouseState;


        public Lab03()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //*******************************************************
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //*******************************************************
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
            model = Content.Load<Model>("bunny");
            effect = Content.Load<Effect>("Diffuse");
            world = Matrix.Identity;
            view = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.UnitY);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
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

            // PART D
            /*if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle += 0.01f;
            }*/

            // PART D
            /*Vector3 cameraPosition = distance * new Vector3((float)System.Math.Sin(angle), 0, (float)System.Math.Cos(angle));
            view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);
            effect.Parameters["View"].SetValue(view);*/

            // PART D
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                MouseState currentMouseState = Mouse.GetState();

                if (previousMouseState != currentMouseState)
                {
                    //angle += 0.01f;
                    //angle2 += 0.01f;

                    int currentX = currentMouseState.X;
                    int currentY = currentMouseState.Y;
                    int previousX = previousMouseState.X;
                    int previousY = previousMouseState.Y;

                    angle = angle + (previousX - currentX) * 0.01f;
                    angle2 = angle2 + (previousY - currentY) * 0.01f;
                }
            }
            
            // PART D
            Vector3 camera = Vector3.Transform(new Vector3(0, 0, 20), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)); 
            view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);

            // PART D
            previousMouseState = Mouse.GetState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // PART B
            //********** Step1
            //model.Draw(world, view, projection);

            // PART B
            //********** Step2
            /*foreach (ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }*/

            // PART C
            effect.CurrentTechnique = effect.Techniques[0];
            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach(ModelMesh mesh in model.Meshes)
                {
                    foreach(ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform); 
                        effect.Parameters["View"].SetValue(view); 
                        effect.Parameters["Projection"].SetValue(projection); 
                        effect.Parameters["AmbientColor"].SetValue(ambient); 
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseLightDirection"].SetValue(diffuseLightDirection);
                        Matrix worldInverseTranspose = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform)); 
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);

                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount);
                    }
                }
            }

            base.Draw(gameTime);
        }
    }
}

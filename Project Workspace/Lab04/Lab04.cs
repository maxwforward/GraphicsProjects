using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using System;

namespace Lab04
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab04 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // USED IN ASSIGNMENT 1
        //#################################################################################################################################
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        float angle = 0;
        float angle2 = 0;
        float distance = 10;
        MouseState previousMouseState;
        //float lightAngle = 0;      
        //float lightAngle2 = 0;
        Vector3 lightPosition = new Vector3(1, 1, 1);
        Model model;
        Effect effect;
        int technique = 0; // flag for Shader's Technique ID
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 600f, 0.1f, 100f);
        float shininess = 10.0f;
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        //#################################################################################################################################


        //SpriteFont font;  ***************** NOT USED YET *****************

        // 3D Model and shader
        //Model model;
        //Effect effect;
        //Texture2D texture;  ***************** NOT USED YET *****************

        // 3D Matrix
        Matrix world = Matrix.CreateTranslation(0, 0, 0); 
        //Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        //Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 600f, 0.1f, 100f);

        // 3D camera & light        
        Vector3 cameraOffset = new Vector3(0, 0, 0);        
        //Vector3 cameraPosition = new Vector3(0, 0, 10);    
        //Vector3 lightPosition = new Vector3(1, 1, 1);
        //float lightAngle = 0;       
        //float lightAngle2 = 0;   
        //float angle = 0;
        //float angle2 = 0;     
        //float distance = 20;

        // object matrials        
        //Vector4 ambient = new Vector4(0, 0, 0, 0);        
        //Vector4 diffuseColor = new Vector4(1, 1, 1, 1);                
        //Vector4 specularColor = new Vector4(1, 1, 1, 1);        
        //float shininess = 10.0f;        
        
        // Mouse & Key Event        
        //MouseState previousMouseState;
        //int previousScrollValue;  ***************** NOT USED YET *****************
        //int technique = 0; // flag for Shader's Technique ID


        public Lab04()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // From Lab03 (Always need this statement)
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

            // From Lab03
            model = Content.Load<Model>("Torus"); // Replaced "bunny" with "Torus"
            effect = Content.Load<Effect>("SimpleShader"); // Replaced "Diffuse" with "SimpleShader"
            //font = Content.Load<SpriteFont>("font");
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


            // *** Lab4 : Change the shader            
            if (Keyboard.GetState().IsKeyDown(Keys.D0)) technique = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) technique = 1;



            // USED IN ASSIGNMENT 1
            //#############################################################################################################################
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angle -= (previousMouseState.X - currentMouseState.X) / 100.0f;
                angle2 -= (previousMouseState.Y - currentMouseState.Y) / 100.0f;
            }
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                float offsetY = (previousMouseState.Y - currentMouseState.Y) / 100.0f;
                distance -= offsetY;
            }
            if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                cameraPosition.X -= (previousMouseState.X - currentMouseState.X) / 100f;
                cameraPosition.Y += (previousMouseState.Y - currentMouseState.Y) / 100f;
            }
            view = Matrix.CreateRotationY(angle) * Matrix.CreateRotationX(angle2) * Matrix.CreateTranslation(new Vector3(cameraPosition.X, 
                cameraPosition.Y, -distance));
            previousMouseState = currentMouseState;
            base.Update(gameTime);
            //#############################################################################################################################

            

            /*
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)            
            {                
                angle -= (previousMouseState.X - currentMouseState.X) / 100.0f;                
                angle2 -= (previousMouseState.Y - currentMouseState.Y) / 100.0f;
                //Console.WriteLine("Left");
            }   
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                float offsetY = (previousMouseState.Y - currentMouseState.Y) / 100.0f;
                distance -= offsetY;
                //Console.WriteLine("Right");
            }
            if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                cameraPosition.X -= (previousMouseState.X - currentMouseState.X) / 100f;
                cameraPosition.Y += (previousMouseState.Y - currentMouseState.Y) / 100f;
                //Console.WriteLine("Middle");
            }
            view = Matrix.CreateRotationY(angle) * Matrix.CreateRotationX(angle2) * Matrix.CreateTranslation(new Vector3(cameraPosition.X, 
                cameraPosition.Y, -distance));
            previousMouseState = currentMouseState
            base.Update(gameTime);
            */



            /*
            //*********************************************************************************** NOT USED YET
            if (currentMouseState.ScrollWheelValue < previousScrollValue)
            {
                shininess -= 10.0f;
            }
            else if (currentMouseState.ScrollWheelValue > previousScrollValue)
            {
                shininess += 10.0f;
            }
            previousScrollValue = currentMouseState.ScrollWheelValue;
            //*********************************************************************************** NOT USED YET
            */



            // NEVER USED
            //-----------------------------------------------------------------------------------------------------------------------------
            //cameraPosition = Vector3.Transform(new Vector3(0, 0, distance), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            //view = Matrix.CreateRotationY(angle) * Matrix.CreateRotationX(angle2) * Matrix.CreateTranslation(new Vector3(0, 0, -distance));           
            //view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            //cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            //view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            //-----------------------------------------------------------------------------------------------------------------------------
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            GraphicsDevice.BlendState = BlendState.Opaque; 
            GraphicsDevice.DepthStencilState = new DepthStencilState(); 
            
            effect.CurrentTechnique = effect.Techniques[technique]; 
            
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
                        effect.Parameters["LightPosition"].SetValue(lightPosition); 
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition); 
                        effect.Parameters["Shininess"].SetValue(shininess); 
                        effect.Parameters["AmbientColor"].SetValue(ambient); 
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor); 
                        effect.Parameters["SpecularColor"].SetValue(specularColor); 
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
            
            // *** Lab3 Option: 2D Text Drawing            
            //spriteBatch.Begin();            
            //spriteBatch.DrawString(font, "angle:" + angle, Vector2.UnitX + Vector2.UnitY * 12, Color.White);            
            //spriteBatch.End();            
            // ************************            
            
            base.Draw(gameTime);
        }
    }
}
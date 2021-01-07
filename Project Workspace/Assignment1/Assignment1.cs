using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Assignment1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        // Global Variables
        MouseState previousMouseState; // The previous state of the mouse
        float cameraHorizontalAngle = 0; // The horizontal angle of the camera
        float cameraVerticalAngle = 0; // The vertical angle of the camera
        float distance = 3; // The distance from the camera to the center
        Vector3 cameraPosition = new Vector3(0, 0, 3); // *** ADD COMMENTS ***
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), Vector3.UnitY); // *** ADD COMMENTS ***
        float lightHorizontalAngle = 0; // *** ADD COMMENTS ***
        float lightVerticalAngle = 0; // *** ADD COMMENTS ***
        Vector3 lightPosition = new Vector3(1, 1, 1); // *** ADD COMMENTS ***
        Model boxModel; // *** ADD COMMENTS ***
        Model sphereModel; // *** ADD COMMENTS ***
        Model torusModel; // *** ADD COMMENTS ***
        Model teapotModel; // *** ADD COMMENTS ***
        Model bunnyModel; // *** ADD COMMENTS ***
        Model currentModel; // *** ADD COMMENTS ***
        Effect effect; // *** ADD COMMENTS ***
        int technique = 0; // *** ADD COMMENTS ***


        // **************************************** TESTS - DELETE/MODIFY LATER ****************************************
        //int technique = 0; // flag for Shader's Technique ID
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 600f, 0.1f, 100f);
        float shininess = 10.0f;
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float lightIntensity = 0.95f;
        float specularIntensity = 0.95f;
        SpriteFont font;
        string help = "";
        string info = "";
        // *************************************************************************************************************


        public Assignment1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            // Set Graphics Profile to HiDef
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


        //=================================================================================================================================
        // LOAD CONTENT METHOD - Called once per game to load all of the game content
        //=================================================================================================================================
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //-----------------------------------------------------------------------------------------------------------------------------
            //  PART B - Load 3D objects
            //-----------------------------------------------------------------------------------------------------------------------------
            boxModel = Content.Load<Model>("Box"); // *** ADD COMMENTS ***
            sphereModel = Content.Load<Model>("Sphere"); // *** ADD COMMENTS ***
            torusModel = Content.Load<Model>("Torus"); // *** ADD COMMENTS ***
            teapotModel = Content.Load<Model>("Teapot"); // *** ADD COMMENTS ***
            bunnyModel = Content.Load<Model>("Bunny"); // *** ADD COMMENTS ***

            // *** ADD COMMENTS ***
            currentModel = boxModel;

            // *** ADD COMMENTS ***
            effect = Content.Load<Effect>("SimpleShading");

            //******************************************************************
            font = Content.Load<SpriteFont>("Text");
            //******************************************************************
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        //=================================================================================================================================
        //  UPDATE METHOD - Contains logic for updating the world and gathering input from the player
        //=================================================================================================================================
        protected override void Update(GameTime gameTime)
        {
            // If "Esc" key is pressed (or "Back" button on GamePad), exit the game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit(); // Exit game
            }

            // Get current state of the mouse
            MouseState currentMouseState = Mouse.GetState();

            //-----------------------------------------------------------------------------------------------------------------------------
            //  PART A - Rotate the camera with Mouse Left Drag
            //-----------------------------------------------------------------------------------------------------------------------------
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                // Calculate the horizontal/vertical translation of mouse coordinates when dragged
                float horizontalTranslation = ((previousMouseState.X - currentMouseState.X) / 100.0f); // *** ADD COMMENTS ***
                float verticalTranslation = ((previousMouseState.Y - currentMouseState.Y) / 100.0f); // *** ADD COMMENTS ***

                // Use horizontal/vertical translation of mouse coordinates to rotate the camera
                cameraHorizontalAngle -= horizontalTranslation;
                cameraVerticalAngle -= verticalTranslation;
            }

            //-----------------------------------------------------------------------------------------------------------------------------
            //  PART A - Change the distance of camera to the center with Mouse Right Drag
            //-----------------------------------------------------------------------------------------------------------------------------
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                // Calculate the vertical translation of mouse coordinates when dragged
                float verticalTranslation = ((previousMouseState.Y - currentMouseState.Y) / 100.0f); // *** ADD COMMENTS ***

                // Use vertical translation of mouse coordinates to change the distance of the camera to the center
                distance -= verticalTranslation;
            }

            //-----------------------------------------------------------------------------------------------------------------------------
            //  PART A - Translate the camera with Mouse Middle Drag
            //-----------------------------------------------------------------------------------------------------------------------------
            if (currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                // *** ADD COMMENTS ***
                cameraPosition.X -= (previousMouseState.X - currentMouseState.X) / 100.0f; // *** ADD COMMENTS ***
                cameraPosition.Y += (previousMouseState.Y - currentMouseState.Y) / 100.0f; // *** ADD COMMENTS ***
            }

            //-----------------------------------------------------------------------------------------------------------------------------
            //  PART A - Rotate the light with Arrow Keys
            //-----------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) // If Left Arrow Key is down
            {
                // *** ADD COMMENTS ***
                lightHorizontalAngle -= 0.02f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) // If Right Arrow Key is down
            {
                // *** ADD COMMENTS ***
                lightHorizontalAngle += 0.02f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) // If Up Arrow Key is down
            {
                // *** ADD COMMENTS ***
                lightVerticalAngle -= 0.02f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) // If Down Arrow Key is down
            {
                // *** ADD COMMENTS ***
                lightVerticalAngle += 0.02f;
            }

            //-----------------------------------------------------------------------------------------------------------------------------
            //  PART A - Reset the camera and light as default with "S" Key
            //-----------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.S)) // If "S" Key is pressed
            {
                // *** ADD COMMENTS ***
                cameraHorizontalAngle = 0;
                cameraVerticalAngle = 0;
                distance = 3;
                cameraPosition.X = 0;
                cameraPosition.Y = 0;
                lightHorizontalAngle = 0;
                lightVerticalAngle = 0;
            }

            //-----------------------------------------------------------------------------------------------------------------------------
            //  PART B - Switch the 3D Object displayed
            //-----------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) currentModel = boxModel; // If "1" Key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) currentModel = sphereModel; // If "2" Key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) currentModel = torusModel; // If "3" Key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) currentModel = teapotModel; // If "4" Key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) currentModel = bunnyModel; // If "5" Key is pressed

            //-----------------------------------------------------------------------------------------------------------------------------
            //  PART C - Change the Shader Effect
            //-----------------------------------------------------------------------------------------------------------------------------
            if (Keyboard.GetState().IsKeyDown(Keys.F1)) technique = 0; // If "F1" Key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) technique = 1; // If "F2" Key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.F3)) technique = 2; // If "F3" Key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.F4)) technique = 3; // If "F4" Key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.F5)) technique = 4; // If "F5" Key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.F6)) technique = 5; // If "F6" Key is pressed

            // **************************************** TESTS - DELETE/MODIFY LATER ****************************************
            if ( (Keyboard.GetState().IsKeyDown(Keys.L)) && // Light Intensity +
                !( (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift)) ) ) 
            {
                lightIntensity += 0.01f;
            }
            if ( (Keyboard.GetState().IsKeyDown(Keys.L)) && // Light Intensity -
                ( (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift)) ) )
            {
                lightIntensity -= 0.01f;
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.R)) && // Red Value +
                !((Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift))))
            {
                ambient.X += 0.01f;
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.R)) && // Red Value -
                ((Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift))))
            {
                ambient.X -= 0.01f;
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.G)) && // Green Value +
                !((Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift))))
            {
                ambient.Y += 0.01f;
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.G)) && // Green Value -
                ((Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift))))
            {
                ambient.Y -= 0.01f;
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.B)) && // Blue Value +
                !((Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift))))
            {
                ambient.Z += 0.01f;
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.B)) && // Blue Value -
                ((Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift))))
            {
                ambient.Z -= 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus)) specularIntensity += 0.01f; // Specular +
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) specularIntensity -= 0.01f; // Specular -
            if ( (Keyboard.GetState().IsKeyDown(Keys.OemPlus)) && (Keyboard.GetState().IsKeyDown(Keys.LeftControl)) ) // Shininess +
            {
                shininess += 0.01f;
            }
            if ( (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) && (Keyboard.GetState().IsKeyDown(Keys.LeftControl)) ) // Shininess -
            {
                shininess -= 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion)) // Help
            {
                help = "Rotate camera: Mouse Left Drag" + "\n"
                    + "Change the distance of camera: Mouse Right Drag" + "\n"
                    + "Translate the camera: Mouse Middle Drag" + "\n"
                    + "Rotate the light: Arrow keys" + "\n"
                    + "Reset camera and light: S Key" + "\n"
                    + "Display Box: 1 Key" + "\n"
                    + "Display Sphere: 2 Key" + "\n"
                    + "Display Torus: 3 Key" + "\n"
                    + "Display Teapot: 4 Key" + "\n"
                    + "Display Bunny: 5 Key" + "\n"
                    + "Gouraud (Phong per vertex) Shading: F1 Key" + "\n"
                    + "Phong per pixel Shading: F2 Key" + "\n"
                    + "PhongBlinn  Shading: F3 Key" + "\n"
                    + "Schlick Shading: F4 Key" + "\n"
                    + "Toon Shading: F5 Key" + "\n"
                    + "HalfLife Shading: F6 Key" + "\n"
                    + "Increase the intensity of light: L Key" + "\n"
                    + "Increase the red value of light: R Key" + "\n"
                    + "Increase the green value of light: G Key" + "\n"
                    + "Increase the blue value of light: B Key" + "\n"
                    + "Increase specular: + Key" + "\n"
                    + "Decrease specular: - Key" + "\n";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.H)) // Info
            {
                info += 0.01f;
            }
            // *************************************************************************************************************

            // *** ADD COMMENTS ***
            view = (Matrix.CreateRotationY(cameraHorizontalAngle) 
                * Matrix.CreateRotationX(cameraVerticalAngle) 
                * Matrix.CreateTranslation(new Vector3(cameraPosition.X, cameraPosition.Y, -(distance))));

            // *** ADD COMMENTS ***
            lightPosition = Vector3.Transform(new Vector3(1, 1, 1), 
                Matrix.CreateRotationX(lightVerticalAngle) * Matrix.CreateRotationY(lightHorizontalAngle));

            // *** ADD COMMENTS ***
            previousMouseState = currentMouseState;

            // *** ADD COMMENTS ***
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

            // **************************************** TESTS - DELETE/MODIFY LATER ****************************************
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            effect.CurrentTechnique = effect.Techniques[technique];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in currentModel.Meshes)
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
                        effect.Parameters["LightIntensity"].SetValue(lightIntensity);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
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
            spriteBatch.Begin();            
            spriteBatch.DrawString(font, help, (Vector2.UnitX * 5) + Vector2.UnitY * 12, Color.White);            
            spriteBatch.End();
            // *************************************************************************************************************

            base.Draw(gameTime);
        }
    }
}
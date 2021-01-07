using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//-----------------------------------------------------------------------------------------------------------------------------------------
//  FROM ASSIGNMENT2
//-----------------------------------------------------------------------------------------------------------------------------------------
using CPI411.SimpleEngine;


namespace Assignment3
{
    //#####################################################################################################################################
    //  This is the main type for your game.
    //#####################################################################################################################################
    public class Assignment3 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        //*********************************************************************************************************************************
        //  FROM TEMPLATE
        //*********************************************************************************************************************************
        SpriteFont font;
        Effect effect;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 20), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 20;
        MouseState preMouse;
        Model model;


        //---------------------------------------------------------------------------------------------------------------------------------
        //  FROM ASSIGNMENT2
        //---------------------------------------------------------------------------------------------------------------------------------
        Skybox skybox;
        KeyboardState previousKeyboardState;
        int technique = 0;
        bool showMenu = false;
        bool showValues = false;


        //=================================================================================================================================
        //  ASSIGNMENT3
        //=================================================================================================================================
        Texture2D art, bumpTest, crossHatch, monkey, round, saint, science, square; // PART B
        TextureCube nm;                                                             // PART B
        Texture texture;                                                            // PART B
        float bumpHeight = 1.0f;                                                    // PART C
        float uScale = 1.0f;                                                        // PART C
        float vScale = 1.0f;                                                        // PART C
        int mipMap = 0;                                                             // PART C


        public Assignment3()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //*****************************************************************************************************************************
            //  FROM TEMPLATE
            //*****************************************************************************************************************************
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }


        //#################################################################################################################################        
        //  Allows the game to perform any initialization it needs to before starting to run.
        //  This is where it can query for any required services and load any non-graphic related content.
        //  Calling base.Initialize will enumerate through any components and initialize them as well.
        //#################################################################################################################################
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }


        //#################################################################################################################################
        //  LoadContent will be called once per game and is the place to load all of your content.
        //#################################################################################################################################
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //*****************************************************************************************************************************
            //  FROM TEMPLATE
            //*****************************************************************************************************************************
            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("Torus");

            //=============================================================================================================================
            //  ASSIGNMENT3
            //=============================================================================================================================
            art = Content.Load<Texture2D>("NormalMaps/art");                // PART B
            bumpTest = Content.Load<Texture2D>("NormalMaps/bumpTest");      // PART B
            crossHatch = Content.Load<Texture2D>("NormalMaps/crossHatch");  // PART B
            monkey = Content.Load<Texture2D>("NormalMaps/monkey");          // PART B
            round = Content.Load<Texture2D>("NormalMaps/round");            // PART B
            saint = Content.Load<Texture2D>("NormalMaps/saint");            // PART B
            science = Content.Load<Texture2D>("NormalMaps/science");        // PART B
            square = Content.Load<Texture2D>("NormalMaps/square");          // PART B
            nm = Content.Load<TextureCube>("NormalMaps/nm");                // PART B
            texture = round;                                                // PART B
            effect = Content.Load<Effect>("BumpMapping");                   // PART C

            //-----------------------------------------------------------------------------------------------------------------------------
            //  FROM ASSIGNMENT2
            //-----------------------------------------------------------------------------------------------------------------------------
            string[] environmentMap =
            {
                "skybox/nvlobby_new_negx", "skybox/nvlobby_new_posx",
                "skybox/nvlobby_new_negy", "skybox/nvlobby_new_posy",
                "skybox/nvlobby_new_negz", "skybox/nvlobby_new_posz"
            };
            skybox = new Skybox(environmentMap, Content, GraphicsDevice, 512);
        }


        //#################################################################################################################################
        //  UnloadContent will be called once per game and is the place to unload game-specific content.
        //#################################################################################################################################
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        //#################################################################################################################################
        //  Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio.
        //#################################################################################################################################
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //*****************************************************************************************************************************
            //  FROM TEMPLATE
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
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f; // Rotate the light: Arrow keys
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f; // Rotate the light: Arrow keys
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f; // Rotate the light: Arrow keys
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f; // Rotate the light: Arrow keys
            if (Keyboard.GetState().IsKeyDown(Keys.S)) // Reset camera and light: S Key
            {
                angle = angle2 = angleL = angleL2 = 0;
                distance = 20;
                cameraTarget = Vector3.Zero;
            }
            preMouse = Mouse.GetState();
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            lightPosition = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));

            //=============================================================================================================================
            //  ASSIGNMENT3
            //=============================================================================================================================
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) texture = round;        // PART B
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) texture = bumpTest;     // PART B
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) texture = square;       // PART B
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) texture = crossHatch;   // PART B
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) texture = saint;        // PART B
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) texture = monkey;       // PART B
            if (Keyboard.GetState().IsKeyDown(Keys.D7)) texture = art;          // PART B
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) texture = science;      // PART B
            if (Keyboard.GetState().IsKeyDown(Keys.D9)) texture = nm;           // PART B
            if (Keyboard.GetState().IsKeyDown(Keys.F1)) technique = 0;          // PART C
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) technique = 1;          // PART C
            if (Keyboard.GetState().IsKeyDown(Keys.F3)) technique = 2;          // PART C
            if (Keyboard.GetState().IsKeyDown(Keys.F4)) technique = 3;          // PART C
            if (Keyboard.GetState().IsKeyDown(Keys.F5)) technique = 4;          // PART C

            //-----------------------------------------------------------------------------------------------------------------------------
            //  FROM ASSIGNMENT2
            //-----------------------------------------------------------------------------------------------------------------------------
            KeyboardState currentKeyboardState = Keyboard.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H)) showValues = !showValues;
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKeyboardState.IsKeyDown(Keys.OemQuestion)) showMenu = !showMenu;
            previousKeyboardState = currentKeyboardState;
            bool shift = false;
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in pressedKeys) if (key == Keys.LeftShift || key == Keys.RightShift) shift = true;

            //=============================================================================================================================
            //  ASSIGNMENT3
            //=============================================================================================================================
            if (Keyboard.GetState().IsKeyDown(Keys.M))                                  // PART C
            {
                if (mipMap == 0) mipMap = 1;
                else if (mipMap == 1) mipMap = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.U) && shift) uScale -= 0.01f;        // PART C
            if (Keyboard.GetState().IsKeyDown(Keys.U) && !shift) uScale += 0.01f;       // PART C
            if (Keyboard.GetState().IsKeyDown(Keys.V) && shift) vScale -= 0.01f;        // PART C
            if (Keyboard.GetState().IsKeyDown(Keys.V) && !shift) vScale += 0.01f;       // PART C
            if (Keyboard.GetState().IsKeyDown(Keys.W) && shift) bumpHeight -= 0.01f;    // PART C
            if (Keyboard.GetState().IsKeyDown(Keys.W) && !shift) bumpHeight += 0.01f;   // PART C

            base.Update(gameTime);
        }


        //#################################################################################################################################
        //  This is called when the game should draw itself.
        //#################################################################################################################################
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            //*****************************************************************************************************************************
            //  FROM TEMPLATE
            //*****************************************************************************************************************************
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //-----------------------------------------------------------------------------------------------------------------------------
            //  FROM ASSIGNMENT2
            //-----------------------------------------------------------------------------------------------------------------------------            
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState ras = new RasterizerState();
            ras.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = ras;
            skybox.Draw(view, projection, cameraPosition);
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
            DrawModelWithEffect();
            spriteBatch.Begin();
            if (showMenu) ShowControls();
            if (showValues) ShowInfo();
            spriteBatch.End();

            base.Draw(gameTime);
        }


        //---------------------------------------------------------------------------------------------------------------------------------
        //  FROM ASSIGNMENT2
        //---------------------------------------------------------------------------------------------------------------------------------
        private void DrawModelWithEffect()
        {
            //*****************************************************************************************************************************
            //  FROM TEMPLATE
            //*****************************************************************************************************************************
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
                        effect.Parameters["CameraPosition"].SetValue(cameraPosition);

                        //-----------------------------------------------------------------------------------------------------------------
                        //  FROM LAB07
                        //-----------------------------------------------------------------------------------------------------------------
                        effect.Parameters["LightPosition"].SetValue(lightPosition);
                        effect.Parameters["DiffuseIntensity"].SetValue(1.0f); 
                        effect.Parameters["DiffuseColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                        effect.Parameters["SpecularIntensity"].SetValue(1.0f); 
                        effect.Parameters["SpecularColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)); 
                        effect.Parameters["Shininess"].SetValue(100.0f);
                        effect.Parameters["NormalMap"].SetValue(texture);

                        //-----------------------------------------------------------------------------------------------------------------
                        //  FROM ASSIGNMENT2
                        //-----------------------------------------------------------------------------------------------------------------
                        effect.Parameters["EnvironmentMap"].SetValue(skybox.skyBoxTexture);

                        //=================================================================================================================
                        //  ASSIGNMENT3
                        //=================================================================================================================
                        effect.Parameters["NormalMapRepeatU"].SetValue(uScale);
                        effect.Parameters["NormalMapRepeatV"].SetValue(vScale);
                        effect.Parameters["BumpHeight"].SetValue(bumpHeight);
                        effect.Parameters["MipMap"].SetValue(mipMap);

                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                            part.VertexOffset, part.StartIndex, part.PrimitiveCount);
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
            spriteBatch.DrawString(font, "Change textures: 1-9 Keys",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Shader: F1 - F5 Keys",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Mip Mapping: M Key",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Tiling (Scale): U,V Keys (+SHIFT Key: decrease)",
                (Vector2.UnitX * 400 + Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Change Bump Height: W Key (+SHIFT Key: decrease)",
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
            spriteBatch.DrawString(font, "Normal Map Repeat U: " + uScale,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Normal Map Repeat V: " + vScale,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Mip Mapping: " + mipMap,
                (Vector2.UnitY * 16 * (i++)), Color.White);
            spriteBatch.DrawString(font, "Bump Height: " + bumpHeight,
                (Vector2.UnitY * 16 * (i++)), Color.White);
        }
    }
}
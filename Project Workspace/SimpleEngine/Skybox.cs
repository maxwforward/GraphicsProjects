using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CPI411.SimpleEngine
{
    public class Skybox
    {
        private Model skyBox;
        public TextureCube skyBoxTexture;
        private Effect skyBoxEffect;
        private float size = 50f;

        public Skybox(string[] skyboxTextures, ContentManager Content, GraphicsDevice g, int imageSize = 512)
        {
            skyBox = Content.Load<Model>("skybox/cube");
            skyBoxTexture = new TextureCube(g, imageSize, false, SurfaceFormat.Color);
            byte[] data = new byte[imageSize * imageSize * 4];
            Texture2D tempTexture = Content.Load<Texture2D>(skyboxTextures[0]);
            tempTexture.GetData<byte>(data);
            skyBoxTexture.SetData<byte>(CubeMapFace.NegativeX, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[1]);
            tempTexture.GetData<byte>(data);
            skyBoxTexture.SetData<byte>(CubeMapFace.PositiveX, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[2]);
            tempTexture.GetData<byte>(data);
            skyBoxTexture.SetData<byte>(CubeMapFace.NegativeY, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[3]);
            tempTexture.GetData<byte>(data);
            skyBoxTexture.SetData<byte>(CubeMapFace.PositiveY, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[4]);
            tempTexture.GetData<byte>(data);
            skyBoxTexture.SetData<byte>(CubeMapFace.NegativeZ, data);

            tempTexture = Content.Load<Texture2D>(skyboxTextures[5]);
            tempTexture.GetData<byte>(data);
            skyBoxTexture.SetData<byte>(CubeMapFace.PositiveZ, data);

            skyBoxEffect = Content.Load<Effect>("skybox/Skybox");
        }

        public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition) 
        { 
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes) 
            { 
                foreach (ModelMesh mesh in skyBox.Meshes) 
                { 
                    foreach (ModelMeshPart part in mesh.MeshParts) 
                    { 
                        part.Effect = skyBoxEffect; 
                        part.Effect.Parameters["View"].SetValue(view);
                        part.Effect.Parameters["Projection"].SetValue(projection);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                        part.Effect.Parameters["World"].SetValue(Matrix.CreateScale(size) * Matrix.CreateTranslation(cameraPosition));
                    } 
                    
                    mesh.Draw(); 
                } 
            } 
        }
    }
}
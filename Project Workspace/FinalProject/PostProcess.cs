//############################################################################################################################################
//  Final Project - Volumetric Light Scattering as a Post-Process
//  PostProcess.cs
//  CPI 411 - Graphics for Games
//  Max Forward
//############################################################################################################################################


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//  FROM ONLINE TUTORIAL
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
namespace PostProcess
{
    public class PostProcessEffects
    {
        private Effect linearFilterEffect;
        private Effect lightShafts;
        private int width;
        private int height;
        private GraphicsDevice device;
        private SpriteBatch spriteBatch;

        //====================================================================================================================================
        //  PostProcessEffects Constructor
        //====================================================================================================================================
        public PostProcessEffects(int Width, int Height, GraphicsDevice Device, ContentManager Content)
        {
            device = Device;
            spriteBatch = new SpriteBatch(device);
            width = Width;
            height = Height;
            linearFilterEffect = Content.Load<Effect>("LinearFilter");
            lightShafts = Content.Load<Effect>("LightShafts");
        }
        //====================================================================================================================================
        //  linearFilter Method
        //====================================================================================================================================
        public void linearFilter(RenderTarget2D RenderTargetSource,RenderTarget2D RenderTargetDestination)
        {
            Vector2 textureSize = new Vector2(width/2, height/2);
            Rectangle rectangle = new Rectangle(0, 0, (int)width / 2, (int)height / 2);
            Effect effect = linearFilterEffect;
            device.SetRenderTarget(RenderTargetDestination);
            device.Clear(Color.Black);
            effect.CurrentTechnique = effect.Techniques[0];
            effect.Parameters["TextureSize"].SetValue(textureSize);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            effect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(RenderTargetSource, rectangle, Color.White);
            spriteBatch.End();
            device.SetRenderTarget(null);
        }
        //====================================================================================================================================
        //  LightShafts Method
        //====================================================================================================================================
        public void LightShafts(RenderTarget2D RenderTargetMask, RenderTarget2D Destination, Vector3 LightPosition, float Density, 
            float Decay, float Weight, float Exposure)
        {
            device.SetRenderTarget(Destination);
            device.Clear(ClearOptions.Target, Vector4.Zero, 1, 0);
            Effect effect = lightShafts;
            effect.CurrentTechnique = effect.Techniques[0];
            effect.Parameters["LightPosition"].SetValue(LightPosition);
            effect.Parameters["Density"].SetValue(Density);
            effect.Parameters["Decay"].SetValue(Decay);
            effect.Parameters["Weight"].SetValue(Weight);
            effect.Parameters["Exposure"].SetValue(Exposure);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                pass.Apply();
                spriteBatch.Draw(RenderTargetMask, Vector2.Zero, Color.White);
                spriteBatch.End();
            }
            device.SetRenderTarget(null);
        }
        //====================================================================================================================================
        //  HalfToFullscreen Method
        //====================================================================================================================================
        public void HalfToFullscreen(RenderTarget2D Source,RenderTarget2D Destination)
        {
            Rectangle screenSize = new Rectangle(0, 0, (int)width, (int)height);
            device.SetRenderTarget(Destination);
            spriteBatch.Begin();
            spriteBatch.Draw(Source, screenSize, Color.White);
            spriteBatch.End();
            device.SetRenderTarget(null);
        }
    }
}
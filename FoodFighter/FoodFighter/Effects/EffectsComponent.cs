using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Timers;

namespace FoodFighter
{
    public class EffectComponent : DrawableGameComponent
    {
        #region Variables
        SpriteBatch spriteBatch; //for drawing to rendertaget

        Effect colorExtractEffect; //effect used to remove colors over the threshold
        Effect gaussianBlurEffect; //effect used to blur the extracted colors(x first then y)
        Effect colorCombineEffect; //effect used to combine extracted blurred colors with original

        RenderTarget2D sceneRenderTarget; //draws the game scene to this texture in Draw() within Game1
        RenderTarget2D renderTarget1; //draws extracted color here then over writes it on 2nd(y) blur
        RenderTarget2D renderTarget2; //draws 1 pass(x) blur of extracted color here

        #endregion

        public EffectComponent(Game game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice); //sets spritebatch with current graphics device(drawing device)

            colorExtractEffect = Game1.Instance().Content.Load<Effect>("BloomExtract");
            gaussianBlurEffect = Game1.Instance().Content.Load<Effect>("GaussianBlur");
            colorCombineEffect = Game1.Instance().Content.Load<Effect>("BloomCombine");
            //loads all 3 effect files

            PresentationParameters pp = GraphicsDevice.PresentationParameters;//stores the parameters for drawing to comp screen(resolution, sizes ...)???

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            //stores the width and height of the screen to be drawn(1280x720 set in Game1())

            SurfaceFormat format = pp.BackBufferFormat; //localizes the format of the play screen to be draw???

            // Create a texture for rendering the main scene, prior to applying bloom.
            sceneRenderTarget = new RenderTarget2D(GraphicsDevice, width, height, false,
                                                    format, pp.DepthStencilFormat, pp.MultiSampleCount,
                                                    RenderTargetUsage.DiscardContents);
            width /= 2;
            height /= 2;
            //halves the height and width for file reasons

            renderTarget1 = new RenderTarget2D(GraphicsDevice, width, height, false, format, DepthFormat.None);
            renderTarget2 = new RenderTarget2D(GraphicsDevice, width, height, false, format, DepthFormat.None);
            //initializes the two rendertarget files used for storing extracted and blured color at half
            //the size of the original render with lowest EffectController.Instance to cut down on the size to the file since
            //its not actualy used, just holds information while editing the screen image
        }

        protected override void UnloadContent()
        {
            //cleans up before exiting???
            sceneRenderTarget.Dispose();
            renderTarget1.Dispose();
            renderTarget2.Dispose();
        }

        public void BeginDraw()
        {
            //sets the current target render file to this components scene render file
            //this will cause the game to not draw anything to the screne and instead
            //draw into a texture file that can be manipulated
            GraphicsDevice.SetRenderTarget(sceneRenderTarget);
        }

        //post processing happens in here
        //TODO: add in a bool check to keep it from processing either the x or the y blur to produce linear blur shifts
        public override void Draw(GameTime gameTime)
        {
            if (Game1.Instance().EffectsOn)
            {
                GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp; // ????

                #region pass 1
                //pass 1(second actual draw): redraws the screen into the first filler rendertarget
                //fill, skipping all pixels that are not brighter then the threshold mark(contained in settins)

                //sets the threshold mark in the effects file to the one declared in the EffectController.Instance class
                colorExtractEffect.Parameters["BloomThreshold"].SetValue(EffectController.Instance.ColotThreshold);

                //draws pixels that pass the threshold test to rendertarget1
                DrawFullscreenQuad(sceneRenderTarget, renderTarget1, colorExtractEffect);
                #endregion

                #region pass 2
                //pass 2(third actual draw) draws from rendertarget1 to rendertarget2
                //applying a horizontal blur, this make the data within rendertarget1 unnessecary

                //sets the blur effect to x using the rendertargets width
                SetBlurEffectParametersX(0, 1.0f / (float)renderTarget1.Height);

                //draws extracted and horizontaly blurred pixels to rendertarget2
                DrawFullscreenQuad(renderTarget1, renderTarget2, gaussianBlurEffect);

                #endregion

                #region pass 3
                //draws rendertarget2 back into rendertarget1, reusing rendertarget1 and eliminating
                //the extracted pixels data, after applying a vertical blur effect

                //sets the blur effect to y using the rendertarget height
                SetBlurEffectParametersY(1.0f / (float)renderTarget1.Height, 0);

                //draws extracted, horizontaly and verticaly blurred pixel into rendertarget1
                DrawFullscreenQuad(renderTarget2, renderTarget1, gaussianBlurEffect);

                #endregion

                #region pass 4
                //draws both the extracted and altered rendertarget1 texture and the original
                //texture to the screen by using the combining effect file resulting in the desired effect

                //sets the render target to nothing so it will render to the screen
                GraphicsDevice.SetRenderTarget(null);

                //localizes the combination effects parameters so they can be set and accessed
                EffectParameterCollection parameters = colorCombineEffect.Parameters;

                //sets each parameter to the parameters within the EffectController.Instance class
                parameters["BloomIntensity"].SetValue(EffectController.Instance.BloomIntensity);
                parameters["BaseIntensity"].SetValue(EffectController.Instance.BaseIntensity);
                parameters["BloomSaturation"].SetValue(EffectController.Instance.BloomSaturation);
                parameters["BaseSaturation"].SetValue(EffectController.Instance.BaseSaturation);

                //sets the first texture to draw equal to the actual game scene???
                GraphicsDevice.Textures[1] = sceneRenderTarget;

                //localizes the veiwport to gain access to the screen that it will be draw in(window dimensions)
                Viewport viewport = GraphicsDevice.Viewport;

                //skips the intermidiate step of setting the texture to draw to(which keeps it drawing to the screen)
                //and draws the combined original and effected textures(using combine effect) to the screen
                DrawFullscreenQuad(renderTarget1, viewport.Width, viewport.Height, colorCombineEffect);

                #endregion
            }
        }

        //intermediate step for changing the graphics render target before rendering effect
        void DrawFullscreenQuad(Texture2D toRender, RenderTarget2D renderTarget, Effect effect)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);

            DrawFullscreenQuad(toRender, renderTarget.Width, renderTarget.Height, effect);
        }

        //renders teh current effect being processed into the renderTarget
        void DrawFullscreenQuad(Texture2D toRender, int renderWidth, int renderHeight, Effect effect)
        {
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(toRender, new Rectangle(0, 0, renderWidth, renderHeight), Color.White);
            spriteBatch.End();
        }

        #region gaussian stuff i dont understand
        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        void SetBlurEffectParametersX(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter EffectController.Instance.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussianX(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussianX(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter EffectController.Instance.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }
        void SetBlurEffectParametersY(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter EffectController.Instance.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussianY(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussianY(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter EffectController.Instance.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }


        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>]
        float ComputeGaussianX(float n)
        {
            float theta = EffectController.Instance.BlurAmountX;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
        float ComputeGaussianY(float n)
        {
            float theta = EffectController.Instance.BlurAmountY;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
        #endregion
    }
}


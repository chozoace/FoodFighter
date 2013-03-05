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
    class EffectController
    {

        float colorThreshold, blurAmountX, blurAmountY, bloomIntensity, baseIntensity,
            bloomSaturation, baseSaturation, yChange, xChange;
        static EffectController instance;
        bool isX;
        int count = 0;
        EventArgs e = new EventArgs();
        EventHandler handle;

        public static EffectController Instance
        {
            get
            {
                if (instance == null)
                    instance = new EffectController();
                return instance;
            }
        }
        public float ColotThreshold
        {
            get
            {
                return colorThreshold;
            }
        }
        public float BlurAmountX
        {
            get
            {
                return blurAmountX;
            }
        }
        public float BlurAmountY
        {
            get
            {
                return blurAmountY;
            }
        }
        public float BloomIntensity
        {
            get
            {
                return bloomIntensity;
            }
        }
        public float BaseIntensity
        {
            get
            {
                return baseIntensity;
            }
        }
        public float BloomSaturation
        {
            get
            {
                return bloomSaturation;
            }
        }
        public float BaseSaturation
        {
            get
            {
                return baseSaturation;
            }
        }
        public void SetShift()
        {
            handle = new EventHandler(ShiftBlur);
        }
        public void SetShift2()
        {
            blurAmountX = 8;
            blurAmountY = 0.0000001f;
            yChange = 0.1f;
            xChange = -0.1f;
            handle = new EventHandler(CircleBlur);
        }
        public void SetMorph()
        {
            handle = new EventHandler(CircleBlur);
        }
        public void Exit()
        {
            yChange = 0.1f;
            xChange = 0.1f;
            //handle = new EventHandler(BlurOut);
        }
        public void SetNone()
        {
            handle = new EventHandler(Nothing);
            colorThreshold = 0;
            blurAmountX = 0.0000001f;
            blurAmountY = 0.0000001f;
            bloomIntensity = 1;
            baseIntensity = 0;
            bloomSaturation = 2;
            baseSaturation = 1;
            yChange = 0.1f;
            xChange = 0.1f;
            isX = true;
        }

        public EffectController()
        {
            instance = this;
            colorThreshold = 0;
            blurAmountX = 0.00001f;
            blurAmountY = 0.00001f;
            bloomIntensity = 1;
            baseIntensity = 0;
            bloomSaturation = 1;
            baseSaturation = 1;
            yChange = 0.1f;
            xChange = 0.1f;
            isX = true;
            handle = new EventHandler(Nothing);
        }

        public void Update()
        {
            KeyboardState newKeys = Keyboard.GetState();
            #region large increase
            if (newKeys.IsKeyDown(Keys.Q))
            {
                if (newKeys.IsKeyDown(Keys.P))
                {
                    colorThreshold = MathHelper.Clamp(colorThreshold + 0.1f, 0, 0.999999f);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.L))
                {
                    blurAmountX += 0.1f;
                    blurAmountY += 0.1f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.O))
                {
                    bloomIntensity += 0.1f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.K))
                {
                    baseIntensity += 0.1f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.I))
                {
                    bloomSaturation += 0.1f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.J))
                {
                    baseSaturation += 0.1f;
                    WriteLine();
                }
            }
            #endregion
            #region medium increase
            if (newKeys.IsKeyDown(Keys.W))
            {
                if (newKeys.IsKeyDown(Keys.P))
                {
                    colorThreshold = MathHelper.Clamp(colorThreshold + 0.01f, 0, 0.999999f);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.L))
                {
                    blurAmountX += 0.01f;
                    blurAmountY += 0.01f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.O))
                {
                    bloomIntensity += 0.01f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.K))
                {
                    baseIntensity += 0.01f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.I))
                {
                    bloomSaturation += 0.01f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.J))
                {
                    baseSaturation += 0.01f;
                    WriteLine();
                }
            }
            #endregion
            #region small increase
            if (newKeys.IsKeyDown(Keys.E))
            {
                if (newKeys.IsKeyDown(Keys.P))
                {
                    colorThreshold = MathHelper.Clamp(colorThreshold + 0.001f, 0, 0.999999f);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.L))
                {
                    blurAmountX += 0.001f;
                    blurAmountY += 0.001f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.O))
                {
                    bloomIntensity += 0.001f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.K))
                {
                    baseIntensity += 0.001f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.I))
                {
                    bloomSaturation += 0.001f;
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.J))
                {
                    baseSaturation += 0.001f;
                    WriteLine();
                }
            }
            #endregion
            #region large decrease
            if (newKeys.IsKeyDown(Keys.A))
            {
                if (newKeys.IsKeyDown(Keys.P))
                {
                    colorThreshold = MathHelper.Clamp(colorThreshold - 0.1f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.L))
                {
                    blurAmountX = MathHelper.Clamp(blurAmountX - 0.1f, 0, 100);
                    blurAmountY = MathHelper.Clamp(blurAmountY - 0.1f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.O))
                {
                    bloomIntensity = MathHelper.Clamp(bloomIntensity - 0.1f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.K))
                {
                    baseIntensity = MathHelper.Clamp(baseIntensity - 0.1f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.I))
                {
                    bloomSaturation = MathHelper.Clamp(bloomSaturation - 0.1f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.J))
                {
                    baseSaturation = MathHelper.Clamp(baseSaturation - 0.1f, 0, 100);
                    WriteLine();
                }
            }
            #endregion
            #region medium decrease
            if (newKeys.IsKeyDown(Keys.S))
            {
                if (newKeys.IsKeyDown(Keys.P))
                {
                    colorThreshold = MathHelper.Clamp(colorThreshold - 0.01f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.L))
                {
                    blurAmountX = MathHelper.Clamp(blurAmountX - 0.01f, 0, 100);
                    blurAmountY = MathHelper.Clamp(blurAmountY - 0.01f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.O))
                {
                    bloomIntensity = MathHelper.Clamp(bloomIntensity - 0.01f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.K))
                {
                    baseIntensity = MathHelper.Clamp(baseIntensity - 0.01f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.I))
                {
                    bloomSaturation = MathHelper.Clamp(bloomSaturation - 0.01f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.J))
                {
                    baseSaturation = MathHelper.Clamp(baseSaturation - 0.01f, 0, 100);
                    WriteLine();
                }
            }
            #endregion
            #region small decrease
            if (newKeys.IsKeyDown(Keys.D))
            {
                if (newKeys.IsKeyDown(Keys.P))
                {
                    colorThreshold = MathHelper.Clamp(colorThreshold - 0.001f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.L))
                {
                    blurAmountX = MathHelper.Clamp(blurAmountX - 0.001f, 0, 100);
                    blurAmountY = MathHelper.Clamp(blurAmountY - 0.001f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.O))
                {
                    bloomIntensity = MathHelper.Clamp(bloomIntensity - 0.001f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.K))
                {
                    baseIntensity = MathHelper.Clamp(baseIntensity - 0.001f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.I))
                {
                    bloomSaturation = MathHelper.Clamp(bloomSaturation - 0.001f, 0, 100);
                    WriteLine();
                }
                if (newKeys.IsKeyDown(Keys.J))
                {
                    baseSaturation = MathHelper.Clamp(baseSaturation - 0.001f, 0, 100);
                    WriteLine();
                }
            }
            #endregion
            //handle(this, e);
        }
        void Nothing(object obj, EventArgs ev)
        {
        }
        void CircleBlur(object obj, EventArgs ev)
        {
            blurAmountX = MathHelper.Clamp(blurAmountX + xChange, 0.0000001f, 8);
            if (blurAmountX == 8 || blurAmountX == 0.0000001f)
            {
                xChange *= -1;
                isX = false;
            }

            blurAmountY = MathHelper.Clamp(blurAmountY + yChange, 0.0000001f, 8);
            if (blurAmountY == 8 || blurAmountY == 0.0000001f)
            {
                yChange *= -1;
                isX = true;
            }

        }
        void ShiftBlur(object obj, EventArgs ev)
        {
            if (isX)
            {
                blurAmountX = MathHelper.Clamp(blurAmountX + xChange, 0.0000001f, 8);
                if (blurAmountX == 8 || blurAmountX == 0.0000001f)
                {
                    xChange *= -1;
                    isX = false;
                }
            }
            else
            {

                blurAmountY = MathHelper.Clamp(blurAmountY + yChange, 0.0000001f, 8);
                if (blurAmountY == 8 || blurAmountY == 0.0000001f)
                {
                    yChange *= -1;
                    isX = true;
                }
            }

        }

        void BlurOut(object obj, EventArgs ev)
        {
            if (count > 30)
            {
                blurAmountX = MathHelper.Clamp(blurAmountX + xChange, 0.0000001f, 8);
                blurAmountY = MathHelper.Clamp(blurAmountY + yChange, 0.0000001f, 8);
                if (blurAmountX == 8)
                {
                    bloomIntensity = MathHelper.Clamp(bloomIntensity - 0.1f, 0, 8);
                }
            }
            else
                count++;

        }

        void WriteLine()
        {
            Debug.WriteLine("Threshold: " + colorThreshold + "  Blur: " + blurAmountX + "  BloomIntensity: " + bloomIntensity +
                        "BaseIntensity: " + baseIntensity + "  BloomSaturation: " + bloomSaturation + "  BaseSaturation: " + baseSaturation);
        }
    }
}

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
    class Menu
    {
        public bool visible = false;
        protected int currentButton = 0;
        protected Texture2D texture;
        protected String myTexture;
        protected Vector2 position;
        protected KeyboardState myKeyState, previousKeyState;
        protected GamePadState myPadState;
        protected int width;
        protected int height;

        protected Rectangle animationRect;
        public String currentAnimation;
        protected int currentFrame;
        protected int totalFrames;
        protected Timer animTimer = new Timer();

        public virtual void Update()
        {
            myKeyState = Keyboard.GetState();
            myPadState = GamePad.GetState(PlayerIndex.One);
        }

        public virtual void UpdateAnimation(object sender, ElapsedEventArgs e)
        {
            currentFrame = animationRect.X / 640;
            totalFrames = (texture.Width / 640) - 1;

            if (currentFrame >= totalFrames)
            {
                //startover
                //currentFrame = 0;
                animationRect = new Rectangle(0, 0, width, height);
            }
            else
            {
                //continue
                animationRect = new Rectangle((currentFrame + 1) * 640, 0, width, height);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }
    }
}

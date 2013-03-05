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

        public virtual void Update()
        {
            myKeyState = Keyboard.GetState();
            myPadState = GamePad.GetState(PlayerIndex.One);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }
    }
}

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
    class GameOverScreen : Menu
    {
        public GameOverScreen()
        {
            texture = Game1.Instance().Content.Load<Texture2D>("Menus/GoScreen");
            visible = false;
        }

        public override void Update()
        {
            if (visible)
            {
                myKeyState = Keyboard.GetState();
                myPadState = GamePad.GetState(PlayerIndex.One);

                if (Game1.Instance().usingController)
                {
                    checkKeysDown(myPadState);
                    //checkKeysUp(myPadState);
                }
                else
                {
                    checkKeysDown(myKeyState);
                    //checkKeysUp(myKeyState);
                }
            }
        }

        public void checkKeysDown(KeyboardState keyState)
        {
            if (visible)
            {
                if (keyState.IsKeyDown(Keys.Enter) == true)
                {
                    Game1.Instance().Exit();
                }
            }
        }

        public void checkKeysDown(GamePadState keyState)
        {
            if (visible)
            {
                if (keyState.IsButtonDown(Buttons.Start) == true)
                {
                    Game1.Instance().Exit();
                }
            }
        }

        public void createMenu()
        {
            visible = true;
        }

    }
}

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
    class MainMenu : Menu
    {
        ContentManager content = Game1.Instance().getContent();

        bool canButtonPress = true;

        public MainMenu()
        {
            myTexture = "Menus/mainMenu";
            texture = content.Load<Texture2D>(myTexture);
            visible = true;
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
                    checkKeysUp(myKeyState);
                }
            }
        }

        public void checkKeysDown(KeyboardState keyState)
        {
            if (visible)
            {
                if (keyState.IsKeyDown(Keys.Enter) == true)
                {
                    removeMainMenu();
                }
            }
        }

        public void checkKeysDown(GamePadState keyState)
        {
            if (visible)
            {
                if (keyState.IsButtonDown(Buttons.Start) == true)
                {
                    removeMainMenu();
                }
            }
        }

        public void createMainMenu()
        {
            visible = true;
        }

        public void removeMainMenu()
        {
            visible = false;
            Game1.Instance().gameState = Game1.GameState.Gameplay;
        }

        public void checkKeysUp(KeyboardState keyState)
        {
            if (visible)
            {

            }
        }

        //public void checkKeysUp(GamePad keyState)
        //{
        //    if (visible)
        //    {

        //    }
        //}
    }
}

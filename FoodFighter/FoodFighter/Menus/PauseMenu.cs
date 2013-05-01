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
    class PauseMenu : Menu
    {
        ContentManager content = Game1.Instance().getContent();
        bool canUnPause = false;

        public PauseMenu()
        {
            myTexture = "Menus/pauseMenu";
            texture = content.Load<Texture2D>(myTexture);
            visible = true;
            width = 640;
            height = 480;
            animTimer = new Timer(650);
            animationRect = new Rectangle(0, 0, width, height);
            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
            animTimer.Enabled = true;
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
                }
                else
                {
                    checkKeysDown(myKeyState);
                }
            }
        }

        public void removePauseMenu()
        {
            visible = false;
            Game1.Instance().gameState = Game1.GameState.Gameplay;
        }

        public void createPauseMenu()
        {
            visible = true;
        }

        public void checkKeysDown(KeyboardState keyState)
        {
            if (visible)
            {
                if (keyState.IsKeyUp(Keys.P) == true)
                {
                    canUnPause = true;
                }
                if (keyState.IsKeyDown(Keys.P) == true && canUnPause)
                {
                    removePauseMenu();
                    canUnPause = false;
                }
            }
        }

        public void checkKeysDown(GamePadState keyState)
        {
            if (visible)
            {
                if (keyState.IsButtonUp(Buttons.Start) == true && canUnPause)
                {
                    canUnPause = true;
                }
                if (keyState.IsButtonDown(Buttons.Start) == true && canUnPause)
                {
                    removePauseMenu();
                    canUnPause = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, animationRect, Color.White);
            }
        }
    }
}

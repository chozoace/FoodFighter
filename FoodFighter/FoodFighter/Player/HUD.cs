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
    class HUD : Sprite
    {
        int xOffset = 80;
        int yOffset = 170;
        public SpriteFont Font { get; set; }

        public int Score { get; set; }

        public HUD(float x)
        {
            //setPosition(x);
            position.Y = 30;
        }

        public void setPosition(float x)
        {
            //position.X = x + xOffset;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera, Vector2 playerPosition)
        {
            spriteBatch.DrawString(
                Font,                           // SpriteFont
                "Calories Burned: " + Score.ToString(),   // Text
                new Vector2((playerPosition.X + xOffset) - camera.X, (playerPosition.Y - yOffset) - camera.Y),   // Position
                Color.Crimson);  
        }
    }
}

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
    class HealthBar : Sprite
    {
        int currentHealth;
        int xOffset = -280;
        int yOffset = 170;

        public HealthBar()
        {
            texture = Game1.Instance().Content.Load<Texture2D>("Health/Health5");
            width = texture.Width;
            height = texture.Height;
            position.Y = 30;
        }

        public void Update(int health)
        {
            currentHealth = health;

            switch (currentHealth)
            {
                case 100:
                    texture = Game1.Instance().Content.Load<Texture2D>("Health/Health5");
                    break;
                case 80:
                    texture = Game1.Instance().Content.Load<Texture2D>("Health/Health4");
                    break;
                case 60:
                    texture = Game1.Instance().Content.Load<Texture2D>("Health/Health3");
                    break;
                case 40:
                    texture = Game1.Instance().Content.Load<Texture2D>("Health/Health2");
                    break;
                case 20:
                    texture = Game1.Instance().Content.Load<Texture2D>("Health/Health1");
                    break;
                case 0:
                    texture = Game1.Instance().Content.Load<Texture2D>("Health/Health5");
                    break;
            }

            base.Update();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera, Vector2 playerPosition)
        {
            position.X = playerPosition.X + xOffset;
           // if (LevelManager.Instance().player.BoundingBox.Y - 208 <= 0)
                position.Y = playerPosition.Y - yOffset;

            spriteBatch.Draw(texture, new Rectangle((int)(position.X - camera.X), (int)(position.Y - camera.Y), 250, 30), Color.White);
        }
    }
}

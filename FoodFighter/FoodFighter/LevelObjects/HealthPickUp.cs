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
    class HealthPickUp : Sprite
    {
        public HealthPickUp(Vector2 newPos)
        {
            position = newPos;
            width = 32;
            height = 32;
            texture = Game1.Instance().Content.Load<Texture2D>("LevelObjects/Apple");
            animTimer = new Timer(150);
            animationRect = new Rectangle(0, 0, width, height);
            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
            animTimer.Enabled = true;
        }

        public override void Update()
        {
            if (CheckCollision(BoundingBox))
            {
                LevelManager.Instance().removefromSpriteList(this);
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            spriteBatch.Draw(texture, position - camera, animationRect, Color.White);
        }

        public override bool CheckCollision(Rectangle collisionBox)
        {
            Player player = LevelManager.Instance().player;

            if (BoundingBox.Intersects(player.BoundingBox))
            {
                player.health += 20;
                return true;
            }

            return false;
        }
    }
}

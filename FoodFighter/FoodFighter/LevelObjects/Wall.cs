﻿using System;
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
    class Wall : Sprite
    {
        protected Rectangle boundingBox;
        protected bool isVisible = true;
        protected ContentManager content;
        public int imageId;

        public Wall(Vector2 Position, int theWidth, int theHeight, int id = 0, string texImage = "", string objName = "Wall")
        {
            // isVisible = Visible;
            position = Position;
            width = theWidth;
            height = theHeight;
            getContent();
            imageId = id;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, width, height);//this is needed to draw
            name = objName;

            if (texImage != "")
            {
                texture = content.Load<Texture2D>(texImage);
            }
        }

        public void getContent()
        {
            content = Game1.instance.getContent();
        }

        public Rectangle BoundingBox { get { return boundingBox; } }//this is needed for collision
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            if (isVisible == true)
            {
                switch (imageId)
                {
                    case 1:
                        texture = content.Load<Texture2D>("LevelObjects/Block1");
                        break;

                    case 2:
                        texture = content.Load<Texture2D>("LevelObjects/Block2");
                        break;

                    case 3:
                        texture = content.Load<Texture2D>("LevelObjects/spike");
                        break;

                    case 4:
                        texture = content.Load<Texture2D>("LevelObjects/DeathBlock");
                        break;
                }
                base.Draw(spriteBatch, camera);
            }
        }

        public virtual void interact()
        {
            
        }
    }
}

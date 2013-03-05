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
    class Background : Sprite
    {
        ContentManager myContent;
        SpriteBatch mySpriteBatch;
        int windowWidth = 640;
       // int windowHeight = 480;
        public override Rectangle BoundingBox { get { return new Rectangle((int)position.X, (int)position.Y, width, height); } }

        public Background(int xPos)
        {
            myContent = Game1.Instance().getContent();
            mySpriteBatch = Game1.Instance().getSpriteBatch();

            texture = myContent.Load<Texture2D>("Background/Background1");
            height = 480;
            width = 640;
            position.X = xPos;
        }
    }
}

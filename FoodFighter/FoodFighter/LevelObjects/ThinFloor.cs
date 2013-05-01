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
    class ThinFloor : Wall
    {
        public ThinFloor(Vector2 Position, int theWidth, int theHeight, int id = 9 /* 9 means no id*/, string texImage = "") 
            : base(Position, theWidth, theHeight, id)
        {
            name = "ThinFloor";
            getContent();
            texture = content.Load<Texture2D>(texImage);
            width = texture.Width;
            height = texture.Height;
        }
    }
}

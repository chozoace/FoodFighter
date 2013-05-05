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
    class ChickenAttack : OnionAttack
    {
        public ChickenAttack(int x, int y, int facing)
            : base(x, y, facing)
        {
            texture = content.Load<Texture2D>("Enemy/Chicken/flameball");
        }
    }
}

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
    class DeathBlock : Wall
    {
        public DeathBlock(Vector2 Position, int theWidth, int theHeight, int id)
            : base(Position, theWidth, theHeight, id, "")
        {
            name = "DeathBlock";
            texture = content.Load<Texture2D>("LevelObjects/Block3");
        }

        //public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        //{
            
        //}

        public override void interact()
        {
            LevelManager.Instance().player.death();
        }
    }
}

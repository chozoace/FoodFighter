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
    class EnemyMeleeAttack : Hitbox
    {
        ContentManager content = Game1.Instance().getContent();

        public EnemyMeleeAttack(int x, int y, int facing)
        {
            visible = false;
            isEnemyAttack = true;
            startup = 50;
            active = 200;
            recovery = 300;
            stunTime = 500;
            knockBackSpeed = new Vector2(5, 0);
            totalFrames = 15;
            startupTimer = new Timer(startup);
            activeTimer = new Timer(active);

            if (facing == 0)
            {
                Xdisposition = 40;
                Ydisposition = 30;
                position.X = x + Xdisposition;
                position.Y = y + Ydisposition;
            }
            else
            {
                Xdisposition = -5;
                Ydisposition = 30;
                position.X = x + Xdisposition;
                position.Y = y + Ydisposition;
            }

            width = 30;
            height = 10;
            texture = content.Load<Texture2D>("LevelObjects/Block2");
            damage = 0;

            //after startup create attack
            startupTimer.Elapsed += new ElapsedEventHandler(createAttack);
            startupTimer.Enabled = true;
            //after active, goes into recovery, remove hitbox, keep controls locked and animation running
            activeTimer.Elapsed += new ElapsedEventHandler(removeAttack);
            activeTimer.Enabled = true;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            if (visible)
                spriteBatch.Draw(texture, position - camera, myHitBox, Color.White);
        }
    }
}

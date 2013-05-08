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
    class OnionAttack : Hitbox
    {
        protected float speed = 70;
        protected int fac;
        //int direction = 1;
        protected float velocity;
        protected GameTime gameTime = new GameTime();
        protected ContentManager content = Game1.Instance().getContent();
        public override Rectangle hitBox { get { return new Rectangle((int)(position.X), (int)(position.Y), width, height); } }

        public OnionAttack(int x, int y, int facing)
            : base()
        {
            fac = facing;

            position.X = x;
            position.Y = y + 32;
            //velocity = speed;

            isEnemyAttack = true;
            startup = 50;
            recovery = 300;
            stunTime = 500;
            knockBackSpeed = new Vector2(0, 0);
            totalFrames = 15;
            startupTimer = new Timer(startup);
            recoveryTimer = new Timer(recovery);
            width = 21;
            height = 13;
            texture = content.Load<Texture2D>("Enemy/onion");
            damage = 20;

            createAttack();
        }

        public void Reload(int x, int y, int facing)
        {
            fac = facing;
            position.X = x;
            position.Y = y + 32;
            visible = true;
            canDamage = true;
        }

        public override void Update()
        {
            if (fac == 1)
            {
                position.X -= speed * 0.05F;
            }
            if (fac == 0)
            {
                position.X += speed * 0.05F;
            }
            
            wallList = LevelConstructor.Instance().getWallList();
            foreach (Wall wall in wallList)
            {
                if(hitBox.Intersects(wall.BoundingBox))
                {
                    //removeAttack();
                    visible = false;
                }
            }

            base.Update();
        }
    }
}

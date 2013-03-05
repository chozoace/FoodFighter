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
using System.Timers;

namespace FoodFighter
{
    class MediumAttack : Hitbox
    {
        public MediumAttack(int x, int y, ContentManager content, int facing)
        {
            visible = false;
            isEnemyAttack = false;
            inChain = true;
            startup = 30;
            active = 200;
            recovery = 800;
            stunTime = 1000;
            //totalFrames = 15; //what is this
            startupTimer = new Timer(startup);
            activeTimer = new Timer(active);
            recoveryTimer = new Timer(recovery);

            if (facing == 0)
            {
                Xdisposition = 40;
                Ydisposition = 30;
                position.X = x + Xdisposition;
                position.Y = y + Ydisposition;
            }
            else
            {
                Xdisposition = -18;
                Ydisposition = 30;
                position.X = x + Xdisposition;
                position.Y = y + Ydisposition;
            }
            width = 40;
            height = 10;
            texture = content.Load<Texture2D>("LevelObjects/Block2");
            damage = 50;

            //begin animation

            //after startup create attack
            startupTimer.Elapsed += new ElapsedEventHandler(createAttack);
            startupTimer.Enabled = true;
            //after active, goes into recovery, remove hitbox, keep controls locked and animation running
            activeTimer.Elapsed += new ElapsedEventHandler(removeAttack);
            activeTimer.Enabled = true;
            //after recovery restore controls
            recoveryTimer.Elapsed += new ElapsedEventHandler(unlockControls);
            recoveryTimer.Enabled = true;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            if (visible)
                spriteBatch.Draw(texture, position - camera, myHitBox, Color.White);
        }
    }
}
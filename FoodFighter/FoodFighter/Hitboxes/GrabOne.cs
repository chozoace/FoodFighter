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
    class GrabOne : Hitbox
    {
        Timer throwStartup;
        Enemy enemyThrown;

        public GrabOne(int x, int y, ContentManager content, int facing)
        {
            inChain = false;
            visible = true;
            isEnemyAttack = false;
            startup = 20;
            active = 200;
            recovery = 600;
            stunTime = 500;
            //totalFrames = 15; //what is this
            startupTimer = new Timer(startup);
            activeTimer = new Timer(active);
            recoveryTimer = new Timer(recovery);
            throwStartup = new Timer(100);

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
            damage = 175;

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

        public override void onHitboxHit(Enemy enemy)
        {
            if (enemy.enemyState != Enemy.EnemyState.Hitstun)
            {
                enemyThrown = enemy;
                canDamage = false;
                enemyThrown.enemyState = Enemy.EnemyState.Hitstun;
                enemy.gravity = 0;
                enemyThrown.position.Y -= 50;

                throwStartup.Elapsed += new ElapsedEventHandler(startThrow);
                throwStartup.Enabled = true;

                //enemy.startThrow(damage, stunTime);
            }
        }

        public void startThrow(object sender, EventArgs e)
        {
            Debug.WriteLine("here");
            enemyThrown.startThrow(damage, stunTime);

            throwStartup.Dispose();
            throwStartup = null;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            if (visible)
                spriteBatch.Draw(texture, position - camera, myHitBox, Color.White);
        }
    }
}

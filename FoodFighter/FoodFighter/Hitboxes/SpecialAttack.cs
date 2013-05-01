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
    class SpecialAttack : Hitbox
    {
        public SpecialAttack(int x, int y, ContentManager content, int facing)
            : base()
        {
            visible = false;
            isEnemyAttack = false;
            inChain = false;
            startup = 60;
            active = 200;
            recovery = 1000;
            stunTime = 1500;
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
            damage = 100;

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
            if (LevelManager.Instance().player.fatState == Player.FatState.Level1)
            {
                if (enemy.enemyState != Enemy.EnemyState.Hitstun)
                {
                    enemy.hitBoxCollide(stunTime);
                    enemy.health -= damage;
                    enemy.position.Y -= 2;
                    enemy.speed.Y = -8;
                    if (enemy.facing == 0)
                        enemy.speed.X = -4;
                    else
                        enemy.speed.X = 4;

                    canDamage = false;
                    powSound.Play();
                }
                else
                {
                    //turn off timers
                    damage = 150;
                    if (enemy.comboTime != null)
                    {
                        enemy.comboTime.Dispose();
                    }
                    enemy.hitBoxCollide(stunTime);
                    enemy.health -= damage;
                    enemy.position.Y -= 2;
                    enemy.speed.Y = -8;
                    if (enemy.facing == 0)
                        enemy.speed.X = -4;
                    else
                        enemy.speed.X = 4;

                    canDamage = false;
                    powSound.Play();
                }
            }
            else
            {
                if (enemy.enemyState != Enemy.EnemyState.Hitstun)
                {
                    enemy.hitBoxCollide(stunTime);
                    enemy.health -= damage;
                    enemy.position.Y -= 2;
                    enemy.speed.Y = -13;
                    if (enemy.facing == 0)
                        enemy.speed.X = -6;
                    else
                        enemy.speed.X = 6;

                    canDamage = false;
                    powSound.Play();
                }
                else
                {
                    //turn off timers
                    if (enemy.comboTime != null)
                    {
                        enemy.comboTime.Dispose();
                    }
                    enemy.hitBoxCollide(stunTime);
                    enemy.health -= damage;
                    enemy.position.Y -= 2;
                    enemy.speed.Y = -13;
                    if (enemy.facing == 0)
                        enemy.speed.X = -6;
                    else
                        enemy.speed.X = 6;

                    canDamage = false;
                    powSound.Play();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            if (visible)
                spriteBatch.Draw(texture, position - camera, myHitBox, Color.White);
        }
    }
}

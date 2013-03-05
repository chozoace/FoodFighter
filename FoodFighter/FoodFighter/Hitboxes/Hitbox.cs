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
    class Hitbox : Sprite
    {
        protected Timer startupTimer;
        protected Timer activeTimer;
        protected Timer recoveryTimer;

        protected int damage;
        protected double startup;
        protected double active;
        protected double recovery;
        protected int stunTime;
        protected Vector2 knockBackSpeed;
        protected int totalFrames;
        protected int Xdisposition;
        protected int Ydisposition;
        protected Rectangle myHitBox;
        public virtual Rectangle hitBox { get { return myHitBox; } }
        public bool visible = true;
        protected bool canDamage = true; //so hitbox only hurts once
        protected bool isEnemyAttack;
        protected bool isGrab = false;

        public Hitbox()
        {

        }

        protected void createAttack()
        {
            myHitBox = new Rectangle((int)(position.X), (int)(position.Y), width, height);
            //LevelManager.Instance().addToSpriteList(this);
        }

        protected virtual void createAttack(object sender, ElapsedEventArgs e)
        {
            myHitBox = new Rectangle((int)(position.X), (int)(position.Y), width, height);
            LevelManager.Instance().addToSpriteList(this);

            if(LevelManager.Instance().player.currentChain < 3 && !isGrab)
                LevelManager.Instance().player.canAttack = true;

            startupTimer.Dispose();
            startupTimer = null;
        }

        public void removeAttack()
        {
            LevelManager.Instance().removefromSpriteList(this);
        }

        protected void removeAttack(object sender, ElapsedEventArgs e)
        {
            //remove hitbox
            LevelManager.Instance().removefromSpriteList(this);
            //LevelManager.Instance().player.attacksToNull();
            activeTimer.Dispose();
            activeTimer = null;
        }

        protected void unlockControls(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("recovered");
            LevelManager.Instance().player.unlockPlayerControls();

            if(LevelManager.Instance().player.myState != Player.PlayerState.Hitstun) //if player is currently not being hit, unlock controls
                LevelManager.Instance().player.myState = Player.PlayerState.Idle;

            LevelManager.Instance().player.canAttack = true;
            LevelManager.Instance().player.currentChain = 0;
            LevelManager.Instance().player.attacksToNull();
            recoveryTimer.Dispose();
            recoveryTimer = null;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            if(visible)
                spriteBatch.Draw(texture, position - camera, Color.White);
        }

        public override void Update()
        {
            //player attack
            if (!isEnemyAttack)
            {
                List<Enemy> enemyList = LevelManager.Instance().getEnemyList();
                foreach (Enemy enemy in enemyList)
                {
                    if (hitBox.Intersects(enemy.BoundingBox) && canDamage/* && !enemy.hitstun*/)
                    {
                        //enemy.hitstun = true;//activate timer
                        //enemy.startHitstun(stunTime);
                        onHitboxHit(enemy);
                    }
                }
            }
            //enemy attack
            else
            {
                Player player = LevelManager.Instance().player;
                if (hitBox.Intersects(player.BoundingBox) && canDamage /*&& !player.hitstun*/)
                {
                    player.speed.X = 0;
                    player.currentAccel = 0;
                    player.health -= damage;
                    player.hitstun = true;
                    player.lockPlayerControls();
                    canDamage = false;
                    visible = false;
                    //removeAttack();
                    player.startHitstun(stunTime);
                }
            }


            base.Update();
        }

        public virtual void onHitboxHit(Enemy enemy)
        {
            if (enemy.enemyState != Enemy.EnemyState.Hitstun)
            {
                enemy.hitBoxCollide(stunTime);
                enemy.health -= damage;
                canDamage = false;
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
                canDamage = false;
            }
        }

        public void removeTimers()//use to delete attack
        {
            if (startupTimer != null)
                startupTimer.Dispose();

            if (activeTimer != null)
                activeTimer.Dispose();

            if (recoveryTimer != null)
            {
                recoveryTimer.Dispose();
            }
        }

    }
}

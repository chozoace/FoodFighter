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
    class Enemy : MoveableObject
    {
        public enum EnemyState
        {
            Jumping,
            Running,
            Idle,
            Attacking,
            Dead,
            Waiting,
            Hitstun,
            Thrown
        }

        public int detectionRange = 300;
        public int scoreAward;
        public EnemyState enemyState;
        protected ContentManager enemyContent = Game1.Instance().getContent();
        protected Rectangle leftDetectionBox;
        protected Rectangle rightDetectionBox;
        protected bool canAnimChange = true;
        protected bool comboable = false;
        protected List<Hitbox> attacks = new List<Hitbox>();
        public Timer comboTime;

        public EventHandler currentEvent;
        public EventArgs eArgs = new EventArgs();
        float throwHeight;
        int damageToBeApplied;
        int stunTimeToBeApplied;
        bool assignGravity = true; //TEMPORARY
        protected SoundEffect deathSound;
        protected SoundEffect deathSoundInstance;

        public Enemy()
        {
            deathSound = Game1.Instance().Content.Load<SoundEffect>("Music/death2");
        }

        public virtual void LoadContent()
        {
            isEnemy = true;
            currentEvent = new EventHandler(nothingState);
            currentAnimation = idleLeftAnim;
            texture = enemyContent.Load<Texture2D>(currentAnimation);
            animationRect = new Rectangle(0, 0, width, height);
            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
            animTimer.Enabled = true;

            //Debug.WriteLine("currentanim " + currentAnimation + " animationrect " + animationRect);

            enemyState = EnemyState.Idle;
        }

        public override void Update()
        {
            currentEvent(this, eArgs);
            //Debug.WriteLine(CheckCollision(BottomBox));
        }

        public void UpdateLife()
        {
            if (health <= 0)
            {
                enemyState = EnemyState.Dead;
            }

            if (enemyState == EnemyState.Dead)
            {
                death();
            }
        }

        public void nothingState(object sender, EventArgs e)
        {

        }

        public void startThrow(int damage, int stunTime)
        {
            damageToBeApplied = damage;
            stunTimeToBeApplied = stunTime;

            throwHeight = position.Y - 40;
            currentEvent = new EventHandler(throwEvent);
        }

        public void throwEvent(object sender, EventArgs e)
        {
            gravity = .8f;
            if(facing == 0)
            {
                speed.X = -6;
            }
            else
            {
                speed.X = 6;
            }
            if (assignGravity)
            {
                speed.Y = -2;
                assignGravity = false;
            }

            if (CheckCollision(BottomBox) && collidingWall != LevelManager.Instance().player)
            {
                currentEvent = new EventHandler(nothingState);
                gravity = 1.5f;
                hitBoxCollide(stunTimeToBeApplied);
                health -= damageToBeApplied;
                assignGravity = true;
            }
            else
            {

            }
        }

        public void hitBoxCollide(int stunTime)
        {
            if (this.enemyState != EnemyState.Hitstun)
            {
                this.enemyState = EnemyState.Hitstun;
                startComboStun(stunTime);
            }
            else
            {
                startComboStun(stunTime);
            }
        }

        public void startComboStun(int stunTime)
        {
            comboTime = new Timer(stunTime);
            comboTime.Elapsed += new ElapsedEventHandler(endComboStun);
            comboTime.Enabled = true;
        }

        public void endComboStun(object sender, ElapsedEventArgs e)
        {
            if (comboTime != null)
            {
                comboTime.Dispose();
                comboTime = null;
                enemyState = EnemyState.Idle;
            }
        }

        public virtual void death()
        {
            LevelManager.Instance().player.hud.Score += scoreAward;
            //do some animation then timer in death
            deathSound.Play();

            LevelManager.Instance().removefromSpriteList(this);
            LevelManager.Instance().removefromEnemyList(this);
        }

        public virtual bool CheckDetection()
        {
            Player player = LevelManager.Instance().player;
            //if (player.BoundingBox.Intersects(leftDetectionBox)) //causes day of reckoning referee collision
            //{
            //    return true;
            //}
            if (player.position.X < position.X)
            {
                facing = 1;
                if (position.X - (player.position.X) < detectionRange)
                {
                    return true;
                }
            }
            else if (player.position.X > position.X)
            {
                facing = 0;
                if (name != "Chicken")
                {
                    if (player.position.X - (position.X) < detectionRange)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            
            return false;
        }

    }
}

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
    class OnionEnemy : Enemy
    {
        ContentManager myContent = Game1.Instance().getContent();

        int attackLength = 4; // total number of frames in attack
        Timer attackCooldown = new Timer(1100);
        public override Rectangle BoundingBox { get { return new Rectangle((int)position.X + 25, (int)position.Y + 25, width - 35, height - 25); } }
        List<OnionAttack> theAttack;
        bool canAttack = true;
        bool canLoop = true;

        public OnionEnemy(Vector2 newPos)
        {
            //attacks.Add(theAttack);
            theAttack = new List<OnionAttack>();
            position = newPos;
            health = 350;
            width = 64;
            height = 64;
            leftDetectionBox = new Rectangle((int)(position.X - 300), (int)(position.Y), 300, 64);

            LoadContent();
        }

        public override void LoadContent()
        {
            idleAnim = "Enemy/OnionIdleRight";
            idleLeftAnim = "Enemy/OnionIdleLeft";
            attackAnim = "Enemy/OnionAttackRight";
            attackLeftAnim = "Enemy/OnionAttackLeft";
            hurtLeft = "Enemy/OnionHurtLeft";
            hurtRight = "Enemy/OnionHurtRight";

            base.LoadContent();
        }

        public override void Update()
        {
            Debug.WriteLine(health);
            UpdateTexture();

            if (enemyState != EnemyState.Hitstun)
            {
                if (CheckDetection() && enemyState != EnemyState.Attacking)
                {
                    Debug.WriteLine("attack called");
                    enemyState = EnemyState.Attacking;
                    attack();
                }
                else if (!CheckDetection() && enemyState != EnemyState.Idle)
                {
                    currentAnimation = idleLeftAnim;
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = enemyContent.Load<Texture2D>(currentAnimation);
                    enemyState = EnemyState.Idle;
                    Debug.WriteLine("out of range");
                    attackCooldown.Stop();
                }

                if (enemyState == EnemyState.Attacking)
                {
                    UpdateAttack();
                }
            }
            
            UpdateLife();
            foreach (OnionAttack o in theAttack)
            {
                if(o.visible == true)
                o.Update();
            }

        }

        public void UpdateTexture()
        {
            if (enemyState == EnemyState.Hitstun)
            {
                if (facing == 0 && currentAnimation != hurtRight)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(hurtRight);
                    currentAnimation = hurtRight;
                }
                else if (facing == 1 && currentAnimation != hurtLeft)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(hurtLeft);
                    currentAnimation = hurtLeft;
                }
            }
        }

        public void attack()//initial attack
        {
            //animation
            Debug.WriteLine("initial attack");
            currentAnimation = attackLeftAnim;
            animationRect = new Rectangle(0, 0, width, height);
            texture = enemyContent.Load<Texture2D>(currentAnimation);
            enemyState = EnemyState.Attacking;
        }

        public void attack(object sender, ElapsedEventArgs e) //loops attack
        {
            if (canLoop && enemyState != EnemyState.Hitstun)
            {
                Debug.WriteLine("from attack loop");
                attack();
            }
            canLoop = false;
        }

        public void UpdateAttack()
        {
            if (currentFrame == 2)
            {
                if (canAttack)
                {
                    bool makeNew = true;
                    foreach (OnionAttack o in theAttack)
                    {
                        if (o.visible == false)
                        {
                            o.Reload((int)position.X, (int)position.Y, facing);
                            makeNew = false;
                            break;
                        }
                    }
                    if (makeNew)
                    {
                        theAttack.Add(new OnionAttack((int)position.X, (int)position.Y, facing));
                    }
                    canAttack = false;
                }
            }

            if (currentFrame >= attackLength)
            {
                Debug.WriteLine("animation resetted");
                canLoop = true;
                canAttack = true;
                currentAnimation = idleLeftAnim;
                animationRect = new Rectangle(0, 0, width, height);
                texture = enemyContent.Load<Texture2D>(idleLeftAnim);
                attackCooldown.Elapsed += new ElapsedEventHandler(attack);
                attackCooldown.Enabled = true;
            }
        }

        public void attacksToNull()
        {
            theAttack = null;
        }

        public void UpdateMovement()
        {

        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            base.Draw(spriteBatch, camera);

            foreach (OnionAttack o in theAttack)
            {
                if (o.visible == true)
                    o.Draw(spriteBatch, camera);
            }

        }
    }
}

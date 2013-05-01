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
    class OnionEnemy : MeleeEnemy
    {
        List<OnionAttack> theAttack;
        public override Rectangle BoundingBox { get { return new Rectangle((int)position.X + 25, (int)position.Y + 25, width - 35, height - 25); } }

        public OnionEnemy(Vector2 newPos) : base(newPos)
        {
            scoreAward = 40;
            attackLength = 4;
            attackRange = 200;
            theAttack = new List<OnionAttack>();
            attackCooldown = new Timer(2500);
            canUpdate = false;

            idleAnim = "Enemy/OnionIdleRight";
            idleLeftAnim = "Enemy/OnionIdleLeft";
            runAnim = "Enemy/OnionIdleRight";
            runLeftAnim = "Enemy/OnionIdleLeft";
            attackAnim = "Enemy/OnionAttackRight";
            attackLeftAnim = "Enemy/OnionAttackLeft";
            hurtLeft = "Enemy/OnionHurtLeft";
            hurtRight = "Enemy/OnionHurtRight";
        }

        public override void Update()
        {
            //Debug.WriteLine(facing);
            UpdateLife();
            if (enemyState != EnemyState.Hitstun)
            {
                UpdateMovement();
            }
            UpdateGravity();
            UpdateTexture();

            if (enemyState != EnemyState.Hitstun)
            {
                if (CheckDetection() && enemyState != EnemyState.Running)
                {
                    canMove = true;
                }
                else if (!CheckDetection() && enemyState != EnemyState.Idle)
                {
                    if (facing == 1)
                    {
                        currentAnimation = idleLeftAnim;
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = enemyContent.Load<Texture2D>(currentAnimation);
                        enemyState = EnemyState.Idle;
                        attackCooldown.Stop();
                        canMove = false;
                    }
                    else if (facing == 0)
                    {
                        currentAnimation = idleAnim;
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = enemyContent.Load<Texture2D>(currentAnimation);
                        enemyState = EnemyState.Idle;
                        attackCooldown.Stop();
                        canMove = false;
                    }
                }

                if (enemyState == EnemyState.Attacking)
                {
                    UpdateAttack();
                }
            }

            if (enemyState == EnemyState.Hitstun)
            {

            }

            foreach (OnionAttack o in theAttack)
            {
                if (o.visible == true)
                    o.Update();
            }

            base.Update();
        }

        public override void UpdateMovement()
        {
            //Debug.WriteLine(CheckCollision(BoundingBox));
            #region Xmovement
            if (canMove)
            {
                if (facing == 1)
                {
                    if (!CheckAttackRange())
                    {
                        if (position.Y - LevelManager.Instance().player.position.Y <= 64 && LevelManager.Instance().player.position.Y - position.Y <= 64)
                        {
                            if (CheckCollision(LeftBox))
                                position.X += Xspeed;
                            position.X -= Xspeed;

                            enemyState = EnemyState.Running;
                        }
                    }
                    else if (enemyState != EnemyState.Attacking)
                    {
                        enemyState = EnemyState.Attacking;
                        attack();
                    }
                }
                if (facing == 0)
                {
                    if (!CheckAttackRange())
                    {
                        if (position.Y - LevelManager.Instance().player.position.Y <= 64 && LevelManager.Instance().player.position.Y - position.Y <= 64)
                        {
                            if (CheckCollision(RightBox))
                                position.X -= Xspeed;
                            position.X += Xspeed;

                            enemyState = EnemyState.Running;
                        }
                    }
                    else if (enemyState != EnemyState.Attacking)
                    {
                        enemyState = EnemyState.Attacking;
                        attack();
                    }
                }
            }

            if (facing == 0)
            {
                if (CheckCollision(RightBox))
                {
                    if (collidingWall != LevelManager.Instance().player)
                    {
                        position.X = collidingWall.BoundingBox.X - 60;//60 is the bounding box position x + its width
                        //currentAccel = 0;
                    }
                }
            }
            else if (facing == 1)
            {
                if (CheckCollision(LeftBox))
                {
                    if (collidingWall != LevelManager.Instance().player)
                    {
                        position.X = collidingWall.BoundingBox.X + collidingWall.BoundingBox.Width;
                        //currentAccel = 0;
                    }
                }
            }

            leftDetectionBox.X = (int)(position.X - 200); //old detection system, not being used
            #endregion

            #region CollisionForHitstun
            if (enemyState == EnemyState.Hitstun)
            {
                if (speed.X > 0)
                {
                    if (CheckCollision(RightBox) && collidingWall != LevelManager.Instance().player)
                    {
                        position.X = collidingWall.BoundingBox.X - 60;//60 is the bounding box position x + its width
                        currentAccel = 0;
                    }
                }

                else if (speed.X < 0)
                {
                    if (CheckCollision(LeftBox) && collidingWall != LevelManager.Instance().player)
                    {

                        position.X = collidingWall.BoundingBox.X + collidingWall.BoundingBox.Width;
                        currentAccel = 0;
                    }
                }
            }
            #endregion
        }

        public override void attack()//initial attack
        {
            //animation
            if (facing == 1)
                currentAnimation = attackLeftAnim;
            else if(facing == 0)
                currentAnimation = attackAnim;
            animationRect = new Rectangle(0, 0, width, height);
            texture = enemyContent.Load<Texture2D>(currentAnimation);
            enemyState = EnemyState.Attacking;
        }

        public override void attack(object sender, ElapsedEventArgs e) //loops attack
        {
            if (canLoop && enemyState != EnemyState.Hitstun)
            {
                attack();
            }
            canLoop = false;
        }

        public override void UpdateAttack()
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
                canLoop = true;
                canAttack = true;
                if (facing == 1)
                    currentAnimation = idleLeftAnim;
                else
                    currentAnimation = idleAnim;
                animationRect = new Rectangle(0, 0, width, height);
                texture = enemyContent.Load<Texture2D>(currentAnimation);
                attackCooldown.Elapsed += new ElapsedEventHandler(attack);
                attackCooldown.Enabled = true;
            }
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

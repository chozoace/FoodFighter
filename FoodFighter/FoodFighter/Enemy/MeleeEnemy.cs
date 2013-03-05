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
    class MeleeEnemy : Enemy
    {
        protected ContentManager myContent = Game1.Instance().getContent();

        protected int attackLength = 5; // total number of frames in attack
        protected int attackRange;

        protected int fallspeed = 10;
        protected int Xspeed = 4;

        protected Timer attackCooldown = new Timer(1000);
        protected Timer activeTimer;
        public override Rectangle BoundingBox { get { return new Rectangle((int)position.X + 25, (int)position.Y + 25, width - 45, height - 25); } }
        public Rectangle AttackBox { get { return new Rectangle((int)position.X - 5, (int)position.Y + 10, width + 10, height - 10); } }
        protected bool canAttack = true;
        protected bool canMove = false;
        protected EnemyMeleeAttack theAttack;

        protected bool canLoop = true;
        protected bool canUpdate = true;

        public MeleeEnemy(Vector2 newPos)
        {
            scoreAward = 20;
            position = newPos;
            //speed.X = 4;
            attackRange = 40;
            health = 350;
            width = 64;
            height = 64;
            leftDetectionBox = new Rectangle((int)(position.X - 200), (int)(position.Y), 200, 64);
            facing = 1; 

            idleAnim = "Enemy/burgeridleright";
            idleLeftAnim = "Enemy/burgeridleleft";
            runAnim = "Enemy/burgerrunright";
            runLeftAnim = "Enemy/burgerrunleft";
            attackAnim = "Enemy/burgerpunchright";
            attackLeftAnim = "Enemy/burgerpunchleft";
            hurtLeft = "Enemy/burgerhurtleft";
            hurtRight = "Enemy/burgerhurtright";

            LoadContent();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update()
        {
            if (canUpdate)
            {
                UpdateLife();
                UpdateMovement();
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
                            canMove = false;
                        }
                        else if (facing == 0)
                        {
                            currentAnimation = idleAnim;
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = enemyContent.Load<Texture2D>(currentAnimation);
                            enemyState = EnemyState.Idle;
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
            }
            base.Update();
        }

        public virtual void UpdateMovement()
        {
            //Debug.WriteLine(CheckCollision(BoundingBox));
            #region Xmovement
            if (canMove && enemyState != EnemyState.Hitstun)
            {
                if (facing == 1)
                {
                    if (!CheckAttackRange())
                    {
                        if (CheckCollision(LeftBox))
                            position.X += Xspeed;
                        position.X -= Xspeed;

                        enemyState = EnemyState.Running;
                    }
                    else if (enemyState != EnemyState.Attacking)
                    {
                        enemyState = EnemyState.Attacking;
                        //currentEvent = new EventHandler(attackEvent);
                        attack();
                    }
                }
                if (facing == 0)
                {
                    if (!CheckAttackRange())
                    {
                        if (CheckCollision(RightBox))
                            position.X -= Xspeed;
                        position.X += Xspeed;

                        enemyState = EnemyState.Running;
                    }
                    else if(enemyState != EnemyState.Attacking)
                    {
                        enemyState = EnemyState.Attacking;
                        //currentEvent = new EventHandler(attackEvent);
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

        public void UpdateGravity()
        {
            bool applyGravity = false;

            #region gravity

            if (!CheckCollision(BottomBox))
            {
                if (enemyState != EnemyState.Hitstun)
                    enemyState = EnemyState.Jumping;
                else if(enemyState == EnemyState.Hitstun)
                {
                    applyGravity = true;
                }
            }
            else
            {
                if (collidingWall != LevelManager.Instance().player)
                {
                    position.Y = collidingWall.BoundingBox.Y - height;
                    applyGravity = false;
                    speed.Y = 0;
                    speed.X = 0;
                }
            }

            if (enemyState == EnemyState.Jumping || applyGravity == true)
            {
                speed.Y += gravity;

                if (speed.Y > fallspeed)
                    speed.Y = fallspeed;
                if (CheckCollision(UpperBox))
                {
                    position.Y = collidingWall.BoundingBox.Y + collidingWall.BoundingBox.Height;
                    speed.Y *= -1;
                }

            }
            #endregion

            position.X += speed.X;
            position.Y += speed.Y;
        }

        public void UpdateTexture()
        {
            if (enemyState == EnemyState.Running)
            {
                if (facing == 0 && currentAnimation != runAnim)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(runAnim);
                    currentAnimation = runAnim;

                }
                else if (facing == 1 && currentAnimation != runLeftAnim)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(runLeftAnim);
                    currentAnimation = runLeftAnim;
                }
            }
            else if (enemyState == EnemyState.Idle)
            {
                if (facing == 0 && currentAnimation != idleAnim)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(idleAnim);
                    currentAnimation = idleAnim;
                }
                else if (facing == 1 && currentAnimation != idleLeftAnim)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(idleLeftAnim);
                    currentAnimation = idleLeftAnim;
                }
            }
            else if (enemyState == EnemyState.Hitstun)
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

        public override bool CheckCollision(Rectangle collisionBox)
        {
            Player player = LevelManager.Instance().player;
            List<Enemy> enemyList = LevelManager.Instance().getEnemyList();
            foreach (Enemy enemy in enemyList)
            {
                if (collisionBox.Intersects(enemy.BoundingBox) && enemy != this)
                {
                    collidingWall = enemy;
                    return true;
                }
            }

            if (BoundingBox.Intersects(player.BoundingBox))
            {
                collidingWall = player;
                return true;
            }

            return base.CheckCollision(collisionBox);
        }

        public void attackEvent(object sender, EventArgs e)
        {
            if (facing == 0 && currentAnimation != attackAnim)
            {
                animationRect = new Rectangle(0, 0, width, height);
                texture = myContent.Load<Texture2D>(attackAnim);
                currentAnimation = attackAnim;

                theAttack = new EnemyMeleeAttack((int)(position.X), (int)(position.Y), facing);
                activeTimer = new Timer(500);
                activeTimer.Elapsed += new ElapsedEventHandler(removeAttack);
                activeTimer.Enabled = true;

                //canLoop = true;
                //canAttack = true;
                currentAnimation = idleAnim;
                animationRect = new Rectangle(0, 0, width, height);
                texture = enemyContent.Load<Texture2D>(currentAnimation);
                //attackCooldown.Elapsed += new ElapsedEventHandler(attack);
                //attackCooldown.Enabled = true;                
            }
            else if (facing == 1 && currentAnimation != attackLeftAnim)
            {
                animationRect = new Rectangle(0, 0, width, height);
                texture = myContent.Load<Texture2D>(attackLeftAnim);
                currentAnimation = attackLeftAnim;

                theAttack = new EnemyMeleeAttack((int)(position.X), (int)(position.Y), facing);
                activeTimer = new Timer(500);
                activeTimer.Elapsed += new ElapsedEventHandler(removeAttack);
                activeTimer.Enabled = true;

                //canLoop = true;
                //canAttack = true;
                currentAnimation = idleLeftAnim;
                animationRect = new Rectangle(0, 0, width, height);
                texture = enemyContent.Load<Texture2D>(idleLeftAnim);
                //attackCooldown.Elapsed += new ElapsedEventHandler(attack);//recovery
                //attackCooldown.Enabled = true;
            }
        }

        public virtual void attack()
        {
            if (facing == 0 && currentAnimation != attackAnim)
            {
                animationRect = new Rectangle(0, 0, width, height);
                texture = myContent.Load<Texture2D>(attackAnim);
                currentAnimation = attackAnim;
            }
            else if (facing == 1 && currentAnimation != attackLeftAnim)
            {
                animationRect = new Rectangle(0, 0, width, height);
                texture = myContent.Load<Texture2D>(attackLeftAnim);
                currentAnimation = attackLeftAnim;
            }
        }

        public virtual void attack(object o, ElapsedEventArgs e)
        {
            if (canLoop)
            {
                attack();
            }
            canLoop = false;
        }

        public virtual void UpdateAttack()
        {
            if (currentFrame == 0 && currentAnimation == attackAnim || currentAnimation == attackLeftAnim)
            {
                //create hitbox
                if (canAttack)
                {
                    theAttack = new EnemyMeleeAttack((int)(position.X), (int)(position.Y), facing);
                    activeTimer = new Timer(500);
                    activeTimer.Elapsed += new ElapsedEventHandler(removeAttack);
                    activeTimer.Enabled = true;

                    canAttack = false;
                }
            }
            if (currentFrame >= 1)//may need to make recovery
            {
                //loop attack
                if (facing == 1)
                {
                    canLoop = true;
                    canAttack = true;
                    currentAnimation = idleLeftAnim;
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = enemyContent.Load<Texture2D>(idleLeftAnim);
                    attackCooldown.Elapsed += new ElapsedEventHandler(attack);//recovery
                    attackCooldown.Enabled = true;
                }
                else if (facing == 0)
                {
                    canLoop = true;
                    canAttack = true;
                    currentAnimation = idleAnim;
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = enemyContent.Load<Texture2D>(currentAnimation);
                    attackCooldown.Elapsed += new ElapsedEventHandler(attack);
                    attackCooldown.Enabled = true;
                }
            }
        }

        public void removeAttack(object sender, ElapsedEventArgs e)
        {
            theAttack = null;
            activeTimer.Dispose();
        }

        public bool CheckAttackRange()
        {
            Player player = LevelManager.Instance().player;
            if (facing == 1)
            {
                if (position.X - (player.position.X) < attackRange)
                {
                    return true;
                }
            }
            else if (facing == 0)
            {
                if (player.position.X - (position.X) < attackRange + 4)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            //drawCollisionBox(spriteBatch, myContent, camera);

            base.Draw(spriteBatch, camera);
        }
    }
}

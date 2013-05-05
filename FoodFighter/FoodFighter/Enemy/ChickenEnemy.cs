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
    class ChickenEnemy : OnionEnemy
    {
        //List<ChickenAttack> theAttack;
        //Attacks way too fast unless I have onionenemy sprites. probably due to the fact that a new attack is called based on the current
        //animation frame. Onion enemy is smaller than the chicken sprites.

        public ChickenEnemy(Vector2 newPos)
            : base(newPos)
        {
            //scoreAward = 40;
            //attackLength = 4;
            //attackRange = 200;
            //theAttack = new List<ChickenAttack>();
            //attackCooldown = new Timer(2500);
            //canUpdate = false;//if false, only does update in this class and enemy class, not melee enemy
            //name = "Chicken";

            idleLeftAnim = "Enemy/Chicken/chickenidle";
            //attackLeftAnim = "Enemy/OnionAttackLeft";
            hurtLeft = "Enemy/Chicken/chickenHurt";
        }

        //public override void Update()
        //{
        //    Debug.WriteLine(currentAnimation);
        //    Debug.WriteLine(enemyState);
        //    UpdateLife();
        //    if (enemyState != EnemyState.Hitstun)
        //    {
        //        if (CheckAttackRange() && enemyState != EnemyState.Attacking)
        //        {
        //            enemyState = EnemyState.Attacking;
        //            //attack();
        //        }
        //    }
        //    UpdateGravity();
        //    UpdateTexture();

        //    if (enemyState != EnemyState.Hitstun)
        //    {
        //        if (!CheckDetection() && enemyState != EnemyState.Idle)
        //        {
        //            if (facing == 1)
        //            {
        //                currentAnimation = idleLeftAnim;
        //                animationRect = new Rectangle(0, 0, width, height);
        //                texture = enemyContent.Load<Texture2D>(currentAnimation);
        //                enemyState = EnemyState.Idle;
        //                attackCooldown.Stop();
        //                canMove = false;
        //            }
        //            else if (facing == 0)
        //            {
        //                currentAnimation = idleLeftAnim;
        //                animationRect = new Rectangle(0, 0, width, height);
        //                texture = enemyContent.Load<Texture2D>(currentAnimation);
        //                enemyState = EnemyState.Idle;
        //                attackCooldown.Stop();
        //                canMove = false;
        //            }
        //        }

        //        if (enemyState == EnemyState.Attacking)
        //        {
        //            UpdateAttack();
        //        }
        //    }

        //    if (enemyState == EnemyState.Hitstun)
        //    {

        //    }

        //    foreach (ChickenAttack o in theAttack)
        //    {
        //        if (o.visible == true)
        //            o.Update();
        //    }
        //    base.Update();
        //}

        //public override void attack()
        //{
        //    //initial attack
        //    if (facing == 1)
        //        currentAnimation = attackLeftAnim;
        //    //else if (facing == 0)
        //    //    currentAnimation = attackAnim;
        //    animationRect = new Rectangle(0, 0, width, height);
        //    texture = enemyContent.Load<Texture2D>(currentAnimation);
        //    enemyState = EnemyState.Attacking; base.attack();
        //}

        //public override void attack(object sender, ElapsedEventArgs e) //loops attack
        //{
        //    if (canLoop && enemyState != EnemyState.Hitstun)
        //    {
        //        attack();
        //    }
        //    canLoop = false;
        //}

        //public override void UpdateAttack()
        //{
        //    if (currentFrame == 2)
        //    {
        //        if (canAttack)
        //        {
        //            bool makeNew = true;
        //            foreach (ChickenAttack o in theAttack)
        //            {
        //                if (o.visible == false)
        //                {
        //                    o.Reload((int)position.X, (int)position.Y, facing);
        //                    makeNew = false;
        //                    break;
        //                }
        //            }
        //            if (makeNew)
        //            {
        //                theAttack.Add(new ChickenAttack((int)position.X, (int)position.Y, facing));
        //            }
        //            canAttack = false;
        //        }
        //    }

        //    if (currentFrame >= attackLength)
        //    {
        //        canLoop = true;
        //        canAttack = true;
        //        if (facing == 1)
        //            currentAnimation = idleLeftAnim;
        //        //else
        //        //    currentAnimation = idleAnim;
        //        animationRect = new Rectangle(0, 0, width, height);
        //        texture = enemyContent.Load<Texture2D>(currentAnimation);
        //        attackCooldown.Elapsed += new ElapsedEventHandler(attack);
        //        attackCooldown.Enabled = true;
        //    }
        //}

        //public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        //{
        //    base.Draw(spriteBatch, camera);

        //    foreach (ChickenAttack o in theAttack)
        //    {
        //        if (o.visible == true)
        //            o.Draw(spriteBatch, camera);
        //    }
        //}
    }
}

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
    class MoveableObject : Sprite
    {
        public int health = 1;
        protected bool controlsLocked = false;
        protected int maxSpeed;
        public int facing;
        public Vector2 speed;
        protected float accelRate;
        protected float decelRate;
        public float currentAccel;
        public float gravity = 1.5f;
        protected float YVelocity;
        protected List<Wall> wallList;
        public virtual Rectangle UpperBox { get { return new Rectangle((int)position.X + 16, (int)position.Y + 8, 16, 10); } }
        public virtual Rectangle BottomBox { get { return new Rectangle((int)position.X + 16, (int)position.Y + 49, 32, 16); } }
        public virtual Rectangle RightBox { get { return new Rectangle((int)position.X + 50, (int)position.Y + 18, 10, 32); } }
        public virtual Rectangle LeftBox { get { return new Rectangle((int)position.X + 0, (int)position.Y + 18, 10, 32); } }
        public float XSpeed { get { return speed.X; } }
        public float YSpeed { get { return speed.Y; } }
        protected Rectangle animationRect;
        protected String currentAnimation;
        protected int currentFrame;
        protected int totalFrames;
        protected Timer animTimer = new Timer();
        protected String idleAnim;
        protected String idleLeftAnim;
        protected String runAnim;
        protected String runLeftAnim;
        protected String attackAnim;
        protected String attackLeftAnim;
        protected String hurtRight;
        protected String hurtLeft;
        protected String jumpLeft;
        protected String jumpRight;
        protected String deathLeft;
        protected String deathRight;
        public bool hitstun = false;//dictates invin time, player cant be comboed so it only uses hitstun
        protected Timer hitstunTimer = new Timer();

        int debugCounter = 0;
        int debugCounter2 = 0;

        public void UpdateAnimation(object sender, ElapsedEventArgs e)
        {
            currentFrame = animationRect.X / 64;
            totalFrames = (texture.Width / 64) - 1;

            if (currentFrame >= totalFrames)
            {
                //startover
                //currentFrame = 0;
                animationRect = new Rectangle(0, 0, width, height);
            }
            else
            {
                //continue
                animationRect = new Rectangle((currentFrame + 1) * 64, 0, width, height);
            }
        }

        public virtual void startHitstun(int stunTime)
        {
            hitstunTimer.Dispose();
            hitstunTimer = new Timer(stunTime);
            hitstunTimer.Elapsed += new ElapsedEventHandler(endHitstun);
            hitstunTimer.Enabled = true;
        }

        public virtual void endHitstun(object sender, ElapsedEventArgs e)
        {
            hitstunTimer.Dispose();
            hitstun = false;
            controlsLocked = false;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            spriteBatch.Draw(texture, position - camera, animationRect, Color.White);
        }

        public void drawCollisionBox(SpriteBatch spriteBatch, ContentManager content, Vector2 camera)
        {
            Texture2D lol;
            lol = content.Load<Texture2D>("LevelObjects/Block1");
            spriteBatch.Draw(lol, new Rectangle((int)(RightBox.X - camera.X), (int)(RightBox.Y - camera.Y), RightBox.Width, RightBox.Height), Color.White);
            spriteBatch.Draw(lol, new Rectangle((int)(BottomBox.X - camera.X), (int)(BottomBox.Y - camera.Y), BottomBox.Width, BottomBox.Height), Color.White);
            spriteBatch.Draw(lol, new Rectangle((int)(LeftBox.X - camera.X), (int)(LeftBox.Y - camera.Y), LeftBox.Width, LeftBox.Height), Color.White);
            spriteBatch.Draw(lol, new Rectangle((int)(UpperBox.X - camera.X), (int)(UpperBox.Y - camera.Y), UpperBox.Width, UpperBox.Height), Color.White);
            //spriteBatch.Draw(lol, new Rectangle((int)(BoundingBox.X - camera.X), (int)(BoundingBox.Y - camera.Y), BoundingBox.Width, BoundingBox.Height), Color.White);
        }
    }
}

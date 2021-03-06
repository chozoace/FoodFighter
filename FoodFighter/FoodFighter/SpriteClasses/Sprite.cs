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
    class Sprite
    {
        ContentManager myContent;
        SpriteBatch mySpriteBatch;

        public Sprite collidingWall;
        public int width;
        public int height;
        public Vector2 position;
        protected Vector2 previousPosition;
        protected Texture2D texture;
        protected List<Wall> wallList;
        public bool isEnemy = false;
        public virtual Rectangle BoundingBox { get { return new Rectangle((int)position.X, (int)position.Y, width, height); } }
        public string name;
        protected Rectangle animationRect;
        public String currentAnimation;
        protected int currentFrame;
        protected int totalFrames;
        protected Timer animTimer = new Timer();
        public bool passable = false;

        public Sprite()
        {
            myContent = Game1.Instance().getContent();
            mySpriteBatch = Game1.Instance().getSpriteBatch();
        }

        public Sprite(String tex, int x = 0, int y = 0)
        {
            myContent = Game1.Instance().getContent();
            mySpriteBatch = Game1.Instance().getSpriteBatch();
            position = new Vector2(x, y);

            texture = myContent.Load<Texture2D>(tex);
        } 

        public virtual void Update()
        {
            //get the keystate and update movement

        }

        public virtual void UpdateAnimation(object sender, ElapsedEventArgs e)
        {
            if (Game1.Instance().gameState == Game1.GameState.Gameplay)
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
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            spriteBatch.Draw(texture, position - camera, Color.White);
        }

        public virtual bool CheckCollision(Rectangle collisionBox)
        {
            wallList = LevelConstructor.Instance().getWallList();
            foreach (Wall wall in wallList)
            {
                if (wall.BoundingBox.Intersects(collisionBox))
                {
                    collidingWall = wall;
                    return true;
                }
            }
            return false;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public virtual void setPosition(Vector2 newPos)
        {
            position = newPos;
        }
    }
}

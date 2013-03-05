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

namespace FoodFighter
{
    class Sprite
    {
        ContentManager myContent;
        SpriteBatch mySpriteBatch;

        public Sprite collidingWall;
        protected int width;
        protected int height;
        public Vector2 position;
        protected Vector2 previousPosition;
        protected Texture2D texture;
        protected List<Wall> wallList;
        public bool isEnemy = false;
        public virtual Rectangle BoundingBox { get { return new Rectangle((int)position.X, (int)position.Y, width, height); } }

        public Sprite()
        {
            myContent = Game1.Instance().getContent();
            mySpriteBatch = Game1.Instance().getSpriteBatch();
        }

        public Sprite(String tex)
        {
            myContent = Game1.Instance().getContent();
            mySpriteBatch = Game1.Instance().getSpriteBatch();

            texture = myContent.Load<Texture2D>(tex);
        }

        public virtual void Update()
        {
            //get the keystate and update movement

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

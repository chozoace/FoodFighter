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
    class LevelManager
    {
        ContentManager lmContent;
        SpriteBatch lmSpriteBatch;

        public enum LevelState
        {
            Gameplay,
            Restarting,
            NextLevel,
        }
        public LevelState levelState = LevelState.Gameplay;

        public Player player = null;
        LevelConstructor level;
        bool clearLists = false;
        List<Wall> listWalls = new List<Wall>();
        List<Sprite> spriteList = new List<Sprite>();
        List<Enemy> enemyList = new List<Enemy>();
        List<Sprite> spritesToAdd = new List<Sprite>();
        List<Sprite> spritesToRemove = new List<Sprite>();
        Vector2 camera = new Vector2(0, 0);
        float camMoveSpeed = 5f;
        bool paused = false;
        public static LevelManager instance;
        public int lives = 3;

        Sprite background = new Sprite("Backgrounds/BLevel1");
        Vector2 backgroundCamera = new Vector2(0, 0);

        Texture2D blackScreen;
        Color fadeColor = Color.Black;
        Rectangle fadeRect = new Rectangle(0, 0, Game1.Instance().GraphicsDevice.Viewport.Width, Game1.Instance().GraphicsDevice.Viewport.Height);
        int fadeCounter = 5;

        public LevelManager(ContentManager content, SpriteBatch spriteBatch)
        {
            lmContent = content;
            lmSpriteBatch = spriteBatch;
            instance = this;
            Initialize();
        }

        public static LevelManager Instance()
        {
            return instance;
        }

        public void Initialize()
        {
            level = new LevelConstructor();

            LoadContent();
            levelState = LevelState.Gameplay;
            clearLists = false;
        }

        public void LoadContent()
        {
            //level.loadLevel("Content/XML/TestLevel.xml");
            level.loadLevel("Content/XML/Level1.xml");
            listWalls = level.getWallList();
            player.LoadContent(lmContent);
            blackScreen = Game1.Instance().Content.Load<Texture2D>("Menus/BlackScreen");
        }

        public void addToSpriteList(Sprite theSprite)
        {
            spritesToAdd.Add(theSprite);
        }

        public void removefromSpriteList(Sprite theSprite)
        {
            spritesToRemove.Add(theSprite);
        }

        public void addToEnemyList(Enemy enemy)
        {
            enemyList.Add(enemy);
        }

        public void removefromEnemyList(Enemy enemy)
        {
            enemyList.Remove(enemy);
        }

        public List<Enemy> getEnemyList()
        {
            return enemyList;
        }

        public void Update()
        {
            if (levelState == LevelState.Gameplay)
            {
                foreach (Sprite sprite in spriteList.ToList<Sprite>())
                {
                    sprite.Update();
                }
                UpdateCamera();
            }
            else if (levelState == LevelState.Restarting)
            {
                fade();
            }
            if (clearLists)//restarts level
            {
                listWalls.Clear();
                spriteList.Clear();
                enemyList.Clear();
                player = null;
                clearLists = false;
                levelState = LevelState.Gameplay;
                Initialize();
            }
        }

        public void UpdateCamera()
        {
            if (player != null)
            {
                if (player.BoundingBox.Y > camera.Y + 208)
                {
                   if(player.BoundingBox.Y - 208 <= 0)
                        camera.Y = player.BoundingBox.Y + 208;
                }
                if (player.BoundingBox.Y < camera.Y + 208)
                {
                       camera.Y = player.BoundingBox.Y - 208;
                }
                if (player.BoundingBox.X > camera.X + 288)
                {
                    camera.X = player.BoundingBox.X - 288;
                    backgroundCamera.X += 2;
                    if (backgroundCamera.X > 640)
                        backgroundCamera.X = 0;
                }
                if (player.BoundingBox.X < camera.X + 288)
                {
                    camera.X = player.BoundingBox.X - 288;
                    backgroundCamera.X -= 2;
                    if (backgroundCamera.X < -640)
                        backgroundCamera.X = 0;
                }
            }

        }

        public void fade()
        {
            fadeColor.A = (byte)MathHelper.Clamp(fadeColor.A + fadeCounter, 0, 255);

            if (fadeColor.A == 0)
            {
                levelState = LevelState.Gameplay;
                fadeCounter *= -1;
            }
            else if (fadeColor.A == 255)
            {
                clearLists = true;
                fadeCounter *= -1;
                camera.X = player.BoundingBox.X - 288;

            }
        }

        public void drawGame(SpriteBatch spriteBatch)
        {
            if (levelState == LevelState.Gameplay)
            {
                background.Draw(spriteBatch, backgroundCamera);
                background.Draw(spriteBatch, backgroundCamera + new Vector2(640, 0));
                background.Draw(spriteBatch, backgroundCamera - new Vector2(640, 0));
                foreach (Sprite sprite in spriteList)
                {
                    sprite.Draw(spriteBatch, camera);
                }

                //player.drawCollisionBox(spriteBatch, lmContent, camera);

                foreach (Sprite addition in spritesToAdd)
                    spriteList.Add(addition);
                spritesToAdd.Clear();

                foreach (Sprite removed in spritesToRemove)
                    spriteList.Remove(removed);
                spritesToRemove.Clear();
            }
            else if (levelState == LevelState.NextLevel || levelState == LevelState.Restarting)
            {
                background.Draw(spriteBatch, backgroundCamera);
                background.Draw(spriteBatch, backgroundCamera + new Vector2(640, 0));
                background.Draw(spriteBatch, backgroundCamera - new Vector2(640, 0));
                foreach (Sprite sprite in spriteList)
                {
                    sprite.Draw(spriteBatch, camera);
                }

                //player.drawCollisionBox(spriteBatch, lmContent, camera);

                foreach (Sprite addition in spritesToAdd)
                    spriteList.Add(addition);
                spritesToAdd.Clear();

                foreach (Sprite removed in spritesToRemove)
                    spriteList.Remove(removed);
                spritesToRemove.Clear();

                spriteBatch.Draw(blackScreen, fadeRect, fadeColor);
            }
            
        }

        public void restartLevel()
        {
            levelState = LevelState.Restarting;
            fadeColor.A = 0;
            //clearLists = true;

            //MediaPlayer.Stop();
            //MediaPlayer.Play(Game1.Instance().song);
        }

        public void pause()
        {
            
        }
    }
}

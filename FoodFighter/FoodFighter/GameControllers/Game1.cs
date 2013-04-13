using System;
using System.Collections.Generic;
using System.Linq;
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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        EffectComponent effect;
        EffectController controlEffects;

        public enum GameState
        {
            Gameplay,
            MainMenu,
            Pause
        }
        public GameState gameState = GameState.MainMenu;//should be main menu

        LevelManager levelManager;
        MainMenu mainMenu;
        public static Game1 instance;
        bool singletonEnforcer = false;

        public bool EffectsOn = false;
        public bool usingController;
        bool songStarted = false;
        public Song song;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 640;
            //graphics.IsFullScreen = true;
            effect = new EffectComponent(this);
            controlEffects = new EffectController();
            Components.Add(effect);
            usingController = false;

            instance = this;
        }

        public static Game1 Instance()
        {
            return instance;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            mainMenu = new MainMenu();
            levelManager = new LevelManager(Content, spriteBatch);
            

            base.Initialize();    
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            song = Content.Load<Song>("Music/EpicSong");
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (gameState == GameState.Gameplay)
            {
                levelManager.Update();

                if (!songStarted)
                {
                    //MediaPlayer.Play(song);
                    songStarted = true;
                }
            }
            else if (gameState == GameState.MainMenu)
            {
                mainMenu.Update();
            }
            else if (gameState == GameState.Pause)
            {

            }



            if (EffectsOn)
                controlEffects.Update();

            base.Update(gameTime);
        }

        public ContentManager getContent()
        {
            return Content;
        }

        public SpriteBatch getSpriteBatch()
        {
            return spriteBatch;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (EffectsOn)
            {
                GraphicsDevice device = graphics.GraphicsDevice;
                Viewport viewPort = device.Viewport;

                effect.BeginDraw();

                device.Clear(Color.CornflowerBlue);

                spriteBatch.Begin();

                if (gameState == GameState.Gameplay)
                {
                    levelManager.drawGame(spriteBatch);
                }
                else if (gameState == GameState.MainMenu)
                {
                    mainMenu.Draw(spriteBatch);
                }

                spriteBatch.End();

                device.DepthStencilState = DepthStencilState.Default;

            }
            else
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Begin();

                if (gameState == GameState.Gameplay)
                {
                    levelManager.drawGame(spriteBatch);
                }
                else if (gameState == GameState.MainMenu)
                {
                    mainMenu.Draw(spriteBatch);
                }

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
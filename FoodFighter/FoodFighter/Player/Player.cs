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
using System.Xml;
using System.Media;

namespace FoodFighter
{
    class Player : MoveableObject
    {
        ContentManager myContent;
        SpriteBatch mySpriteBatch;

        //variables and properties
        public enum FatState
        {
            Level1,
            Level2,
            Level3
        }
        public FatState fatState;

        public enum PlayerState 
        { 
            Jumping,
            Boosting, 
            Running, 
            Idle,
            Attacking,
            Dead,
            Hitstun,
            AirAttack
        }
        public PlayerState myState, previousState;
        KeyboardState myKeyState, previousKeyState;
        GamePadState previousButtonState;
        public Rectangle healthBar { get { return new Rectangle((int)(position.X - 290), (int)(position.Y - 200), 250, 80); } }
        HealthBar myHealth;
        public HUD hud;
        

        //////////
        //X Move variables
        //////////
        bool leftMove = false;
        bool rightMove = false;
        bool upMove = false;
        float runningDecelRate = 5f;
        

        int maxSpeedLevel2 = 10;
        //////////
        //Jump variables
        //////////
        bool canUpPress = true;
        int jumpSpeedLimit = 10;//???
        int jumpSpeed = -13;//-15
        int jumpSpeedLevel2 = -18;
        float gravityLevel2;
        int fallSpeedLevel2;
        float accelRateLevel2;
        float decelRateLevel2;
        int fallSpeed = 10;
        int wallJumpSpeed = -8;
        int doubleJumpSpeed = -9;
        int jumpCount = 0;
        //////////
        //Attack variables
        //////////
        public int currentChain = 0;
        LightAttack lAttack = null;
        MediumAttack mAttack = null;
        HeavyAttack hAttack = null;
        SpecialAttack sAttack = null;
        GrabOne grab1 = null;
        public bool canAttack = true;
        bool canJpress = true;
        bool canKpress = true;
        bool canLpress = true;
        string attackType = "";
        public bool canChain = false;
        //////////
        //Animation variables
        //////////
        String lightRight;
        String lightLeft;
        String midRight;
        String midLeft;
        String heavyRight;
        String heavyLeft;
        String airAttackLeft;
        String airAttackRight;
        String groundHeavyRight;
        String groundHeavyLeft;
        String throwRight;
        String throwLeft;

        GamePadState padState;
        XmlDocument myStats;
        int firstTransformThreshold = 60;
        int secondTransformThreshold;
        SoundEffect levelUpSound;
        SoundEffectInstance levelUpSoundInstance;

        Timer passableTimer;

        public Player(Vector2 newPos)
        {
            fatState = FatState.Level1;

            hud = new HUD(position.X);
            hud.Font = Game1.Instance().Content.Load<SpriteFont>("Arial");
            hud.Score = 0;
            gravity = 1f;

            myHealth = new HealthBar();

            myStats = new XmlDocument();
            myStats.Load("Content/XML/PlayerSample.xml");
            XmlNode playerNode = myStats.FirstChild;

            maxSpeed = int.Parse(playerNode.Attributes.GetNamedItem("maxSpeed").Value);
            accelRate = float.Parse(playerNode.Attributes.GetNamedItem("accelRate").Value); //0.8
            accelRateLevel2 = float.Parse(playerNode.Attributes.GetNamedItem("accelRateLevel2").Value);
            decelRate = float.Parse(playerNode.Attributes.GetNamedItem("decelRate").Value);//1.8
            decelRateLevel2 = float.Parse(playerNode.Attributes.GetNamedItem("decelRateLevel2").Value);
            fallSpeed = int.Parse(playerNode.Attributes.GetNamedItem("fallSpeed").Value);
            fallSpeedLevel2 = int.Parse(playerNode.Attributes.GetNamedItem("fallSpeedLevel2").Value);
            gravity = float.Parse(playerNode.Attributes.GetNamedItem("gravity").Value);
            gravityLevel2 = float.Parse(playerNode.Attributes.GetNamedItem("gravityLevel2").Value);
            jumpSpeed = int.Parse(playerNode.Attributes.GetNamedItem("jumpSpeed").Value);
            maxSpeedLevel2 = int.Parse(playerNode.Attributes.GetNamedItem("maxSpeedLevel2").Value);
            jumpSpeedLevel2 = int.Parse(playerNode.Attributes.GetNamedItem("jumpSpeedLevel2").Value);
            firstTransformThreshold = int.Parse(playerNode.Attributes.GetNamedItem("firstTransformThreshold").Value);
            secondTransformThreshold = int.Parse(playerNode.Attributes.GetNamedItem("secondTransformThreshold").Value);

            maxHealth = 100;
            health = maxHealth;
            height = 64;
            width = 64;
            position = newPos;
            //maxSpeed = 8;
            speed = new Vector2(0, 0);
            accelRate = 0.8f;
            decelRate = 1.8f;
            currentAccel = 0;

            idleAnim = "Player/HeroIdleRight";
            idleLeftAnim = "Player/HeroIdleLeft";
            runAnim = "Player/HeroWalkRight";
            runLeftAnim = "Player/HeroWalkLeft";
            lightRight = "Player/HeroLightRight";
            lightLeft = "Player/HeroLightLeft";
            midRight = "Player/midChainRight";
            midLeft = "Player/midChainLeft";
            heavyRight = "Player/HeroHeavyRight";
            heavyLeft = "Player/HeroHeavyLeft";
            hurtLeft = "Player/HeroLeftHurt";
            hurtRight = "Player/HeroRightHurt";
            jumpLeft = "Player/HeroJumpLeft";
            jumpRight = "Player/HeroJumpRight";
            airAttackLeft = "Player/HeroAirAttackLeft";
            airAttackRight = "Player/HeroAirAttackRight";
            groundHeavyRight = "Player/HeroAirAttackRight";
            groundHeavyLeft = "Player/HeroAirAttackLeft";
            throwRight = "Player/HeroGrabRight";
            throwLeft = "Player/HeroGrabLeft";
            hitstun = false;
        }

        public void LoadContent(ContentManager content)
        {
            currentAnimation = idleAnim;
            texture = content.Load<Texture2D>(currentAnimation);
            animationRect = new Rectangle(0, 0, width, height);
            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
            animTimer.Enabled = true;

            levelUpSound = content.Load<SoundEffect>("Music/LevelUp");
            levelUpSoundInstance = levelUpSound.CreateInstance();

            jumpCount = 0;
            myState = PlayerState.Idle;
            myContent = content;
        }

        public override void Update()
        {
            myKeyState = Keyboard.GetState();
            padState = GamePad.GetState(PlayerIndex.One);
            if (myState != PlayerState.Dead)
            {
                if (Game1.Instance().usingController)
                {
                    checkKeysDown(padState);
                    checkKeysUp(padState);
                }
                else
                {
                    checkKeysDown(myKeyState);
                    checkKeysUp(myKeyState);
                }
                UpdateMovement(myKeyState);
                UpdateTexture();
                myHealth.Update(health);
            }

            if (health < 1)
            {
                myState = PlayerState.Dead;
                controlsLocked = true;
                death();
            }
            else if (health > maxHealth)
            {
                health = maxHealth;
            }

            if (fatState == FatState.Level1 && hud.Score >= firstTransformThreshold)
            {
                if(LevelManager.Instance().currentLevel == "Content/XML/TutorialLevel.xml")
                    transform();
            }
            if (fatState == FatState.Level1 && hud.Score >= secondTransformThreshold)
            {
                if (LevelManager.Instance().currentLevel != "Content/XML/TutorialLevel.xml")
                    transform();
            }

            if (animTimer.Interval != 100 && (myState != PlayerState.Attacking || myState != PlayerState.AirAttack))
            {
                //animTimer.Dispose();
                //animTimer = new Timer();
                //animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                //animTimer.Enabled = true;
            }
        }

        public void transform()
        {
            if (fatState == FatState.Level1)
            {
                levelUpSoundInstance.Play();

                fatState = FatState.Level2;
                jumpSpeed = jumpSpeedLevel2;
                maxSpeed = maxSpeedLevel2;
                fallSpeed = fallSpeedLevel2;
                gravity = gravityLevel2;
                accelRate = accelRateLevel2;
                decelRate = decelRateLevel2;

                idleAnim = "Player/Level2/HeroIdleRight";
                idleLeftAnim = "Player/Level2/HeroIdleLeft";
                runAnim = "Player/Level2/HeroWalkRight";
                runLeftAnim = "Player/Level2/HeroWalkLeft";
                lightRight = "Player/Level2/HeroLightRight";
                lightLeft = "Player/Level2/HeroLightLeft";
                midRight = "Player/Level2/midChainRight";
                midLeft = "Player/Level2/midChainLeft";
                heavyRight = "Player/Level2/HeroHeavyRight";
                heavyLeft = "Player/Level2/HeroHeavyLeft";
                hurtLeft = "Player/Level2/HeroLeftHurt";
                hurtRight = "Player/Level2/HeroRightHurt";
                jumpLeft = "Player/Level2/HeroJumpLeft";
                jumpRight = "Player/Level2/HeroJumpRight";
                groundHeavyLeft = "Player/Level2/groundHeavyLeft";
                groundHeavyRight = "Player/Level2/groundHeavyRight";

            }
            else if (fatState == FatState.Level2)
            {

            }
        }

        public void death()
        {
            LevelManager.Instance().restartLevel();
        }

        public void UpdateTexture()
        {
            if (!hitstun)
            {
                if (myState == PlayerState.Running)
                {
                    if (facing == 0 && currentAnimation != runAnim)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(runAnim);
                        currentAnimation = runAnim;
                        animTimer.Dispose();
                        animTimer = new Timer(100);
                        animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                        animTimer.Enabled = true;

                    }
                    else if (facing == 1 && currentAnimation != runLeftAnim)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(runLeftAnim);
                        currentAnimation = runLeftAnim;
                        animTimer.Dispose();
                        animTimer = new Timer(100);
                        animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                        animTimer.Enabled = true;
                    }
                }
                else if (myState == PlayerState.Idle)
                {
                    if (facing == 0 && currentAnimation != idleAnim)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(idleAnim);
                        currentAnimation = idleAnim;
                        animTimer.Dispose();
                        animTimer = new Timer(100);
                        animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                        animTimer.Enabled = true;
                    }
                    else if (facing == 1 && currentAnimation != idleLeftAnim)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(idleLeftAnim);
                        currentAnimation = idleLeftAnim;
                        animTimer.Dispose();
                        animTimer = new Timer(100);
                        animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                        animTimer.Enabled = true;
                    }
                }
                else if (myState == PlayerState.Jumping)
                {
                    if (facing == 0 && currentAnimation != jumpRight)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(jumpRight);
                        currentAnimation = jumpRight;
                        animTimer.Dispose();
                        animTimer = new Timer(100);
                        animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                        animTimer.Enabled = true;
                    }
                    else if (facing == 1 && currentAnimation != jumpLeft)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(jumpLeft);
                        currentAnimation = jumpLeft;
                        animTimer.Dispose();
                        animTimer = new Timer(100);
                        animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                        animTimer.Enabled = true;
                    }
                }
                else if (myState == PlayerState.AirAttack)
                {
                    if (facing == 0 && currentAnimation != airAttackRight)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(airAttackRight);
                        currentAnimation = airAttackRight;
                        //animTimer.Dispose();
                        //animTimer = new Timer(200);
                        //animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                        //animTimer.Enabled = true;
                    }
                    else if (facing == 1 && currentAnimation != airAttackLeft)
                    {
                        animationRect = new Rectangle(0, 0, width, height);
                        texture = myContent.Load<Texture2D>(airAttackLeft);
                        currentAnimation = airAttackLeft;
                        //animTimer.Dispose();
                        //animTimer = new Timer(200);
                        //animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                        //animTimer.Enabled = true;
                    }
                }
                else if (myState == PlayerState.Attacking)//light attack
                {
                    if (attackType == "Heavy")
                    {
                        if (facing == 0 && currentAnimation != groundHeavyRight)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(groundHeavyRight);
                            currentAnimation = groundHeavyRight;
                            animTimer.Dispose();
                            if (fatState == FatState.Level1)
                                animTimer = new Timer(200);
                            else
                                animTimer = new Timer(80);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                        else if (facing == 1 && currentAnimation != groundHeavyLeft)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(groundHeavyLeft);
                            currentAnimation = groundHeavyLeft;
                            animTimer.Dispose();
                            if (fatState == FatState.Level1)
                                animTimer = new Timer(200);
                            else
                                animTimer = new Timer(80);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                    }
                    if (attackType == "Throw")
                    {
                        if (facing == 0 && currentAnimation != throwRight)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(throwRight);
                            currentAnimation = throwRight;
                            animTimer.Dispose();
                            animTimer = new Timer(200);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                        else if (facing == 1 && currentAnimation != throwLeft)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(throwLeft);
                            currentAnimation = throwLeft;
                            animTimer.Dispose();
                            animTimer = new Timer(200);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                    }
                    if (currentChain == 1)
                    {
                        if (facing == 0 && currentAnimation != lightRight)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(lightRight);
                            currentAnimation = lightRight;
                            animTimer.Dispose();
                            animTimer = new Timer(200);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                        else if (facing == 1 && currentAnimation != lightLeft)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(lightLeft);
                            currentAnimation = lightLeft;
                            animTimer.Dispose();
                            animTimer = new Timer(200);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                    }
                    if (currentChain == 2)
                    {
                        if (facing == 0 && currentAnimation != midRight)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(midRight);
                            currentAnimation = midRight;
                            animTimer.Dispose();
                            animTimer = new Timer(220);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                        else if (facing == 1 && currentAnimation != midLeft)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(midLeft);
                            currentAnimation = midLeft;
                            animTimer.Dispose();
                            animTimer = new Timer(220);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                    }
                    if (currentChain == 3)
                    {
                        if (facing == 0 && currentAnimation != heavyRight)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(heavyRight);
                            currentAnimation = heavyRight;
                            animTimer.Dispose();
                            animTimer = new Timer(200);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                        else if (facing == 1 && currentAnimation != heavyLeft)
                        {
                            animationRect = new Rectangle(0, 0, width, height);
                            texture = myContent.Load<Texture2D>(heavyLeft);
                            currentAnimation = heavyLeft;
                            animTimer.Dispose();
                            animTimer = new Timer(200);
                            animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                            animTimer.Enabled = true;
                        }
                    }
                }
            }
            else if (hitstun)
            {
                if (facing == 1 && currentAnimation != hurtLeft)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(hurtLeft);
                    currentAnimation = hurtLeft;
                    animTimer.Dispose();
                    animTimer = new Timer(100);
                    animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                    animTimer.Enabled = true;
                }
                else if (facing == 0 && currentAnimation != hurtRight)
                {
                    animationRect = new Rectangle(0, 0, width, height);
                    texture = myContent.Load<Texture2D>(hurtRight);
                    currentAnimation = hurtRight;
                    animTimer.Dispose();
                    animTimer = new Timer(100);
                    animTimer.Elapsed += new ElapsedEventHandler(UpdateAnimation);
                    animTimer.Enabled = true;
                }
            }
        }

        public void lockPlayerControls()
        {
            if (!controlsLocked)
            {
                currentAccel = 0;
                controlsLocked = true;
            }
        }

        public void unlockPlayerControls()
        {
            if (controlsLocked)
                controlsLocked = false;
        }

        public void checkKeysDown(KeyboardState keyState)
        {
            if (!controlsLocked && myState != PlayerState.Hitstun)
            {
                if (keyState.IsKeyDown(Keys.I) == true && previousKeyState.IsKeyUp(Keys.I) == true)
                {
                    hud.Score = 60;
                }
                if (keyState.IsKeyDown(Keys.Escape) == true && previousKeyState.IsKeyDown(Keys.Escape) == true)
                {
                    Game1.Instance().Exit();
                }
                if (keyState.IsKeyDown(Keys.D) == true && previousKeyState.IsKeyDown(Keys.D) == true && myState != PlayerState.Attacking && myState != PlayerState.AirAttack)
                {
                    rightMove = true;
                    facing = 0;
                }
                if (keyState.IsKeyDown(Keys.A) == true && previousKeyState.IsKeyDown(Keys.A) == true && myState != PlayerState.Attacking && myState != PlayerState.AirAttack)
                {
                    leftMove = true;
                    facing = 1;
                }
                if (keyState.IsKeyDown(Keys.W) == true && previousKeyState.IsKeyDown(Keys.W) == true && myState != PlayerState.Jumping)
                {
                    if (!controlsLocked && myState != PlayerState.Attacking && canUpPress)
                    {
                        myState = PlayerState.Jumping;
                        position.Y -= 2;
                        speed.Y = jumpSpeed;
                        canUpPress = false;
                    }
                }
                if (keyState.IsKeyDown(Keys.S) == true && previousKeyState.IsKeyUp(Keys.S) == true && myState != PlayerState.Jumping)
                {
                    if (CheckCollision(BottomBox))
                    {
                        if (collidingWall.name == "ThinFloor" && collidingWall.passable == false)
                        {
                            collidingWall.passable = true;
                            if(passableTimer != null)
                                passableTimer.Dispose();
                            passableTimer = new Timer(1000);
                            passableTimer.Elapsed += new ElapsedEventHandler(solidifyWalls);
                            passableTimer.Enabled = true;
                            //myState = PlayerState.Jumping;
                        }
                    }
                }
                if (keyState.IsKeyDown(Keys.J) == true && previousKeyState.IsKeyDown(Keys.J) == true)
                {
                    if (canAttack && CheckCollision(BottomBox) && canJpress)
                    {
                        attackType = "Light";
                        if (currentChain == 0)
                        {
                            currentChain = 1;

                            currentAccel = 0;
                            speed.X = 0;
                            myState = PlayerState.Attacking;
                            //controlsLocked = true;
                            canAttack = false;
                            canJpress = false;
                            attack();
                        }

                        else if (currentChain == 1)
                        {
                            currentChain = 2;

                            myState = PlayerState.Attacking;//just in case
                            canJpress = false;
                            canAttack = false;

                            if (lAttack != null)
                            {
                                lAttack.removeTimers();
                                lAttack.removeAttack();
                                lAttack = null;
                            }
                            attack();
                        }
                        else if (currentChain == 2 && fatState == FatState.Level2)
                        {
                            currentChain = 3;

                            myState = PlayerState.Attacking;//just in case
                            canJpress = false;
                            canAttack = false;

                            if (mAttack != null)
                            {
                                mAttack.removeTimers();
                                mAttack.removeAttack();
                                mAttack = null;
                            }
                            attack();
                        }
                    }
                    
                }
                if (keyState.IsKeyDown(Keys.K) == true && previousKeyState.IsKeyDown(Keys.K) == true)
                {
                    if (CheckCollision(BottomBox) && currentChain < 1 && canAttack && canKpress)
                    {
                        currentAccel = 0;
                        speed.X = 0;
                        myState = PlayerState.Attacking;
                        canAttack = false;
                        canKpress = false;
                        attackType = "Throw";

                        grab();
                    }
                }
                if (keyState.IsKeyDown(Keys.L) == true && previousKeyState.IsKeyDown(Keys.L) == true)
                {
                    if (CheckCollision(BottomBox) && currentChain < 1 && canAttack && canLpress)
                    {
                        currentAccel = 0;
                        speed.X = 0;
                        myState = PlayerState.Attacking;
                        attackType = "Heavy";
                        canAttack = false;
                        canLpress = false;

                        heavyAttack();
                    }
                    //else if (myState == PlayerState.Jumping && currentChain < 1 && canAttack && canLpress)
                    //{
                    //    currentAccel = 0;
                    //    speed.X = 0;
                    //    speed.Y = 0;
                    //    myState = PlayerState.AirAttack;
                    //    canAttack = false;
                    //    canLpress = false;
                    //    rightMove = false;
                    //    leftMove = false;

                    //    heavyAttack();
                    //}
                }
                if (keyState.IsKeyDown(Keys.P) == true && previousKeyState.IsKeyUp(Keys.P) == true)
                {
                    Debug.WriteLine("hdera");
                    Game1.Instance().pause();
                   // Game1.Instance().gameState = Game1.GameState.Pause;
                }
            }
            previousKeyState = keyState;
        }

        public void checkKeysDown(GamePadState keyState)
        {
            if (!controlsLocked && myState != PlayerState.Hitstun)
            {
                if (keyState.ThumbSticks.Left.X >= 0.3 /* && previousKeyState.IsKeyDown(Keys.D) == true */&& myState != PlayerState.Attacking)
                {
                    rightMove = true;
                    facing = 0;
                }
                if (keyState.ThumbSticks.Left.X <= -0.3 /* && previousKeyState.IsKeyDown(Keys.A) == true*/ && myState != PlayerState.Attacking)
                {
                    leftMove = true;
                    facing = 1;
                }
                if (keyState.IsButtonDown(Buttons.A) == true && previousButtonState.IsButtonDown(Buttons.A) == true && myState != PlayerState.Jumping)
                {
                    if (!controlsLocked && myState != PlayerState.Attacking && canUpPress)
                    {
                        myState = PlayerState.Jumping;
                        position.Y -= 2;
                        speed.Y = jumpSpeed;
                        canUpPress = false;
                    }
                }
                if (keyState.IsButtonDown(Buttons.X) == true && previousButtonState.IsButtonDown(Buttons.X) == true)
                {
                    if (canAttack && CheckCollision(BottomBox) && canJpress)
                    {
                        if (currentChain == 0)
                        {
                            currentChain = 1;

                            currentAccel = 0;
                            speed.X = 0;
                            myState = PlayerState.Attacking;
                            //controlsLocked = true;
                            canAttack = false; //change this, if it stays, cant continue in chain
                            canJpress = false;
                            attack();
                        }

                        else if (currentChain == 1)
                        {
                            currentChain = 2;

                            myState = PlayerState.Attacking;//just in case
                            canJpress = false;
                            canAttack = false;

                            if (lAttack != null)
                            {
                                lAttack.removeTimers();
                                lAttack.removeAttack();
                                lAttack = null;
                            }
                            attack();
                        }
                        else if (currentChain == 2)
                        {
                            currentChain = 3;

                            myState = PlayerState.Attacking;//just in case
                            canJpress = false;
                            canAttack = false;

                            if (mAttack != null)
                            {
                                mAttack.removeTimers();
                                mAttack.removeAttack();
                                mAttack = null;
                            }
                            attack();
                        }
                    }
                }
                if (keyState.IsButtonDown(Buttons.Y) == true && previousButtonState.IsButtonDown(Buttons.Y) == true)
                {
                    if (CheckCollision(BottomBox) && currentChain < 1 && canAttack && canKpress)
                    {
                        currentAccel = 0;
                        speed.X = 0;
                        myState = PlayerState.Attacking;
                        canAttack = false;
                        canKpress = false;

                        grab();
                    }
                }
                if (keyState.IsButtonDown(Buttons.B) == true && previousButtonState.IsButtonDown(Buttons.B) == true)
                {
                    if (CheckCollision(BottomBox) && currentChain < 1 && canAttack && canLpress)
                    {
                        currentAccel = 0;
                        speed.X = 0;
                        myState = PlayerState.Attacking;
                        canAttack = false;
                        canLpress = false;

                        heavyAttack();
                    }
                }
            }
            previousButtonState = keyState;
        }

        public void checkKeysUp(KeyboardState keyState)
        {
            if (myState != PlayerState.Hitstun)
            {
                if (keyState.IsKeyUp(Keys.J) == true)
                {
                    if (canJpress == false)
                        canJpress = true;
                }
                if (keyState.IsKeyUp(Keys.K) == true)
                {
                    if (canKpress == false)
                        canKpress = true;
                }
                if (keyState.IsKeyUp(Keys.L) == true)
                {
                    if (canLpress == false)
                        canLpress = true;
                }
            }

            if (!controlsLocked && myState != PlayerState.Hitstun && myState != PlayerState.Attacking && myState != PlayerState.AirAttack)
            {
                if (keyState.IsKeyUp(Keys.D) == true)
                {
                    rightMove = false;

                    if (CheckCollision(LeftBox) || CheckCollision(RightBox))
                    {
                        if (collidingWall.name != "ThinFloor")
                            currentAccel = 0;
                        if (CheckCollision(BottomBox))
                            currentAccel = 0;
                    }
                    else if (CheckCollision(BottomBox))
                    {
                        currentAccel = 0;
                    }
                }
                if (keyState.IsKeyUp(Keys.A) == true)
                {
                    leftMove = false;

                    if (CheckCollision(LeftBox) || CheckCollision(RightBox))
                    {
                        if (collidingWall.name != "ThinFloor")
                            currentAccel = 0;
                        if (CheckCollision(BottomBox))
                            currentAccel = 0;
                    }
                    else if (CheckCollision(BottomBox))
                    {
                        currentAccel = 0;
                    }
                }
                if (keyState.IsKeyUp(Keys.W) == true)
                {
                    canUpPress = true;
                }
                //if (keyState.IsKeyUp(Keys.J) == true)
                //{
                //    if (canJpress == false)
                //        canJpress = true;
                //}

                if (!leftMove && !rightMove && CheckCollision(BottomBox) && currentChain == 0)
                {
                    myState = PlayerState.Idle;
                }
            }
        }

        public void checkKeysUp(GamePadState keyState)
        {
            if (myState != PlayerState.Hitstun)
            {
                if (keyState.IsButtonUp(Buttons.X) == true)
                {
                    if (canJpress == false)
                        canJpress = true;
                }
                if (keyState.IsButtonUp(Buttons.Y) == true)
                {
                    if (canKpress == false)
                        canKpress = true;
                }
                if (keyState.IsButtonUp(Buttons.B) == true)
                {
                    if (canLpress == false)
                        canLpress = true;
                }
            }

            if (!controlsLocked && myState != PlayerState.Hitstun && myState != PlayerState.Attacking)
            {
                if (keyState.ThumbSticks.Left.X <= 0.3 && keyState.ThumbSticks.Left.X >= 0)
                {
                    rightMove = false;
                    currentAccel = 0;
                }
                if (keyState.ThumbSticks.Left.X >= -0.3 && keyState.ThumbSticks.Left.X <= 0)
                {
                    leftMove = false;
                    currentAccel = 0;
                }
                if (keyState.IsButtonUp(Buttons.A))
                {
                    canUpPress = true;
                }
                //if (keyState.IsKeyUp(Keys.J) == true)
                //{
                //    if (canJpress == false)
                //        canJpress = true;
                //}

                if (!leftMove && !rightMove && CheckCollision(BottomBox) && currentChain == 0)
                {
                    myState = PlayerState.Idle;
                }
            }
        }

        public void UpdateMovement(KeyboardState keyState)
        {
            #region Xmovement
            //if (!hitstun)
            //{
                if (rightMove && !controlsLocked && myState != PlayerState.Attacking)
                {

                    if (CheckCollision(BottomBox))
                    {
                        myState = PlayerState.Running;
                    }

                    if (!CheckCollision(RightBox))
                    {
                        currentAccel = accelRate;
                    }
                }
                if (leftMove && !controlsLocked && myState != PlayerState.Attacking)
                {
                    if (CheckCollision(BottomBox))
                    {
                        myState = PlayerState.Running;
                    }

                    if (!CheckCollision(LeftBox))
                    {
                        currentAccel = -accelRate;
                    }
                }
            //}
            #endregion

            #region gravity
            //makes play fall off ledges when he walks off
            if (!CheckCollision(BottomBox) && myState != PlayerState.AirAttack)
            {
                myState = PlayerState.Jumping;
            }
            else
            {
                if (collidingWall.isEnemy)
                {
                    if (position.X < collidingWall.position.X && speed.Y > 0)
                    {
                        collidingWall.position.X += 15;
                    }
                    else if(position.X > collidingWall.position.X && speed.Y > 0)
                    {
                        collidingWall.position.X -= 15;
                    }
                }
                else if (speed.Y > 0)
                {
                    //if (collidingWall.name != "ThinFloor")
                    //{
                        position.Y = collidingWall.BoundingBox.Y - height;
                        speed.Y = 0;
                    //}
                }
            }

            if (myState == PlayerState.Jumping)
            {
                speed.Y += gravity;

                if (speed.Y > fallSpeed)
                    speed.Y = fallSpeed;

                if(CheckCollision(UpperBox))
                {
                    if (collidingWall.name != "ThinFloor")
                    {
                        position.Y = collidingWall.BoundingBox.Y + collidingWall.BoundingBox.Height;
                        speed.Y *= -1;
                    }
                }
            }
            #endregion

            #region Acceleration
            if (speed.X - 1 < maxSpeed && speed.X + 1 > -maxSpeed)//accelerate player
            {
                if (rightMove && !leftMove)
                {
                    //if (speed.X < maxSpeed || speed.X == 0)
                    //    speed.X += currentAccel;
                    //else if (speed.X > -maxSpeed)
                    //    speed.X -= runningDecelRate;
                    if (myState != PlayerState.Hitstun && myState != PlayerState.Attacking)
                        speed.X = maxSpeed;
                    else
                        speed.X = 0;
                }
                if (leftMove && !rightMove)
                {
                    //if (speed.X > -maxSpeed || speed.X == 0)
                    //    speed.X += currentAccel;
                    //else if (speed.X < maxSpeed)
                    //    speed.X += runningDecelRate;
                    if (myState != PlayerState.Hitstun && myState != PlayerState.Attacking)
                        speed.X = -maxSpeed;
                    else
                        speed.X = 0;
                }
            }

            else if (speed.X > maxSpeed && myState != PlayerState.Boosting)
            {
                speed.X = maxSpeed;
            }
            else if (speed.X < -maxSpeed && myState != PlayerState.Boosting)
            {
                speed.X = -maxSpeed;
            }

            if (currentAccel == 0 && speed.X != 0 && myState != PlayerState.Boosting)
            {
                if (Math.Abs(speed.X) < 1)
                {
                    speed.X = 0;
                }

                else
                {
                    if (myState != PlayerState.Jumping)
                    {
                        if (speed.X > 0)
                            speed.X -= decelRate;
                        else
                            speed.X += decelRate;
                    }
                    else
                    {
                        if (speed.X > 0)
                            speed.X -= 0.4f;
                        else
                            speed.X += 0.4f;
                    }
                }
            }

            #endregion

            #region Collisions
            if (speed.X > 0)
            {
                if (CheckCollision(RightBox))
                {
                    if (collidingWall.name != "ThinFloor")
                    {
                        position.X = collidingWall.BoundingBox.X - 60;//60 is the bounding box position x + its width
                        currentAccel = 0;
                    }
                }
            }

            else if (speed.X < 0)
            {
                if (CheckCollision(LeftBox))
                {
                    if (collidingWall.name != "ThinFloor")
                    {
                        position.X = collidingWall.BoundingBox.X + collidingWall.BoundingBox.Width;
                        currentAccel = 0;
                    }
                }
            }
            #endregion

            previousState = myState;
            previousPosition = position;
            position += speed;
        }

        public void grab()
        {
            grab1 = new GrabOne((int)(position.X), (int)(position.Y), Game1.Instance().getContent(), facing);
        }

        public void attack()
        {
            if (currentChain == 1)
            {
                lAttack = new LightAttack((int)(position.X), (int)(position.Y), Game1.Instance().getContent(), facing);
            }
            else if (currentChain == 2)
            {
                if(fatState == FatState.Level1)
                    mAttack = new MediumAttack((int)(position.X), (int)(position.Y), Game1.Instance().getContent(), facing, true);
                else if(fatState == FatState.Level2)
                    mAttack = new MediumAttack((int)(position.X), (int)(position.Y), Game1.Instance().getContent(), facing, false);
            }
            else if (currentChain == 3)
            {
                hAttack = new HeavyAttack((int)(position.X), (int)(position.Y), Game1.Instance().getContent(), facing);
            }
        }

        public void heavyAttack()
        {
            sAttack = new SpecialAttack((int)(position.X), (int)(position.Y), Game1.Instance().getContent(), facing);
        }

        public void attacksToNull()
        {
            lAttack = null;
            mAttack = null;
            hAttack = null;
            sAttack = null;
        }

        public override bool CheckCollision(Rectangle collisionBox)
        {
            wallList = LevelConstructor.Instance().getWallList();
            List<Enemy> enemyList = LevelManager.Instance().getEnemyList();
            foreach (Wall wall in wallList)
            {
                if (wall.BoundingBox.Intersects(collisionBox) && !wall.passable)
                {
                    collidingWall = wall;
                    if (wall.name == "NextLevel")
                    {
                        LevelManager.Instance().nextLevel();
                    }
                    else if (wall.name == "DeathBlock")
                    {
                        death();
                    }
                    return true;
                }
            }
            foreach (Enemy enemy in enemyList)
            {
                if (collisionBox.Intersects(enemy.BoundingBox))
                {
                    collidingWall = enemy;
                    currentAccel = 0;
                    return true;
                }
            }
            return false;
        }

        public void solidifyWalls(object sender, ElapsedEventArgs e)
        {
            wallList = LevelConstructor.Instance().getWallList();
            foreach (Wall wall in wallList)
            {
                wall.passable = false;
            }
            passableTimer.Dispose();
        }

        public override void startHitstun(int stunTime)
        {
            myState = PlayerState.Hitstun;
            base.startHitstun(stunTime);
        }

        public override void endHitstun(object sender, ElapsedEventArgs e)
        {
            myState = PlayerState.Idle;
            base.endHitstun(sender, e);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            //drawCollisionBox(spriteBatch, myContent, camera);
            Texture2D healthImage = myContent.Load<Texture2D>("Player/healthBarBig");

            myHealth.Draw(spriteBatch, camera, position);
            hud.Draw(spriteBatch, camera, position);

            //spriteBatch.Draw(healthImage, new Rectangle((int)(healthBar.X - camera.X), (int)(healthBar.Y - camera.Y), healthBar.Width, healthBar.Height), Color.White);
            base.Draw(spriteBatch, camera);
        }

    }
}

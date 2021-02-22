/*Author: Denis Kontorovich
 File Name: TowerTrek
 Project Name: MainGame.cs
 Creation Date: May 25 2019
 Modified Date: June 13 2019
 Description:  Game where the player attempts to jump on top of falling platforms to avoid falling into the lava below.
 The player has to avoid additional spikes placed on the right and left wall of the games. The player starts off with
 three lives and every time they hit any sort of spike they lose a life. When the player has zero lives left then the game is over.
 There are four types of platforms that can fall, either a regular platform, a mud platform, an ice platform, or spike platform. If a player
 lands on a mud platform it is harder for the player to move left or right, and they can't jump up as high. If they land on an ice platform
 they can easily slide off of the platform and into the lava. If the player lands on a spike platform they bounce off and lose a life.
 At the same time as all of this is occuring a power up and a health badge fall from the sky. If the player gets a health badge than they
 get one more life if they had less than 3 lives already. If the player gets a power up then they become invisible until the power up bar
 runs out where they go back to normal. While invinsible the player travels extremely fast towards the top passing by multiple platforms and 
 increasing their distance travelled and score exponentially. Note, the player can also perform a double jump if they double tap jump. However
 this can only occur when the double jump bar is full to a certain extent. If the double jump is at its lower limit than a double jump cannot occur.
 The point of the game is for the player to get the highest score possible, and distance travelled.
 */
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;

namespace TowerTrek
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Store both the current and previous mouse states for user input
        MouseState mouse;
        MouseState prevMouse;

        //Keyboard state variables
        KeyboardState kb;
        KeyboardState prevKb;

        //The random object allowing for random number generation during Shuffle
        Random rng = new Random();

        //Store the various timers used throughout the game
        float difficultyTimer;
        float startingTimer;

        //Store the value of the horizontal acceleration, friction, and height of a jump for the player
        float acceleration = 0.2f;
        float friction = 0f;
        float jumpSpeed = -8f;

        //Maximum horizontal speed the player can travel
        float maxSpeed = 15f;
        
        //Constant values to represent the different screens in Game
        const int MAIN_MENU = 0;
        const int INSTRUCTIONS = 1;
        const int OPTIONS = 2;
        const int GAMEPLAY = 3;
        const int GAMEOVER = 4;

        //Constant values to represent the different types of platforms
        const int REG = 0;
        const int MUD = 1;
        const int ICE = 2;
        const int SPIKE = 3;

        //Spacer or initial value for x, y coordinates and height for various images
        const int X_WALL_SPIKES_SPACER = 60;
        const int Y_WALL_SPIKES_SPACER = 7;
        const int X_LIVES_SPACER = 175;
        const int Y_SELECTION_SPACER = 35;
        const int H_S_PLATFORM_SPACER = 40;
        const int P_FEET_X_SPACER = 12;
        const int P_FEET_Y_SPACER = 60;
        
        //Volume of the Audio in game
        int audioVolume = 5;

        //Store the current game state
        int gameState = MAIN_MENU;

        //Store the dimenstions of the screen
        int screenWidth;
        int screenHeight;

        //Store the players score
        int direction;

        //Store statistics for game
        int score = 0;
        int highScore;
        int dTravelled = 0;
        int gamesPlayed = 0;

        //Store speed for starting platform
        int startingPlatformSpeed = 1;

        //Store number of lives the player has
        int lives = 3;

        //Array for types of platforms
        int[] platformType = new int[5];

        //Store speed at which the screen is scorlling
        int scrollingSpeed = 1;

        //Store the number of jumps the player has made
        int jumpCounter = 0;

        //Store counter for whether player has hit platform, or spikes
        int hitCounter = 0;
        int spikeWallHitCounter = 0;

        //Store the number of jumps the player has available
        int jumpsAvailable = 2;

        //Store the difficulty of game
        int difficulty = 0;

        //Store platform type
        int platTypeNum;

        //Store the maximum volume
        int maxVolume = 10;

        //Store all images to be used throughout the game
        Texture2D gameplayBgImg;
        Texture2D playerImg;
        Texture2D platformImg;
        Texture2D blankImg;
        Texture2D leftWallSpikesImg;
        Texture2D rightWallSpikesImg;
        Texture2D lavaImg;
        Texture2D livesImg;
        Texture2D blackBoxImg;
        Texture2D leftWallImg;
        Texture2D rightWallImg;
        Texture2D jumpBarImg;
        Texture2D powerBarImg;
        Texture2D mudPlatformImg;
        Texture2D iceyPlatformImg;
        Texture2D spikesPlatformImg;
        Texture2D powerUpImg;
        Texture2D healthUpImg;
        Texture2D titleBgImg;
        Texture2D plusImg;
        Texture2D minusImg;
        Texture2D whitePageImg;
        Texture2D backArrowImg;
        Texture2D instructionsBgImg;
        Texture2D optionsBgImg;
        Texture2D gameOverBgImg;
        Texture2D mainMenuButtonImg;
        Texture2D continueButtonImg;
        Texture2D playAgainButtonImg;
        Texture2D leftButtonImg;
        Texture2D rightButtonImg;
        Texture2D controlsTextImg;
        Texture2D soundTextImg;
        Texture2D instructionsTextImg;
        Texture2D number1Img;
        Texture2D number2Img;
        Texture2D number3Img;
        Texture2D stoneFloorImg;
        Texture2D deadLinkImg;

        //Store value of the rectangles that will hold the images
        Rectangle plusRec;
        Rectangle minusRec;
        Rectangle playerRec;
        Rectangle gameplayBgRec1;
        Rectangle gameplayBgRec2;
        Rectangle[] leftWallSpikesRec = new Rectangle[6];
        Rectangle[] rightWallSpikesRec = new Rectangle[6];
        Rectangle lavaRec;
        Rectangle[] livesRec = new Rectangle[3];
        Rectangle[] selectionBoxRec = new Rectangle[3];
        Rectangle leftWallRec;
        Rectangle rightWallRec;
        Rectangle playerFeetRec;
        Rectangle[] platformRec = new Rectangle[5];
        Rectangle startingPlatformRec;
        Rectangle jumpBarOutline;
        Rectangle jumpBarRec;
        Rectangle powerBarOutline;
        Rectangle powerBarRec;
        Rectangle powerUpRec;
        Rectangle healthUpRec;
        Rectangle whiteRec;
        Rectangle backArrowRec;
        Rectangle playerRec2;
        Rectangle bgRec;
        Rectangle mainMenuButtonRec;
        Rectangle playAgainButtonRec;
        Rectangle continueButtonRec;
        Rectangle leftButtonRec;
        Rectangle rightButtonRec;
        Rectangle controlsTextRec;
        Rectangle soundTextRec;
        Rectangle instructionsTextRec;
        Rectangle numberRec;
        Rectangle deadLinkRec;
        Rectangle playAgainButtonRec2;
        Rectangle mainMenuButtonRec2;
        Rectangle instructionsDoubleJumpRec;
        Rectangle instructionsPowerUpRec;

        //Store all the fonts to be used throughout the game
        SpriteFont gameLabel;
        SpriteFont subtitleLabel;

        //Store location of the texts displayed throughout the game
        Vector2 scoreLoc = new Vector2(120, 0);
        Vector2 highScoreLoc = new Vector2(120, 20);
        Vector2 dTravelledLoc = new Vector2(120, 40);Vector2 instructionsLoc;
        Vector2 optionsLoc;
        Vector2 startGameLoc;
        Vector2 audioVolumeLoc = new Vector2(342, 263);
        Vector2 pauseLoc;
        Vector2 moveLeftLoc = new Vector2(250, 550);
        Vector2 moveRightLoc = new Vector2(250, 590);
        Vector2 jumpLoc = new Vector2(250, 630);
        Vector2 finalScoreLoc = new Vector2(250, 460);
        Vector2 finalHighScoreLoc = new Vector2(250, 500);
        Vector2 finalDTravelledLoc = new Vector2(250, 540);
        Vector2 instructionsPowerUpLoc = new Vector2(200, 620);
        Vector2 instructionsDoubleJumpLoc = new Vector2(450, 620);

        //Store the true locations of the platforms, player, power up, and health potion
        Vector2[] platformLoc = new Vector2[5];
        Vector2 playerLoc;
        Vector2 powerUpLoc;
        Vector2 healthUpLoc;

        //Store vector values for speed and gravity for various objects
        Vector2 platformSpeed;
        Vector2 playerVelocity;
        Vector2 gravity = new Vector2(0f, 9.81f / 60f);

        //Store the texts to be displayed throughout the game
        string instructionsOutput = "INSTRUCTIONS";
        string optionsOutput = "OPTIONS";
        string scoreOutput = "Score: ";
        string highScoreOutput = "High Score: ";
        string startGameOutput = "START GAME ";
        string dTravelledOutput = "Distance: ";
        string pauseOutput = "PAUSE";
        string leftKey = "Left";
        string rightKey = "Right";
        string moveUp = "Space";

        //Store whether the game is paused
        bool pause = false;

        //Store whether the user has a power up
        bool powerUp = false;

        //Store whether the user is right handed
        bool rightHanded = true;

        //Store whether the game is ready to begin
        bool gameStart = false;

        //Store whether the starting platform is to be displayed
        bool platformBGone = false;

        //Store variable for whether to reset the game or not
        bool resetGame = false;

        //Store Music
        Song bgMusic;
        Song gameBgMusic;

        //Store sound effects 
        SoundEffect jumpEffect;
        SoundEffect spikeEffect;
        SoundEffect powerUpEffect;
        SoundEffect gameOverEffect;
        SoundEffect healthUpEffect;
        SoundEffect buttonClickEffect;
        SoundEffect platformLandEffect;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Allows cursor to be seen by user in game
            this.IsMouseVisible = true;

            //Change the length and width of the screen
            this.graphics.PreferredBackBufferWidth = 700;
            this.graphics.PreferredBackBufferHeight = 752;
            this.graphics.ApplyChanges();
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load all images to be used throughout the game
            gameplayBgImg = Content.Load<Texture2D>("Images/Backgrounds/brickBackground2");
            playerImg = Content.Load<Texture2D>("Images/Sprites/Player");
            platformImg = Content.Load<Texture2D>("Images/Sprites/Platform");
            blankImg = Content.Load<Texture2D>("Images/Sprites/whiteBoxPng");
            leftWallSpikesImg = Content.Load<Texture2D>("Images/Backgrounds/LeftWallSpikes");
            rightWallSpikesImg = Content.Load<Texture2D>("Images/Backgrounds/rightWallSpikes");
            lavaImg = Content.Load<Texture2D>("Images/Backgrounds/lava");
            livesImg = Content.Load<Texture2D>("Images/Sprites/lives");
            blackBoxImg = Content.Load<Texture2D>("Images/Sprites/blackBox");
            leftWallImg = Content.Load<Texture2D>("Images/Backgrounds/leftBrickWall");
            rightWallImg = Content.Load<Texture2D>("Images/Backgrounds/rightBrickWall");
            jumpBarImg = Content.Load<Texture2D>("Images/Sprites/blueBox");
            powerBarImg = Content.Load<Texture2D>("Images/Sprites/pinkBox");
            mudPlatformImg = Content.Load<Texture2D>("Images/Sprites/mudPlatform");
            iceyPlatformImg = Content.Load<Texture2D>("Images/Sprites/iceyPlatform");
            spikesPlatformImg = Content.Load<Texture2D>("Images/Sprites/spikePlatform");
            powerUpImg = Content.Load<Texture2D>("Images/Sprites/powerUp");
            healthUpImg = Content.Load<Texture2D>("Images/Sprites/healthUp");
            titleBgImg = Content.Load<Texture2D>("Images/Backgrounds/titleBackground");
            plusImg = Content.Load<Texture2D>("Images/Sprites/plusSign");
            minusImg = Content.Load<Texture2D>("Images/Sprites/minusSign");
            whitePageImg = Content.Load<Texture2D>("Images/Backgrounds/whiteBox");
            backArrowImg = Content.Load<Texture2D>("Images/Sprites/backArrow");
            instructionsBgImg = Content.Load<Texture2D>("Images/Backgrounds/instructionsBackground");
            optionsBgImg = Content.Load<Texture2D>("Images/Backgrounds/optionsBackground");
            gameOverBgImg = Content.Load<Texture2D>("Images/Backgrounds/gameOverBackground");
            mainMenuButtonImg = Content.Load<Texture2D>("Images/Sprites/mainMenuButton");
            continueButtonImg = Content.Load<Texture2D>("Images/Sprites/continueButton");
            playAgainButtonImg = Content.Load<Texture2D>("Images/Sprites/playAgainButton");
            leftButtonImg = Content.Load<Texture2D>("Images/Sprites/leftButton");
            rightButtonImg = Content.Load<Texture2D>("Images/Sprites/rightButton");
            controlsTextImg = Content.Load<Texture2D>("Images/Sprites/controlsText");
            soundTextImg = Content.Load<Texture2D>("Images/Sprites/soundText");
            instructionsTextImg = Content.Load<Texture2D>("Images/Sprites/instructionsText");
            number1Img = Content.Load<Texture2D>("Images/Sprites/number1");
            number2Img = Content.Load<Texture2D>("Images/Sprites/number2");
            number3Img = Content.Load<Texture2D>("Images/Sprites/number3");
            stoneFloorImg = Content.Load<Texture2D>("Images/Sprites/stoneFloor");
            deadLinkImg = Content.Load<Texture2D>("Images/Sprites/deadLink3");

            //Load all the fonts to be used throughout the game
            gameLabel = Content.Load<SpriteFont>("Fonts/gameLabelFont");
            subtitleLabel = Content.Load<SpriteFont>("Fonts/subtitleFont");

            //Load all the sound effects to be used throughout the game
            jumpEffect = Content.Load<SoundEffect>("Audio/Effects/jumpEffect");
            spikeEffect = Content.Load<SoundEffect>("Audio/Effects/spikeEffect");
            gameOverEffect = Content.Load<SoundEffect>("Audio/Effects/gameOverEffect");
            powerUpEffect = Content.Load<SoundEffect>("Audio/Effects/powerUpEffect");
            healthUpEffect = Content.Load<SoundEffect>("Audio/Effects/healthUpEffect");
            buttonClickEffect = Content.Load<SoundEffect>("Audio/Effects/buttonClick");
            platformLandEffect = Content.Load<SoundEffect>("Audio/Effects/platformLand");

            //Load all music to be used throughout the game
            bgMusic = Content.Load<Song>("Audio/Music/castleBackgroundMusic");
            gameBgMusic = Content.Load<Song>("Audio/Music/gameplayBgMusic");

            //Load all Rectangles to be used throughout the game
            gameplayBgRec1 = new Rectangle(0, 0, screenWidth, screenHeight);
            gameplayBgRec2 = new Rectangle(0, -screenHeight, screenWidth, screenHeight);
            lavaRec = new Rectangle(0, screenHeight - (int)(lavaImg.Height * 1.5), screenWidth, (int)(lavaImg.Height * 1.5));
            leftWallRec = new Rectangle(0, 0, 60, (int)(leftWallImg.Height));
            rightWallRec = new Rectangle(screenWidth - 60, 0, 75, (int)(rightWallImg.Height));
            powerUpRec = new Rectangle(rng.Next(100, 460), rng.Next(-1500, -1000), (int)(powerUpImg.Width / 13), (int)(powerUpImg.Height / 14));
            healthUpRec = new Rectangle(rng.Next(100, 460), rng.Next(-1500, -1000), (int)(healthUpImg.Width / 20), (int)(healthUpImg.Height / 20));
            playerRec = new Rectangle(300, 570, (int)(playerImg.Width / 3.8), (int)(playerImg.Height / 3.8));
            playerRec2 = new Rectangle(300, 300, (int)(playerImg.Width / 3.2), (int)(playerImg.Height / 3.2));
            plusRec = new Rectangle(200, 250, (int)(plusImg.Width / 12), (int)(plusImg.Height / 12));
            minusRec = new Rectangle(440, 250, (int)(minusImg.Width / 12), (int)(minusImg.Height / 12));
            whiteRec = new Rectangle(0, 200, screenWidth, 300);
            backArrowRec = new Rectangle(0, screenHeight - (int)(backArrowImg.Height / 8), (int)(backArrowImg.Width / 8), (int)(backArrowImg.Height / 8));
            bgRec = new Rectangle(0, 0, screenWidth, screenHeight);
            playerFeetRec = new Rectangle(playerRec.X + 12, playerRec.Y + 60, playerRec.Width - 48, playerRec.Height - 60);
            jumpBarOutline = new Rectangle(5, 200, 40, 200);
            jumpBarRec = new Rectangle(5, 200, 40, 200);
            powerBarOutline = new Rectangle(655, 200, 40, 200);
            powerBarRec = new Rectangle(655, 200, 40, 200);
            mainMenuButtonRec = new Rectangle(150, 350, (int)(mainMenuButtonImg.Width), (int)(mainMenuButtonImg.Height));
            continueButtonRec = new Rectangle(350, 350, (int)(continueButtonImg.Width), (int)(continueButtonImg.Height));
            playAgainButtonRec = new Rectangle(380, 350, (int)(playAgainButtonImg.Width), (int)(playAgainButtonImg.Height));
            leftButtonRec = new Rectangle(220, 470, (int)(leftButtonImg.Width), (int)(leftButtonImg.Height));
            rightButtonRec = new Rectangle(370, 470, (int)(rightButtonImg.Width), (int)(rightButtonImg.Height));
            controlsTextRec = new Rectangle(240, 360, (int)(controlsTextImg.Width / 1.23), (int)(controlsTextImg.Height / 1.23));
            soundTextRec = new Rectangle(260, 150, (int)(soundTextImg.Width / 1.23), (int)(soundTextImg.Height / 1.23));
            instructionsTextRec = new Rectangle(135, 120, (int)(instructionsTextImg.Width), 500);
            numberRec = new Rectangle(300, 250, (int)(number3Img.Width / 5), (int)(number3Img.Height / 5));
            deadLinkRec = new Rectangle(80, 280, (int)(deadLinkImg.Width / 2), (int)(deadLinkImg.Height / 2));
            mainMenuButtonRec2 = new Rectangle(150, 650, (int)(mainMenuButtonImg.Width), (int)(mainMenuButtonImg.Height));
            playAgainButtonRec2 = new Rectangle(400, 650, (int)(mainMenuButtonImg.Width), (int)(mainMenuButtonImg.Height));
            instructionsDoubleJumpRec = new Rectangle(220, 670, 140, 50);
            instructionsPowerUpRec = new Rectangle(485, 670, 140, 50);

            //For all the numbers between 0 and the leftWallSpikesRec.Length set the appropriate rectangles
            for (int i = 0; i < leftWallSpikesRec.Length; i++)
            {
                //Create rectangle for the spikes on the left wall
                leftWallSpikesRec[i] = new Rectangle(-2 + X_WALL_SPIKES_SPACER, 0 + (leftWallSpikesImg.Height * i) - (Y_WALL_SPIKES_SPACER * i), (int)(leftWallSpikesImg.Width / 2.5), (int)(leftWallSpikesImg.Height));
            }

            //For all the numbers between 0 and the rightWallSpikesRec.Length set the appropriate rectangles 
            for (int i = 0; i < rightWallSpikesRec.Length; i++)
            {
                //Create rectangle for the spikes on the right wall
                rightWallSpikesRec[i] = new Rectangle(screenWidth - X_WALL_SPIKES_SPACER - (int)(leftWallSpikesImg.Width / 2.5) + 2, (rightWallSpikesImg.Height * i) - (Y_WALL_SPIKES_SPACER * i), (int)(rightWallSpikesImg.Width / 2.5), (int)(rightWallSpikesImg.Height));
                
            }
            
            //For all the numbers between 0 and livesRec.Length set the appropriate rectangles
            for (int i = 0; i < livesRec.Length; i++)
            {
                //Create rectangle for the hearts which represent the players lives 
                livesRec[i] = new Rectangle(screenWidth - X_LIVES_SPACER - ((int)(livesImg.Width / 2) * i), 0, (int)(livesImg.Width / 2.2), (int)(livesImg.Height / 2.2));
            }

            //For all the numbers between  0 and selectionBoxRec.Length set the appropriate rectangles
            for (int i = 0; i < selectionBoxRec.Length; i++)
            {
                //Create rectangles to be used to select which  screen to go to while in the main menu
                selectionBoxRec[i] = new Rectangle((screenWidth / 2) - ((int)(blackBoxImg.Width / 5.5) / 2), 450 + ((int)(blackBoxImg.Height / 12.4) * i) + (Y_SELECTION_SPACER * i), (int)(blackBoxImg.Width / 5.15), (int)(blackBoxImg.Height / 12.4));
            }

            //For all the numbers between  0 and platformRec.Length set the appropriate rectangles
            for (int i = 1; i < platformRec.Length; i++)
            {
                //Create rectangles for the platforms which will come falling down
                platformRec[i] = new Rectangle(rng.Next(120, 440), 0 - (((screenHeight - lavaRec.Height) / 4) * i), (int)(platformImg.Width / 2.5), (int)(platformImg.Height / 1.2));

                //Create Vector2 for the true location of the platforms
                platformLoc[i] = new Vector2(platformRec[i].X, platformRec[i].Y);
            }

            //For all the numbers between  0 and platformRec.Length
            for (int i = 0; i < platformRec.Length; i++)
            {
                //Set the first for platforms which come down to be regular platforms
                platformType[i] = REG;
            }
            
            //Set the speed of various objects in game
            platformSpeed = new Vector2(0, 3f);
            playerVelocity = new Vector2(0, 0);
            powerUpLoc = new Vector2(powerUpRec.X, powerUpRec.Y);
            healthUpLoc = new Vector2(healthUpRec.X, healthUpRec.Y);

            //Set the true location of the player
            playerLoc = new Vector2(playerRec.X, playerRec.Y);

            //Set the location of the text which will display the possibles screens available
            instructionsLoc = new Vector2(selectionBoxRec[0].X, selectionBoxRec[0].Y);
            optionsLoc = new Vector2(selectionBoxRec[1].X + 50, selectionBoxRec[1].Y);
            startGameLoc = new Vector2(selectionBoxRec[2].X, selectionBoxRec[2].Y);
            pauseLoc = new Vector2 (whiteRec.X + 300, whiteRec.Y + 50);

            //Set the platform which the player will start off standing on at the beginning of the game
            startingPlatformRec = new Rectangle(0, screenHeight - lavaRec.Height - H_S_PLATFORM_SPACER, screenWidth, 40);

            //Set the volume of the audio in game
            MediaPlayer.Volume = 0.5f;
            
            //PLay background music on repeat
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgMusic);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Track the state of the keyboard
            prevKb = kb;
            kb = Keyboard.GetState();

            //Get the current mouse state
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //Perform the appropriate task based on which screen the player is on
            switch (gameState)
            {
                case MAIN_MENU:
                    {
                        //When the mouse is clicked perform the appropriate actions
                        if (NewMouseClick())
                        {
                            //Check for the location of the mouse
                            if (MouseClicked(selectionBoxRec[0]) == true)
                            {
                                //Play the button clicked sound effect
                                buttonClickEffect.CreateInstance().Play();

                                //Current game state is in instructions
                                gameState = INSTRUCTIONS;
                            }
                            else if (MouseClicked(selectionBoxRec[1]) == true)
                            {
                                //Play the button clicked sound effect
                                buttonClickEffect.CreateInstance().Play();

                                //Current game state is in options
                                gameState = OPTIONS;
                            }
                            else if (MouseClicked(selectionBoxRec[2]) == true)
                            {
                                //Play the button clicked sound effect
                                buttonClickEffect.CreateInstance().Play();

                                //Play the gameplay background music
                                MediaPlayer.Play(gameBgMusic);

                                //When player has played at least one game
                                if(gamesPlayed >= 1)
                                {
                                    //Call Reset game
                                    ResetGame();
                                }

                                //Current game state is in game play
                                gameState = GAMEPLAY;
                            } 
                        }
                    }
                    break;
                case INSTRUCTIONS:
                    {
                        //When the mouse is clicked perform the appropriate actions
                        if(NewMouseClick())
                        {
                            //Check mouse location
                            if(MouseClicked(backArrowRec))
                            {
                                //Play the button clicked sound effect
                                buttonClickEffect.CreateInstance().Play();

                                //Current game state is in main menu
                                gameState = MAIN_MENU;
                            }
                        }
                    }
                    break;
                case OPTIONS:
                    {
                        //When the mouse is clicked perform the appropriate actions
                        if(NewMouseClick())
                        {
                            //Check mouse location
                            if (MouseClicked(plusRec))
                            {
                                //When the volume of the audio is less than the maximum limit for the volume
                                if (audioVolume < maxVolume)
                                { 
                                    //Play the button clicked sound effect
                                    buttonClickEffect.CreateInstance().Play();

                                    //Increase the volume of the audio
                                    audioVolume += 1;
                                    MediaPlayer.Volume += 0.1f;
                                }
                            }
                            else if (MouseClicked(minusRec))
                            {
                                //When the volume of the audio is less than 0
                                if (audioVolume > 0)
                                {
                                    //Play the button clicked sound effect
                                    buttonClickEffect.CreateInstance().Play();

                                    //Decrease the volume of the audio
                                    audioVolume += -1;
                                    MediaPlayer.Volume += -0.1f;
                                }
                            }
                            //When the mouse location is within the back arrow button
                            else if (MouseClicked(backArrowRec))
                            {
                                //Play the button clicked sound effect
                                buttonClickEffect.CreateInstance().Play();

                                //Current game state is in the main menu
                                gameState = MAIN_MENU;
                            }
                            else if(MouseClicked(rightButtonRec))
                            {
                                //Play the button clicked sound effect
                                buttonClickEffect.CreateInstance().Play();

                                //Player is right handed
                                rightHanded = true;

                                //Set appropriate left and right key
                                leftKey = "Left";
                                rightKey = "Right";
                            }
                            else if (MouseClicked(leftButtonRec))
                            {
                                //Play the button click sound effect
                                buttonClickEffect.CreateInstance().Play();

                                //Player is left handed
                                rightHanded = false;

                                //Set appropriate left and right key
                                leftKey = "A";
                                rightKey = "D";
                            }
                        }
                    }
                    break;
                case GAMEPLAY:
                    {
                        //When player has 3 lives and the game is to be reset
                        if (lives == 3 && resetGame == true)
                        {
                            //Set player,health up, and power up to starting location
                            playerLoc.X = 300;
                            playerLoc.Y = 570;
                            playerRec.X = (int)(playerLoc.X);
                            playerRec.Y = (int)(playerLoc.Y);
                            powerUpLoc.X = rng.Next(100, 440);
                            powerUpLoc.Y = rng.Next(-1500, -1000);
                            powerUpRec.X = (int)(powerUpLoc.X);
                            powerUpRec.Y = (int)(powerUpLoc.Y);
                            healthUpLoc.X = rng.Next(100, 440);
                            healthUpLoc.Y = rng.Next(-1500, -1000);
                            healthUpRec.X = (int)(healthUpLoc.X);
                            healthUpRec.Y = (int)(healthUpLoc.Y);
                        }

                        //Start timer to find out whent the game is going to start
                        startingTimer += (float)(gameTime.ElapsedGameTime.TotalSeconds);

                        //When timer is greater than about 3 seconds
                        if(startingTimer > 3.2)
                        {
                            //The game has now started
                            gameStart = true;
                        }

                        //When the game has started perform the appropriate actions
                        if (gameStart == true)
                        {
                            //Game is to no longer be reset
                            resetGame = false;

                            //Start difficulty timer
                            difficultyTimer += (float)(gameTime.ElapsedGameTime.TotalSeconds);

                            //When the game is not paused perform the appropriate actions
                            if (pause == false)
                            {
                                //Call increase difficulty sub program
                                IncreaseDifficulty();

                                //Call the scroll screen sub program
                                ScrollScreen();

                                //Call the horizontal movement subprogram
                                HorizontalMovement();

                                //For every number between 0 and platformRec.Length perform the appropriate tasks
                                for (int i = 0; i < platformRec.Length; i++)
                                {
                                    //When the platform has reached the lava
                                    if (BoxBoxCollision(platformRec[i], lavaRec))
                                    {
                                        //Bring the platform back up to the top of the screen
                                        platformLoc[i].Y -= screenHeight;

                                        //Bring the platform to a random x location
                                        platformLoc[i].X = rng.Next(100, 460);

                                        //Set the location of the platform rectangle to the true location of the platform
                                        platformRec[i].X = (int)(platformLoc[i].X);
                                        platformRec[i].Y = (int)(platformLoc[i].Y);

                                        //Generate random platform type
                                        platTypeNum = rng.Next(1, 101);

                                        //Check for the platform type
                                        if (platTypeNum <= 70)
                                        {
                                            //The type of platform is a regular platform
                                            platformType[i] = REG;
                                        }
                                        else if (platTypeNum <= 85)
                                        {
                                            //The type of platform is a mud platform
                                            platformType[i] = MUD;
                                        }
                                        else if (platTypeNum <= 95)
                                        {
                                            //The type of platform is a ice platform
                                            platformType[i] = ICE;
                                        }
                                        else if (platTypeNum <= 100)
                                        {
                                            //The type of platform is a spike platform
                                            platformType[i] = SPIKE;
                                        }
                                    }
                                }

                                //When the player does not possess the power up
                                if (powerUp == false)
                                {
                                    //When the starting platform is off of the screen
                                    if (startingPlatformRec.Y > lavaRec.Y)
                                    {
                                        //Increase distance travelled
                                        dTravelled += 1;
                                    }

                                    //For every number between 0 and platformRec.Length perform the appropriate tasks
                                    for (int i = 0; i < platformRec.Length; i++)
                                    {
                                        //Check player and platform collision
                                        if (BoxBoxCollision(playerFeetRec, platformRec[i]) == true)
                                        {
                                            //When the platform is a certain type then perform the appropriate tasks
                                            switch (platformType[i])
                                            {
                                                case REG:
                                                    {
                                                        //Set friction
                                                        friction = 0.1f;

                                                        //Set number of jumps performed by player to 0
                                                        jumpCounter = 0;

                                                        //When player has collided with the platform for the first time
                                                        if (hitCounter == 0)
                                                        {
                                                            //Call the increment sub program
                                                            Increment();
                                                        }

                                                        //When the player's trajectory is in the downward direction
                                                        if (playerVelocity.Y > 0)
                                                        {
                                                            //Call Set player platform program
                                                            SetPlayerPlatform(i);
                                                        }
                                                    }
                                                    break;
                                                case MUD:
                                                    {
                                                        //Set friction value
                                                        friction = 0.19f;

                                                        //Set jump speed 
                                                        jumpSpeed = -5f;

                                                        //Set number of jumps performed by player to 0
                                                        jumpCounter = 0;

                                                        //When player has collided with the platform for the first time
                                                        if (hitCounter == 0)
                                                        {
                                                            //Call the increment sub program
                                                            Increment();
                                                        }

                                                        //When the player's trajectory is in the downward direction
                                                        if (playerVelocity.Y > 0)
                                                        {
                                                            //Call Set player platform program
                                                            SetPlayerPlatform(i);
                                                        }
                                                    }
                                                    break;
                                                case ICE:
                                                    {
                                                        //Set friction
                                                        friction = 0f;

                                                        //Set number of jumps performed by player to 0
                                                        jumpCounter = 0;

                                                        //When the player has collided with the platform for the first time
                                                        if (hitCounter == 0)
                                                        {
                                                            //Call the increment sub program
                                                            Increment();
                                                        }
                                                        
                                                        //When the player's trajectory is in the downward direction
                                                        if (playerVelocity.Y > 0)
                                                        {
                                                            //Call Set player platform program
                                                            SetPlayerPlatform(i);
                                                        }
                                                    }
                                                    break;
                                                case SPIKE:
                                                    {
                                                        //Set number of jumps performed by player to 0
                                                        jumpCounter = 0;

                                                        //When the player's trajectory is in the downward direction
                                                        if (playerVelocity.Y > 0)
                                                        {
                                                            //Play spike sound effect
                                                            spikeEffect.CreateInstance().Play();

                                                            //Bounce the player off of the platform
                                                            playerVelocity.Y += -15;

                                                            //Decrease lives by 1
                                                            lives += - 1;
                                                        }
                                                    }
                                                    break;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            //Set friction and jump speed to original value
                                            friction = 0;
                                            jumpSpeed = -8f;
                                        }
                                    }

                                    //When the player is in the air
                                    if (playerVelocity.Y != 0)
                                    {
                                        //Apply gravity on the player
                                        playerVelocity.Y = playerVelocity.Y + gravity.Y;
                                    }

                                    //When the jump bar is greater than it's lower limit
                                    if (jumpBarRec.Height >= 80)
                                    {
                                        //The player has two jumps available
                                        jumpsAvailable = 2;
                                    }

                                    //When the player's location is greater than 0 perform the appropriate tasks
                                    if (playerLoc.Y > 0)
                                    {
                                        //When space key is pressed and the number of jumps performed by the player is less than the jumps available
                                        if (((kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))) && jumpCounter < jumpsAvailable)
                                        {
                                            //Play the jump effect
                                            jumpEffect.CreateInstance().Play();

                                            //Move the starting platform off of the screen to a certain y coordinate
                                            platformBGone = true;

                                            //Set the hit counter to 0
                                            hitCounter = 0;

                                            //Increase the player's y velocity by the speed of the jump
                                            playerVelocity.Y += jumpSpeed;

                                            //Increase the number jumps performed by the player
                                            jumpCounter++;

                                            //When the user jumped twice meaning they performed a double jump
                                            if (jumpCounter == 2)
                                            {
                                                //Call double jump bar subprogram
                                                jumpBarRec.Height = jumpBarRec.Height - 50;
                                                jumpBarRec.Y = jumpBarRec.Y + 50;
                                                jumpsAvailable = 1;
                                            }
                                        }
                                    }

                                    //When the starting platform is not to be shown
                                    if (platformBGone == true)
                                    {
                                        //Move starting platform off of the screen
                                        startingPlatformRec.Y += startingPlatformSpeed;
                                    }

                                    //For every number between 0 and the leftWallSpikesRec.Length
                                    for (int i = 0; i < leftWallSpikesRec.Length; i++)
                                    {
                                        //Check player collision with spikes on the left wall
                                        if (BoxBoxCollision(playerRec, leftWallSpikesRec[i]))
                                        {
                                            //Play the spike sound effect
                                            spikeEffect.CreateInstance().Play();

                                            //When the player has collided with the spike wall for the first time
                                            if (spikeWallHitCounter == 0)
                                            {
                                                //Decrease lives
                                                lives += -1;

                                                //Reset the player's velocity to 0
                                                playerVelocity.X = 0;
                                                playerVelocity.Y = 0;
                                            }

                                            //Bounce player off of the left wall
                                            playerVelocity.X += 3;
                                            playerVelocity.Y += -3;

                                            //Increase the counter for the number of times the player has his the wall
                                            spikeWallHitCounter++;
                                        }
                                        else if (BoxBoxCollision(playerRec, rightWallSpikesRec[i]))
                                        {
                                            //Play the spike sound effect
                                            spikeEffect.CreateInstance().Play();

                                            //When the player has collided with the spike wall for the first time
                                            if (spikeWallHitCounter == 0)
                                            {
                                                //Decrease lives
                                                lives -= 1;

                                                //Reset the player's velocity to 0
                                                playerVelocity.X = 0;
                                                playerVelocity.Y = 0;
                                            }

                                            //Bounce player off of the right wall
                                            playerVelocity.X += -3;
                                            playerVelocity.Y += -3;

                                            //Increase the counter for the number of times the player has his the wall
                                            spikeWallHitCounter++;
                                        }
                                        else
                                        {
                                            //Reset the spike wall hit counter to 0
                                            spikeWallHitCounter = 0;
                                        }
                                    }

                                    //When the player has collided with the lava
                                    if (BoxBoxCollision(playerRec, lavaRec))
                                    {
                                        //Set the number of lives the player has to 0
                                        lives = 0;
                                    }

                                    //When the player's y location is within the top of the screen
                                    if (playerLoc.Y >= 0)
                                    {
                                        //Add player velocity to the player location
                                        playerLoc = playerLoc + playerVelocity;
                                    }
                                    else if (playerLoc.Y < 0)
                                    {
                                        //Set the player's y location to 0
                                        playerLoc.Y = 0;

                                        //Set the player's y velocity to about 0 so gravity can take over 
                                        playerVelocity.Y = 0.01f;
                                    }

                                    //When the player collides with the power up
                                    if(BoxBoxCollision(playerRec, powerUpRec))
                                    {
                                        //Play the power up effect
                                        powerUpEffect.CreateInstance().Play();

                                        //Set the x, and y location of the power up to a random location off of the screen
                                        powerUpLoc.X = rng.Next(120, 440);
                                        powerUpLoc.Y = rng.Next(-1500, -1000);

                                        //Player currently has the power up
                                        powerUp = true;
                                    }
                                }

                                //When the player has the power up perform the appropriate tasks
                                if (powerUp == true)
                                {
                                    //The player's x velocity is equal to 0
                                    playerVelocity.X = 0;

                                    //Decrease the power bar
                                    powerBarRec.Y += 1;
                                    powerBarRec.Height += -1;

                                    //Increase the score by a certain amount
                                    score += 10;

                                    //Increase the speed of screen and platform
                                    platformSpeed.Y = 6f;
                                    scrollingSpeed = 4;

                                    //Increase the distance travelled 
                                    dTravelled += 2;

                                    //When the power bar is empty
                                    if (powerBarRec.Height <= 0)
                                    {
                                        //Reset the power bar to it's full capacity
                                        powerBarRec.Y = 200;
                                        powerBarRec.Height = 200;

                                        //Reset the platform, and scrolling speed to it's regular speed
                                        platformSpeed.Y = 2f;
                                        scrollingSpeed = 1;

                                        //Player no longer has the power up
                                        powerUp = false;
                                    }
                                }

                                //If the player's x velocity is at about 0
                                if (Math.Abs(playerVelocity.X) < 0.2)
                                {
                                    //Set direction and player x velocity to 0
                                    playerVelocity.X = 0;
                                    direction = 0;
                                }

                                //Decrease the x velocity by friction
                                playerVelocity.X = playerVelocity.X + (-direction * friction);

                                //Have the power up and health badge move down the screen
                                powerUpLoc = powerUpLoc + platformSpeed;
                                healthUpLoc = healthUpLoc + platformSpeed;

                                //The coordinates of the player rectangle is equal to the true location of the player
                                playerRec.X = (int)(playerLoc.X);
                                playerRec.Y = (int)(playerLoc.Y);

                                //Have the rectangle for the player follow the player
                                playerFeetRec.X = playerRec.X + P_FEET_X_SPACER;
                                playerFeetRec.Y = playerRec.Y + P_FEET_Y_SPACER;

                                //Call set platform location subprogram
                                SetPlatformLoc();

                                //Set the x and y coordinate of the power up and health badge to their true location
                                powerUpRec.X = (int)(powerUpLoc.X);
                                powerUpRec.Y = (int)(powerUpLoc.Y);
                                healthUpRec.X = (int)(healthUpLoc.X);
                                healthUpRec.Y = (int)(healthUpLoc.Y);

                                //When the power Up collides with the lava
                                if (BoxBoxCollision(powerUpRec, lavaRec))
                                {
                                    //Set the x, and y location of the power up to a random location off of the screen
                                    powerUpLoc.X = rng.Next(100, 460);
                                    powerUpLoc.Y = rng.Next(-1500, -1000);
                                }
                                
                                //When the health badge collides with the lava
                                if (BoxBoxCollision(healthUpRec, lavaRec))
                                {
                                    //Set the x, and y location of the health badge to a random location off of the screen
                                    healthUpLoc.X = rng.Next(100, 460);
                                    healthUpLoc.Y = rng.Next(-1500, -1000);
                                }

                                //When high score is equal to the score
                                if(score > highScore)
                                {
                                    //High score is equal to the score
                                    highScore = score;
                                }

                                //When the player has at least one life lost but is not yet dead
                                if (lives > 0 && lives < 3)
                                {
                                    //When the player collides with the health badge
                                    if (BoxBoxCollision(playerRec, healthUpRec))
                                    {
                                        //Play the health up sound effect
                                        healthUpEffect.CreateInstance().Play();

                                        //Set the x, and y location of the health badge to a random location off of the screen
                                        healthUpLoc.X = rng.Next(120, 440);
                                        healthUpLoc.Y = rng.Next(-1500, -1000);

                                        //Increase the number of lives the player has
                                        lives += 1;
                                    }
                                }

                                //When the player presses the P key
                                if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
                                {
                                    //game is currently paused
                                    pause = true;
                                }
                            }
                            //When the game is currently paused perform the appropriate tasks
                            else
                            {
                                //When the mouse is clicked perform the appropriate actions
                                if (NewMouseClick())
                                {
                                    //Check mouse location
                                    if (MouseClicked(mainMenuButtonRec))
                                    {
                                        //Continue to play gameplay background music
                                        MediaPlayer.Play(bgMusic);

                                        //The game is currently in the main menu
                                        gameState = MAIN_MENU;
                                    }
                                    else if (MouseClicked(continueButtonRec))
                                    {
                                        //The game is no longer paused
                                        pause = false;
                                    }
                                }
                            }

                            //When the player has no more lives
                            if (lives <= 0)
                            {
                                //Increase game played
                                gamesPlayed++;

                                //Play game over sound effect
                                gameOverEffect.CreateInstance().Play();

                                //Play other background music
                                MediaPlayer.Play(bgMusic);

                                //Game is currently over
                                gameState = GAMEOVER;
                            }
                        }
                    }
                    break;
                case GAMEOVER:
                    {
                        //When the mouse is clicked perform the appropriate actions
                        if (NewMouseClick())
                        {
                            //Check mouse location
                            if (MouseClicked(mainMenuButtonRec2))
                            {
                                //Play the button click sound effect
                                buttonClickEffect.CreateInstance().Play();

                                //The player is currently in the main menu
                                gameState = MAIN_MENU;
                            }
                            else if(MouseClicked(playAgainButtonRec2))
                            {
                                //Call the reset game subprogram
                                ResetGame();

                                //Play the button click effect
                                buttonClickEffect.CreateInstance().Play();

                                //Player is currently in game play
                                gameState = GAMEPLAY;
                            }
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //When the player is in a current game state draw the following images or texts
            switch (gameState)
            {
                case MAIN_MENU:
                    {
                        //Draw main menu images
                        spriteBatch.Draw(titleBgImg, bgRec, Color.White);
                        spriteBatch.Draw(playerImg, playerRec2, Color.White);

                        //For every number between 0 and selectionBoxRec.Length 
                        for (int i = 0; i < selectionBoxRec.Length; i++)
                        {
                            //Draw the selection boxes
                            spriteBatch.Draw(blackBoxImg, selectionBoxRec[i], Color.White);
                        }

                        //Draw the main menu texts
                        spriteBatch.DrawString(subtitleLabel, instructionsOutput, instructionsLoc, Color.White);
                        spriteBatch.DrawString(subtitleLabel, optionsOutput, optionsLoc, Color.White);
                        spriteBatch.DrawString(subtitleLabel, startGameOutput, startGameLoc, Color.White);
                    }
                    break;
                case INSTRUCTIONS:
                    {
                        //Draw instruction images
                        spriteBatch.Draw(instructionsBgImg, bgRec, Color.White);
                        spriteBatch.Draw(instructionsTextImg, instructionsTextRec, Color.White);
                        spriteBatch.Draw(backArrowImg, backArrowRec, Color.White);
                        spriteBatch.Draw(jumpBarImg, instructionsDoubleJumpRec, Color.White);
                        spriteBatch.Draw(powerBarImg, instructionsPowerUpRec, Color.White);

                        //Draw instruction texts
                        spriteBatch.DrawString(subtitleLabel, "Double Jump Bar", instructionsDoubleJumpLoc, Color.White);
                        spriteBatch.DrawString(subtitleLabel, "Power Up Bar", instructionsPowerUpLoc, Color.White);
                    }
                    break;
                case OPTIONS:
                    {
                        //Draw option images
                        spriteBatch.Draw(optionsBgImg, bgRec, Color.White);
                        spriteBatch.Draw(plusImg, plusRec, Color.White);
                        spriteBatch.Draw(minusImg, minusRec, Color.White);
                        spriteBatch.Draw(backArrowImg, backArrowRec, Color.White);
                        spriteBatch.Draw(soundTextImg, soundTextRec, Color.White);
                        spriteBatch.Draw(controlsTextImg, controlsTextRec, Color.White);
                        spriteBatch.Draw(leftButtonImg, leftButtonRec, Color.White);
                        spriteBatch.Draw(rightButtonImg, rightButtonRec, Color.White);

                        //Draw option texts
                        spriteBatch.DrawString(subtitleLabel, "" + audioVolume, audioVolumeLoc, Color.White);
                        spriteBatch.DrawString(subtitleLabel, "Jump: " + moveUp + " Bar", jumpLoc, Color.White);

                        //When the user is right handed
                        if (rightHanded == true)
                        {
                            //Display that the arrow keys control the horizontal movement of the player
                            spriteBatch.DrawString(subtitleLabel, "Left: " + leftKey + " Arrow", moveLeftLoc, Color.White);
                            spriteBatch.DrawString(subtitleLabel, "Right: " + rightKey + " Arrow", moveRightLoc, Color.White);
                        }
                        else
                        {
                            //Display that the A and D keys control the horizontal movement of the player
                            spriteBatch.DrawString(subtitleLabel, "Left: " + leftKey, moveLeftLoc, Color.White);
                            spriteBatch.DrawString(subtitleLabel, "Right: " + rightKey, moveRightLoc, Color.White);
                        }
                    }
                    break;
                case GAMEPLAY:
                    {
                        //Draw game play images
                        spriteBatch.Draw(blackBoxImg, bgRec, Color.White);
                        spriteBatch.Draw(gameplayBgImg, gameplayBgRec1, Color.White);
                        spriteBatch.Draw(gameplayBgImg, gameplayBgRec2, Color.White);
                        spriteBatch.Draw(leftWallImg, leftWallRec, Color.White);
                        spriteBatch.Draw(rightWallImg, rightWallRec, Color.White);
                        spriteBatch.Draw(blankImg, jumpBarOutline, Color.White);
                        spriteBatch.Draw(jumpBarImg, jumpBarRec, Color.White);
                        spriteBatch.Draw(blankImg, powerBarOutline, Color.White);
                        spriteBatch.Draw(powerBarImg, powerBarRec, Color.White);

                        //Draw game play texts
                        spriteBatch.DrawString(gameLabel, scoreOutput + score, scoreLoc, Color.White);
                        spriteBatch.DrawString(gameLabel, highScoreOutput + highScore, highScoreLoc, Color.White);
                        spriteBatch.DrawString(gameLabel, dTravelledOutput + dTravelled + " m", dTravelledLoc, Color.White);

                        //When the player has the power up
                        if (powerUp == true)
                        {
                            //Draw a transparent player
                            spriteBatch.Draw(playerImg, playerRec, Color.White * 0.5f);
                        }
                        else
                        {
                            //Draw regular player image
                            spriteBatch.Draw(playerImg, playerRec, Color.White);
                        }

                        //For every number between 0 and platformRec.Length
                        for (int i = 0; i < platformRec.Length; i++)
                        {
                            //When the platform is a specific type
                            switch (platformType[i])
                            {
                                case REG:
                                    {
                                        //Draw regular platform
                                        spriteBatch.Draw(platformImg, platformRec[i], Color.White);
                                    }
                                    break;
                                case MUD:
                                    {
                                        //Draw mud platform
                                        spriteBatch.Draw(mudPlatformImg, platformRec[i], Color.White);
                                    }
                                    break;
                                case ICE:
                                    {
                                        //Draw ice platform
                                        spriteBatch.Draw(iceyPlatformImg, platformRec[i], Color.White);
                                    }
                                    break;
                                case SPIKE:
                                    {
                                        //Draw ice platform
                                        spriteBatch.Draw(spikesPlatformImg, platformRec[i], Color.White);
                                    }
                                    break;
                            }
                        }

                        //For every number between 0 and leftWallSpikes.Length
                        for (int i = 0; i < leftWallSpikesRec.Length; i++)
                        {
                            //Draw spikes for the left wall
                            spriteBatch.Draw(leftWallSpikesImg, leftWallSpikesRec[i], Color.White);
                        }

                        //For every number between 0 and rightWallSpikes.Length
                        for (int i = 0; i < rightWallSpikesRec.Length; i++)
                        {
                            //Draw spikes for the right wall
                            spriteBatch.Draw(rightWallSpikesImg, rightWallSpikesRec[i], Color.White);
                        }

                        //When player has a certain amount of lives
                        if (lives == 3)
                        {
                            //For every heart/lives image
                            for (int i = 0; i < livesRec.Length; i++)
                            {
                                //Draw all the hearts without any transparency
                                spriteBatch.Draw(livesImg, livesRec[i], Color.White);
                            }
                        }
                        else if (lives == 2)
                        {
                            //Draw all the hearts with one heart transparent
                            spriteBatch.Draw(livesImg, livesRec[2], Color.White * 0.5f);
                            spriteBatch.Draw(livesImg, livesRec[1], Color.White);
                            spriteBatch.Draw(livesImg, livesRec[0], Color.White);
                        }
                        else if (lives == 1)
                        {
                            //Draw all the hearts with two hears transparent
                            spriteBatch.Draw(livesImg, livesRec[2], Color.White * 0.5f);
                            spriteBatch.Draw(livesImg, livesRec[1], Color.White * 0.5f);
                            spriteBatch.Draw(livesImg, livesRec[0], Color.White);
                        }
                        
                        //Check for the starting timer
                        if(startingTimer >= 0 && startingTimer <= 1.1)
                        {
                            //Draw number 3
                            spriteBatch.Draw(number3Img, numberRec, Color.White);
                        }
                        else if (startingTimer > 1.1 && startingTimer <= 2.1)
                        {
                            //Draw number 2
                            spriteBatch.Draw(number2Img, numberRec, Color.White);
                        }
                        else if (startingTimer > 1.1 && startingTimer <= 3.1)
                        {
                            //Draw number 1
                            spriteBatch.Draw(number1Img, numberRec, Color.White);
                        }

                        //When the game is paused
                        if (pause == true)
                        {
                            //Draw appropriate images for when the game is paused
                            spriteBatch.Draw(whitePageImg, whiteRec, Color.Black * 0.5f);
                            spriteBatch.Draw(mainMenuButtonImg, mainMenuButtonRec, Color.White);
                            spriteBatch.Draw(continueButtonImg, continueButtonRec, Color.White);

                            //Draw the appropriate texts for when the game is paused
                            spriteBatch.DrawString(subtitleLabel, pauseOutput, pauseLoc, Color.Red);
                        }

                        //Draw power up, health up, lava, and starting platform images
                        spriteBatch.Draw(powerUpImg, powerUpRec, Color.White);
                        spriteBatch.Draw(healthUpImg, healthUpRec, Color.White);
                        spriteBatch.Draw(platformImg, startingPlatformRec, Color.White);
                        spriteBatch.Draw(lavaImg, lavaRec, Color.White);
                    }
                    break;
                case GAMEOVER:
                    {
                        //Draw the appropriate images for when the game is over
                        spriteBatch.Draw(gameOverBgImg, bgRec, Color.White);
                        spriteBatch.Draw(mainMenuButtonImg, mainMenuButtonRec2, Color.White);
                        spriteBatch.Draw(playAgainButtonImg, playAgainButtonRec2, Color.White);
                        spriteBatch.Draw(deadLinkImg, deadLinkRec, Color.White);
                        spriteBatch.DrawString(subtitleLabel, "Score: " + score, finalScoreLoc, Color.White);
                        spriteBatch.DrawString(subtitleLabel, "High Score: " + highScore, finalHighScoreLoc, Color.White);
                        spriteBatch.DrawString(subtitleLabel, "Distance: " + dTravelled + " m", finalDTravelledLoc, Color.White);
                    }
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Pre: r1 and r2 are two valid rectangles
        //Post: Return true if r1 intersects with r2 in any way, false otherwise
        //Description: Determine if r1 intersects with r2 by checking for impossible collisions
        private bool BoxBoxCollision(Rectangle box1, Rectangle box2)
        {
            //If any one of the four impossiblities is true there is no collision
            if (box1.Right < box2.Left || box1.Left > box2.Right ||
                box1.Bottom < box2.Top || box1.Top > box2.Bottom)
            {
                return false;
            }
            else
            {
                //All impossibilities failed, so it MUST be a collision
                return true;
            }
        }
        
        //Pre:N/A
        //Post:N/A
        //Desc:Determines whether the mouse has been clicked or not
        private bool NewMouseClick()
        {
            //When the left button of the mouse is pressed and the left button was not previously pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //A new mouse click has occured
                return true;
            }

            //No new mouse click has occured
            return false;
        }

        //Pre: Rectangle
        //Post: Returns a bool variable 
        //Desc: Determines whether the x and y coordinates of the mouse where
        //within the x and y coordinates of the Rectangle being tested
        private bool MouseClicked(Rectangle box)
        {
            //When the mouses location is within the x,y coordinates, length and width of the box
            if (mouse.X <= box.X + box.Width && mouse.X >= box.X && mouse.Y <= box.Y + box.Height && mouse.Y >= box.Y)
            {
                //Mouse was within the box
                return true;
            }
            else
            {
                //Mouse was not within the box
                return false;
            }
        }
        //Pre: N/A
        //Post: N/A
        //Description: Reset all the values back to their original values 
        private void ResetGame()
        {
            //When player has only played one game
            if (gamesPlayed == 1)
            {
                //High score is equal to the score
                highScore = score;
            }

            //Reset all necessary values to the original
            resetGame = true;
            platformBGone = false;
            startingTimer = 0;
            difficultyTimer = 0;
            friction = 0f;
            jumpSpeed = -8f;
            score = 0;
            lives = 3;
            scrollingSpeed = 1;
            jumpCounter = 0;
            hitCounter = 0;
            spikeWallHitCounter = 0;
            jumpsAvailable = 2;
            difficulty = 0;
            pause = false;
            powerUp = false;
            gameStart = false;
            dTravelled = 0;

            //Reset the jump and power bar
            jumpBarRec = new Rectangle(5, 200, 40, 200);
            powerBarRec = new Rectangle(655, 200, 40, 200);
           
            //for every number between 0 and platformRec.Length
            for (int i = 1; i < platformRec.Length; i++)
            {
                //Set the platform rectangles and the true location
                platformRec[i] = new Rectangle(rng.Next(120, 440), 0 - (((screenHeight - lavaRec.Height) / 4) * i), (int)(platformImg.Width / 2.5), (int)(platformImg.Height / 1.2));
                platformLoc[i] = new Vector2(platformRec[i].X, platformRec[i].Y);
            }

            //for every number between 0 and platformRec.Length
            for (int i = 0; i < platformRec.Length; i++)
            {
                //Set the first four platforms to regular platforms
                platformType[i] = REG;
            }

            //Reset the platform speed and player velocity to its original value
            platformSpeed = new Vector2(0, 2f);
            playerVelocity = new Vector2(0, 0);
           
            //Reset starting platform to original location
            startingPlatformRec = new Rectangle(0, screenHeight - lavaRec.Height - 40, screenWidth, 40);

        }
        //Pre: N/A
        //Post: N/A
        //Description: Reset all the values back to their original values
        private void ScrollScreen()
        {
            //Have the two background screens for game play scroll down
            gameplayBgRec1.Y += scrollingSpeed;
            gameplayBgRec2.Y += scrollingSpeed;

            //When a background image is below the screen
            if (gameplayBgRec1.Y >= screenHeight)
            {
                //Move background back up 
                gameplayBgRec1.Y = -screenHeight;
            }

            //Move a background image is below the screen
            if (gameplayBgRec2.Y >= screenHeight)
            {
                //Move background back up
                gameplayBgRec2.Y = -screenHeight;
            }
        }

        //Pre: N/A
        //Post:N/A
        //Description: Move the player left and right depending on the keys pressed
        private void HorizontalMovement()
        {
            //When player is right handed
            if (rightHanded == true)
            {
                //Check whether left or right key is pressed
                if (kb.IsKeyDown(Keys.Right))
                {
                    //When player x velocity is below the maximum speed
                    if (playerVelocity.X < maxSpeed)
                    {
                        //Increase player x velocity by acceleration
                        playerVelocity.X += acceleration;
                    }

                }
                else if (kb.IsKeyDown(Keys.Left))
                {
                    //When player x velocity is below the maximum speed
                    if (playerVelocity.X < maxSpeed)
                    {
                        //Increase player x velocity by acceleration
                        playerVelocity.X -= acceleration;
                    }
                }
            }
            else
            {
                //Check whether A or D key is pressed
                if (kb.IsKeyDown(Keys.D))
                {
                    //When player x velocity is below the maximum speed
                    if (playerVelocity.X < maxSpeed)
                    {
                        //Increase player x by velocity
                        playerVelocity.X += acceleration;
                    }
                }
                else if (kb.IsKeyDown(Keys.A))
                {
                    //When player x velocity is below the maximum speed
                    if (playerVelocity.X < maxSpeed)
                    {
                        //Increase player x by acceleration
                        playerVelocity.X -= acceleration;
                    }
                }
            }

            //When player is moving
            if (playerVelocity.X != 0)
            {
                //Check which direction player is moving
                if (playerVelocity.X > 0)
                {
                    //Player is moving right
                    direction = 1;
                }
                else if (playerVelocity.X < 0)
                {
                    //Player is moving left
                    direction = -1;
                }
            }

        }

        //Pre: N/A
        //Post: N/A
        //Descriptiion: Increase the speed of the platforms which are falling based on a timer
        private void IncreaseDifficulty()
        {
            //Check difficulty timer
            if (difficultyTimer >= (4.9 + (difficulty)) && difficultyTimer < (5.1 + (difficulty)))
            {
                //Increase platform speed
                platformSpeed.Y += 0.3f;

                //Increase difficulty
                difficulty += 5;
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Set the location of the platforms to their true location
        private void SetPlatformLoc()
        {
            //Set first platform to true location and increase location by speed
            platformLoc[1] = platformLoc[1] + platformSpeed;
            platformRec[1].X = (int)(platformLoc[1].X);
            platformRec[1].Y = (int)(platformLoc[1].Y);

            //Set Second platform to true location and increase location by speed
            platformLoc[2] = platformLoc[2] + platformSpeed;
            platformRec[2].X = (int)(platformLoc[2].X);
            platformRec[2].Y = (int)(platformLoc[2].Y);

            //Set Third platform to true location and increase location by speed
            platformLoc[3] = platformLoc[3] + platformSpeed;
            platformRec[3].X = (int)(platformLoc[3].X);
            platformRec[3].Y = (int)(platformLoc[3].Y);

            //Set Fourth platform to true location and increase location by speed
            platformLoc[4] = platformLoc[4] + platformSpeed;
            platformRec[4].X = (int)(platformLoc[4].X);
            platformRec[4].Y = (int)(platformLoc[4].Y);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Increase the score, the hit counter which represents
        //number of times player has hit the platform and the double jump bar
        private void Increment()
        {
            //Increase score and hit counter
            score += 100;
            hitCounter++;

            //Check double jump bar height
            if (jumpBarRec.Height <= 190)
            {
                //Decrease the double jump bar
                jumpBarRec.Y = jumpBarRec.Y - 10;
                jumpBarRec.Height = jumpBarRec.Height + 10;
            }

        }

        //Pre: integer representing the platform number as subprogram is called within a for loop
        //Post: N/A
        //Description: Set the player on the platform, called for when the
        //player has collided with the platform
        private void SetPlayerPlatform(int i)
        {
            //Player's velocity in the y direction is equal to the speed of the platform
            playerVelocity.Y = platformSpeed.Y;

            //Set the player on top of the platform
            playerLoc.Y = platformRec[i].Y - playerRec.Height;
        }
    }
}

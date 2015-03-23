#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Elemental_Ascension
{
    // added this comment
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Attributes
        GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont _debug;
        Song alchemistsTower;
        Song failing;
        bool songstart = false;
        bool songstart2 = false;
        private MouseState mouse;
        private Environment env;
        private Vector2 center;
        private Menus menu;
        private Vector2 start;
        private Vector2 control;
        private Vector2 credit;
        private Texture2D _background;
        private Rectangle screensize;
        private Texture2D _title;
        private Rectangle titleLocation;
        private Texture2D _Crosshairs;
        private SoundEffect _gameOver;
        private bool gameOverPlayed = false;
        // enum for the FSM of the game
        public  enum GameState
        {
            Menu,
            Controls,
            Credits,
            Game,
            Pause,
            GameOver
        }
        private GameState gState;
        private GameManager gM;
        #endregion

        #region Properties
        public GameState GState { get { return gState; } set { gState = value; } }
        #endregion

        #region Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }
        #endregion

        #region Initialization
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            graphics.PreferredBackBufferWidth = Constants.VIEWPORT_WIDTH;
            graphics.PreferredBackBufferHeight = Constants.VIEWPORT_HEIGHT;
            graphics.ApplyChanges();
            gState = GameState.Menu;
            center = new Vector2((GraphicsDevice.Viewport.Width / 2)- 50, GraphicsDevice.Viewport.Height / 2);
            start = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, (GraphicsDevice.Viewport.Height / 2) + 100);
            control = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, (GraphicsDevice.Viewport.Height / 2) + 150);
            credit = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, (GraphicsDevice.Viewport.Height / 2) + 200);
            menu = new Menus(start,control,credit);
            env = new Environment(this);
            gM = new GameManager(env,this);

            screensize = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            

            base.Initialize();
        }
        #endregion

        #region Load Content
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _debug = this.Content.Load<SpriteFont>("Debug");
            _background = this.Content.Load<Texture2D>("MenuDrawing");
            _title = this.Content.Load<Texture2D>("TitleText");
            titleLocation = new Rectangle(GraphicsDevice.Viewport.Width / 2 - _title.Width/2, GraphicsDevice.Viewport.Height / 6, _title.Width, _title.Height);
            _Crosshairs = this.Content.Load<Texture2D>("Crosshairs");
            //TODO: need legit crosshairs sprite
            _gameOver = this.Content.Load<SoundEffect>("GameOver");
            // Load the graphics for the sprite attributes
            gM.Player.Load(this);
            
            
            alchemistsTower = this.Content.Load<Song>("Alchemists Tower");
            failing = this.Content.Load<Song>("Failing Defense");
            
            MediaPlayer.IsRepeating = true; 
        }
        #endregion

        #region Unload Content
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }
        #endregion

        #region Update
        /// <summary>
        /// 
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();




            gM.UpdateInput();
            
            // GameState FSM for update
            switch (gState)
            {
                case GameState.Menu:
                    {
                        this.IsMouseVisible = true;

                        if(menu.UpdateMain(mouse) == 1)
                        {
                            gM.Player.Reset();
                            gM.NewGame();
                            this.IsMouseVisible = false;
                            MediaPlayer.Stop();
                            songstart = true;
                            gState = GameState.Game;
                        }
                        else if(menu.UpdateMain(mouse) == 2)
                        {
                            this.IsMouseVisible = false;
                            songstart = false;
                            gState = GameState.Controls;
                        }
                        else if (menu.UpdateMain(mouse) == 3)
                        {
                            this.IsMouseVisible = false;
                            gState = GameState.Credits;
                        }


                        if (!songstart)
                        {
                            MediaPlayer.Play(alchemistsTower);
                            MediaPlayer.Volume = 10;
                            songstart = true;
                        }  
                         
                        break;
                    }
                case GameState.Controls:
                    {
                        // TODO: implement options code
                        if (gM.SingleKeyPress(Keys.Enter))
                        {
                            gM.Player.Reset();
                            gM.NewGame();
                            MediaPlayer.Stop();
                            songstart2 = false;
                            gState = GameState.Game;

                        }
                        break;
                    }
                case GameState.Credits:
                    {
                        if (gM.SingleKeyPress(Keys.Enter))
                        {
                            gState = GameState.Menu;
                        }
                        break;
                    }
                case GameState.Game:
                    {
                        
                       
                        gM.Update(gameTime);

                        if (gM.SingleKeyPress(Keys.P))
                        {
                            gState = GameState.Pause;
                        }
                        if (!songstart2)
                        {
                            MediaPlayer.Play(failing);
                            MediaPlayer.Volume = .65F;
                            songstart2 = true;
                        }

                        if(gM.Player.Loc.Y > GraphicsDevice.Viewport.Height+Constants.PLAYER_HEIGHT)
                        {
                            menu.GameOver(gM.Player.Score);
                            gState = GameState.GameOver;
                        }
                        


                        break;
                    }
                case GameState.Pause:
                    {
                        

                        if (gM.SingleKeyPress(Keys.P))
                        {
                            gState = GameState.Game;
                        }
                        break;
                    }
                case GameState.GameOver:
                    {
                        if (!gameOverPlayed)
                        {
                            _gameOver.Play();
                            gameOverPlayed = true;
                        }
                        if (gM.SingleKeyPress(Keys.Enter))
                        {
                            gameOverPlayed = false;
                            gState = GameState.Menu;
                        }
                        MediaPlayer.Stop();
                        songstart = false;
                        songstart2 = false;
                        break;
                        
                    }
            }

            

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin();

            gM.DrawBackground(spriteBatch);

            // GameState FSM for Draw 
            switch (gState)
            {
                case GameState.Menu:
                    {
                        GraphicsDevice.Clear(Color.Black);
                        menu.MenuDraw(_title, this.spriteBatch, titleLocation ,_background,screensize,_debug,center);
                        break;
                    }
                case GameState.Controls:
                    {
                        GraphicsDevice.Clear(Color.Black);
                        menu.ControlDraw(_debug, this.spriteBatch, Vector2.Zero);
                        break;
                    }
                case GameState.Credits:
                    {
                        GraphicsDevice.Clear(Color.Black);
                        menu.CreditDraw(_debug, this.spriteBatch, Vector2.Zero);
                        break;
                    }
                case GameState.Game:
                    {
                        //spriteBatch.Draw(_gameBackgroundTop, new Rectangle(0, 0,Constants.VIEWPORT_WIDTH,Constants.VIEWPORT_HEIGHT), Color.White);

                        env.Draw(spriteBatch);

                        gM.DrawPlayer(spriteBatch);

                        spriteBatch.Draw(_Crosshairs, new Vector2(gM.MState.X, gM.MState.Y), null, Color.White, 0, Vector2.Zero, .05f, SpriteEffects.None, 0);
                        // TODO: When the crosshairs sprite is added, this line will have to be changed


                        // display debug information if space is held down.  Otherwise show player score.
                        if (gM.KbState.IsKeyDown(Keys.Space))
                        {
                            spriteBatch.DrawString(_debug, "Player Location: " + gM.Player.Loc.X + ", " + gM.Player.Loc.Y +
                                " Player state: " + gM.Player.PState.ToString() + " Velocity: " + gM.Player.XVelocity + "," + gM.Player.YVelocity, Vector2.Zero, Color.Black);
                        }
                        else if (gM.KbState.IsKeyUp(Keys.Space))
                        {
                            menu.GameScreenDraw(_debug, this.spriteBatch, gM.Player.Score);
                        }
                        break;
                    }
                case GameState.Pause:
                    {
                        GraphicsDevice.Clear(Color.Black);
                        menu.PauseDraw(_debug, this.spriteBatch, center);
                        break;
                    }
                case GameState.GameOver:
                    {
                        //spriteBatch.Draw(_gameBackgroundTop, new Rectangle(0, 0, Constants.VIEWPORT_WIDTH, Constants.VIEWPORT_HEIGHT), Color.White);
                        env.Draw(spriteBatch);
                        menu.GameOverDraw(_debug, this.spriteBatch, Vector2.Zero);
                        break;
                    }
                        
                        
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}

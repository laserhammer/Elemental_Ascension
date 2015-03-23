using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Elemental_Ascension
{
    class GameManager
    {

        #region Attributes

        private Player player;
        private List<Enemy> enemies;
        private List<Collectibles> collectibles;
        private Environment env;
        private float scrollSpeed;
        private double timeElapsed;
        private MouseState mState;
        private MouseState prevMState;
        private KeyboardState kbState;
        private KeyboardState prevKbState;
        private Game1 game;
        private Texture2D _gameBackgroundTop;
        private Texture2D _gameBackgroundBottom;
        private float _topBGLoc;
        private float _bottomBGLoc;

        #endregion

        #region Properties
        public double TimeElapsed { get { return timeElapsed; } }
        public MouseState MState { get { return mState;}}
        public MouseState PrevMState { get { return prevMState; } }
        public KeyboardState KbState { get { return kbState; } }
        public KeyboardState PrevKbState { get { return prevKbState; } }
        public Player Player { get { return player; } }
        public List<Enemy> Enemies { get { return enemies; } }
        #endregion

        #region Constructor

        public GameManager( Environment env, Game1 game)
        {
            this.env = env;
            scrollSpeed = .8f;
            mState = new MouseState();
            kbState = new KeyboardState();
            player = new Player(Constants.PLAYER_WIDTH, Constants.PLAYER_HEIGHT, new Vector2(Constants.PLAYER_START_X, Constants.PLAYER_START_Y), this);
            enemies = env.Enemies;

            collectibles = env.Collectibles;
            this.game = game;

            //background stuff.
            _gameBackgroundTop = game.Content.Load<Texture2D>("BackgroundTop");
            _gameBackgroundBottom = game.Content.Load<Texture2D>("BackgroundBottom");
            _bottomBGLoc = 0;
            _topBGLoc = -Constants.VIEWPORT_HEIGHT;
        }

        #endregion


        #region Methods

        public void Gravity(GameTime gameTime)
        {
            foreach (Enemy enem in enemies)
            {
                if (enem.falling == true)
                {
                    enem.YVelocity += timeElapsed * Constants.GRAVITY / 1000;
                }
            }
            if(player.PState != Player.PlayerState.DashingLeft && player.PState != Player.PlayerState.DashingRight)
                player.YVelocity += timeElapsed * Constants.GRAVITY / 1000;

        }

        // scroll all the things
        public void Scroll(GameTime gameTime)
        {
            timeElapsed = gameTime.ElapsedGameTime.Milliseconds;


            //Different scrollSpeeds.
            scrollSpeedCheck();


            env.Update(scrollSpeed);
            player.YLoc += scrollSpeed;
            foreach (Fireball fireball in player.Projectiles)
            {
                fireball.YLoc += scrollSpeed;
            }
                
            _bottomBGLoc += scrollSpeed;
            _topBGLoc += scrollSpeed;
        }

        private void scrollSpeedCheck()
        {
            if (player.Score >= 4000)
            {
                scrollSpeed = 2.1f;
            }
            else if (player.Score >= 3000 && player.Score < 4000)
            {
                scrollSpeed = 1.8f;
            }
            else if (player.Score >= 2000 && player.Score < 3000)
            {
                scrollSpeed = 1.5f;
            }
            else if (player.Score >= 1000 && player.Score < 2000)
            {
                scrollSpeed = 1.2f;
            }
            else if (player.Score >= 500 && player.Score < 1000)
            {
                scrollSpeed = 1.0f;
            }

        }

        public void NewGame()
        {
            player.PState = Elemental_Ascension.Player.PlayerState.IdleRight;
            scrollSpeed = .8f;
            env.ActiveWalls.Clear();
            env.Collectibles.Clear();
            player.Projectiles.Clear();
            env.AddWalls(128 * (Constants.WALLS_TOGENERATE - 4), Constants.WALLS_TOGENERATE);
            env.Enemies.Clear();
            //TODO: Implement new game code so after you lose, you can restart
            // Place the player on a wall
            int wall = env.ActiveWalls.Count / 2 + env.ActiveWalls.Count / 4; 
            int startX = (int)env.ActiveWalls[wall].Loc.X + Constants.WALL_WIDTH / 2 - Constants.PLAYER_WIDTH / 2;
            int startY = (int)env.ActiveWalls[wall].Loc.Y - Constants.PLAYER_HEIGHT;
            player.SetPosition(startX, startY);
        }

        public void Update(GameTime gameTime)
        {
            player.Update(timeElapsed, mState, prevMState, kbState, prevKbState);
            ScreenWrap();
            player.GetFrame(gameTime);
            Gravity(gameTime);
            Scroll(gameTime);
            player.CheckCollisions(env.ActiveWalls, env.Collectibles, enemies);
            foreach (Collectibles i in collectibles)
                i.GetFrame(gameTime);

            foreach (Enemy enemy in enemies)
                {
                    enemy.getFrames(gameTime);
                    enemy.PlayerX = Player.Rect.X;
                    enemy.Update(TimeElapsed, player);
                }

            foreach (Wall w in env.ActiveWalls)
                w.getFrames(gameTime);

            BGWrap();

            if (SingleKeyPress(Keys.R))
            {
                Player.Reset();
            }

            
        }

        private void BGWrap()
        {
            if (_topBGLoc > Constants.VIEWPORT_HEIGHT)
            {
                _topBGLoc = _bottomBGLoc - Constants.VIEWPORT_HEIGHT;
            }

            if (_bottomBGLoc > Constants.VIEWPORT_HEIGHT)
            {
                _bottomBGLoc = _topBGLoc - Constants.VIEWPORT_HEIGHT;
            }
        }

        // Wraps the player from one side of the screen to the other
        public void ScreenWrap()
        {
            if (player.Loc.X > game.GraphicsDevice.Viewport.Width)
                player.Loc = new Vector2(0, player.Loc.Y);
            if (player.Loc.X < 0 - Constants.PLAYER_WIDTH)
                player.Loc = new Vector2(game.GraphicsDevice.Viewport.Width, player.Loc.Y);
            
        }

        public bool SingleKeyPress(Keys press)
        {
            if (kbState.IsKeyDown(press) && prevKbState.IsKeyUp(press))
                return true;
            else
                return false;
        }

        public bool SingleMouseClickLeft()
        {
            if (mState.LeftButton == ButtonState.Pressed && prevMState.LeftButton == ButtonState.Released)
                return true;
            else
                return false;
        }

        public void KeepMouseInWindow()
        {
            /*
            // Make sure that the cursor stays inside the window
            if (mState.X < 0)
                Mouse.SetPosition(0, mState.Y);
            if (mState.X > Constants.VIEWPORT_WIDTH)
                Mouse.SetPosition(Constants.VIEWPORT_WIDTH, mState.Y);
            if (mState.Y < 0)
                Mouse.SetPosition(mState.X, 0);
            if (mState.Y > Constants.VIEWPORT_HEIGHT)
                Mouse.SetPosition(mState.X, Constants.VIEWPORT_HEIGHT);
             */
        }


        /// <summary>
        /// Only updates the Keyboard and mouse states
        /// </summary>
        public void UpdateInput()
        {
            KeepMouseInWindow();
            prevMState = mState;
            mState = Mouse.GetState();
            prevKbState = kbState;
            kbState = Keyboard.GetState();
            if (SingleKeyPress(Keys.Escape))
                game.Exit(); 
        }


        public void DrawPlayer(SpriteBatch spriteBatch)
        {
            player.Draw(spriteBatch);
        }
       
        public void DrawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_gameBackgroundTop, new Rectangle(0, (int)_topBGLoc, Constants.VIEWPORT_WIDTH, Constants.VIEWPORT_HEIGHT), Color.White);
            spriteBatch.Draw(_gameBackgroundBottom, new Rectangle(0, (int)_bottomBGLoc, Constants.VIEWPORT_WIDTH, Constants.VIEWPORT_HEIGHT), Color.White);
        }

        public bool KeyReleased(Keys key)
        {
            if (KbState.IsKeyUp(key) && PrevKbState.IsKeyDown(key))
                return true;
            else
                return false;
        }
        #endregion

      
    }
}

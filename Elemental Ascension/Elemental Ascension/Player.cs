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
    class Player : Entity
    {
        #region Attributes
        // enum for the FSM of the player
        public enum PlayerState
        {
            IdleRight,
            IdleLeft,
            CrouchingRight,
            CrouchingLeft,
            WalkingRight,
            WalkingLeft,
            JumpingRight,
            JumpingLeft,
            DashingRight,
            DashingLeft
        }

        private PlayerState pState;
        private Texture2D _spritesheet;
        private Texture2D _dash;
        private Texture2D _fball;
        private Texture2D _cloud;
        private bool onPlatform;
        private bool notColliding;
        private List<Fireball> projectiles;
        private int _score;
        private Wall attacheWall;
        private int frame;
        private int framesElapsed;
        private bool canDoubleJump;
        private int dJumpFrames;
        private bool inProgress;
        private int jFrame;
        private bool dJNow;
        private int cloudX;
        private int cloudY;
        private double timeDashing;
        private int dashFrame;
        private bool dashPrimedRight;
        private bool dashPrimedLeft;
        private double timeSinceTap;
        private bool impact;
        private GameManager gM;
        private SoundEffect _step1;
        private SoundEffect _step0;
        private SoundEffect _dashSound;
        private SoundEffect _landing;
        private SoundEffect _shoot;
        private SoundEffect _doubleJump;
        private SoundEffect _collect;
        private SoundEffect _burning;
        private SoundEffect _airImpact;
        private SoundEffect _earthImpact;
        private SoundEffect _iceImpact;
        private SoundEffect _fireImpact;
        private bool step;
        private bool stepped;
        private bool dashed;
        #endregion

        #region Properties
        public PlayerState PState { get { return pState; } set { pState = value; } }
        public bool OnPlatform { get { return onPlatform; } set { onPlatform = value; } }
        public int Score { get { return _score; } set { _score = value; } }
        public bool CanDoubleJump {get { return canDoubleJump;} set { canDoubleJump = value;}}
        public Wall AttacheWall { get { return attacheWall; } set { attacheWall = value; } }
        public double TimeDashing { get { return timeDashing; } set { timeDashing = value; } }
        public List<Fireball> Projectiles { get { return projectiles; } set { projectiles = value; } }
        public bool Impact { get { return impact; } set { impact = value; } }
        #endregion

        #region Constructor
        public Player(int width, int height, Vector2 location, GameManager gM)
            : base(width, height, location) 
        {
            projectiles = new List<Fireball>();
            pState = PlayerState.IdleRight;
            this.gM = gM;
        }
        #endregion

        
        #region Methods

        public void Load(Game1 game)
        {
            _spritesheet = game.Content.Load <Texture2D>("CharacterSpriteSheet");
            _dash = game.Content.Load<Texture2D>("playerDashSheet");
            _fball = game.Content.Load<Texture2D>("FireBall");
            _cloud = game.Content.Load<Texture2D>("DoubleJumpCloud");
            frame = 0;
            _step0 = game.Content.Load<SoundEffect>("Step0");
            _step1 = game.Content.Load<SoundEffect>("Step1");
            _dashSound = game.Content.Load<SoundEffect>("DashSound");
            _landing = game.Content.Load<SoundEffect>("Landing");
            _shoot = game.Content.Load<SoundEffect>("Shoot");
            _doubleJump = game.Content.Load<SoundEffect>("DoubleJump");
            _collect = game.Content.Load<SoundEffect>("Collect");
            _burning = game.Content.Load<SoundEffect>("Burning");
            _airImpact = game.Content.Load<SoundEffect>("air_impact");
            _earthImpact = game.Content.Load<SoundEffect>("earth_impact");
            _fireImpact = game.Content.Load<SoundEffect>("fireball_impact");
            _iceImpact = game.Content.Load<SoundEffect>("snowball_impact");
        }

        /// <summary>
        /// Checks to see if the player is colliding with any walls, nothing else yet
        /// </summary>
        public void CheckCollisions(List<Wall> walls, List<Collectibles> collectibles, List<Enemy> enemies)
        {
            if (attacheWall != null)
            {
                if (attacheWall.IsAlive == false)
                {
                    attacheWall = null;
                    notColliding = true;
                    onPlatform = false;
                }
            }

            notColliding = true;
            foreach (Wall wall in walls)
            {
                if (Rect.Intersects(wall.Rect) && wall.IsAlive == true)
                {
                    if (Rect.Y + Rect.Height - YVelocity < wall.top + 1 && wall.IsAlive)
                    {
                        if (attacheWall == null)
                            _landing.Play(.1f,.0f,.0f);
                        onPlatform = true;
                        attacheWall = wall;
                        yVelocity = 0;
                        notColliding = false;
                        continue;
                    }
                    else if( Rect.Y - 2 * YVelocity > wall.bottom && wall.IsAlive)
                    {
                        YVelocity = -YVelocity*.5;
                        continue;
                    }
                         /*
                    else if (Rect.Center.X < wall.Rect.Center.X && Rect.X + Rect.Width + XVelocity > wall.left)
                    {
                        collidingLeft = true;
                        loc.X += (wall.left - Rect.X - Rect.Width);
                        rect.X = (int)Loc.X;
                        xVelocity = 0;
                        continue;
                    }
                        /*
                    else if (Rect.X + XVelocity < wall.right) 
                    {
                        collidingRight = true;
                        loc.X += (wall.right - Rect.X);
                        rect.X = (int)Loc.X;
                        xVelocity = 0;
                        continue;
                    }
                         */
                }
            }

            for (int i = 0; i < collectibles.Count; i++)
            {
                if (rect.Intersects(collectibles[i].Rect))
                {
                    _collect.Play(.3f,0,0);
                    Score += 100;
                    collectibles.RemoveAt(i);
                    i--;
                }
            }


            //Fireball collision
            for (int j = 0; j < projectiles.Count; j++)
            {
                //collision against walls.
                for (int i = 0; i < walls.Count; i++)
                {
                    if (projectiles[j].FireBallRect.Intersects(walls[i].Rect)&&walls[i].IsAlive == true)
                    {
                        projectiles.RemoveAt(j);
                        if (walls[i].WType == Wall.WallType.BurnableSmall || walls[i].WType == Wall.WallType.BurnableRight || walls[i].WType == Wall.WallType.BurnableLeft)
                        {
                            _burning.Play(.4f, 0, 0);
                            walls[i].IsAlive = false;
                        }

                        if(j != 0)
                            j--;
                    }
                    if (projectiles.Count == 0)
                        break;
                }
                if (projectiles.Count == 0)
                    break;
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (projectiles[j].FireBallRect.Intersects(enemies[i].Rect)&& enemies[i].IsAlive == true)
                    {
                        _burning.Play(.3f,0,0);
                        projectiles.RemoveAt(j);
                        enemies[i].IsAlive = false;
                        enemies[i].isDying = true;
                        Score += 100;
                        if (j != 0)
                            j--;
                    }
                    if (projectiles.Count == 0)
                        break;
                }
                if (projectiles.Count == 0)
                    break;
            }
            

            if (notColliding)
            {
                onPlatform = false;
                if (attacheWall != null)
                {

                    if (rect.X < attacheWall.Loc.X - Constants.PLAYER_WIDTH || rect.X > attacheWall.Loc.X + Constants.WALL_WIDTH)
                    {
                        attacheWall = null;
                    }
                    else if (Rect.Y > Constants.VIEWPORT_HEIGHT||attacheWall.IsAlive == false)
                    {
                        onPlatform = false;
                        attacheWall = null;
                    }
            
                }
            }
        }

        /// <summary>
        /// Called during update in game class
        /// </summary>
        public void Update(double timeElapsed, MouseState mState, MouseState prevMstate, KeyboardState kbState, KeyboardState prevKbState)
        {

            //Checks for a double tap for a dash and also the fireball command
            if (pState != PlayerState.CrouchingLeft && pState != PlayerState.CrouchingRight)
            {
                if (gM.KeyReleased(Keys.D))
                {
                    dashPrimedRight = true;
                    dashPrimedLeft = false;
                    timeSinceTap = 0;
                }
                if (gM.KeyReleased(Keys.A))
                {
                    dashPrimedLeft = true;
                    dashPrimedRight = false;
                    timeSinceTap = 0;
                }

                timeSinceTap += timeElapsed;
                if (timeSinceTap >= Constants.DOUBLETAP_TIME)
                {
                    dashPrimedRight = false;
                    dashPrimedLeft = false;
                }
                if (gM.SingleMouseClickLeft())
                {
                    _shoot.Play();
                    Fireball f = new Fireball(this, mState);
                    f.FireBallT2D = _fball;
                    projectiles.Add(f);
                }
            }

            #region pState FSM
            switch (pState)
            {
                #region IdleRight
                case PlayerState.IdleRight:
                    {
                        canDoubleJump = true;
                        if(impact == false)
                            XVelocity = 0;

                        if (attacheWall != null)
                            yVelocity = 0;
                        if (YVelocity != 0)
                        {
                            pState = PlayerState.JumpingRight;
                        }
                        if (gM.SingleKeyPress(Keys.D) && dashPrimedRight)
                        {
                            attacheWall = null;
                            YLoc--;
                            pState = PlayerState.DashingRight;
                        }
                        else if (kbState.IsKeyDown(Keys.D))
                        {
                            pState = PlayerState.WalkingRight;
                        }
                        if (kbState.IsKeyDown(Keys.A))
                        {
                            pState = PlayerState.WalkingLeft;
                        }
                        if (kbState.IsKeyDown(Keys.S))
                        {
                            pState = PlayerState.CrouchingRight;
                        }
                        if (kbState.IsKeyDown(Keys.W) && onPlatform)
                        {
                            pState = PlayerState.JumpingRight;
                            yVelocity = Constants.PLAYER_JUMP_SPEED;
                        }
                        else
                        {
                            if (attacheWall != null)
                            {
                                YLoc = attacheWall.top - Constants.PLAYER_HEIGHT + 1;
                            }
                        }
                        break;
                    }
                #endregion
                #region IdleLeft
                case PlayerState.IdleLeft:
                    {
                        canDoubleJump = true;
                        if (impact == false)
                            XVelocity = 0;

                        if (attacheWall != null)
                            yVelocity = 0;
                        
                        if (YVelocity != 0)
                        {
                            pState = PlayerState.JumpingLeft;
                        }
                        if (kbState.IsKeyDown(Keys.D) && !collidingRight)
                        {
                            pState = PlayerState.WalkingRight;
                        }
                        if (gM.SingleKeyPress(Keys.A) && dashPrimedLeft)
                        {
                            attacheWall = null;
                            YLoc--;
                            pState = PlayerState.DashingLeft;
                        }
                        else if (kbState.IsKeyDown(Keys.A) && !collidingLeft)
                        {
                            pState = PlayerState.WalkingLeft;
                        }
                        if (kbState.IsKeyDown(Keys.S))
                        {
                            pState = PlayerState.CrouchingLeft;
                        }
                        if (kbState.IsKeyDown(Keys.W) && onPlatform)
                        {
                            pState = PlayerState.JumpingLeft;
                            yVelocity = Constants.PLAYER_JUMP_SPEED;
                        }
                        else
                        {
                            if (attacheWall != null)
                                YLoc = attacheWall.top - Constants.PLAYER_HEIGHT + 1;
                        }
                        break;
                    }
                #endregion
                #region WalkingRight
                case PlayerState.WalkingRight:
                    {
                        canDoubleJump = true;
                        xVelocity = Constants.WALK_SPEED;
                        Step();
                        if (attacheWall != null)
                            yVelocity = 0;
                        if (YVelocity != 0)
                        {
                            pState = PlayerState.JumpingRight;
                        }
                        if (gM.SingleKeyPress(Keys.D) && dashPrimedRight)
                        {
                            attacheWall = null;
                            YLoc--;
                            pState = PlayerState.DashingRight;
                        }
                        if (kbState.IsKeyUp(Keys.D))
                        {
                            pState = PlayerState.IdleRight;
                        }
                        if (kbState.IsKeyDown(Keys.S))
                        {
                            pState = PlayerState.CrouchingRight;
                        }
                        if (kbState.IsKeyDown(Keys.W) && onPlatform)
                        {
                            pState = PlayerState.JumpingRight;
                            yVelocity = Constants.PLAYER_JUMP_SPEED;
                        }
                        else
                        {
                            if (attacheWall != null)
                                YLoc = attacheWall.top - Constants.PLAYER_HEIGHT + 1;
                        }
                        if (collidingRight)
                            pState = PlayerState.IdleRight;
                        break;
                    }
                #endregion
                #region WalkingLeft
                case PlayerState.WalkingLeft:
                    {
                        canDoubleJump = true;
                        xVelocity = -Constants.WALK_SPEED;
                        Step();
                        if (attacheWall != null)
                            yVelocity = 0;
                        if (YVelocity != 0)
                        {
                            pState = PlayerState.JumpingLeft;
                        }
                        if (YVelocity != 0)
                        {
                            pState = PlayerState.JumpingRight;
                        }
                        if (gM.SingleKeyPress(Keys.A) && dashPrimedLeft)
                        {
                            attacheWall = null;
                            YLoc--;
                            pState = PlayerState.DashingLeft;
                        }
                        if (kbState.IsKeyUp(Keys.A)) 
                        {
                            pState = PlayerState.IdleLeft;
                        }
                        if (kbState.IsKeyDown(Keys.S))
                        {
                            pState = PlayerState.CrouchingLeft;
                        }
                        if (kbState.IsKeyDown(Keys.W) && onPlatform)
                        {
                            pState = PlayerState.JumpingRight;
                            yVelocity = Constants.PLAYER_JUMP_SPEED;

                        }
                        else
                        {
                            if (attacheWall != null)
                                YLoc = attacheWall.top - Constants.PLAYER_HEIGHT + 1;
                        }
                        if (collidingLeft)
                            pState = PlayerState.IdleLeft;
                        break;
                    }
                #endregion
                #region JumpingRight
                case PlayerState.JumpingRight:
                    {
                        if (YVelocity == 0)
                            pState = PlayerState.IdleRight;
                        else
                            attacheWall = null;
                        // The double jump
                        if (gM.SingleKeyPress(Keys.W) && canDoubleJump)
                        {
                            _doubleJump.Play(.3f,0,0);
                            YVelocity = Constants.PLAYER_JUMP_SPEED;
                            canDoubleJump = false;
                            dJNow = true;
                            
                        }
                        if (kbState.IsKeyDown(Keys.S))
                            pState = PlayerState.CrouchingRight;
                        if (kbState.IsKeyDown(Keys.A) && kbState.IsKeyUp(Keys.D))
                        {
                            pState = PlayerState.JumpingLeft;
                            xVelocity = -Constants.WALK_SPEED;
                        }
                        if (gM.SingleKeyPress(Keys.D) && dashPrimedRight && canDoubleJump)
                        {
                            YVelocity = 0;
                            pState = PlayerState.DashingRight;
                        }
                        else if (kbState.IsKeyDown(Keys.D))
                            xVelocity = Constants.WALK_SPEED;
                        if (kbState.IsKeyUp(Keys.W) && yVelocity < 0)
                            yVelocity /= 1.5;
                        break;
                    }
                #endregion
                #region JumpingLeft
                case PlayerState.JumpingLeft:
                    {
                        
                        if (YVelocity == 0)
                            pState = PlayerState.IdleLeft;
                        else
                            attacheWall = null;
                        // The double jump;
                        if (gM.SingleKeyPress(Keys.W) && canDoubleJump)
                        {
                            _doubleJump.Play(.3f, 0, 0);
                            YVelocity = Constants.PLAYER_JUMP_SPEED;
                            canDoubleJump = false;
                            dJNow = true; 
                        }
                        if (kbState.IsKeyDown(Keys.S))
                            pState = PlayerState.CrouchingLeft;
                        if (kbState.IsKeyDown(Keys.D))
                        {
                            pState = PlayerState.JumpingRight;
                            xVelocity = Constants.WALK_SPEED;
                        }
                        if (gM.SingleKeyPress(Keys.A) && dashPrimedLeft && canDoubleJump)
                        {
                            YVelocity = 0;
                            pState = PlayerState.DashingLeft;
                        }
                        else if (kbState.IsKeyDown(Keys.A) && !collidingLeft)
                            xVelocity = -Constants.WALK_SPEED;
                        if (kbState.IsKeyUp(Keys.W) && yVelocity < 0)
                            yVelocity /= 1.5;
                        break;
                    }
                #endregion
                #region CrouchingRight
                case PlayerState.CrouchingRight:
                    {
                        if (attacheWall != null)
                            yVelocity = 0;
                        if (YVelocity == 0)
                            xVelocity = 0;
                        if (kbState.IsKeyUp(Keys.S))
                        {
                            if (YVelocity == 0)
                                pState = PlayerState.IdleRight;
                            else
                                pState = PlayerState.JumpingRight;
                        }
                        break;
                    }
                #endregion
                #region CrouchingLeft
                case PlayerState.CrouchingLeft:
                    {
                        if (attacheWall != null)
                            yVelocity = 0;
                        if (YVelocity == 0)
                            xVelocity = 0;
                        if (kbState.IsKeyUp(Keys.S))
                        {
                            if ( YVelocity == 0)
                                pState = PlayerState.IdleLeft;
                            else
                                pState = PlayerState.JumpingLeft;
                        }
                        break;
                    }
                #endregion
                #region DashingRight
                case PlayerState.DashingRight:
                    {
                        dashPrimedRight = false;
                        attacheWall = null;
                        XVelocity = Constants.DASH_SPEED;
                        timeDashing += timeElapsed;
                        if (timeDashing >= Constants.DASH_TIME)
                        {
                            timeDashing = 0;
                            XVelocity = Constants.WALK_SPEED;
                            canDoubleJump = false;
                            dashed = false;
                            pState = PlayerState.JumpingRight;
                        }
                        if (!dashed)
                        {
                            _dashSound.Play();
                            dashed = true;
                        }
                        break;
                    }
                #endregion
                #region DashingLeft
                case PlayerState.DashingLeft:
                    {
                        dashPrimedLeft = false;
                        attacheWall = null;
                        XVelocity = -Constants.DASH_SPEED;
                        timeDashing += timeElapsed;
                        if (timeDashing >= Constants.DASH_TIME)
                        {
                            timeDashing = 0;
                            XVelocity = -Constants.WALK_SPEED;
                            canDoubleJump = false;
                            dashed = false;
                            pState = PlayerState.JumpingLeft;
                        }
                        if (!dashed)
                        {
                            _dashSound.Play();
                            dashed = true;
                        }
                        break;
                    }
                #endregion
            }
            #endregion


            foreach (Fireball f in projectiles)
            {
                f.Update();
            }


            rect = new Rectangle((int)loc.X, (int)loc.Y, Constants.PLAYER_WIDTH, Constants.PLAYER_HEIGHT);
            base.Update();
            impact = false;
        }

        /// <summary>
        /// Called during draw in game class
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {

            // player sprite FSM
            switch (pState)
            {
                case PlayerState.IdleRight:
                    {
                        spriteBatch.Draw(_spritesheet, rect, new Rectangle((Constants.PLAYER_WIDTH * 4) * 0, 0, Constants.PLAYER_WIDTH * 4, Constants.PLAYER_HEIGHT * 4), Color.White);
                        break;
                    }
                case PlayerState.IdleLeft:
                    {
                        spriteBatch.Draw(_spritesheet, rect, new Rectangle((Constants.PLAYER_WIDTH * 4) * 0, 0, Constants.PLAYER_WIDTH * 4, Constants.PLAYER_HEIGHT * 4), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
                case PlayerState.WalkingRight:
                    {
                        spriteBatch.Draw(_spritesheet, rect, new Rectangle((Constants.PLAYER_WIDTH * 4) * (3+frame), 0, Constants.PLAYER_WIDTH * 4, Constants.PLAYER_HEIGHT * 4), Color.White);
                        break;
                    }
                case PlayerState.WalkingLeft:
                    {
                        spriteBatch.Draw(_spritesheet, rect, new Rectangle((Constants.PLAYER_WIDTH * 4) * (3+frame), 0, Constants.PLAYER_WIDTH * 4, Constants.PLAYER_HEIGHT * 4), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
                case PlayerState.JumpingRight:
                    {
                        spriteBatch.Draw(_spritesheet, rect, new Rectangle((Constants.PLAYER_WIDTH * 4) * 1, 0, Constants.PLAYER_WIDTH * 4, Constants.PLAYER_HEIGHT * 4), Color.White);
                        break;
                    }
                case PlayerState.JumpingLeft:
                    {
                        spriteBatch.Draw(_spritesheet, rect, new Rectangle((Constants.PLAYER_WIDTH * 4) * 1, 0, Constants.PLAYER_WIDTH * 4, Constants.PLAYER_HEIGHT * 4), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
                case PlayerState.CrouchingRight:
                    {
                        spriteBatch.Draw(_spritesheet, rect, new Rectangle((Constants.PLAYER_WIDTH * 4) * 2, 0, Constants.PLAYER_WIDTH * 4, Constants.PLAYER_HEIGHT * 4), Color.White);
                        break;
                    }
                case PlayerState.CrouchingLeft:
                    {
                        spriteBatch.Draw(_spritesheet, rect, new Rectangle((Constants.PLAYER_WIDTH * 4) * 2, 0, Constants.PLAYER_WIDTH * 4, Constants.PLAYER_HEIGHT * 4), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
                case PlayerState.DashingRight:
                    {
                        spriteBatch.Draw(_dash, new Rectangle(rect.X,rect.Y+5,Constants.PLAYER_WIDTH*2,Constants.PLAYER_HEIGHT),
                            new Rectangle((Constants.PLAYER_WIDTH*8) * dashFrame, 0, Constants.PLAYER_WIDTH*8,Constants.PLAYER_HEIGHT*4), Color.White);
                        break;
                    }
                case PlayerState.DashingLeft:
                    {
                        spriteBatch.Draw(_dash, new Rectangle(rect.X, rect.Y+5, Constants.PLAYER_WIDTH * 2, Constants.PLAYER_HEIGHT),
                            new Rectangle((Constants.PLAYER_WIDTH * 8) * dashFrame, 0, Constants.PLAYER_WIDTH * 8, Constants.PLAYER_HEIGHT * 4),
                            Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        break;
                    }
            }


            foreach (Fireball f in projectiles)
            {
                spriteBatch.Draw(f.FireBallT2D, f.FireBallRect, Color.White);
            }

            if (inProgress == true)
            {
                spriteBatch.Draw(_cloud,new Rectangle(cloudX,cloudY,Constants.PLAYER_CLOUD_WIDTH,Constants.PLAYER_CLOUD_HEIGHT), new Rectangle(( 0 +(272 * jFrame)),0,272, 160),Color.White);
            }
        }


        public void GetFrame(GameTime time)
        {
            framesElapsed = (int)(time.TotalGameTime.TotalMilliseconds / 100);
            frame = framesElapsed % 3;


            // get frame for double jump animation
            #region Doublejump
            if (dJNow == true)
            {
                dJumpFrames = framesElapsed;
                cloudX = (rect.X + Constants.PLAYER_WIDTH / 2)-(Constants.PLAYER_WIDTH);
                cloudY = (rect.Y + Constants.PLAYER_HEIGHT)-20;

                inProgress = true;
                dJNow = false;
            }
            if (inProgress == true)
            {
                if ((framesElapsed - dJumpFrames) < 7)
                {
                    jFrame = (framesElapsed - dJumpFrames) % 7;

                }
                else
                {
                    inProgress = false;
                }
            }
            #endregion

            // get frame for dash animation
            #region Dash
            if (pState == PlayerState.DashingRight || pState == PlayerState.DashingLeft)
            {
                dashFrame = (int)timeDashing / ((int)Constants.DASH_TIME/4);
            }
            #endregion
        }

        public void Reset()
        {
            Loc = new Vector2(Constants.PLAYER_START_X, Constants.PLAYER_START_Y);
            XVelocity = 0;
            YVelocity = 0;
            OnPlatform = false;
            AttacheWall = null;
            CanDoubleJump = true;
            TimeDashing = 0;
            dashPrimedRight = false;
            dashPrimedLeft = false;
            Score = 0;
        }

        public void Step()
        {
            if (frame == 0 && !stepped)
            {
                stepped = true;
                if (step)
                    _step1.Play(.3f,0,0);
                else
                    _step0.Play(.3f, 0, 0);
            }
            else if (frame != 0)
                stepped = false;
        }

        public void SetPosition(int x, int y)
        {
            loc.X = x;
            loc.Y = y;
            rect.X = (int)loc.X;
            rect.Y = (int)loc.Y;
        }

        public void CollideWithFireball(List<Fireball> enemyFireballs, Enemy enemy)
        {
            for (int i = 0; i < enemyFireballs.Count; i++)
            {
                if (enemyFireballs[i].FireBallRect.Intersects(Rect))
                {
                    Impact = true;
                    if (PState != Player.PlayerState.CrouchingLeft && PState != Player.PlayerState.CrouchingRight)
                    {
                        if (PState == Player.PlayerState.JumpingLeft || PState == Player.PlayerState.JumpingRight)
                        {
                            YVelocity += enemyFireballs[i].YVelocity / 1.75;
                            XVelocity += enemyFireballs[i].XVelocity / 1.75;
                        }
                        else
                        {
                            XVelocity += enemyFireballs[i].XVelocity;
                        }
                    }
                    // play sound effect based on type of fireball
                    switch (enemy.EType)
                    {
                        case Enemy.EnemyType.Earth:
                            _earthImpact.Play(.5f,0,0);
                            break;
                        case Enemy.EnemyType.Fire:
                            _fireImpact.Play(.5f, 0, 0);
                            break;
                        case Enemy.EnemyType.Water:
                            _iceImpact.Play(.5f, 0, 0);
                            break;
                        case Enemy.EnemyType.Wind:
                            _airImpact.Play(.5f, 0, 0);
                            break;
                    }
 
                    enemyFireballs.RemoveAt(i);
                    if (i != 0)
                        i--;

                    if (enemyFireballs.Count == 0)
                        break;
                }
            }
        }
        
        #endregion


    }
}

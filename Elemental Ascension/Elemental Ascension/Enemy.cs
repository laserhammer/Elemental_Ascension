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
    class Enemy : Entity

    {
        public enum EnemyType
        {
            Fire,
            Water,
            Wind,
            Earth
        }

        #region Attributes
        Texture2D _enemyImage;
        Texture2D _enemyDeath;
        private int frame;
        private int framesElapsed;
        private int startFrame;
        private int playerX;
        private bool dying;
        public Wall wallItsOn;
        public bool falling;
        public List<Fireball> _enemyFireballs;
        private Texture2D _fball;
        public double time;
        private EnemyType eType;       
        
        #endregion

        public bool isDying { get { return dying; } set { dying = value; } }
        public int PlayerX { get { return playerX; } set { playerX = value; } }
        public EnemyType EType { get { return eType; } }

     
        #region Constructor
        public Enemy(int x, int y, Texture2D image, EnemyType t,Wall w,Texture2D death, Texture2D fball)
            : base(Constants.ENEMY_WIDTH, Constants.ENEMY_HEIGHT, new Vector2(x, y))
        {
            _enemyFireballs = new List<Fireball>();
            _fball = fball;
            _enemyImage = image;
            _enemyDeath = death;
            wallItsOn = w;
            falling = false;
            eType = t;
            time = 2;
        }
        #endregion

        

        #region Methods


        #region Update

        public void Update(double TimeElapsed, Player p)
        {
            
            time = time - TimeElapsed;
            if (IsAlive)
            {
                if (time <= 0 && loc.Y > 0)
                {
                    Fireball f = new Fireball(this, p, _fball);
                    _enemyFireballs.Add(f);
                    time = Constants.INT_BETWEEN_SHOTS;
                }
            }
            foreach (Fireball f in _enemyFireballs)
            {
                f.Update();
            }
            
            // check collision against player
            p.CollideWithFireball(_enemyFireballs, this);
        }

        #endregion
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                if (playerX > rect.X + (Constants.ENEMY_WIDTH / 2))
                {
                    spriteBatch.Draw(_enemyImage, rect, Color.White);
                }
                else
                {
                    spriteBatch.Draw(_enemyImage, rect, new Rectangle(0,0,_enemyImage.Width, _enemyImage.Height), Color.White,0,Vector2.Zero,SpriteEffects.FlipHorizontally,0);
                }
                
                else
                {
                    if (dying == true)
                    {
                        if (playerX > rect.X + (Constants.ENEMY_WIDTH / 2))
                        {
                            if (frame < 6)
                            {
                                spriteBatch.Draw(_enemyImage, rect, new Rectangle(0, 0, _enemyImage.Width, _enemyImage.Height), Color.White);
                            }

                            spriteBatch.Draw(_enemyDeath, rect, new Rectangle(Constants.ENEMY_SPRITE_WIDTH * frame, 0, Constants.ENEMY_SPRITE_WIDTH, Constants.ENEMY_SPRITE_HEIGHT), Color.White);
                        }
                        else
                        {
                            if (frame < 6)
                            {
                                spriteBatch.Draw(_enemyImage, rect, new Rectangle(0, 0, _enemyImage.Width, _enemyImage.Height), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                            }

                            spriteBatch.Draw(_enemyDeath, rect, new Rectangle(Constants.ENEMY_SPRITE_WIDTH * frame, 0, Constants.ENEMY_SPRITE_WIDTH, Constants.ENEMY_SPRITE_HEIGHT), Color.White, 0,Vector2.Zero, SpriteEffects.FlipHorizontally,0);
                        }
                    }
                }
            //Draws each fireball object that the enemy has generated
            foreach (Fireball f in _enemyFireballs)
            {
                switch (eType)
                {
                    case EnemyType.Wind:
                    {
                        spriteBatch.Draw(f.FireBallT2D, f.FireBallRect, Color.Aquamarine);
                        break;
                    }
                    case EnemyType.Fire:
                    {
                        spriteBatch.Draw(f.FireBallT2D, f.FireBallRect, Color.Red);
                        break;
                    }
                    case EnemyType.Water:
                    {
                        spriteBatch.Draw(f.FireBallT2D, f.FireBallRect, Color.Blue);
                        break;
                    }
                    case EnemyType.Earth:
                    {
                        spriteBatch.Draw(f.FireBallT2D, f.FireBallRect, Color.Azure);
                        break;
                    }
                }
            }

        }
        
        // frames for death animation;
        public void getFrames(GameTime time)
        {
            framesElapsed = (int)(time.TotalGameTime.TotalMilliseconds / 100);

            if (dying == false)
            {
                startFrame = framesElapsed;
            }
            else if (dying == true)
            {
                if (framesElapsed - startFrame < 9)
                {
                    frame = (framesElapsed - startFrame) % 9;
                }
                else
                {
                    dying = false;
                }
            }
            
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Elemental_Ascension
{
    class Wall : Entity
    {
        #region Attributes

        public int top;
        public int bottom;
        public int left;
        public int right;
        public int frame;
        public int framesElapsed;
        public int startFrame;
        public bool burning;
        public Texture2D _burn;
        public enum WallType
        {
            BurnableSmall,
            BurnableLeft,
            BurnableRight,
            Crumbling,
            Solid
        }
        private WallType wType;

        #endregion

        #region Properties

        public WallType WType { get { return wType; } }


        #endregion

        #region Constructor
       
        /// <summary>
        /// Use this overload for Long walls
        /// </summary>
        /// <param name="wall">The type of wall that you want</param>
        /// <param name="x">x-coordinate of the location of the wall</param>
        /// <param name="y">y-coordinate of the location of the wall</param>
        public Wall(WallType wall, int x, int y)
            : base(Constants.WALL_WIDTH, Constants.WALL_HEIGHT, new Microsoft.Xna.Framework.Vector2(x, y))
        {
            this.wType = wall;
            top = rect.Y;
            bottom = rect.Y + rect.Height;
            right = rect.X + rect.Width;
            left = rect.X;
        }

        public Wall(Wall wall, Texture2D burningAnim)
            :base(Constants.WALL_WIDTH, Constants.WALL_HEIGHT, new Microsoft.Xna.Framework.Vector2(wall.Rect.X, wall.Rect.Y))
        {
            wType = wall.wType;
            top = wall.rect.Y;
            bottom = wall.rect.Y + wall.rect.Height;
            right = wall.rect.X + wall.rect.Width;
            left = wall.rect.X;
            _burn = burningAnim;
        }
        #endregion

        #region Methods
        public override void Update()
        {
            rect.X = (int)loc.X;
            rect.Y = (int)loc.Y;
            top = rect.Y;
            bottom = rect.Y + rect.Height;
            right = rect.X + rect.Width;
            left = rect.X;


        }



        public void Draw(SpriteBatch spriteBatch, Texture2D image)
        {
            if (isAlive)
                spriteBatch.Draw(image, rect, new Rectangle(0, 0, 256, 128), Color.White);
            else
            {
                //TODO: wall crumbling animations
                if (this.wType == WallType.Crumbling)
                {
                }
                if (this.wType == WallType.BurnableLeft || this.wType == WallType.BurnableRight||this.wType == WallType.BurnableSmall)
                {
                    burning = true;
                    if (frame < 5)
                    {
                        spriteBatch.Draw(image, rect, new Rectangle(0, 0, 256, 128), Color.White);
                    }
                    spriteBatch.Draw(_burn, new Rectangle(rect.X, (rect.Bottom - Constants.WALL_BURN_HEIGHT), Constants.WALL_WIDTH, Constants.WALL_BURN_HEIGHT), new Rectangle((270 * frame), 0, 270, 190), Color.White);

                }
            }
        }

        public void getFrames(GameTime time)
        {
            framesElapsed = (int)(time.TotalGameTime.TotalMilliseconds / 100);

            if (burning == false)
            {
                startFrame = framesElapsed;
            }
            else if (burning == true)
            {
                if (framesElapsed - startFrame < 7)
                {
                    frame = (framesElapsed - startFrame);
                }
            }

        }
        #endregion
    }
}

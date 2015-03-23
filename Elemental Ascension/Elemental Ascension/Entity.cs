using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Elemental_Ascension
{
    abstract class Entity
    {
        #region Attributes
        protected Rectangle rect;
        protected Vector2 loc;
        protected double xVelocity;
        protected double yVelocity;
        protected bool collidingRight;
        protected bool collidingLeft;
        protected bool isAlive;
        #endregion

        #region Properties
        public Rectangle Rect { get { return rect; } set { rect = value; } }
        public double XVelocity { get { return xVelocity; } set { xVelocity = value; } }
        public double YVelocity { get { return yVelocity; } set { yVelocity = value; } }
        public Vector2 Loc { get { return loc; } set { loc = value; } }
        public float YLoc
        {
            get { return loc.Y; }
            set
            {
                loc.Y = value;
                rect.Y = (int)value;
            }
        }
        public bool CollidingRight { get { return collidingRight; } set { collidingRight = value; } }
        public bool CollidingLeft { get { return collidingLeft; } set { collidingLeft = value; } }
        public bool IsAlive { get { return isAlive; } set { isAlive = value; } }
        #endregion

        #region Constructor
        public Entity(int width, int height, Vector2 loc)
        {
            rect = new Rectangle((int)loc.X, (int)loc.Y, width, height);
            xVelocity = 0;
            yVelocity = 0;
            this.loc = loc;
            isAlive = true;
        }
        #endregion 

        #region Methods

        public virtual void Update()
        {
            // move the entity based on the entity's velocity
            //if(!collidingRight && XVelocity > 0 || XVelocity < 0 && !collidingLeft )
            loc.X += (int)XVelocity; 

            loc.Y += (int)YVelocity;

            // move the entity's rectangle along with them
            rect.X = (int)loc.X;
            rect.Y = (int)loc.Y;
            collidingLeft = false;
            collidingRight = false;

        }

        /// <summary>
        /// Draws image of the object on the screen
        /// </summary>
        /// <param name="image">Image to be used</param>
        /// <param name="spriteBatch"></param>
        public void Draw(Texture2D image, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, rect, Color.White);
        }
        #endregion
    }
}

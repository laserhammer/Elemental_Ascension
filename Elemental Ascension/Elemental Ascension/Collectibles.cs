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
    class Collectibles : Entity
    {
        #region Attributes
        Texture2D _collectible;
        int frame;
        int framesElapsed;
        //TODO: need collectible sprites
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Collectibles(int x, int y, Texture2D image)
            : base(Constants.COLLECTABLE_WIDTH, Constants.COLLECTABLE_HEIGHT, new Vector2(x, y))
        {
            _collectible = image;
        }
        #endregion

        #region Methods
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_collectible, rect, new Rectangle((128*frame),0,128,128), Color.White);
        }

        // frames for collectible animation
        public void GetFrame(GameTime time)
        {
            framesElapsed = (int)(time.TotalGameTime.TotalMilliseconds / 100);
            frame = framesElapsed % 6;
        }
        #endregion
    }
}

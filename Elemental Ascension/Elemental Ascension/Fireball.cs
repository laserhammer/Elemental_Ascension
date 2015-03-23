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
    class Fireball
    {
        //Attributes
        Rectangle fireball = new Rectangle(0, 0, 10, 10);
        Player player;
        Texture2D firet2D;
        Vector2 firepos = Vector2.Zero;
        Vector2 direction;

        const int fireballspeed = 10;
        double fireballxs = 0;
        double fireballys = 0;
        double rad;

        //Properties
        public float YLoc { get { return firepos.Y; } set { firepos.Y = value; fireball.Y = (int)firepos.Y; } }
        public Rectangle FireBallRect { get { return fireball; } set { fireball = value; } }
        public Texture2D FireBallT2D { get { return firet2D; } set { firet2D = value; } }
        public double XVelocity { get { return fireballxs; } }
        public double YVelocity { get { return fireballys; } }

        /// <summary>
        /// Fireball that the player fires from the character's center to the point he or she clicked on
        /// </summary>
        /// <param name="p">The player</param>
        /// <param name="ms">The mousestate passed in to find the direction that fireball travels</param>
        public Fireball(Player p, MouseState ms)
        {
            player = p;
            direction = new Vector2(ms.X - p.Rect.Center.X, ms.Y - p.Rect.Center.Y);
            fireball.X = p.Rect.Center.X;
            fireball.Y = p.Rect.Center.Y;
            firepos.X = fireball.X;
            firepos.Y = fireball.Y;
            rad = Math.Atan2(((float)direction.Y), ((float)direction.X));
            fireballxs = fireballspeed * Math.Cos(rad);
            fireballys = fireballspeed * Math.Sin(rad);

        }


        /// <summary>
        /// Used by the enemy class to fire at the player
        /// </summary>
        /// <param name="e">Enemy that fires</param>
        /// <param name="p">The player on screen</param>
        /// <param name="t">Fireball texture</param>
        public Fireball(Enemy e, Player p, Texture2D t)
        {
            firet2D = t;
            direction = new Vector2(p.Rect.Center.X - e.Rect.Center.X, p.Rect.Center.Y - e.Rect.Center.Y);
            fireball.X = e.Rect.Center.X;
            fireball.Y = e.Rect.Center.Y;
            firepos.X = fireball.X;
            firepos.Y = fireball.Y;
            rad = Math.Atan2(((float)direction.Y), ((float)direction.X));
            fireballxs = fireballspeed * Math.Cos(rad);
            fireballys = fireballspeed * Math.Sin(rad);
        }


        public void Update()
        {
            firepos.X += (float)fireballxs;
            firepos.Y += (float)fireballys;

            fireball.X = (int)firepos.X;
            fireball.Y = (int)firepos.Y;
        }
    }
}
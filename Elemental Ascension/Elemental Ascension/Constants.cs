using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elemental_Ascension
{
    public static class Constants
    {
        
        // the acceleration due to gravity
        public static readonly double GRAVITY = 15.0;
        //the speed at which the player can run
        public static readonly int WALK_SPEED = 4;
        public static readonly int DASH_SPEED = 16;
        public static readonly double DASH_TIME = 150;
        // size of the player's sprite
        public static readonly int PLAYER_HEIGHT = 64;
        public static readonly int PLAYER_WIDTH = 32;
        // size of player double jump
        public static readonly int PLAYER_CLOUD_HEIGHT = 80;
        public static readonly int PLAYER_CLOUD_WIDTH = 80;
        // Starting position of the player on the screen
        public static readonly int PLAYER_START_X = 400;
        public static readonly int PLAYER_START_Y = 50;
        //initial speed of the player's jump
        public static readonly int PLAYER_JUMP_SPEED = -9;
        //height and width of NORMAL walls
        public static readonly int WALL_WIDTH = 128;
        public static readonly int WALL_HEIGHT = 32;
        public static readonly int WALL_BURN_HEIGHT = 48;

        //Environment WallList file
        public static readonly string WALL_FILENAME = "wallList.txt";
        public static readonly int WALL_ARRAYWIDTH = 7;
        public static readonly int WALLS_TOGENERATE = 6;
        
        // Viewport dimensions
        public static readonly int FLOOR_WIDTH = 896;
        public static readonly int VIEWPORT_HEIGHT = 480;
        public static readonly int VIEWPORT_WIDTH = 896;
        public static readonly int HEIGHT_BETWEEN_WALLS = (VIEWPORT_HEIGHT - WALL_HEIGHT) / WALLS_TOGENERATE;
        
        //WallChecker values
        public static readonly int WALLCHECKER_SPAWNHEIGHT = -10;

        // Time allowed between taps for a double-tap
        public static readonly int DOUBLETAP_TIME = 120;

        // Collectable dimensions
        public static readonly int COLLECTABLE_WIDTH = 32;
        public static readonly int COLLECTABLE_HEIGHT = 32;

        //Enemy stuff
        public static readonly int ENEMY_HEIGHT = 64;
        public static readonly int ENEMY_WIDTH = 32;
        public static readonly int ENEMY_SPRITE_HEIGHT = 256;
        public static readonly int ENEMY_SPRITE_WIDTH = 128;
        public static readonly int INT_BETWEEN_SHOTS = 1500;

    }
}

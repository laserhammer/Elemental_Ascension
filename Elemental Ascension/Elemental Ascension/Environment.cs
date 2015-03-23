using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Elemental_Ascension
{

    class Environment
    {
        private List<Wall[]> wallList;
        private List<Wall> activeWalls;
        public List<Wall> ActiveWalls { get { return activeWalls; } set { activeWalls = value; } }
        private List<Collectibles> collectibles;
        public List<Collectibles> Collectibles { get { return collectibles; } set { collectibles = value; } }
        private List<Enemy> enemies;
        public List<Enemy> Enemies { get { return enemies; } set { enemies = value; } }
        private Random r;
        private int highestWall;
        
        //Collectible sprites
        private Texture2D _CollectibleRed;
        private Texture2D _CollectibleBlue;
        private Texture2D _CollectibleGreen;
        private Texture2D randCollectible;

        // Wall sprites
        private Texture2D _BurningSmall;
        private Texture2D _Solid;
        private Texture2D _Crumbling;
        private Texture2D _BurnLeft;
        private Texture2D _BurnRight;
        private Texture2D _waterMage;
        private Texture2D _earthMage;
        private Texture2D _windMage;
        private Texture2D _fireMage;
        private Texture2D _enemyDeath;
        private Texture2D _fireball;
        private Texture2D _wallShadow;
        private Texture2D _burning;


        public Environment(Game1 game)
        {
            r = new Random();
            //sets the wallList
            wallList = new List<Wall[]>();
            activeWalls = new List<Wall>();
            collectibles = new List<Collectibles>();
            enemies = new List<Enemy>();
            //Reads wall file
            readWallFile();
            Load(game);
            highestWall = -Constants.VIEWPORT_HEIGHT;
        }



        #region Methods


        public void Load(Game1 game)
        {
            //Load the wall sprites

            _Crumbling = game.Content.Load<Texture2D>("CrumblingWall");
            _Solid = game.Content.Load<Texture2D>("SolidWall1");
            _BurningSmall = game.Content.Load<Texture2D>("BurnableSmall");
            _BurnLeft = game.Content.Load<Texture2D>("BurnableLeft");
            _BurnRight = game.Content.Load<Texture2D>("BurnableRight");
            _CollectibleRed = game.Content.Load<Texture2D>("redCollectibleSheet");
            _CollectibleBlue = game.Content.Load<Texture2D>("blueCollectibleSheet");
            _CollectibleGreen = game.Content.Load<Texture2D>("greenCollectibleSheet");
            _windMage = game.Content.Load<Texture2D>("mageWind");
            _earthMage = game.Content.Load<Texture2D>("mageEarth");
            _waterMage = game.Content.Load<Texture2D>("mageWater");
            _fireMage = game.Content.Load<Texture2D>("mageFire");
            _enemyDeath = game.Content.Load<Texture2D>("EnemyBurnComplete");
            _fireball = game.Content.Load<Texture2D>("Fireball");
            _wallShadow = game.Content.Load<Texture2D>("wallShadow");
            _burning = game.Content.Load<Texture2D>("BurningWalls");
            
        }
        
        //modifier alters the distance the walls are placed.
        //I use it to place walls higher up.
        public void AddWalls(int modifier, int wallsToMake)
        {
            Wall[] floor;
            for (int i = 0; i < wallsToMake; i++)
            {
                int index = r.Next(wallList.Count);

                floor = wallList[index];

                foreach(Wall w in floor)
                {
                    if(w != null)
                    {
                        bool collectiblePlaced = false;
                        w.YLoc = (i * 128)-modifier;
                        activeWalls.Add(new Wall(w,_burning));
                        collectiblePlaced = PlaceCollectible(w);
                        if (collectiblePlaced == false)
                        {
                            PlaceEnemy(activeWalls.Last<Wall>());
                        }
                        
                    }
                }
                 
            }
        }

      
        
        public void Update(float scrollSpeed)
        {
            bool wallRemoved = false;

            for ( int i = 0; i < activeWalls.Count; i++)
            {
                
                activeWalls[i].YLoc += scrollSpeed;
                activeWalls[i].Update();
                if (activeWalls[i].top > Constants.VIEWPORT_HEIGHT+Constants.PLAYER_HEIGHT)
                {
                    activeWalls.RemoveAt(i);
                    i--;
                    wallRemoved = true;
                }
            }

            for (int i = 0; i < collectibles.Count; i++)
            {
                collectibles[i].YLoc += scrollSpeed;
                if (collectibles[i].Rect.X > Constants.VIEWPORT_HEIGHT)
                {
                    collectibles.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].YLoc += scrollSpeed;
                enemies[i].YLoc += (float)enemies[i].YVelocity;

                //Checking to see if wall has been destroyed
                //If it is destroyed, the enemy falls.
                if (enemies[i].wallItsOn != null)
                {
                    if (enemies[i].wallItsOn.IsAlive == false)
                    {
                        enemies[i].falling = true;
                    }
                }

                //if falling, check for collisions with other walls.
                if (enemies[i].falling == true)
                {
                    for (int j = 0; j < activeWalls.Count; j++)
                    {
                        if (activeWalls[j].IsAlive == true)
                        {
                            if (enemies[i].Rect.Intersects(activeWalls[j].Rect))
                            {
                                enemies[i].wallItsOn = activeWalls[j];
                                enemies[i].falling = false;
                                enemies[i].YLoc = activeWalls[j].top - Constants.ENEMY_HEIGHT;
                                enemies[i].YVelocity = 0;
                            }
                        }
                    }
                }

                if (enemies[i].Rect.Y > Constants.VIEWPORT_HEIGHT)
                {
                    enemies.RemoveAt(i);
                    i--;
                }
            }

            if (wallRemoved == true)
            {
                AddWalls((128*(Constants.WALLS_TOGENERATE-4))+Constants.WALL_HEIGHT-Constants.PLAYER_HEIGHT, 1);
            }
           


        }
       

        public void Draw(SpriteBatch sb)
        {

            foreach (Wall w in activeWalls)
            {
                if (w.WType == Wall.WallType.Crumbling || w.WType == Wall.WallType.Solid)
                    sb.Draw(_wallShadow, new Rectangle(w.Rect.X, w.Rect.Y + 16, Constants.WALL_WIDTH, Constants.WALL_HEIGHT), Color.White);
                else if(w.IsAlive)
                    sb.Draw(_wallShadow, new Rectangle(w.Rect.X, w.Rect.Y, Constants.WALL_WIDTH, Constants.WALL_HEIGHT), Color.White);
                w.Draw(sb, GetWallImage(w));

            }
            foreach (Collectibles collectible in collectibles)
            {
                collectible.Draw(sb);
            }
            foreach (Enemy e in enemies)
            {
                e.Draw(sb);
            }
        }


        //retieves the image that a particualr wall requires
        private Texture2D GetWallImage(Wall wall)
        {
            switch (wall.WType)
            {
                case Wall.WallType.Solid:
                    {
                        return _Solid;
                    }
                case Wall.WallType.Crumbling:
                    {
                        return _Crumbling;
                    }
                case Wall.WallType.BurnableSmall:
                    {
                        return _BurningSmall;
                    }
                case Wall.WallType.BurnableLeft:
                    {
                        return _BurnLeft;
                    }
                case Wall.WallType.BurnableRight:
                    {
                        return _BurnRight;
                    }
            }

            return null;
        }

        #region fileReadingStuff
        //Reads the txt file containing the walls.
        //Each line represents a different floor.
        //each line is added to an array of length 7,
        //which is then added to the list<Wall[]>
        public void readWallFile()
        {
            StreamReader input = null;
            string line = null;
           
                input = new StreamReader(Constants.WALL_FILENAME);
                while ((line = input.ReadLine()) != null)
                {
                    wallList.Add(splitAndBuild(line)); //Should take each line, and build a wall.
                }
            

            //This may need to be fixed. Might be a sneaky error.
            

            if (input != null)
                input.Close();

        }

        //will take a string, and return the list of walls.
        public Wall[] splitAndBuild(string line)
        {
            Wall[] walls = new Wall[Constants.WALL_ARRAYWIDTH];
            string[] wallString = new string[Constants.WALL_ARRAYWIDTH];
            string[] wallData = new string[2];

            //breaks up the entire line
            wallString = line.Split('-');

            for (int i = 0; i < wallString.Length; i++)
            {
                //breaks up segment into type/xCoord
                wallData = wallString[i].Split(',');

                //determines the type of wall, then adds it.
                switch (wallData[0])
                {
                    case "BurnL": walls[i] = new Wall(Wall.WallType.BurnableLeft, int.Parse(wallData[1]) , 0);
                        break;
                    case "BurnR": walls[i] = new Wall(Wall.WallType.BurnableRight, int.Parse(wallData[1]), 0);
                        break;
                    case "BurnS": walls[i] = new Wall(Wall.WallType.BurnableSmall, int.Parse(wallData[1]), 0);
                        break;
                    case "Break": walls[i] = new Wall(Wall.WallType.Crumbling, int.Parse(wallData[1]), 0);
                        break;
                    case "Solid": walls[i] = new Wall(Wall.WallType.Solid, int.Parse(wallData[1]), 0);
                        break;
                    default: walls[i] = null; // Later when placing walls, must check for null. Null means empty space.
                        break;
                }
            }

            return walls;
        }
        #endregion


        public bool PlaceCollectible(Wall wall)
        {
            if (r.Next(100) <= 30)
            {
                // randomly choose collectible
                int collectType = r.Next(0, 3);
                switch (collectType)
                {
                    case 0:
                        randCollectible = _CollectibleRed;
                        break;
                    case 1:
                        randCollectible = _CollectibleBlue;
                        break;
                    case 2:
                        randCollectible = _CollectibleGreen;
                        break;
                }
                collectibles.Add(new Collectibles(wall.Rect.X + wall.Rect.Width / 2 - Constants.COLLECTABLE_WIDTH / 2, wall.Rect.Y - Constants.COLLECTABLE_HEIGHT,randCollectible));
                return true;
            }
            return false;
        }

        public void PlaceEnemy(Wall wall)
        {
            if (r.Next(100) <= 10)
            {
                int mageType = r.Next(100);
                if (mageType >=0 && mageType<25)
                {
                     enemies.Add(new Enemy(wall.Rect.X + wall.Rect.Width / 2 - Constants.ENEMY_WIDTH / 2,
                         wall.Rect.Y - Constants.ENEMY_HEIGHT, _earthMage,Enemy.EnemyType.Earth,wall,_enemyDeath, _fireball));
                }
                else if (mageType >= 25 && mageType < 50)
                {
                    enemies.Add(new Enemy(wall.Rect.X + wall.Rect.Width / 2 - Constants.ENEMY_WIDTH / 2,
                        wall.Rect.Y - Constants.ENEMY_HEIGHT, _waterMage,Enemy.EnemyType.Water,wall,_enemyDeath, _fireball));
                }
                else if (mageType >= 50 && mageType < 75)
                {
                    enemies.Add(new Enemy(wall.Rect.X + wall.Rect.Width / 2 - Constants.ENEMY_WIDTH / 2,
                        wall.Rect.Y - Constants.ENEMY_HEIGHT, _windMage,Enemy.EnemyType.Wind,wall,_enemyDeath, _fireball));
                }
                else if (mageType >= 75 && mageType < 100)
                {
                    enemies.Add(new Enemy(wall.Rect.X + wall.Rect.Width / 2 - Constants.ENEMY_WIDTH / 2,
                        wall.Rect.Y - Constants.ENEMY_HEIGHT, _fireMage,Enemy.EnemyType.Fire,wall,_enemyDeath, _fireball));
                }
            }
        }

        #endregion // Methods
    }
}

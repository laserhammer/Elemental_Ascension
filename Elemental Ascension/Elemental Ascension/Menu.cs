using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Elemental_Ascension
{
    class Menus
    {
        #region Attributes
        private string gameoverMessage;
        private Rectangle startButton;
        private Rectangle controlsButton;
        private Rectangle creditsButton;
        private Rectangle selectBox;
        #endregion

        #region Properties
        public string GameoverMessage { get { return gameoverMessage; } set { gameoverMessage = value; } }
        #endregion

        #region Constructor
        public Menus(Vector2 startLoc, Vector2 controlLoc, Vector2 creditsLoc)
        {
            startButton = new Rectangle((int)startLoc.X, (int)startLoc.Y, 200, 50);
            controlsButton = new Rectangle((int)controlLoc.X, (int)controlLoc.Y, 200, 50);
            creditsButton = new Rectangle((int)creditsLoc.X, (int)creditsLoc.Y, 200, 50);
            selectBox = new Rectangle(0, 0, 1, 1);
        }
        #endregion

        #region Methods
        public int UpdateMain(MouseState mouse)
        {
            mouse = Mouse.GetState();


            if (mouse.LeftButton == ButtonState.Pressed)
            {
                selectBox.X = mouse.X;
                selectBox.Y = mouse.Y;
            }

            if (selectBox.Intersects(startButton))
            {
                selectBox.X = 0;
                return 1;
            }
            else if (selectBox.Intersects(controlsButton))
            {
                selectBox.X = 0;
                return 2;
            }
            else if (selectBox.Intersects(creditsButton))
            {
                selectBox.X = 0;
                return 3;
            }

            return 0;

        }

        /// Draws the main menu
        public void MenuDraw(Texture2D text, SpriteBatch sprite, Rectangle title, Texture2D background, Rectangle location, SpriteFont font,Vector2 center)
        {
            sprite.Draw(background, location, Color.White);
            sprite.Draw(text, title, Color.White);
            sprite.DrawString(font, "Start",new Vector2(startButton.X,startButton.Y), Color.Black);
            sprite.DrawString(font, "Controls", new Vector2(controlsButton.X, controlsButton.Y), Color.Black);
            sprite.DrawString(font, "Credits", new Vector2(creditsButton.X, creditsButton.Y), Color.Black);
        }

        //Draw Control screen
        public void ControlDraw(SpriteFont text, SpriteBatch sprite, Vector2 center)
        {
            sprite.DrawString(text, "Behold our controls\n\nUse WASD to move\nClick to throw a Fireball\nPress W in midAir to doublejump\nDouble-tap A or D to dash\nPress P to pause\n\n\n(press enter to continue)", center, Color.White);
        }

        // Draw credits screen
        public void CreditDraw(SpriteFont text, SpriteBatch sprite, Vector2 center)
        {
            sprite.DrawString(text, "Team Frenergetic:\n\nZach Butler\nBen Robbins\nBrandon Walruth\nAlex Worley\n\nMusic: \"Alchemist's Tower\" and \"Failing Defenses\" by Kevin Macleod\n\nSound Effects: www.freeSFX.co.uk\n\nCollectible's Images: DeviantArt DaveAleuma\nTextures: Brusheezy\n\n\n\n(press enter to return)", center, Color.White);
        }

        /// Draws the game display
        public void GameScreenDraw(SpriteFont text, SpriteBatch sprite, int Score)
        {
            string pScore = ""+Score;
            sprite.DrawString(text, pScore, Vector2.Zero, Color.White);
        }

        /// Draws pause menu
        public void PauseDraw(SpriteFont text, SpriteBatch sprite, Vector2 center)
        {
            sprite.DrawString(text, "Paused\n\n\n (press P to resume)", center, Color.White);
        }


        /// Draws Game Over Screen
        public void GameOverDraw(SpriteFont text, SpriteBatch sprite, Vector2 center)
        {
            sprite.DrawString(text, GameoverMessage, center, Color.White);
        }

        // Allow us to taunt the player
        #region GameOver
        public string GameOver(int score)
        {
            gameoverMessage = "Game Over \n\nYour score was "+score+"\n\n\n";

            if (score == 0)
            {
                gameoverMessage += "...Would you like to try again?";
            }
            else if(score <= 100)
            {
                gameoverMessage += "Might as well have been 0.";
            }
            else if(score <= 500)
            {
                gameoverMessage+= "Would you like a jetPack?  You might need one.";
            }
                else if(score <= 800)
            {
                gameoverMessage+= "What an accomplishment.";
            }
            else if(score <= 1000)
            {
                gameoverMessage+= "Great, now lets try to get a good score.";
            }
            else if(score < 1500)
            {
                gameoverMessage+= "Nice job, pro.";
            }
            else if(score < 2000)
            {
                gameoverMessage+= "Do you feel good about yourself?";
            }
            else if(score < 3000)
            {
                gameoverMessage+= "Isn't it glorious.";
            }
            else if(score < 4000)
            {
                gameoverMessage+= "Not bad...";
            }
            else if(score < 5000)
            {
                gameoverMessage+= "Alright, let someone else play.";
            }
            else if(score < 7000)
            {
                gameoverMessage+= "Someone has a lot of time on their hands...";
            }
            else if (score < 9000)
            {
                gameoverMessage += "Don't you have anything better to do?";
            }
            else
            {
                gameoverMessage += "I think that's enough.";
            }

            gameoverMessage += "\n\n\n(Press enter to return to Menu)";
            return gameoverMessage;
        }
        #endregion

        #endregion
    }
}

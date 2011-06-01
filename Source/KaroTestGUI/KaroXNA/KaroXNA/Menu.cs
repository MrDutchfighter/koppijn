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

namespace KaroXNA
{
    public enum MenuState
    {
        NEW,
        STARTED,
        OPTIONS,
        CREDITS

    }
    public class Menu : DrawableGameComponent
    {
        public List<MenuItem> menuList;
        public SpriteBatch spriteBatch;
        public SpriteFont spriteFont;
        Vector2 fontPosition;
        public int selectedItem;
        public KeyboardState oldState;
        public MouseState curMousePos;
        public Game1 game;
        public MenuState menuState;
        ScrollingBackground background;

        public Menu(Game game, int drawOrder, SpriteBatch spriteBatch) : base(game)
        {
            this.game = (Game1)game;
            this.DrawOrder = drawOrder;
            this.spriteBatch = spriteBatch;

            Initialize();
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
            menuState = MenuState.NEW;
            spriteFont = Game.Content.Load<SpriteFont>("MenuFont");
            // half the width and half the height -200
            fontPosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, (Game.GraphicsDevice.Viewport.Height / 2) - 200);
            
            // 
            oldState = Keyboard.GetState();

            menuList = new List<MenuItem>();

            // make menu items
            menuList.Add(new MenuItem("Play"));
            menuList.Add(new MenuItem("Options"));
            menuList.Add(new MenuItem("Credits"));
            menuList.Add(new MenuItem("Exit"));

            //background

            Texture2D backgroundImage = game.Content.Load<Texture2D>("koppijnkaro");
            background = new ScrollingBackground(GraphicsDevice, backgroundImage, 100);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (game.gameState == GameState.MENU)
            {
                UpdateInput();
                background.Update(gameTime);

                if (menuState == MenuState.STARTED && menuList[0].MenuName != "New")
                {
                    // remove Play
                    menuList.RemoveAt(0);
                    // insert Resume
                    menuList.Insert(0, new MenuItem("New"));
                    menuList.Insert(1, new MenuItem("Resume"));
                }
                else if (menuState == MenuState.NEW && menuList[0].MenuName != "Play")
                {
                    // Remove New
                    menuList.RemoveAt(1);
                    // Remove Resume
                    menuList.RemoveAt(0);
                    menuList.Insert(0, new MenuItem("Play"));
                }
            }
            

        }

        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();

            // ARROW DOWN
            if (newState.IsKeyDown(Keys.Down))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.Down))
                {
                    if (selectedItem < menuList.Count - 1)
                        selectedItem++;
                    else
                        selectedItem = 0;
                }
            }

            // ARROW UP
            if (newState.IsKeyDown(Keys.Up))
            {
                if (!oldState.IsKeyDown(Keys.Up))
                {
                    if (selectedItem > 0)
                        selectedItem--;
                    else
                        selectedItem = menuList.Count - 1;
                }
            }

            // ENTER
            if (newState.IsKeyDown(Keys.Enter))
            {
                if (!oldState.IsKeyDown(Keys.Enter))
                {
                    // do item action
                    switch (menuList[selectedItem].MenuName)
                    {
                        case "Play":
                            game.gameState = GameState.PLAYING;
                            menuState = MenuState.STARTED;
                            break;
                        case "Options":
                            Texture2D backgroundImage = game.Content.Load<Texture2D>("menubackground");
                            background.mytexture = backgroundImage;
                            menuState = MenuState.OPTIONS;
                            break;
                        case "Credits":
                            Texture2D backgroundImageCredits = game.Content.Load<Texture2D>("menubackground");
                            background.mytexture = backgroundImageCredits;
                            menuState = MenuState.CREDITS;
                            break;
                        case "Exit":
                            game.Exit();
                            break;
                        case "Resume":
                            game.gameState = GameState.PLAYING;
                            break;
                        case "New":
                            game.gameState = GameState.PLAYING;
                            // call game function to reset the board 
                            game.NewGame();
                            break;
                    }
                }
            }

            // Update saved state.
            oldState = newState;

            // MOUSE
            curMousePos = Mouse.GetState();
        }

        public override void Draw(GameTime gameTime)
        {
            if (game.gameState == GameState.MENU)
            {
                spriteBatch.Begin();
                background.Draw(spriteBatch);

                for (int i = 0; i < menuList.Count; i++)
                {
                    // Find the center of the string
                    Vector2 currentFontPosition = Vector2.Subtract(fontPosition, (spriteFont.MeasureString(menuList[i].MenuName) / 2));
                    currentFontPosition.Y = currentFontPosition.Y + (i * 46);

                    // Draw selected item
                    if (selectedItem == i)
                        spriteBatch.DrawString(spriteFont, menuList[i].MenuName, currentFontPosition, Color.White, 0, new Vector2(spriteFont.MeasureString(menuList[i].MenuName).X /2, spriteFont.MeasureString(menuList[i].MenuName).Y / 2), 1.0f, SpriteEffects.None, 0);
                    else
                        spriteBatch.DrawString(spriteFont, menuList[i].MenuName, currentFontPosition, Color.White, 0, new Vector2(spriteFont.MeasureString(menuList[i].MenuName).X / 2, spriteFont.MeasureString(menuList[i].MenuName).Y / 2), 0.8f, SpriteEffects.None, 0);
                }

                spriteBatch.End();
            }
        }
    }
}
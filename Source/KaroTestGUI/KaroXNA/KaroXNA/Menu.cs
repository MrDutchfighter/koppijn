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
    public class Menu : DrawableGameComponent
    {
        public List<MenuItem> menuList;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Vector2 fontPosition;
            
        public Menu(Game game, int drawOrder)
            : base(game)
        {
            
            this.DrawOrder = drawOrder;
            
            
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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Game.Content.Load<SpriteFont>("MenuFont");
            fontPosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
               (Game.GraphicsDevice.Viewport.Height / 2) - 200);
            

            menuList = new List<MenuItem>();
            // make menu items
            menuList.Add(new MenuItem("Play"));
            menuList.Add(new MenuItem("Options"));
            menuList.Add(new MenuItem("Credits"));
            menuList.Add(new MenuItem("Exit"));

        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //if (!Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
              //  MessageBox.Show("Update called in drawable game component....");

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            //spriteBatch.DrawString(spriteFont, "Play", new Vector2(300, 300), Color.LightGreen);
            for(int i = 0; i < menuList.Count; i++)
            {
                // Find the center of the string
                Vector2 FontOrigin = spriteFont.MeasureString(menuList[i].MenuName) / 2;
                //fontPosition = Vector2.Subtract(fontPosition, FontOrigin);
                Vector2 currentFontPosition = fontPosition;
                currentFontPosition = Vector2.Subtract(fontPosition, FontOrigin);
                currentFontPosition.Y = currentFontPosition.Y + i * 14;
                spriteBatch.DrawString(spriteFont, menuList[i].MenuName, currentFontPosition, Color.LightGreen);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

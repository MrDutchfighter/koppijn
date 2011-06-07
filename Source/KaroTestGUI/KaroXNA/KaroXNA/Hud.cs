using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace KaroXNA
{
    public class Hud : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game;
        SpriteBatch spriteBatch;
        Texture2D hudTexture;
        Texture2D whiteTexture;
        Texture2D redTexture;
        SpriteFont font;
        Texture2D Turn;

        public Hud(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            this.game = (Game1)game;
            this.spriteBatch = spriteBatch;
            this.DrawOrder = 100000;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.hudTexture = Game.Content.Load<Texture2D>("Hud");
            this.whiteTexture = Game.Content.Load<Texture2D>("White");
            this.redTexture = Game.Content.Load<Texture2D>("Red");
            
            this.font = Game.Content.Load<SpriteFont>("SpriteFont1");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            

            if (game.gameState != GameState.MENU)
            {
                if (!game.computerIsThinking)
                {
                    if (game.engine.GetTurn() == KaroEngine.Player.RED)
                        Turn = redTexture;
                    else
                        Turn = whiteTexture;
                }

                spriteBatch.Draw(Turn, new Vector2((Game.GraphicsDevice.Viewport.Width - Turn.Width - 10), 10), Color.White);
            }
            
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}

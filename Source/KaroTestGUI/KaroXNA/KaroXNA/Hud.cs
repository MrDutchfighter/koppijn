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
        SpriteBatch spriteBatch;
        Texture2D hudTexture;
        Texture2D whiteTexture;
        Texture2D redTexture;
        SpriteFont font;

        public Hud(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            
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

            //spriteBatch.Draw(this.hudTexture, new Vector2(Game.GraphicsDevice.Viewport.Height / 2 - 510, 0), Color.White); 

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}

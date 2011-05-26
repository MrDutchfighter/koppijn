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
    public class ScrollingBackground
    {
        // class ScrollingBackground
        private Vector2 screenpos, origin, texturesize;
        private Texture2D mytexture;
        private int screenheight;
        private int screenwidth;
        private int speed;

        public ScrollingBackground(GraphicsDevice Device, Texture2D BackgroundTexture, int speed)
        {
            mytexture = BackgroundTexture;
            screenheight = Device.Viewport.Height;
            screenwidth = Device.Viewport.Width;
            // Set the origin so that we're drawing from the 
            // center of the left edge
            origin = new Vector2(0, Device.Viewport.Width / 2);
            // Set the screen position to the center of the screen.
            screenpos = new Vector2(screenwidth / 2, screenheight / 2);
            // Offset to draw the second texture, when necessary.
            texturesize = new Vector2(mytexture.Width, 0);

            //Speed in pixels per second
            this.speed = speed;
        }
        // ScrollingBackground.Update
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)(gameTime.ElapsedGameTime.TotalSeconds * speed);

            screenpos.X += elapsed;
            screenpos.X = screenpos.X % mytexture.Width;
        }
        // ScrollingBackground.Draw
        public void Draw(SpriteBatch Batch)
        {
            // Draw the texture, if it is still onscreen
            if (screenpos.X < screenwidth)
            {

                Batch.Draw(mytexture, screenpos, null,
                     Color.White, 0, origin, 1, SpriteEffects.None, 0f);
            }
            // Draw the texture a second time, behind the first,
            // to create the scrolling illusion
            Batch.Draw(mytexture, screenpos - texturesize, null,
                 Color.White, 0, origin, 1, SpriteEffects.None, 0f);
        }
    }
}

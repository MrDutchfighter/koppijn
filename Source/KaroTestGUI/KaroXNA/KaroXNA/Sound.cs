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
using Microsoft.Xna.Framework.Storage;


namespace KaroXNA
{
    public class Sound
    {
        private static Sound instance;
        private Game1 game;
        private SoundEffect marioTheme;

        private Sound(Game game) {
            this.game = (Game1)game;
            // somehow xna can't load in big wave files so this class for now only works for small sounds!
            marioTheme = game.Content.Load<SoundEffect>("stagecleared");
        
        }

        public void playTheme()
        {
            marioTheme.Play();
        }

        public static Sound Instance(Game game)
        {

                if (instance == null)
                {
                    instance = new Sound(game);
                }
                return instance;
        }
    }
}



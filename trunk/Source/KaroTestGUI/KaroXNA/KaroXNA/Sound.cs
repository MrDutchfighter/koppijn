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
        static WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
        public void playTheme()
        {
            //marioTheme.Play();
            
            player.URL = "http://livestreams.omroep.nl/npo/3fm-bb";
            player.controls.play();
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



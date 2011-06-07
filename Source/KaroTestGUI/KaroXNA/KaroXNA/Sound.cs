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
    public class Sound {

        private static Sound instance;

        /// <summary>
        /// the sound constructor
        /// </summary>
        /// <param name="game"></param>
        private Sound(Game game) {
            player.URL = "http://livestreams.omroep.nl/npo/3fm-bb"; //3fm
        }

        static WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();

        public void playTheme(){}

        /// <summary>
        ///  set to URL to qmusic and start playing
        /// </summary>
        public void PlayQMusic() {
            player.controls.stop();
            player.URL = "http://www.q-music.nl/asx/q-music.asx";//qmuxic
            player.controls.play();
        }

        /// <summary>
        ///  set to URL to 3fm and start playing
        /// </summary>
        public void Play3FM() {
            player.controls.stop();
            player.URL = "http://livestreams.omroep.nl/npo/3fm-bb"; //3fm
            player.controls.play();
        }
        /// <summary>
        /// Stop the musicplayer
        /// </summary>
        public void Stop() {
            player.controls.stop();
        }

        /// <summary>
        /// Start playing the current URL
        /// </summary>
        public void Play() {
            player.controls.play();
        }

        /// <summary>
        /// Access the music player
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
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



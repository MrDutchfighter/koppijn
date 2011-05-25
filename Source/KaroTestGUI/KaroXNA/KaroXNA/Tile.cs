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
    public class Tile : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Game1 game;

        public Model TileModel { get; set; }
        public Matrix TileMatrix { get; set; }
        public bool IsMovable { get; set; }

        public Point Location { get; set; }

        public Tile(Game game, Model model, bool isMovable, Point location)
            : base(game)
        {
            this.game = (Game1)game;

            TileModel = model;
            TileMatrix = Matrix.Identity;
            IsMovable = isMovable;

            Location = location;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {            
            base.Update(gameTime);
        }
        

        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in TileModel.Meshes)
            {
                foreach (BasicEffect e in mesh.Effects)
                {
                    e.EnableDefaultLighting();
                    e.World = TileMatrix;
                    e.View = game.cam.View;
                    e.Projection = game.cam.Projection;
                }

                mesh.Draw();
            }
            
            base.Draw(gameTime);
        }
    }
}

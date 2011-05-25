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
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Point positionFrom = new Point(game.move[0] % Game1.BOARDWIDTH, game.move[0] / Game1.BOARDWIDTH);
                Point positionTo = new Point(game.move[1] % Game1.BOARDWIDTH, game.move[1] / Game1.BOARDWIDTH);
                Point tileFrom = new Point(game.move[2] % Game1.BOARDWIDTH, game.move[2] / Game1.BOARDWIDTH);
                    
                if (game.engine.GetGameState() != KaroEngine.GameState.INSERTION || game.insertionCount > 12)
                {
                    if (Location == tileFrom)
                    {
                        TileMatrix *= Matrix.CreateTranslation(new Vector3((positionTo.X - Location.X) * 5.5f, 0, (positionTo.Y - Location.Y) * 5.5f));
                        Location = positionTo;
                    }
                }
            }
            
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

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
        private Vector3 moveDirection;
        private Matrix world;
        
        public bool IsMovable { get; set; }
        public bool IsMoving { get; set; }

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
            if (game.gameState == GameState.PLAYING)
            {
                if (IsMoving)
                {
                    float x = this.moveDirection.X / 100;
                    float y = this.moveDirection.Y / 100;
                    float z = this.moveDirection.Z / 100;
                    this.world *= Matrix.CreateTranslation(x, y, z);
                    Vector3 moving = TileMatrix.Translation - this.world.Translation;
                    if (moving.X < 0.1f && moving.Y < 0.1f && moving.Z < 0.1f)
                    {
                        IsMoving = false;
                    }
                }
            }
            base.Update(gameTime);
        }

        public void moveTo(Matrix toLocation) {
            moveDirection = toLocation.Translation - TileMatrix.Translation;
            this.world = TileMatrix;
            TileMatrix = toLocation;
            this.IsMoving = true;

        }
        public override void Draw(GameTime gameTime)
        {
            if (game.gameState == GameState.PLAYING)
            {
                foreach (ModelMesh mesh in TileModel.Meshes)
                {
                    foreach (BasicEffect e in mesh.Effects)
                    {
                        e.EnableDefaultLighting();
                        if (IsMoving)
                        {
                            e.World = this.world;
                        }
                        else
                        {
                            e.World = TileMatrix;
                        }
                        e.View = game.cam.View;
                        e.Projection = game.cam.Projection;
                    }

                    mesh.Draw();
                }
            }
            
            base.Draw(gameTime);
        }
    }
}

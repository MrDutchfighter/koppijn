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
    using XNAColor = Microsoft.Xna.Framework.Color;
        
    public class Piece : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Game1 game;
        public Model PieceModel { get; set; }
        public Tile OnTopofTile { get; set; }
        public Vector3 Color { get; set; }
        
        public bool IsFlipped { get; set; }
        public bool IsMoving { get; set; }
        public bool IsVisible { get; set; }

        private Vector3 moveDirection;
        private Matrix world;

        public Piece(Game game, Model pieceModel, bool visible, Tile onTopofTile, Vector3 color)
            : base(game)
        {
            this.game = (Game1)game;
            PieceModel = pieceModel;
            OnTopofTile = onTopofTile;
            Color = color;

            IsFlipped = false;
            IsMoving = false;
            IsVisible = visible;
            
            world= Matrix.Identity;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMoving) 
            {
                float x = moveDirection.X / 100;
                float y = moveDirection.Y / 100;
                float z = moveDirection.Z / 100;

                world *= Matrix.CreateTranslation(x, y, z);
                Vector3 moving = OnTopofTile.TileMatrix.Translation - world.Translation;

                if (moving.X < 0.1f && moving.Y < 0.1f && moving.Z < 0.1f) 
                {
                    IsMoving = false;
                }
            }

            base.Update(gameTime);
        }

        public void MoveTo(Tile newTile)
        {
            moveDirection = newTile.TileMatrix.Translation - OnTopofTile.TileMatrix.Translation;
            world = OnTopofTile.TileMatrix;
            OnTopofTile = newTile;
            IsMoving = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                foreach (ModelMesh mesh in PieceModel.Meshes)
                {
                    foreach (BasicEffect e in mesh.Effects)
                    {
                        e.EnableDefaultLighting();
                        //e.DirectionalLight0.Enabled = true;
                        //e.DirectionalLight0.DiffuseColor = Color;
                        //e.DirectionalLight0.Direction = new Vector3(0, -50, 0);
                        
                        e.PreferPerPixelLighting = true;
                        e.DiffuseColor = Color;
                        e.World = Matrix.Identity;

                        if (!this.IsMoving)
                        {
                            if (!IsFlipped)
                                e.World *= Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateTranslation(0f, 3.4f, 0f);
                            else
                                e.World *= Matrix.CreateTranslation(0f, 1f, 0f);

                            e.World *= this.OnTopofTile.TileMatrix;
                        } 
                        else 
                            e.World *= this.world;
                        
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

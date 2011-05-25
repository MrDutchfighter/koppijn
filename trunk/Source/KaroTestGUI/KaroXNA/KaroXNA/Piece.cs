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
        public Tile onTopofTile { get; set; }
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
            this.IsMoving = false;
            PieceModel = pieceModel;
            IsVisible = visible;
            this.onTopofTile=onTopofTile;
            Color = color;
            world= Matrix.Identity;
            IsFlipped = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime){
            if (IsMoving) {
                float x =this.moveDirection.X / 100;
                float y =this.moveDirection.Y / 100;
                float z = this.moveDirection.Z / 100;
                this.world *= Matrix.CreateTranslation(x, y, z);
                Vector3 moving = this.onTopofTile.TileMatrix.Translation - this.world.Translation ;
                if (moving.X < 0.1f && moving.Y < 0.1f && moving.Z < 0.1f) {
                    IsMoving = false;
                }
            }
            base.Update(gameTime);
        }
        public void MoveTo(Tile newTile){
            moveDirection = newTile.TileMatrix.Translation - this.onTopofTile.TileMatrix.Translation;
            this.world = this.onTopofTile.TileMatrix;
            this.onTopofTile = newTile;
            this.IsMoving = true;
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
                        e.PreferPerPixelLighting = true;
                        e.DiffuseColor = Color;
                        e.World = Matrix.Identity;
                        if (!IsFlipped){
                            e.World *= Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateTranslation(0f, 3.4f, 0f);
                        }
                        else {
                            e.World *= Matrix.CreateTranslation(0f, 1f, 0f);
                        }
                        if (!this.IsMoving){
                            e.World *= this.onTopofTile.TileMatrix ;
                        } else {
                            //hier kan later de matrix voor het door de lucht laten zweven etc voor een mooie animatie.
                            e.World *= this.world;
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

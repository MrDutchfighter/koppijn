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
        public bool IsVisible { get; set; }
        public Tile onTopofTile { get; set; }
        public Vector3 Color { get; set; }

        public Matrix PieceMatrix { get; set; }
        public Matrix T { get; set; }
        public bool IsFlipped { get; set; }
        public bool isMoving { get; set; }

        public Piece(Game game, Model pieceModel, bool visible, Tile onTopofTile, Vector3 color)
            : base(game)
        {
            this.game = (Game1)game;
            this.isMoving = false;
            PieceModel = pieceModel;
            IsVisible = visible;
            this.onTopofTile=onTopofTile;
            Color = color;

            PieceMatrix = Matrix.Identity;
            T = Matrix.Identity;
            IsFlipped = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime){
            base.Update(gameTime);
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
                        if (!this.isMoving){
                            e.World *= this.onTopofTile.TileMatrix ;
                        }else{
                            //hier kan later de matrix voor het door de lucht laten zweven etc voor een mooie animatie.
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

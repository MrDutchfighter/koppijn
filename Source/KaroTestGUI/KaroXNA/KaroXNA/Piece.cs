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

    public enum Rotations {
        NONE, ROTATIONPLUS, ROTATIONMIN
    }
    public class Piece : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Game1 game;
        public Model PieceModel { get; set; }
        public Tile OnTopofTile { get; set; }
        public Vector3 Color { get; set; }
        
        public bool IsFlipped { get; set; }
        public bool IsMoving { get; set; }
        public bool IsVisible { get; set; }


        private Vector3 moveDirection,moveDestination;
        public Rotations rotationDirectionX;
        public Rotations rotationDirectionZ;
        private Matrix world;
        private int rotationX;
        private int rotationZ;

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
                float x = moveDirection.X / 180;
                float z = moveDirection.Z / 180;

                switch (this.rotationDirectionX) {
                    case Rotations.ROTATIONPLUS:
                        rotationX += 1;
                        break;
                    case Rotations.ROTATIONMIN:
                        rotationX -= 1;
                        break;
                }
                switch (this.rotationDirectionZ) {
                    case Rotations.ROTATIONPLUS:
                        rotationZ += 1;
                        break;
                    case Rotations.ROTATIONMIN:
                        rotationZ -= 1;
                        break;
                }

                world *= Matrix.CreateTranslation(x, 0, z);
                
                Vector3 moving = moveDestination - world.Translation;

                if (moving.X < 0.1f && moving.Z < 0.1f){
                    IsMoving = false;
                }
            }

            base.Update(gameTime);
        }

        public void MoveTo(Tile newTile)
        {
            moveDirection = newTile.TileMatrix.Translation - OnTopofTile.TileMatrix.Translation;
            moveDestination = newTile.TileMatrix.Translation;
            world = OnTopofTile.TileMatrix;
            world *= Matrix.CreateTranslation(0f,4f,0f);
            OnTopofTile = newTile;
            IsMoving = true;
            this.rotationX = (this.IsFlipped) ? 0 : 180;
            this.rotationZ = 0;
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
                        else {
                            e.World *= Matrix.CreateFromQuaternion(new Quaternion(rotationX, 0, rotationZ, 0));
                            //e.World *= Matrix.CreateRotationZ(MathHelper.ToRadians(this.rotationZ));
                            //e.World *= Matrix.CreateRotationX(MathHelper.ToRadians(this.rotationX));
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

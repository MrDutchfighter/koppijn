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
        public bool IsSelected { get; set; }


        private Vector3 moveDirection,moveDestination;
        public Rotations rotationDirectionX;
        public Rotations rotationDirectionZ;
        public Matrix world;
        private int rotationX;
        private int rotationZ;
        private float y;


        private VertexBuffer vb;
        private IndexBuffer ib;
        private BasicEffect effect;
        RasterizerState rsWire = new RasterizerState();
        RasterizerState rsSolid = new RasterizerState();

        public Piece(Game game, Model pieceModel, bool visible, Tile onTopofTile, Vector3 color) : base(game) {
            this.game = (Game1)game;
            PieceModel = pieceModel;
            OnTopofTile = onTopofTile;
            Color = color;

            IsFlipped = false;
            IsMoving = false;
            IsVisible = visible;
            IsSelected = false;
            world = onTopofTile.TileMatrix;
            world *= Matrix.CreateTranslation(0f, 1f, 0f);
        }

        public override void Initialize()
        {
            base.Initialize();

            ib = new IndexBuffer(GraphicsDevice, typeof(short), game.BoxIndexes.Length, BufferUsage.None);
            ib.SetData<short>(game.BoxIndexes);

            BoundingBox b = (BoundingBox)PieceModel.Tag;
            VertexPositionColor[]  points = new VertexPositionColor[8];

            //front
            points[0] = new VertexPositionColor(new Vector3(b.Min.X, b.Min.Y, b.Min.Z), XNAColor.Gold);
            points[1] = new VertexPositionColor(new Vector3(b.Max.X, b.Min.Y, b.Min.Z), XNAColor.Gold);
            points[2] = new VertexPositionColor(new Vector3(b.Min.X, b.Max.Y, b.Min.Z), XNAColor.Gold);
            points[3] = new VertexPositionColor(new Vector3(b.Max.X, b.Max.Y, b.Min.Z), XNAColor.Gold);
            //back
            points[4] = new VertexPositionColor(new Vector3(b.Min.X, b.Min.Y, b.Max.Z), XNAColor.Gold);
            points[5] = new VertexPositionColor(new Vector3(b.Max.X, b.Min.Y, b.Max.Z), XNAColor.Gold);
            points[6] = new VertexPositionColor(new Vector3(b.Min.X, b.Max.Y, b.Max.Z), XNAColor.Gold);
            points[7] = new VertexPositionColor(new Vector3(b.Max.X, b.Max.Y, b.Max.Z), XNAColor.Gold);

            vb = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 8, BufferUsage.None);
            vb.SetData<VertexPositionColor>(points);

            effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;

            rsWire.CullMode = CullMode.None;
            rsWire.FillMode = FillMode.WireFrame;

            rsSolid.FillMode = FillMode.Solid;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMoving) {
                float x = moveDirection.X / 180;
                float z = moveDirection.Z / 180;
                Vector3 moving = moveDestination - world.Translation;

                switch (this.rotationDirectionX) {
                    case Rotations.ROTATIONPLUS:
                        rotationX += 1;
                        break;
                    case Rotations.ROTATIONMIN:
                        rotationX -= 1;
                        break;
                }

                if (moving.X < 0.1f && moving.Z < 0.1f) {
                    IsMoving = false;
                }

                if((moving.X <= (moveDirection.X / 2) && moving.Y <= (moveDirection.X / 2))) {
                    if (world.Translation.Y > 0) {
                        y = -0.07f;
                    }
                    else {
                        y = 0;
                    }
                    switch (this.rotationDirectionZ) {
                        case Rotations.ROTATIONPLUS:
                            rotationZ += 1;
                            break;
                        case Rotations.ROTATIONMIN:
                            rotationZ -= 1;
                            break;
                    }
                }
                else {
                    y = 0.07f;
                    switch (this.rotationDirectionZ) {
                        case Rotations.ROTATIONPLUS:
                            rotationZ -= 1;
                            break;
                        case Rotations.ROTATIONMIN:
                            rotationZ -= 1;
                            break;
                    }
                }
                world *= Matrix.CreateTranslation(x, y, z);
            }
            
            if(!IsMoving){
                this.world = this.OnTopofTile.TileMatrix;
                this.world *= Matrix.CreateTranslation(0f, 1f, 0f);
                this.rotationDirectionX = Rotations.NONE;
                this.rotationDirectionZ = Rotations.NONE;
            }
            base.Update(gameTime);
        }

        public void MoveTo(Tile newTile) {
            moveDirection = newTile.TileMatrix.Translation - OnTopofTile.TileMatrix.Translation;
            moveDestination = newTile.TileMatrix.Translation;
            world = Matrix.Identity;
            if (!IsFlipped){
                world *= Matrix.CreateRotationX(MathHelper.ToRadians(180));
            }
            
            world *= Matrix.CreateTranslation(OnTopofTile.TileMatrix.Translation);
            OnTopofTile = newTile;
            IsMoving = true;            

            if (IsFlipped)
                world *= Matrix.CreateTranslation(0f, 4.4f, 0f);
            else
                world *= Matrix.CreateTranslation(0f, 2f, 0f) ;
            y = 0;
            this.rotationX = 0;
            this.rotationZ = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                foreach (ModelMesh mesh in PieceModel.Meshes)
                {
                    Matrix boxWorld = Matrix.Identity;
                    foreach (BasicEffect e in mesh.Effects)
                    {
                        e.EnableDefaultLighting();
                        
                        e.PreferPerPixelLighting = true;
                       
                        e.World = Matrix.Identity;

                        if (!this.IsMoving) {
                            if (!IsFlipped)
                                e.World *= Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateTranslation(0f, 2.4f, 0f);
                            e.World *= this.world;
                        }
                        else {
                            e.World *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationZ), MathHelper.ToRadians(rotationX), 0);
                            e.World *= this.world;
                        }

                        if (IsSelected)
                        {
                            e.DiffuseColor = XNAColor.Yellow.ToVector3();
                        }
                        else
                        {
                            e.DiffuseColor = Color;
                        }

                        e.View = game.cam.View;
                        e.Projection = game.cam.Projection;
                        boxWorld = e.World;
                    }

                    mesh.Draw();


                    if (game.ShowBoxes)
                    {
                        //Draw bounding box
                        GraphicsDevice.RasterizerState = rsWire; //wire box
                        effect.World = boxWorld;
                        //effect.World = Matrix.Identity;
                        effect.View = game.cam.View;
                        effect.Projection = game.cam.Projection;
                        effect.CurrentTechnique.Passes[0].Apply();

                        GraphicsDevice.SetVertexBuffer(vb);
                        GraphicsDevice.Indices = ib;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vb.VertexCount, 0, ib.IndexCount / 3);
                        GraphicsDevice.RasterizerState = rsSolid; //reset
                    }
                }
            }

            base.Draw(gameTime);
        }
    }
}

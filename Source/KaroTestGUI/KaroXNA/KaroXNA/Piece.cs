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
        public Matrix world;
        private int rotationX, rotationY, rotationZ;
        public int rotateDegrees;
        private float y;
        private double totalDistance;


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
        private double CalculateDistance(Vector3 p1, Vector3 p2){
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Z - p2.Z, 2));
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMoving) {
                float x = moveDirection.X / 180;
                float z = moveDirection.Z / 180;

                Vector3 moving = moveDestination - world.Translation;
                double distance = this.CalculateDistance(world.Translation,moveDestination);

                switch (rotateDegrees)
                {
                    case 0:
                        if (!IsFlipped)
                            rotationZ += 1;
                        else
                            rotationZ -= 1;
                        break;

                    case 45:
                        if (!IsFlipped)
                        {
                            rotationZ += 1;
                            rotationY = -45;
                        }
                        else
                        {
                            rotationZ -= 1;
                            rotationY = 45;
                        }

                        break;

                    case 90:
                        rotationX -= 1;
                        break;
                        
                    case 135:
                        if (!IsFlipped)
                        {
                            rotationY = 45;
                            rotationZ -= 1;
                        }
                        else
                        {
                            rotationY = -45;
                            rotationZ += 1;
                        }
                        break;

                    case 180:
                        if (!IsFlipped)
                            rotationZ -= 1;
                        else
                            rotationZ += 1;
                        break;

                    case -45:
                        if (!IsFlipped)
                        {
                            rotationY = 45;
                            rotationZ += 1;
                        }
                        else
                        {
                            rotationY = -45;
                            rotationZ -= 1;
                        }
                        break;

                    case -90:
                        rotationX += 1;
                        break;

                    case -135:
                        if (!IsFlipped)
                        {
                            rotationZ -= 1;
                            rotationY = -45;
                        }
                        else
                        {
                            rotationZ += 1;
                            rotationY = 45;
                        }
                        
                        break;
                }

                if (moving.X < 0) { moving.X *= -1; }
                if (moving.Y < 0) { moving.Y *= -1; }
                if (distance < 0.1) {
                    IsMoving = false;
                    this.world = this.OnTopofTile.TileMatrix;
                    this.world *= Matrix.CreateTranslation(0f, 1f, 0f);
                    this.rotateDegrees = 360;
                } else {
                    if (this.rotateDegrees != 360){
                        if (distance < (totalDistance / 2))
                        {
                            if (world.Translation.Y > 3.4f && this.rotateDegrees == 1000) {
                                y = -0.07f; //insertionstate
                            }
                            else if ((world.Translation.Y > 1 && IsFlipped))
                            {
                                y = -0.11f; //to marked
                            }
                            else if(!IsFlipped && world.Translation.Y > 3.4f){
                                y = -0.043f; //to unmarked
                            }
                            else
                            {
                                y = 0;
                            }
                        }
                        else
                        {
                            y = 0.07f;
                        }
                    }
                    else { y = 0; }
                    world *= Matrix.CreateTranslation(x, y, z);
                }
            }
            base.Update(gameTime);
        }

        public void MoveTo(Tile newTile) {
            moveDirection = newTile.TileMatrix.Translation - OnTopofTile.TileMatrix.Translation;
            moveDestination = newTile.TileMatrix.Translation;
            world = Matrix.Identity;
            if (!IsFlipped)
            {
                world *= Matrix.CreateRotationX(MathHelper.ToRadians(180));
                world *= Matrix.CreateTranslation(0f, 4.4f, 0f);
            }
            else { world*= Matrix.CreateTranslation(0f, 1.0f, 0f);}
            world *= Matrix.CreateTranslation(OnTopofTile.TileMatrix.Translation);
            OnTopofTile = newTile;
            IsMoving = true;
          
            y = 0;
            this.rotationX = 0;
            this.rotationY = 0;
            this.rotationZ = 0;
            this.totalDistance = this.CalculateDistance(moveDestination, world.Translation);
        }

        public override void Draw(GameTime gameTime)
        {
            if (game.gameState == GameState.PLAYING)
            {
                foreach (ModelMesh mesh in PieceModel.Meshes)
                {
                    Matrix boxWorld = Matrix.Identity;
                    foreach (BasicEffect e in mesh.Effects)
                    {
                        //e.EnableDefaultLighting();
                        e.LightingEnabled = true;
                        e.DirectionalLight0.Enabled = true;
                        e.DirectionalLight0.DiffuseColor = XNAColor.White.ToVector3();
                        e.DirectionalLight0.Direction = new Vector3(1, -1, 0);


                        e.PreferPerPixelLighting = true;
                       
                        e.World = Matrix.Identity;

                        if (!this.IsMoving) {
                            if (!IsFlipped)
                                e.World *= Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateTranslation(0f, 2.4f, 0f);
                            e.World *= this.world;
                        }
                        else {
                            e.World *= Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ));
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

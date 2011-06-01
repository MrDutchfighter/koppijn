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
        public Matrix world;
        
        public bool IsMovable { get; set; }
        public bool IsMoving { get; set; }
        public bool IsSelected { get; set; }
        public bool IsPossibleMove { get; set; }

        public Point Location { get; set; }
        
        private VertexBuffer vb;
        private IndexBuffer ib;
        private BasicEffect effect;
        RasterizerState rsWire = new RasterizerState();
        RasterizerState rsSolid = new RasterizerState();

        public Tile(Game game, Model model, bool isMovable, Point location) : base(game)
        {
            this.game = (Game1)game;

            TileModel = model;
            TileMatrix = Matrix.Identity;
            TileMatrix *= Matrix.CreateTranslation(new Vector3(location.X * 5.5f, 0, location.Y * 5.5f));
            IsMovable = isMovable;
            IsSelected = false;
            IsPossibleMove = false;

            Location = location;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();

            ib = new IndexBuffer(GraphicsDevice, typeof(short), game.BoxIndexes.Length, BufferUsage.None);
            ib.SetData<short>(game.BoxIndexes);

            BoundingBox b = (BoundingBox)TileModel.Tag;
            VertexPositionColor[] points = new VertexPositionColor[8];

            //front
            points[0] = new VertexPositionColor(new Vector3(b.Min.X, b.Min.Y, b.Min.Z), Color.Gold);
            points[1] = new VertexPositionColor(new Vector3(b.Max.X, b.Min.Y, b.Min.Z), Color.Gold);
            points[2] = new VertexPositionColor(new Vector3(b.Min.X, b.Max.Y, b.Min.Z), Color.Gold);
            points[3] = new VertexPositionColor(new Vector3(b.Max.X, b.Max.Y, b.Min.Z), Color.Gold);
            //back
            points[4] = new VertexPositionColor(new Vector3(b.Min.X, b.Min.Y, b.Max.Z), Color.Gold);
            points[5] = new VertexPositionColor(new Vector3(b.Max.X, b.Min.Y, b.Max.Z), Color.Gold);
            points[6] = new VertexPositionColor(new Vector3(b.Min.X, b.Max.Y, b.Max.Z), Color.Gold);
            points[7] = new VertexPositionColor(new Vector3(b.Max.X, b.Max.Y, b.Max.Z), Color.Gold);

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
                    Matrix boxWorld = Matrix.Identity;
                    foreach (BasicEffect e in mesh.Effects)
                    {
                        //e.EnableDefaultLighting();
                        e.LightingEnabled = true;
                        e.DirectionalLight0.Enabled = true;
                        e.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
                        e.DirectionalLight0.Direction = new Vector3(1, -1, 0);


                        if (IsMoving)
                        {
                            e.World = this.world;
                        }
                        else
                        {
                            e.World = TileMatrix;
                        }

                        if (IsSelected)
                        {
                            e.DiffuseColor = Color.White.ToVector3();
                        }
                        else if (IsMovable || IsPossibleMove) {
                            e.DiffuseColor = Color.Aqua.ToVector3();
                        }
                        else
                        {
                            e.DiffuseColor = Color.Gray.ToVector3();
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
                        effect.View = game.cam.View;
                        effect.Projection = game.cam.Projection;
                        effect.CurrentTechnique.Passes[0].Apply();

                        GraphicsDevice.SetVertexBuffer(vb);
                        GraphicsDevice.Indices = ib;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vb.VertexCount, 0, ib.IndexCount / 3);

                        GraphicsDevice.RasterizerState = rsSolid; //reset
                    }
                }

                base.Draw(gameTime);
            }
        }
    }
}

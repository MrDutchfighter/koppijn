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
using Microsoft.Xna.Framework.Storage;

namespace KaroXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice device;
        Model model;
        Camera cam;
        Matrix world, view, proj;
        float f;
        Effect effect;
        KaroEngine.KaroEngineWrapper engine;
        List<Piece> gamePieces;
        List<Tile> gameTiles;

        public Game1()
        {
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            this.Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            this.Window.Title = "Karo XNA";
            Content.RootDirectory = "Content";
            gamePieces = new List<Piece>();
            gameTiles = new List<Tile>();
            cam = new Camera();

        }

        protected override void Initialize()
        {
            device = graphics.GraphicsDevice;

            world = Matrix.Identity;
            view = cam.View;
            proj = cam.Projection;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            for (int i = 0; i < 12; i++)
            {
                Piece p = new Piece(Content.Load<Model>("piece"), false);
                world = Matrix.CreateTranslation(new Vector3(i * 5.5f, 1f, 0));
                p.PieceMatrix = world;
                p.IsVisible = true;
                gamePieces.Add(p);
            }

            for (int i = 0; i < 20; i++)
            {
                Tile t = new Tile(Content.Load<Model>("tile"), false);
                world = Matrix.CreateTranslation(new Vector3(i * 5.5f, 0, 0));
                t.TileMatrix= world;
                gameTiles.Add(t);
            }
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.R))
                cam.DoYRotation(1);
            //world = Matrix.CreateRotationY(MathHelper.ToDegrees(f));
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Piece p in gamePieces)
            {
                if (!p.IsVisible)
                {
                    continue;
                }
                foreach (ModelMesh mesh in p.PieceModel.Meshes)
                {
                    foreach (BasicEffect e in mesh.Effects)
                    {
                        e.PreferPerPixelLighting = true;
                        e.DiffuseColor = Color.Tomato.ToVector3();
                        e.SpecularColor = Color.White.ToVector3();
                        e.EnableDefaultLighting();
                        e.World = p.PieceMatrix;
                        e.View = cam.View;
                        e.Projection = cam.Projection;
                    }
                    mesh.Draw();
                }
            }

            foreach (Tile t in gameTiles)
            {
                foreach (ModelMesh mesh in t.TileModel.Meshes)
                {
                    foreach (BasicEffect e in mesh.Effects)
                    {
                        e.EnableDefaultLighting();

                        e.World = t.TileMatrix;
                        e.View = cam.View;
                        e.Projection = cam.Projection;
                    }

                    mesh.Draw();
                }
            }
        }
    }
}

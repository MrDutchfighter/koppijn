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
using KaroEngine;

namespace KaroXNA
{
    public enum GameState { MENU, PLAYING };
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
        Menu gameMenu;
        GameState gameState;
        MouseState oldMouseState;
        const int BOARDWIDTH = 17;


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
            gameState = GameState.MENU;
            gameMenu = new Menu(this, 0);
            //Components.Add(gameMenu);
            engine = new KaroEngineWrapper();

            engine.InsertByXY(5, 4);
            engine.InsertByXY(6, 4);
            engine.InsertByXY(7, 4);
            engine.InsertByXY(8, 4);
            engine.InsertByXY(9, 4);

            engine.InsertByXY(5, 5);
            engine.InsertByXY(6, 5);
            engine.InsertByXY(7, 5);
            engine.InsertByXY(8, 5);
            engine.InsertByXY(9, 5);

            engine.InsertByXY(5, 6);
            engine.InsertByXY(6, 6);

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
            Tile t;
            Piece p;

            for (int x = 0; x < BOARDWIDTH; x++)
            {
                for (int y = 0; y < BOARDWIDTH; y++)
                {
                    switch (engine.GetByXY(x, y))
                    {
                        case KaroEngine.Tile.BORDER:
                            break;
                        case KaroEngine.Tile.EMPTY:
                            break;
                        case KaroEngine.Tile.MOVEABLETILE:
                            t = new Tile(Content.Load<Model>("tile"), false);
                            t.TileMatrix = Matrix.CreateTranslation(new Vector3(x * 5.5f, 0, y * 5.5f));
                            gameTiles.Add(t);
                            break;
                        case KaroEngine.Tile.SOLIDTILE:
                            t = new Tile(Content.Load<Model>("tile"), false);
                            t.TileMatrix = Matrix.CreateTranslation(new Vector3(x * 5.5f, 0, y * 5.5f));
                            gameTiles.Add(t);
                            break;
                        case KaroEngine.Tile.REDUNMARKED:
                            t = new Tile(Content.Load<Model>("tile"), false);
                            t.TileMatrix = Matrix.CreateTranslation(new Vector3(x * 5.5f, 0, y * 5.5f));
                            gameTiles.Add(t);

                            p = new Piece(Content.Load<Model>("piece"), false, new Point(x, y), Color.Tomato.ToVector3());
                            world = Matrix.CreateTranslation(new Vector3(x * 5.5f, 1, y * 5.5f));
                            p.PieceMatrix = world;
                            p.IsVisible = true;
                            gamePieces.Add(p);
                            break;
                        case KaroEngine.Tile.REDMARKED:
                            t = new Tile(Content.Load<Model>("tile"), false);
                            t.TileMatrix = Matrix.CreateTranslation(new Vector3(x * 5.5f, 0, y * 5.5f));
                            gameTiles.Add(t);

                            p = new Piece(Content.Load<Model>("piece"), false, new Point(x, y), Color.Tomato.ToVector3());
                            world = Matrix.CreateTranslation(new Vector3(x * 5.5f, 1, y * 5.5f));
                            p.PieceMatrix = world;
                            p.IsVisible = true;
                            gamePieces.Add(p);
                            break;
                        case KaroEngine.Tile.WHITEUNMARKED:
                            t = new Tile(Content.Load<Model>("tile"), false);
                            t.TileMatrix = Matrix.CreateTranslation(new Vector3(x * 5.5f, 0, y * 5.5f));
                            gameTiles.Add(t);

                            p = new Piece(Content.Load<Model>("piece"), false, new Point(x, y), Color.White.ToVector3());
                            world = Matrix.CreateTranslation(new Vector3(x * 5.5f, 1, y * 5.5f));
                            p.PieceMatrix = world;
                            p.IsVisible = true;
                            gamePieces.Add(p);
                            break;
                        case KaroEngine.Tile.WHITEMARKED:
                            t = new Tile(Content.Load<Model>("tile"), false);
                            t.TileMatrix = Matrix.CreateTranslation(new Vector3(x * 5.5f, 0, y * 5.5f));
                            gameTiles.Add(t);

                            p = new Piece(Content.Load<Model>("piece"), false, new Point(x, y), Color.White.ToVector3());
                            world = Matrix.CreateTranslation(new Vector3(x * 5.5f, 1, y * 5.5f));
                            p.PieceMatrix = world;
                            p.IsVisible = true;
                            gamePieces.Add(p);
                            break;
                    }
                }
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
            if (Keyboard.GetState().IsKeyDown(Keys.R) || Keyboard.GetState().IsKeyDown(Keys.Right))
                cam.DoYRotation(0.5f);
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                cam.DoYRotation(-0.5f);
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp) || Keyboard.GetState().IsKeyDown(Keys.Up))
                cam.DoZoom(-0.01f);
            if (Keyboard.GetState().IsKeyDown(Keys.PageDown) || Keyboard.GetState().IsKeyDown(Keys.Down))
                cam.DoZoom(0.01f);
            if (Mouse.GetState().ScrollWheelValue != oldMouseState.ScrollWheelValue)
            {
                float difference = (Mouse.GetState().ScrollWheelValue - oldMouseState.ScrollWheelValue);
                cam.DoZoom(difference / 1000);
            }

            oldMouseState = Mouse.GetState();

            //world = Matrix.CreateRotationY(MathHelper.ToDegrees(f));
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

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

                        e.SpecularColor = Color.Black.ToVector3();
                    }

                    mesh.Draw();
                }
            }

            RasterizerState rs = new RasterizerState();
            rs.FillMode = FillMode.Solid;
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rs;

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

                        e.DiffuseColor = p.Color;
                        e.SpecularColor = Color.White.ToVector3();
                        e.EnableDefaultLighting();
                        e.World = p.PieceMatrix;
                        e.View = cam.View;
                        e.Projection = cam.Projection;
                    }
                    mesh.Draw();
                }
            }

            base.Draw(gameTime);
        }
    }
}

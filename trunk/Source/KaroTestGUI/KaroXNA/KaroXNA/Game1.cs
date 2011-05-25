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
        GraphicsDevice device;
        public Model tileModel, pieceModel, tableModel;
        Matrix tableMatrix;
        public Camera cam;
        Matrix world, view, proj;
        public KaroEngine.KaroEngineWrapper engine;
        Menu gameMenu;
        public int[] move;
        GameState gameState;
        public int insertionCount = 0;
        MouseState oldMouseState;
        public const int BOARDWIDTH = 17;
        bool spacePressed = false;
        Random random = new Random();
        public float frames = 0f;
        public float deltaFPSTime = 0f;
        public float FPS { get { return this.frames; } set { this.frames = value; } }

        float rotY = 0.0f;

        public Game1()
        {
            this.IsFixedTimeStep = false;
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            this.Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            this.Window.Title = "Karo XNA";
            Content.RootDirectory = "Content";
            cam = new Camera(graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height);
            gameState = GameState.MENU;
            gameMenu = new Menu(this, 0);
            Components.Add(gameMenu);
            engine = new KaroEngineWrapper();

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
            tileModel = Content.Load<Model>("tile");
            pieceModel = Content.Load<Model>("piece");
            tableModel = Content.Load<Model>("test");

            for (int x = 0; x < BOARDWIDTH; x++)
            {
                for (int y = 0; y < BOARDWIDTH; y++)
                {
                    KaroEngine.Tile tile = engine.GetByXY(x, y);

                    if (tile != KaroEngine.Tile.BORDER && tile != KaroEngine.Tile.EMPTY)
                    {
                        Tile t = new Tile(this, tileModel, false, new Point(x, y));
                        t.TileMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(random.Next(0, 4) * 90)) * Matrix.CreateTranslation(new Vector3(x * 5.5f, 0, y * 5.5f));

                        Components.Add(t);

                        //if (tile == KaroEngine.Tile.WHITEUNMARKED || tile == KaroEngine.Tile.WHITEMARKED || tile == KaroEngine.Tile.REDUNMARKED || tile == KaroEngine.Tile.REDMARKED)
                        //{
                        //    Piece p = new Piece(pieceModel, true, new Point(x, y), Color.Black.ToVector3());

                        //    if (tile == KaroEngine.Tile.REDUNMARKED || tile == KaroEngine.Tile.REDMARKED)
                        //        p.Color = Color.Tomato.ToVector3();

                        //    if (tile == KaroEngine.Tile.WHITEUNMARKED || tile == KaroEngine.Tile.WHITEMARKED)
                        //        p.Color = Color.White.ToVector3();

                        //    if (tile == KaroEngine.Tile.WHITEMARKED || tile == KaroEngine.Tile.REDMARKED)
                        //        p.T = Matrix.CreateTranslation(new Vector3(x * 5.5f, 1, y * 5.5f));
                        //    else
                        //        p.T = Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateTranslation(new Vector3(x * 5.5f, 3.4f, y * 5.5f));

                        //    gamePieces.Add(p);
                        //}
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
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (!spacePressed)
                {
                    spacePressed = true;

                    move = engine.CalculateComputerMove();

                    Point positionTo = new Point(move[1] % Game1.BOARDWIDTH, move[1] / Game1.BOARDWIDTH);
                    
                    if (engine.GetGameState() == KaroEngine.GameState.INSERTION || insertionCount < 12)
                    {
                        Piece p = new Piece(this, pieceModel, true, new Point(positionTo.X, positionTo.Y), Color.Black.ToVector3());

                        if (engine.GetTurn() == KaroEngine.Player.RED)
                            p.Color = Color.Tomato.ToVector3();
                        else
                            p.Color = Color.White.ToVector3();

                        //Turn the piece upside down, default is flipped, which we don't want!
                        p.T = Matrix.Identity;
                        p.T *= Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateTranslation(new Vector3(positionTo.X * 5.5f, 3.4f, positionTo.Y * 5.5f));

                        Components.Add(p);
                        insertionCount++;
                    }
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.R) || Keyboard.GetState().IsKeyDown(Keys.Right)) 
                rotY += 0.1f; cam.DoYRotation(rotY);

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) 
                rotY += 0.1f; cam.DoYRotation(rotY * -1);

            if (!Keyboard.GetState().IsKeyDown(Keys.R) && !Keyboard.GetState().IsKeyDown(Keys.Right) && !Keyboard.GetState().IsKeyDown(Keys.Left)) 
                rotY = 0.5f;

            if (rotY > 4) 
                rotY = 4f;

            if (Keyboard.GetState().IsKeyDown(Keys.PageUp) || Keyboard.GetState().IsKeyDown(Keys.Up))
                cam.DoZoom(-0.01f);

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown) || Keyboard.GetState().IsKeyDown(Keys.Down))
                cam.DoZoom(0.01f);

            if (Mouse.GetState().ScrollWheelValue != oldMouseState.ScrollWheelValue)
            {
                float difference = (Mouse.GetState().ScrollWheelValue - oldMouseState.ScrollWheelValue);
                cam.DoZoom(difference / 1000);
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                spacePressed = false;
            }

            oldMouseState = Mouse.GetState();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float fps = 1 / elapsed;
            deltaFPSTime += elapsed;

            if (deltaFPSTime > 1)
            {
                this.FPS = fps;
                deltaFPSTime -= 1;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
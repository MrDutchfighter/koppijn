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
    public class Game1 : Microsoft.Xna.Framework.Game{
        DepthStencilState dss;
        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        public KaroEngine.KaroEngineWrapper engine;
        public const int BOARDWIDTH = 17;
        public Menu gameMenu;
        public GameState gameState;

        Matrix world, view, proj, roomMatrix;
        public Model pieceModel, roomModel; 
        public Camera cam;

        public int[] move;
        
        public int insertionCount;
        MouseState oldMouseState;
        public bool spacePressed;
        public bool leftMouseDown;
        private bool f1Pressed;

        private int selectedPiece, selectedTile;
        Random random = new Random();
        float rotX = 0.0f;
        float rotY = 0.0f;

        public float frames = 0f;
        public float deltaFPSTime = 0f;
        public float FPS { get { return this.frames; } set { this.frames = value; } }

        Dictionary<int, Tile> TileComponents;
        Dictionary<int, Piece> PieceComponents;

        public short[] BoxIndexes { get; set; }
        public bool ShowBoxes { get; set; }

        bool tPressed, rPresssed;

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
            spacePressed = false;
            f1Pressed = false;
            insertionCount = 0;
            engine = new KaroEngineWrapper();

            this.PieceComponents = new Dictionary<int, Piece>();
            this.TileComponents = new Dictionary<int, Tile>();


            BoxIndexes = new short[36] 
            {
                //front
                0,2,3,
                0,1,3,

                //right
                1,3,7,
                1,5,7,
                
                //left
                0,2,6,
                0,4,6,

                //back
                4,6,7,
                4,5,7,

                //bottom
                6,2,3,
                6,7,3,

                //top
                4,0,1,
                4,5,1
            };
            ShowBoxes = false;

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
        }

        protected override void Initialize()
        {
            device = graphics.GraphicsDevice;

            world = Matrix.Identity;
            view = cam.View;
            proj = cam.Projection;

            roomMatrix = Matrix.CreateScale(1.4f) * Matrix.CreateTranslation(new Vector3(-20, -20, 45));


            base.Initialize();
        }

        protected override void LoadContent()
        {
            Model tileModel = Content.Load<Model>("tile");
            pieceModel = Content.Load<Model>("piece");
            roomModel = Content.Load<Model>("room");

            for (int x = 0; x < BOARDWIDTH; x++)
            {
                for (int y = 0; y < BOARDWIDTH; y++)
                {
                    KaroEngine.Tile tile = engine.GetByXY(x, y);
                    if (tile != KaroEngine.Tile.BORDER && tile != KaroEngine.Tile.EMPTY){
                        Tile t = new Tile(this, tileModel, false, new Point(x, y));
                        t.TileMatrix =  Matrix.CreateTranslation(new Vector3(x * 5.5f, 0, y * 5.5f));
                        this.TileComponents.Add(y * BOARDWIDTH + x, t);
                        Components.Add(t);
                    }
                }
            }
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        private void DoMove(int piece,int tile) {
            if (tile > 0) { //If the move should be on a tile
                if (engine.GetGameState() == KaroEngine.GameState.INSERTION) {
                    Point location2 = TileComponents[tile].Location;
                    if (engine.InsertByXY(location2.X, location2.Y)){
                        this.ShowMove(location2,location2,location2);
                        Console.WriteLine("TODO => Animation for insertionstate!");
                    }
                }
                else if (engine.GetGameState() == KaroEngine.GameState.PLAYING) {
                    TileComponents[tile].IsSelected = true;
                    this.selectedTile = tile;
                    if (this.selectedPiece > 0){
                        Point location = PieceComponents[this.selectedPiece].OnTopofTile.Location;
                        int from = location.X + (location.Y * BOARDWIDTH);
                        Point location2 = TileComponents[this.selectedTile].Location;
                        int to = location2.X + (location2.Y * BOARDWIDTH);
                        this.ClearSelectedItems();
                        if (engine.DoMove(from, to, -1)){
                            this.ShowMove(location, location2, new Point());
                        }
                    }
                }
            }
            if (piece > 0) {
             
                if (engine.GetGameState() == KaroEngine.GameState.PLAYING || engine.GetGameState() == KaroEngine.GameState.INSERTION) {
                    
                    Player player = engine.GetTurn();
                    Vector3 red = Color.Tomato.ToVector3();
                    Vector3 white = Color.White.ToVector3();
                    if (PieceComponents[piece].Color.Equals(white) && player == Player.WHITE ||
                        PieceComponents[piece].Color.Equals(red) && player == Player.RED)
                    {
                        this.selectedPiece = piece;
                        PieceComponents[piece].IsSelected = true;
                    }
                }
            }
        }

        private void ShowMove(Point positionFrom, Point positionTo, Point tileFrom)
        {
            if (engine.GetGameState() == KaroEngine.GameState.INSERTION || insertionCount < 12)
            {
                Piece p = new Piece(this, pieceModel, true, this.TileComponents[positionTo.Y * BOARDWIDTH + positionTo.X], Color.Black.ToVector3());
                if (engine.GetTurn() == KaroEngine.Player.WHITE) //move just executed, get previous color
                {
                    p.Color = Color.Tomato.ToVector3();
                }
                else
                {
                    p.Color = Color.White.ToVector3();
                }
                //Turn the piece upside down, default is flipped, which we don't want!
                //p.T = Matrix.Identity;
                //p.T *= Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateTranslation(new Vector3(positionTo.X * 5.5f, 3.4f, positionTo.Y * 5.5f));
                Components.Add(p);
                this.PieceComponents.Add(positionTo.Y * BOARDWIDTH + positionTo.X, p);
                insertionCount++;
            }
            else
            {
                if (tileFrom.X > 0)
                {
                    Tile movedTile = this.TileComponents[tileFrom.Y * BOARDWIDTH + tileFrom.X];
                    this.TileComponents.Remove(tileFrom.Y * BOARDWIDTH + tileFrom.X);
                    movedTile.Location = positionTo;
                    movedTile.moveTo(Matrix.CreateTranslation(new Vector3(positionTo.X * 5.5f, 0, positionTo.Y * 5.5f)));
                    this.TileComponents.Add(positionTo.Y * BOARDWIDTH + positionTo.X, movedTile);
                }
                Piece movedPiece = this.PieceComponents[positionFrom.Y * BOARDWIDTH + positionFrom.X];
                this.PieceComponents.Remove(positionFrom.Y * BOARDWIDTH + positionFrom.X);
                movedPiece.MoveTo(this.TileComponents[(positionTo.Y * BOARDWIDTH) + positionTo.X]);
                KaroEngine.Tile t = engine.GetByXY(positionTo.X, positionTo.Y);
                bool flipping;
                if (t == KaroEngine.Tile.REDMARKED || t == KaroEngine.Tile.WHITEMARKED)
                {
                    flipping = true;
                }
                else
                {
                    flipping = false;
                }
                if (flipping != movedPiece.IsFlipped)
                {
                    movedPiece.IsFlipped = flipping;
                    //x en y op het bord(engine) zijn x en z in de karogui
                    if (positionFrom.X == positionTo.X)
                    {
                        movedPiece.rotationDirectionX = Rotations.NONE;
                    }
                    else if (positionFrom.X < positionTo.X)
                    {
                        movedPiece.rotationDirectionX = Rotations.ROTATIONPLUS;
                    }
                    else
                    {
                        movedPiece.rotationDirectionX = Rotations.ROTATIONMIN;
                    }

                    //x en y op het bord(engine) zijn x en z in de karogui
                    if (positionFrom.Y == positionTo.Y)
                    {
                        movedPiece.rotationDirectionZ = Rotations.NONE;
                    }
                    else if (positionFrom.Y < positionTo.Y)
                    {
                        movedPiece.rotationDirectionZ = Rotations.ROTATIONPLUS;
                    }
                    else
                    {
                        movedPiece.rotationDirectionZ = Rotations.ROTATIONMIN;
                    }
                }
                else
                {
                    movedPiece.rotationDirectionZ = Rotations.NONE;
                    movedPiece.rotationDirectionX = Rotations.NONE;
                }
                this.PieceComponents.Add(positionTo.Y * BOARDWIDTH + positionTo.X, movedPiece);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.PLAYING)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space)){
                    if (!spacePressed){
                        spacePressed = true;

                        if (engine.GetGameState() == KaroEngine.GameState.PLAYING || engine.GetGameState() == KaroEngine.GameState.INSERTION || insertionCount < 12)
                        {
                            move = engine.CalculateComputerMove();
                            Point positionFrom = new Point(move[0] % Game1.BOARDWIDTH, move[0] / Game1.BOARDWIDTH);
                            Point positionTo = new Point(move[1] % Game1.BOARDWIDTH, move[1] / Game1.BOARDWIDTH);
                            Point tileFrom = new Point(move[2] % Game1.BOARDWIDTH, move[2] / Game1.BOARDWIDTH);
                            this.ShowMove(positionFrom, positionTo, tileFrom);
                            this.ClearSelectedItems();
                        }
                    }
                }

                // handles mouse and keyboard inputs
                UpdateInput();
            }

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

        private void UpdateInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.gameState = GameState.MENU;

            #region Rotation
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                rotY += 0.1f;
                cam.DoYRotation(rotY);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                rotY += 0.1f;
                cam.DoYRotation(rotY*-1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Home) || Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                rotX -= 0.5f;
                cam.DoXRotation(rotX);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.End) || Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                rotX += 0.5f;
                cam.DoXRotation(rotX);
            }

            // Reset rotation
            if (!Keyboard.GetState().IsKeyDown(Keys.Left) && !Keyboard.GetState().IsKeyDown(Keys.Right))
                rotY = 0.0f;
            if (!Keyboard.GetState().IsKeyDown(Keys.Down) && !Keyboard.GetState().IsKeyDown(Keys.Up))
                rotX = 0.0f;
            if (!Keyboard.GetState().IsKeyDown(Keys.Home) && !Keyboard.GetState().IsKeyDown(Keys.End))
                rotX = 0.0f;

            // Rest rotX & rotY
            if (rotY >= 1.0f)
                rotY = 0.9f;
            if (rotX >= 1.0f)
                rotX = 0.9f;
            #endregion

            #region Zoom
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp) || Keyboard.GetState().IsKeyDown(Keys.Up))
                cam.DoZoom(-0.01f);

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown) || Keyboard.GetState().IsKeyDown(Keys.Down))
                cam.DoZoom(0.01f);

            if (Mouse.GetState().ScrollWheelValue != oldMouseState.ScrollWheelValue)
            {
                float difference = (Mouse.GetState().ScrollWheelValue - oldMouseState.ScrollWheelValue);
                cam.DoZoom(difference / 1000);
            }
            #endregion

            #region FixedCameraPositions
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !tPressed) {
                cam.SetFixedTop();
                tPressed = true;
            }

            if ((Keyboard.GetState().IsKeyDown(Keys.R) || Mouse.GetState().MiddleButton == ButtonState.Pressed) && !rPresssed)
            {
                rPresssed = true;
                cam.DoYRotation(180f);
            }

            // Set view player 1 & 2 with the number keys
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
                cam.SetFixedViewPlayerOne();
            if (Keyboard.GetState().IsKeyDown(Keys.D2))
                cam.SetFixedViewPlayerTwo();

            // Restore pressed states
            if (Keyboard.GetState().IsKeyUp(Keys.T))
                tPressed = false;
            if (Keyboard.GetState().IsKeyUp(Keys.R) && Mouse.GetState().MiddleButton == ButtonState.Released)
                rPresssed = false;
            #endregion

            if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                spacePressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                if (!f1Pressed)
                {
                    ShowBoxes = (ShowBoxes) ? false : true;
                    f1Pressed = true;
                }
            }

            if (Keyboard.GetState().IsKeyUp(Keys.F1))
                f1Pressed = false;

            #region Handle Mouse
            oldMouseState = Mouse.GetState();

            if (oldMouseState.LeftButton == ButtonState.Pressed)
                this.leftMouseDown=true;

            if (this.leftMouseDown && oldMouseState.LeftButton == ButtonState.Released) {

                this.leftMouseDown = false;
                Vector2 mousePosition = new Vector2(oldMouseState.X, oldMouseState.Y);

                List<KeyValuePair<float?, KeyValuePair<int, Type>>> results = new List<KeyValuePair<float?, KeyValuePair<int, Type>>>();

                foreach (var tile in TileComponents){
                    tile.Value.IsSelected = false;

                    Vector3 nearPlane = new Vector3(mousePosition.X, mousePosition.Y, 0);
                    Vector3 farPlane = new Vector3(mousePosition.X, mousePosition.Y, 1);
                    nearPlane = GraphicsDevice.Viewport.Unproject(nearPlane, cam.Projection, cam.View, tile.Value.TileMatrix);
                    farPlane = GraphicsDevice.Viewport.Unproject(farPlane, cam.Projection, cam.View, tile.Value.TileMatrix);
                    Vector3 direction = farPlane - nearPlane;
                    direction.Normalize();
                    Ray ray = new Ray(nearPlane, direction);

                    float? result = ray.Intersects((BoundingBox)tile.Value.TileModel.Tag);
                    if (result.HasValue)
                    {
                        results.Add(new KeyValuePair<float?, KeyValuePair<int, Type>>(result, new KeyValuePair<int, Type>(tile.Key, typeof(Tile))));
                    }
                }

                foreach (var piece in PieceComponents){
                    piece.Value.IsSelected = false;

                    Vector3 nearPlane = new Vector3(mousePosition.X, mousePosition.Y, 0);
                    Vector3 farPlane = new Vector3(mousePosition.X, mousePosition.Y, 1);
                    nearPlane = GraphicsDevice.Viewport.Unproject(nearPlane, cam.Projection, cam.View, piece.Value.world);
                    farPlane = GraphicsDevice.Viewport.Unproject(farPlane, cam.Projection, cam.View, piece.Value.world);
                    Vector3 direction = farPlane - nearPlane;
                    direction.Normalize();
                    Ray ray = new Ray(nearPlane, direction);
                    float? result = ray.Intersects((BoundingBox)piece.Value.PieceModel.Tag);
                    if (result.HasValue) {
                        results.Add(new KeyValuePair<float?, KeyValuePair<int, Type>>(result, new KeyValuePair<int, Type>(piece.Key, typeof(Piece))));
                    }
                }


                float shortestDistance = float.PositiveInfinity;
                int index = 0;
                int i=0;
                foreach (var result in results)
                {
                    if (result.Key < shortestDistance)
                        index = i;
                    i++;
                }


                if (results.Count > 0)
                {
                    if (results[index].Value.Value == typeof(Tile))
                    {                        
                        this.DoMove(0,results[index].Value.Key);
                    }
                    else
                    {
                        this.DoMove(results[index].Value.Key, 0);
                    }
                }
            }

            if (oldMouseState.RightButton == ButtonState.Pressed)
            {
                this.ClearSelectedItems();
            }
            #endregion
        }

        private void ClearSelectedItems() {
            if (this.selectedPiece > 0){
                this.PieceComponents[this.selectedPiece].IsSelected = false;
                this.selectedPiece = 0;
            }
            if (this.selectedTile > 0){
                this.TileComponents[this.selectedTile].IsSelected = false;
                this.selectedTile = 0;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);  
          
            if (gameState == GameState.PLAYING) {
                GraphicsDevice.DepthStencilState = dss;
                
                Matrix[] transforms = new Matrix[roomModel.Bones.Count];
                roomModel.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in roomModel.Meshes)
                {
                    foreach (BasicEffect e in mesh.Effects)
                    {
                        e.EnableDefaultLighting();
                        e.World = transforms[mesh.ParentBone.Index] * roomMatrix;
                        e.Projection = cam.Projection;
                        e.View = cam.View;
                    }

                    mesh.Draw();
                }
            }

            // Roep de base draw aan van andere klasse
            base.Draw(gameTime);

            // Draw FPS (Teken NADAT het menu getekend wordt, anders verdwijnt hij achter de background)
            gameMenu.spriteBatch.Begin();
            Vector2 pos = new Vector2((GraphicsDevice.Viewport.Width - (gameMenu.spriteFont.MeasureString("FPS: " + FPS).X + 10)), (GraphicsDevice.Viewport.Height - 24));
            gameMenu.spriteBatch.DrawString(gameMenu.spriteFont, "FPS: " + FPS, pos, Color.Blue);
            gameMenu.spriteBatch.End();
        }
    }
}
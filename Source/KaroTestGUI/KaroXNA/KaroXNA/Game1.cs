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
using System.Threading;
using KaroEngine;

namespace KaroXNA
{
    public enum GameState { MENU, PLAYING };
    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Properties
        #region General Properties
        DepthStencilState dss;
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;

        public Menu gameMenu;
        public GameState gameState;

        Random random = new Random();
       

        int selectedPiece, selectedTile,selectedStartingPiece;
        private int hourGlassRotation = 0;
        private float hourGlassSinus = 0f;
        private bool computerIsThinking = false;
        #endregion

        #region Engine Properties
        public KaroEngine.KaroEngineWrapper engine;
        public const int BOARDWIDTH = 17;

        public int insertionCount;

        double undoTimer;
        bool moveUndone;
        bool startUndoTimer = false;

        Dictionary<int, Tile> TileComponents;
        Dictionary<int, Piece> PieceComponents;
        List<Piece> StartingPieces;
        List<Tile> moveToList;

        public int[] move;
        public short[] BoxIndexes { get; set; }
        public bool ShowBoxes { get; set; }
        #endregion   

        #region Input Properties
        MouseState oldMouseState;
        public bool spacePressed;
        public bool leftMouseDown;

        bool f1Pressed;
        bool tPressed, rPresssed;
        #endregion

        #region Camera Properties
        Matrix world, view, proj, roomMatrix;
        public Model pieceModel, roomModel, tileModel;
        public Camera cam;
      
        float rotX = 0.0f;
        float rotY = 0.0f;

        public float frames = 0f;
        public float deltaFPSTime = 0f;
        public float FPS { get { return this.frames; } set { this.frames = value; } }
        #endregion        
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Game1(){
            
            moveToList = new List<Tile>();
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Karo XNA";
            Content.RootDirectory = "Content";
            cam = new Camera(graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height);
            gameState = GameState.MENU;
            
            spacePressed = false;
            f1Pressed = false;
            insertionCount = 0;
            undoTimer = 0;

            this.PieceComponents = new Dictionary<int, Piece>();
            this.TileComponents = new Dictionary<int, Tile>();
            this.StartingPieces = new List<Piece>();

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

        /// <summary>
        /// Initialise game
        /// </summary>
        protected override void Initialize()
        {
            device = graphics.GraphicsDevice;

            world = Matrix.Identity;
            view = cam.View;
            proj = cam.Projection;

            roomMatrix = Matrix.CreateScale(1.4f) * Matrix.CreateTranslation(new Vector3(-20, -20, 45));
            
            base.Initialize();
            //------------------TESTCODE--------------------------------
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;
            //-----------------------------------------------------------
        }

        /// <summary>
        /// Load the content
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameMenu = new Menu(this, 0, spriteBatch);
            Components.Add(gameMenu);
            Components.Add(new Hud(this, spriteBatch));

            tileModel = Content.Load<Model>("tile");
            pieceModel = Content.Load<Model>("piece");
            roomModel = Content.Load<Model>("room");
            this.NewGame();
        }

        /// <summary>
        /// Unloads the content
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Starts a new game
        /// </summary>
        public void NewGame()
        {
            engine = new KaroEngineWrapper();
            this.selectedPiece = 0;
            this.selectedTile = 0;
            this.selectedStartingPiece = -1;
            // clear tiles
            foreach (var tile in TileComponents)
            {
                Components.Remove(tile.Value);
            }
            TileComponents.Clear();

            // clear pieces
            foreach (var piece in PieceComponents)
            {
                Components.Remove(piece.Value);
            }
            PieceComponents.Clear();

            // add the tiles
            for (int x = 0; x < BOARDWIDTH; x++){
                for (int y = 0; y < BOARDWIDTH; y++)
                {
                    KaroEngine.Tile tile = engine.GetByXY(x, y);
                    if (tile != KaroEngine.Tile.BORDER && tile != KaroEngine.Tile.EMPTY)
                    {
                        Tile t = new Tile(this, tileModel, false, new Point(x, y));
                        this.TileComponents.Add(y * BOARDWIDTH + x, t);
                        Components.Add(t);
                    }
                }
            }
            StartingPieces.Clear();

            //White pawns
            for (int i = 0; i < 3; i+=1){
                Tile t = new Tile(this, tileModel, false, new Point(i + 7, 3));
                t.TileMatrix *= Matrix.CreateTranslation(0f, -1f, 0f);
                Piece p = new Piece(this, pieceModel, true, t, Color.White.ToVector3());
                this.StartingPieces.Add(p);
                t = new Tile(this, tileModel, false, new Point(i + 7, 4));
                t.TileMatrix *= Matrix.CreateTranslation(0f, -1f, 0f);
                p = new Piece(this, pieceModel, true, t, Color.White.ToVector3());
                this.StartingPieces.Add(p);                
            }

            //Red pawns
            for (int i = 0; i < 3; i += 1)
            {
                Tile t = new Tile(this, tileModel, false, new Point(i + 7, 9));
                t.TileMatrix *= Matrix.CreateTranslation(0f, -1f, 0f);
                Piece p = new Piece(this, pieceModel, true, t, Color.Tomato.ToVector3());
                this.StartingPieces.Add(p);
                t = new Tile(this, tileModel, false, new Point(i + 7, 10));
                t.TileMatrix *= Matrix.CreateTranslation(0f, -1f, 0f);
                p = new Piece(this, pieceModel, true, t, Color.Tomato.ToVector3());
                this.StartingPieces.Add(p);
            }
        }

        /// <summary>
        /// Executes a given move
        /// </summary>
        /// <param name="piece">Index of piece</param>
        /// <param name="tile">Index of tile to</param>
        /// <param name="tileFrom">Index of tile from</param>
        private void DoMove(int piece, int tile, int tileFrom) {
            if (tileFrom >= 0) {
                if (engine.GetGameState() == KaroEngine.GameState.PLAYING) {
                    Point location = PieceComponents[this.selectedPiece].OnTopofTile.Location;
                    int from = location.X + (location.Y * BOARDWIDTH);
                    Point location2 = TileComponents[this.selectedTile].Location;
                    int fromTile = location2.X + (location2.Y * BOARDWIDTH);
                    Point location3 = this.moveToList[tileFrom].Location;
                    int to= location3.X + (location3.Y * BOARDWIDTH);
                    bool result = engine.DoMove(from, to, fromTile);
                    if (result){
                        this.ClearSelectedItems();
                        this.ShowMove(location, location3, location2);
                        //start undo timer
                        startUndoTimer = true;
                        moveUndone = false;
                    }
                }
            }
            if (tile >= 0) { //If the move should be on a tile
                if (engine.GetGameState() == KaroEngine.GameState.INSERTION) {
                    if (this.selectedStartingPiece >= 0) {
                        Point location2 = TileComponents[tile].Location;                        
                        if (engine.InsertByXY(location2.X, location2.Y)){
                            this.ShowMove(location2, location2, location2);
                        }
                    }
                }
                else if (engine.GetGameState() == KaroEngine.GameState.PLAYING) {
                    Point location2 = TileComponents[tile].Location;
                    int to = location2.X + (location2.Y * BOARDWIDTH);
                    if (engine.GetByXY(location2.X, location2.Y) == KaroEngine.Tile.MOVEABLETILE){
                        TileComponents[tile].IsSelected = true;
                        this.selectedTile = tile;
                    }
                    if (this.selectedPiece > 0){
                        Point location = PieceComponents[this.selectedPiece].OnTopofTile.Location;
                        int from = location.X + (location.Y * BOARDWIDTH);
                        this.ClearSelectedItems();
                        if (engine.DoMove(from, to, -1)) {
                            this.ShowMove(location, location2, new Point());
                        }
                        //start undo timer
                        startUndoTimer = true;
                        moveUndone = false;
                    }
                }
            }
            if (piece >= 0) {
                // If the game is still running.
                if(engine.GetGameState() == KaroEngine.GameState.INSERTION || StartingPieces.Count!=0){
                    Player player = engine.GetTurn();
                    Vector3 red = Color.Tomato.ToVector3();
                    Vector3 white = Color.White.ToVector3();
                    if (StartingPieces[piece].Color.Equals(white) && player == Player.WHITE ||
                        StartingPieces[piece].Color.Equals(red) && player == Player.RED){
                            if (this.selectedStartingPiece >= 0) {
                                StartingPieces[selectedStartingPiece].IsSelected = false;
                            }
                            StartingPieces[piece].IsSelected = true;
                            this.selectedStartingPiece = piece;
                    }
                }
                if (engine.GetGameState() == KaroEngine.GameState.PLAYING ) {
                    
                    Player player = engine.GetTurn();
                    Vector3 red = Color.Tomato.ToVector3();
                    Vector3 white = Color.White.ToVector3();
                    if (PieceComponents[piece].Color.Equals(white) && player == Player.WHITE ||
                        PieceComponents[piece].Color.Equals(red) && player == Player.RED)
                    {
                        this.selectedPiece = piece;
                        PieceComponents[piece].IsSelected = true;
                        if (this.selectedTile > 0) {
                            Point location = PieceComponents[this.selectedPiece].OnTopofTile.Location;
                            Point location2 = TileComponents[this.selectedTile].Location;                            
                            int[][] possibleMoves = engine.GetPossibleMoves(location.X, location.Y, location2.X, location2.Y);
                            moveToList.Clear();
                            foreach (int[] item in possibleMoves){
                                moveToList.Add(new Tile(this,tileModel,true,new Point(item[0],item[1])));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the movement animation of the pawns & tiles
        /// </summary>
        /// <param name="positionFrom">Move from</param>
        /// <param name="positionTo">Move to</param>
        /// <param name="tileFrom">Tile from</param>
        private void ShowMove(Point positionFrom, Point positionTo, Point tileFrom)
        {
            if (engine.GetGameState() == KaroEngine.GameState.INSERTION || this.StartingPieces.Count != 0) {
                if (this.selectedStartingPiece >= 0){
                    Piece p = this.StartingPieces[this.selectedStartingPiece];
                    p.IsSelected = false;
                    this.ClearSelectedItems();
                    StartingPieces.Remove(p);

                    p.MoveTo(this.TileComponents[positionTo.Y * BOARDWIDTH + positionTo.X]);
                    Components.Add(p);
                    this.PieceComponents.Add(positionTo.Y * BOARDWIDTH + positionTo.X, p);
                }
            } else {
                if (tileFrom.X > 0) {
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
                
                int direction = (int)Math.Floor(MathHelper.ToDegrees((float)Math.Atan2(positionFrom.Y - positionTo.Y, positionFrom.X - positionTo.X)));

                if (t == KaroEngine.Tile.REDMARKED || t == KaroEngine.Tile.WHITEMARKED) {
                    flipping = true;
                } else {
                    flipping = false;
                }

                

                if (flipping != movedPiece.IsFlipped) {
                    movedPiece.IsFlipped = flipping;
                    /*if (positionFrom.X < positionTo.X){
                        movedPiece.rotationDirectionX = Rotations.ROTATIONPLUS;
                    }
                    else
                    {
                        movedPiece.rotationDirectionX = Rotations.ROTATIONMIN;
                    }*/
                    movedPiece.rotateDegrees = direction;
                   // int k = 3; ;



                } else {
                    //movedPiece.rotationDirectionX = Rotations.NONE;
                    movedPiece.rotateDegrees = 360;
                }
                this.PieceComponents.Add(positionTo.Y * BOARDWIDTH + positionTo.X, movedPiece);
            }
        }

        /// <summary>
        /// Game update, handles the updates
        /// </summary>
        /// <param name="gameTime">Elapsed time</param>
        protected override void Update(GameTime gameTime)
        {
            hourGlassRotation++;
            hourGlassSinus += 0.1f;
            if (this.startUndoTimer)
            {
                this.undoTimer = gameTime.TotalGameTime.TotalMilliseconds;
                startUndoTimer = false;
            }

            if (gameState == GameState.PLAYING)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space)){
                    if (!spacePressed){
                        spacePressed = true;

                        if (engine.GetGameState() == KaroEngine.GameState.PLAYING || engine.GetGameState() == KaroEngine.GameState.INSERTION || this.StartingPieces.Count != 0){
                            if(engine.GetGameState() != KaroEngine.GameState.INSERTION)
                                computerIsThinking = true;
                            Thread t = new Thread(new ThreadStart(ThreadedMove));
                            t.Start();
                        }
                    }
                }

                // handles mouse and keyboard inputs
                UpdateInput(gameTime);
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

        /// <summary>
        /// Generates computer move (for multithreading)
        /// </summary>
        private void ThreadedMove()
        {
            lock (engine) {
                move = engine.CalculateComputerMove();
                Point positionFrom = new Point(move[0] % Game1.BOARDWIDTH, move[0] / Game1.BOARDWIDTH);
                Point positionTo = new Point(move[1] % Game1.BOARDWIDTH, move[1] / Game1.BOARDWIDTH);
                Point tileFrom = new Point(move[2] % Game1.BOARDWIDTH, move[2] / Game1.BOARDWIDTH);
                if (this.StartingPieces.Count != 0) {
                    this.selectedStartingPiece=0;
                    Player player = engine.GetTurn();
                    Vector3 red = Color.Tomato.ToVector3();
                    Vector3 white = Color.White.ToVector3();

                    for (int i = 0; i < this.StartingPieces.Count; i++) {
                        if (StartingPieces[i].Color.Equals(white) && player == Player.WHITE ||
                        StartingPieces[i].Color.Equals(red) && player == Player.RED){
                            this.selectedStartingPiece = i;
                            break;
                        }
                    }
                }
                this.ShowMove(positionFrom, positionTo, tileFrom);
                this.ClearSelectedItems();
                computerIsThinking = false;
            }
        }

        /// <summary>
        /// Updates the user input, handles all the keypresses, mouse movements etc.
        /// </summary>
        /// <param name="gameTime">Elapsed time</param>
        private void UpdateInput(GameTime gameTime)
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
            //Disable moving while computer is calculating move
            if (!computerIsThinking)
            {
                oldMouseState = Mouse.GetState();

                if (oldMouseState.LeftButton == ButtonState.Pressed)
                    this.leftMouseDown = true;

                if (this.leftMouseDown && oldMouseState.LeftButton == ButtonState.Released)
                {

                    this.leftMouseDown = false;
                    Vector2 mousePosition = new Vector2(oldMouseState.X, oldMouseState.Y);

                    List<KeyValuePair<float?, KeyValuePair<int, Type>>> results = new List<KeyValuePair<float?, KeyValuePair<int, Type>>>();

                    foreach (var tile in TileComponents)
                    {
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

                    Dictionary<float, int> moveableClick = new Dictionary<float, int>();
                    Dictionary<float, int> StartingClick = new Dictionary<float, int>();
                    if (this.StartingPieces.Count == 0){
                        foreach (var piece in PieceComponents)
                        {
                            piece.Value.IsSelected = false;
                            Vector3 nearPlane = new Vector3(mousePosition.X, mousePosition.Y, 0);
                            Vector3 farPlane = new Vector3(mousePosition.X, mousePosition.Y, 1);
                            nearPlane = GraphicsDevice.Viewport.Unproject(nearPlane, cam.Projection, cam.View, piece.Value.world);
                            farPlane = GraphicsDevice.Viewport.Unproject(farPlane, cam.Projection, cam.View, piece.Value.world);
                            Vector3 direction = farPlane - nearPlane;
                            direction.Normalize();
                            Ray ray = new Ray(nearPlane, direction);
                            float? result = ray.Intersects((BoundingBox)piece.Value.PieceModel.Tag);
                            if (result.HasValue)
                            {
                                results.Add(new KeyValuePair<float?, KeyValuePair<int, Type>>(result, new KeyValuePair<int, Type>(piece.Key, typeof(Piece))));
                            }
                        }

                        for (int i = 0; i < moveToList.Count; i++)
                        {
                            Vector3 nearPlane = new Vector3(mousePosition.X, mousePosition.Y, 0);
                            Vector3 farPlane = new Vector3(mousePosition.X, mousePosition.Y, 1);
                            nearPlane = GraphicsDevice.Viewport.Unproject(nearPlane, cam.Projection, cam.View, moveToList[i].TileMatrix);
                            farPlane = GraphicsDevice.Viewport.Unproject(farPlane, cam.Projection, cam.View, moveToList[i].TileMatrix);
                            Vector3 direction = farPlane - nearPlane;
                            direction.Normalize();
                            Ray ray = new Ray(nearPlane, direction);
                            float? result = ray.Intersects((BoundingBox)moveToList[i].TileModel.Tag);
                            if (result.HasValue)
                            {
                                moveableClick.Add(result.Value, i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.StartingPieces.Count; i++)
                        {
                            Vector3 nearPlane = new Vector3(mousePosition.X, mousePosition.Y, 0);
                            Vector3 farPlane = new Vector3(mousePosition.X, mousePosition.Y, 1);
                            nearPlane = GraphicsDevice.Viewport.Unproject(nearPlane, cam.Projection, cam.View, StartingPieces[i].OnTopofTile.TileMatrix);
                            farPlane = GraphicsDevice.Viewport.Unproject(farPlane, cam.Projection, cam.View, StartingPieces[i].OnTopofTile.TileMatrix);
                            Vector3 direction = farPlane - nearPlane;
                            direction.Normalize();
                            Ray ray = new Ray(nearPlane, direction);
                            float? result = ray.Intersects((BoundingBox)StartingPieces[i].PieceModel.Tag);
                            if (result.HasValue)
                            {
                                StartingClick.Add(result.Value, i);
                            }
                        }
                    }


                    float shortestDistance = float.PositiveInfinity;
                    int index = 0;
                    int j = 0;
                    foreach (var result in results){
                        if (result.Key < shortestDistance)
                            index = j;
                        j++;
                    }
                    
                    if (StartingClick.Count > 0) {
                        if (index != 0){
                            if (results[index].Key < StartingClick.First().Key){
                                DoMove(StartingClick.First().Value, -1, -1);
                                results.Clear();
                                moveableClick.Clear();
                            }
                        }
                        else{
                            DoMove(StartingClick.First().Value, -1, -1);
                        }
                    }


                    if (moveableClick.Count > 0){
                        if (index != 0){
                            if (results[index].Key < moveableClick.First().Key)
                            {
                                DoMove(-1, -1, moveableClick.First().Value);
                                results.Clear();
                            }
                        }
                        else
                        {
                            DoMove(-1, -1, moveableClick.First().Value);
                        }
                    }

                    if (results.Count > 0)
                    {
                        if (results[index].Value.Value == typeof(Tile))
                        {
                            this.DoMove(-1, results[index].Value.Key, -1);
                        }
                        else
                        {
                            this.DoMove(results[index].Value.Key, -1, -1);
                        }
                    }
                }

                if (oldMouseState.RightButton == ButtonState.Pressed)
                {
                    if ((gameTime.TotalGameTime.TotalMilliseconds - this.undoTimer) < 1000 && !moveUndone)
                    {
                        //undo
                        int[] move = engine.UndoLastMove();
                        Point positionTo = new Point(move[0] % Game1.BOARDWIDTH, move[0] / Game1.BOARDWIDTH);
                        Point positionFrom = new Point(move[1] % Game1.BOARDWIDTH, move[1] / Game1.BOARDWIDTH);
                        Point tileFrom = new Point(move[2] % Game1.BOARDWIDTH, move[2] / Game1.BOARDWIDTH);
                        this.ShowMove(positionFrom, positionTo, tileFrom);
                        moveUndone = true;
                    }
                    else
                    {
                        this.ClearSelectedItems();
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Clears all the items that were selected
        /// </summary>
        private void ClearSelectedItems() {
            if (selectedStartingPiece >= 0) {
                this.StartingPieces[this.selectedStartingPiece].IsSelected = false;
                this.selectedStartingPiece = -1;
            }
            if (this.selectedPiece > 0){
                this.PieceComponents[this.selectedPiece].IsSelected = false;
                this.selectedPiece = 0;
            }
            if (this.selectedTile > 0){
                this.TileComponents[this.selectedTile].IsSelected = false;
                this.selectedTile = 0;
            }
            this.moveToList.Clear();
        }

        /// <summary>
        /// Draws the whore game
        /// </summary>
        /// <param name="gameTime">Elapsed time</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);            
            if (gameState == GameState.PLAYING) {
                GraphicsDevice.DepthStencilState = dss;
                
                Matrix[] transforms = new Matrix[roomModel.Bones.Count];
                roomModel.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in roomModel.Meshes)
                {
                    if (mesh.Name.Equals("hourglass"))
                    {
                        foreach (BasicEffect e in mesh.Effects)
                        {
                            e.EnableDefaultLighting();
                            if (computerIsThinking)
                            {

                                e.World =

                                    Matrix.CreateTranslation(0, 200, (float)Math.Sin(hourGlassSinus) * 10)
                                    * Matrix.CreateRotationZ(MathHelper.ToRadians(hourGlassRotation))
                                    * transforms[mesh.ParentBone.Index] 
                                    * Matrix.CreateWorld(new Vector3(33f, -10f, 60f), Vector3.Forward, Vector3.Up); //offset to middle of board
                            }
                            else
                            {
                                e.World = transforms[mesh.ParentBone.Index] * roomMatrix;// hourGlassMatrix;
                            }
                            e.Projection = cam.Projection;
                            e.View = cam.View;
                        }

                    }
                    else
                    {
                        foreach (BasicEffect e in mesh.Effects)
                        {
                            //e.EnableDefaultLighting();

                            e.World = transforms[mesh.ParentBone.Index] * roomMatrix;
                            e.Projection = cam.Projection;
                            e.View = cam.View;
                        }
                    }

                    mesh.Draw();
                }
                foreach (var item in this.StartingPieces){
                    item.Draw(gameTime);
                }
                foreach (var item in this.moveToList){
                    item.Draw(gameTime);
                }
            }


            // Roep de base draw aan van andere klasse
            base.Draw(gameTime);
            // Draw FPS (Teken NADAT het menu getekend wordt, anders verdwijnt hij achter de background)
            spriteBatch.Begin();
            Vector2 pos = new Vector2((GraphicsDevice.Viewport.Width - (gameMenu.spriteFont.MeasureString("FPS: " + FPS).X + 10)), (GraphicsDevice.Viewport.Height - 24));
            spriteBatch.DrawString(gameMenu.spriteFont, "FPS: " + FPS, pos, Color.Blue, 0, new Vector2(0,0), 0.6f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
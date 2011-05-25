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
        public Point Location { get; set; }
        public Vector3 Color { get; set; }

        public Matrix PieceMatrix { get; set; }
        public Matrix T { get; set; }

        public bool IsFlipped { get; set; }

        public Piece(Game game, Model pieceModel, bool visible, Point location, Vector3 color)
            : base(game)
        {
            this.game = (Game1)game;

            PieceModel = pieceModel;
            IsVisible = visible;
            Location = location;
            Color = color;

            PieceMatrix = Matrix.Identity;
            T = Matrix.Identity;

            IsFlipped = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (game.gameState == GameState.PLAYING)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Point positionFrom = new Point(game.move[0] % Game1.BOARDWIDTH, game.move[0] / Game1.BOARDWIDTH);
                    Point positionTo = new Point(game.move[1] % Game1.BOARDWIDTH, game.move[1] / Game1.BOARDWIDTH);
                    Point tileFrom = new Point(game.move[2] % Game1.BOARDWIDTH, game.move[2] / Game1.BOARDWIDTH);


                    if (game.engine.GetGameState() != KaroEngine.GameState.INSERTION || game.insertionCount > 12)
                    {
                        if (Location == positionFrom)
                        {
                            T = Matrix.Identity;

                            if (!IsFlipped)
                                T *= Matrix.CreateRotationX(MathHelper.ToRadians(180));

                            if (game.move[3] == 1)
                            {
                                T *= Matrix.CreateRotationX(MathHelper.ToRadians(180));
                                T *= Matrix.CreateTranslation(new Vector3((positionTo.X) * 5.5f, 1f, (positionTo.Y) * 5.5f));

                                if (IsFlipped)
                                    IsFlipped = false;
                                else
                                    IsFlipped = true;
                            }
                            else
                            {
                                T *= Matrix.CreateTranslation(new Vector3((positionTo.X) * 5.5f, 1f, (positionTo.Y) * 5.5f));
                            }

                            Location = positionTo;
                        }
                    }
                }
            }

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
                        
                        e.World = PieceMatrix * T;
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

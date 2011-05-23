using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
namespace KaroXNA
{
    class Piece
    {
        public Model PieceModel { get; set; }
        public Matrix PieceMatrix { get; set; }
        public bool IsVisible { get; set; }
        public bool IsFlipped { get; set; }
        public Point Location { get; set; }

        public Piece(Model model, bool visible, Point location)
        {
            PieceModel = model;
            IsVisible = visible;
            Location = location;
        }
    }
}

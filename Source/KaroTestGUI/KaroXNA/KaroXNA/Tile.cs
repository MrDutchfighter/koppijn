using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KaroXNA
{
    class Tile
    {
        public Model TileModel { get; set; }
        public Matrix TileMatrix { get; set; }
        public bool IsMovable { get; set; }

        public Tile(Model model, bool isMovable)
        {
            TileModel = model;
            IsMovable = isMovable;
        }
    }
}

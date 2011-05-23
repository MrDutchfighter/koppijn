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
    using XNAColor = Microsoft.Xna.Framework.Color;

    class Piece
    {
        public Model PieceModel { get; set; }
        public Matrix PieceMatrix { get; set; }
        public bool IsVisible { get; set; }
        public bool IsFlipped { get; set; }
        public Point Location { get; set; }
        public Vector3 Color { get; set; }
        public Matrix T { get; set; }
        
        public Piece(Model model, bool visible, Point location, Vector3 color)
        {
            PieceModel = model;
            IsVisible = visible;
            Location = location;
            Color = color;
            PieceMatrix = Matrix.Identity;
            T = Matrix.Identity;
            IsFlipped = false;
        }

        public void Draw(Camera cam)
        {
            foreach (ModelMesh mesh in PieceModel.Meshes)
            {
                foreach (BasicEffect e in mesh.Effects)
                {
                    e.PreferPerPixelLighting = true;

                    e.DiffuseColor = Color;
                    e.SpecularColor = XNAColor.White.ToVector3();
                    e.EnableDefaultLighting();
                    e.World = PieceMatrix * T;
                    e.View = cam.View;
                    e.Projection = cam.Projection;
                }

                mesh.Draw();
            }
        }


    }
}

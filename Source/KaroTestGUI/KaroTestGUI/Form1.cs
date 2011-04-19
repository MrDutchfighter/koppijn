using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KaroEngine;
using System.Runtime.InteropServices;    

namespace KaroTestGUI
{
    public enum Tile
    {
        EMPTY = 0,
        SOLIDTILE = 1,
        MOVEABLETILE = 2,
        WHITEUNMARKED = 3,
        WHITEMARKED = 4,
        REDUNMARKED = 5,
        REDMARKED = 6
    };

    public partial class Form1 : Form
    {
        KaroEngine.KaroEngine   engine;
        Pen                     penBlack;
        Pen                     penGray;
        Brush                   brushBlack;
        Brush                   brushWhite;
        Brush                   brushRed;
        Brush                   brushBlue;
        int                     boxSize = 25;
        bool                    gameOver = false; 

        Point                   clickedFirst;
        Point                   clickedSecond;

        public Form1()
        {
            engine = new KaroEngine.KaroEngine();
            penBlack        = Pens.Black;
            penGray         = new Pen(Color.Gray, 2);    
            brushBlack      = Brushes.Black;            
            brushRed        = Brushes.Red;
            brushWhite      = Brushes.White;
            brushBlue       = Brushes.Blue;

            clickedFirst    = new Point(-1, -1);
            clickedSecond   = new Point(-1, -1);

            InitializeComponent();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;  
          
            for (int y = 0; y < 15; y++)
            {
                for (int x = 0; x < 15; x++) 
                {
                    // Draw the board
                    if ((Tile)engine.GetByXY(x, y) != Tile.EMPTY)
                    {
                        g.FillRectangle(brushBlack, x * boxSize, y * boxSize, boxSize, boxSize);
                    }

                    // Draw the 'selected' tiles
                    if (x == clickedFirst.X && y == clickedFirst.Y)
                    {
                        g.FillRectangle(brushBlue, x * boxSize, y * boxSize, boxSize, boxSize);
                    }
                    if (x == clickedSecond.X && y == clickedSecond.Y)
                    {
                        g.FillRectangle(brushBlue, x * boxSize, y * boxSize, boxSize, boxSize);
                    }

                    // Check what kind of tiles, pawns etc are on the board.
                    switch ((Tile)engine.GetByXY(x, y)){ 
                        case Tile.EMPTY:
                            break;
                        case Tile.MOVEABLETILE:
                            break;
                        case Tile.SOLIDTILE:
                            break;
                        case Tile.REDUNMARKED:
                            g.FillEllipse(brushRed, x * boxSize+1, y * boxSize+1, boxSize-2, boxSize-2);                            
                            break;
                        case Tile.REDMARKED:
                            g.FillEllipse(brushRed, x * boxSize + 1, y * boxSize + 1, boxSize - 2, boxSize -2 );
                            g.DrawEllipse(penGray, x * boxSize + 5, y * boxSize + 5, boxSize - 10, boxSize - 10);
                            break;
                        case Tile.WHITEUNMARKED:
                            g.FillEllipse(brushWhite, x * boxSize + 1, y * boxSize + 1, boxSize - 2, boxSize - 2);                            
                            break;
                        case Tile.WHITEMARKED:
                            g.FillEllipse(brushWhite, x * boxSize + 1, y * boxSize + 1, boxSize - 2, boxSize - 2);
                            g.DrawEllipse(penGray, x * boxSize + 5, y * boxSize + 5, boxSize - 10, boxSize - 10);
                            break;                    
                    }

                    // Draw a grid
                    g.DrawRectangle(Pens.Gray, x * boxSize, y * boxSize, boxSize, boxSize);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!this.gameOver)
            {
                if (clickedFirst.X == -1)
                {
                    clickedFirst.X = (e.X - 1) / this.boxSize;
                    clickedFirst.Y = (e.Y - 1) / this.boxSize;
                }
                else if (clickedSecond.X == -1)
                {
                    clickedSecond.X = (e.X - 1) / this.boxSize;
                    clickedSecond.Y = (e.Y - 1) / this.boxSize;

                    engine.DoMove((clickedFirst.Y * 15) + clickedFirst.X, (clickedSecond.Y * 15) + clickedSecond.X);

                    clickedFirst = new Point(-1, -1);
                    clickedSecond = new Point(-1, -1);
                }
                else 
                {
                    clickedFirst = new Point(-1, -1);
                    clickedSecond = new Point(-1, -1);
                }
            }

            pictureBox1.Invalidate();
        }
    }
}

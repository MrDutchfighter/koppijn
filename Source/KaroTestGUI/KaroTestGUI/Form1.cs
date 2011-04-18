﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KaroTestGUI
{

    public partial class Form1 : Form
    {

        Pen penBlack;
        Pen penGray;
        Brush   brushBlack;
        Brush   brushWhite;
        Brush   brushRed;
        Brush   brushBlue;
        int     boxSize = 25;
        bool    gameOver=false; 



        Point clickedFirst;
        Point clickedSecond;
        public enum BOARDTILES { EMPTY,TILE, MOVEABLETILE,WHITEMARKED,WHITEUNMARKED,REDMARKED,REDUNMARKED }

        BOARDTILES[] board;

        public Form1()
        {            
            penBlack        = Pens.Black;

            penGray = new Pen(Color.Gray, 2);    

            brushBlack      = Brushes.Black;            
            brushRed        = Brushes.Red;
            brushWhite      = Brushes.White;
            brushBlue       = Brushes.Blue;

            clickedFirst    = new Point(-1, -1);
            clickedSecond   = new Point(-1, -1);

            board = new BOARDTILES[225];
            for (int y = 0; y < 15; y++) {
                for (int x = 0; x < 15; x++) {
                    board[(y * 15) + x] = BOARDTILES.EMPTY;
                }
            }

            for (int y = 4; y < 8; y++) {
                for (int x = 5; x < 10; x++) {                    
                    board[(y * 15) + x] = BOARDTILES.TILE;
                }
            }



            InitializeComponent();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;            
            for (int y = 0; y < 15; y++) {
                for (int x = 0; x < 15; x++) {
                    

                    //Draw the board
                    if (board[(y * 15) + x] != BOARDTILES.EMPTY ) {
                        g.FillRectangle(brushBlack, x * boxSize, y * boxSize, boxSize, boxSize);
                    }
                    //draw the 'selected' tiles
                    if (x == clickedFirst.X && y == clickedFirst.Y)
                    {
                        g.FillRectangle(brushBlue, x * boxSize, y * boxSize, boxSize, boxSize);
                    }
                    if (x == clickedSecond.X && y == clickedSecond.Y)
                    {
                        g.FillRectangle(brushBlue, x * boxSize, y * boxSize, boxSize, boxSize);
                    }

                    //check what kind of tiles, pawns etc are on the board.
                    switch (board[(y * 15) + x]) { 
                        case BOARDTILES.EMPTY:
                            break;
                        case BOARDTILES.TILE:
                            break;
                        case BOARDTILES.MOVEABLETILE:
                            break;
                        case BOARDTILES.REDUNMARKED:
                            g.FillEllipse(brushRed, x * boxSize+1, y * boxSize+1, boxSize-2, boxSize-2);                            
                            break;
                        case BOARDTILES.REDMARKED:
                            g.FillEllipse(brushRed, x * boxSize + 1, y * boxSize + 1, boxSize - 2, boxSize -2 );
                            g.DrawEllipse(penGray, x * boxSize + 5, y * boxSize + 5, boxSize - 10, boxSize - 10);
                            break;
                        case BOARDTILES.WHITEUNMARKED:
                            g.FillEllipse(brushWhite, x * boxSize + 1, y * boxSize + 1, boxSize - 2, boxSize - 2);
                            
                            break;
                        case BOARDTILES.WHITEMARKED:
                            g.FillEllipse(brushWhite, x * boxSize + 1, y * boxSize + 1, boxSize - 2, boxSize - 2);
                            g.DrawEllipse(penGray, x * boxSize + 5, y * boxSize + 5, boxSize - 10, boxSize - 10);
                            break;                    
                    }

                    //Draw a grid
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
                    board[(clickedFirst.Y * 15) + clickedFirst.X] = BOARDTILES.REDMARKED;
                }
                else if (clickedSecond.X == -1)
                {
                    clickedSecond.X = (e.X - 1) / this.boxSize;
                    clickedSecond.Y = (e.Y - 1) / this.boxSize;
                    board[(clickedSecond.Y * 15) + clickedSecond.X] = BOARDTILES.WHITEMARKED;
                }
                else {
                    clickedFirst = new Point(-1, -1);
                    clickedSecond = new Point(-1, -1);
                }
            }
            pictureBox1.Invalidate();
        }
    }
}

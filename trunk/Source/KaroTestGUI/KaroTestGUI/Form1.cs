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
 
    public partial class Form1 : Form
    {
        KaroEngine.KaroEngineWrapper    engine;
        Pen                             penBlack;
        Pen                             penGray;
        Brush                           brushBlack;
        Brush                           brushWhite;
        Brush                           brushRed;
        Brush                           brushBlue;
        int                             boxSize = 25;
        bool                            gameOver = false;

        Point                           clickedTile;
        Point                           clickedFirst;
        Point                           clickedSecond;
        String                          lastMessage;
                

        public Form1()
        {
            engine          = new KaroEngineWrapper();
            penBlack        = Pens.Black;
            penGray         = new Pen(Color.Gray, 2);    
            brushBlack      = Brushes.Black;            
            brushRed        = Brushes.Red;
            brushWhite      = Brushes.White;
            brushBlue       = Brushes.Blue;

            clickedFirst    = new Point(-1, -1);
            clickedSecond   = new Point(-1, -1);
            clickedTile     = new Point(-1, -1);
            
            InitializeComponent();

            UpdateGUI();
        }

        private void UpdateGUI() {
            string log = engine.getMessageLog();
            if (!log.Equals("")) {
                this.lastMessage = log;
                this.txtMessageLog.Text = this.lastMessage + this.txtMessageLog.Text;
            }
            GetTurn();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;            
            Font drawFont = new Font("Arial", 10);
          
            for (int y = 0; y < 15; y++)
            {
                for (int x = 0; x < 15; x++) 
                {
                    // Draw the board
                    if (engine.GetByXY(x, y) != Tile.EMPTY)
                    {
                        g.FillRectangle(brushBlack, x * boxSize, y * boxSize, boxSize, boxSize);

                    }
                    else {
                        if (tileNumbersToolStripMenuItem.Checked)
                        {
                            g.DrawString(((y * 15 + x) + ""), drawFont, brushBlack, x * boxSize, y * boxSize);
                        }
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
                    if (x == clickedTile.X && y == clickedTile.Y)
                    {
                        g.FillRectangle(Brushes.BlueViolet, x * boxSize, y * boxSize, boxSize, boxSize);
                    }

                    // Check what kind of tiles, pawns etc are on the board.
                    switch (engine.GetByXY(x, y)){ 
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
                    if (engine.GetGameState() == GameState.INSERTION)
                    {                        
                        engine.InsertByXY((e.X - 1) / this.boxSize,(e.Y - 1) / this.boxSize);
                    }
                    else if (engine.GetGameState() == GameState.PLAYING){
                        Tile tempTile=engine.GetByXY((e.X - 1) / this.boxSize, (e.Y - 1) / this.boxSize);
                        if (tempTile == Tile.MOVEABLETILE)
                        {
                            if (this.clickedTile.X == -1)
                            {
                                clickedTile.X = (e.X - 1) / this.boxSize;
                                clickedTile.Y = (e.Y - 1) / this.boxSize;
                            }
                        }
                        else
                        {
                            if (tempTile != Tile.EMPTY && tempTile != Tile.SOLIDTILE)
                            {
                                clickedFirst.X = (e.X - 1) / this.boxSize;
                                clickedFirst.Y = (e.Y - 1) / this.boxSize;
                            }
                        }
                    }
                    
                }
                else if (clickedSecond.X == -1)
                {
                    clickedSecond.X = (e.X - 1) / this.boxSize;
                    clickedSecond.Y = (e.Y - 1) / this.boxSize;

                    if (clickedTile.X == -1)
                    {
                        engine.DoMove((clickedFirst.Y * 15) + clickedFirst.X, (clickedSecond.Y * 15) + clickedSecond.X, -1);
                    }
                    else {
                        engine.DoMove((clickedFirst.Y * 15) + clickedFirst.X, (clickedSecond.Y * 15) + clickedSecond.X, (clickedTile.Y * 15) + clickedTile.X);
                    }

                    clickedTile     = new Point(-1, -1);
                    clickedFirst    = new Point(-1, -1);
                    clickedSecond   = new Point(-1, -1);
                }
                else 
                {
                    clickedFirst = new Point(-1, -1);
                    clickedSecond = new Point(-1, -1);
                }
            }

            UpdateGUI();
        }

        private void GetTurn()
        {
            if (engine.GetTurn() == Player.WHITE)
                pictureBox2.BackColor = Color.White;
            else
                pictureBox2.BackColor = Color.Red;
        }

        private void btnDoMove_Click(object sender, EventArgs e)
        {            
            engine.CalculateComputerMove();
            UpdateGUI();
        }

        private void tileNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tileNumbersToolStripMenuItem.Checked)
            {
                tileNumbersToolStripMenuItem.Checked = false;
            }
            else {
                tileNumbersToolStripMenuItem.Checked = true;
            }
            pictureBox1.Invalidate();
        }
       
    }
}

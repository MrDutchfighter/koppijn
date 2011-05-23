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
        KaroEngine.KaroEngineWrapper engine;
        Pen penBlack;
        Pen penGray;
        Pen penGreen;
        Brush brushBlack;
        Brush brushWhite;
        Brush brushRed;
        Brush brushBlue;
        int boxSize = 25;
        bool gameOver = false;
        const int BOARDWIDTH = 17;
        Point clickedTile;
        Point clickedFirst;
        Point clickedSecond;
        String lastMessage;
        int[][] possibleMoves;

        // Debug options

        public Form1()
        {
            engine = new KaroEngineWrapper();
            penBlack = new Pen(Color.Black, 1);
            penGray = new Pen(Color.Gray, 2);
            penGreen = new Pen(Color.Green, 2);
            brushBlack = Brushes.Black;
            brushRed = Brushes.Red;
            brushWhite = Brushes.White;
            brushBlue = Brushes.Blue;

            clickedFirst = new Point(-1, -1);
            clickedSecond = new Point(-1, -1);
            clickedTile = new Point(-1, -1);
            possibleMoves = null;

            InitializeComponent();
            textBox1.Text = "5";
            textBox2.Text = "1";
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            string log = engine.GetMessageLog();
            if (!log.Equals(""))
            {
                this.lastMessage = log;
                this.txtMessageLog.Text = this.lastMessage + this.txtMessageLog.Text;
            }
            this.lblEvalScore.Text = "Eval score: " + engine.GetEvaluationScore();
            GetTurn();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Font drawFont = new Font("Arial", 10);

            for (int y = 0; y < BOARDWIDTH; y++)
            {
                for (int x = 0; x < BOARDWIDTH; x++)
                {
                    // Draw the board                    
                    bool drawn = false;
                    if (possibleMoves != null)
                    {
                        foreach (int[] move in possibleMoves)
                        {
                            if (move[0] == x && move[1] == y)
                            {
                                g.FillRectangle(Brushes.HotPink, x * boxSize, y * boxSize, boxSize, boxSize);
                                drawn = true;
                            }
                        }
                    }

                    if (engine.GetByXY(x, y) != Tile.EMPTY)
                    {


                        if (!drawn)
                        {
                            g.FillRectangle(brushBlack, x * boxSize, y * boxSize, boxSize, boxSize);
                        }
                    }
                    else
                    {
                        if (tileNumbersToolStripMenuItem.Checked)
                        {
                            g.DrawString(((y * BOARDWIDTH + x) + ""), drawFont, brushBlack, x * boxSize, y * boxSize);
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
                    switch (engine.GetByXY(x, y))
                    {
                        case Tile.BORDER:
                            break;
                        case Tile.EMPTY:
                            break;
                        case Tile.MOVEABLETILE:
                            break;
                        case Tile.SOLIDTILE:
                            break;
                        case Tile.REDUNMARKED:
                            g.FillEllipse(brushRed, x * boxSize + 1, y * boxSize + 1, boxSize - 2, boxSize - 2);
                            break;
                        case Tile.REDMARKED:
                            g.FillEllipse(brushRed, x * boxSize + 1, y * boxSize + 1, boxSize - 2, boxSize - 2);
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

                    // If debug (draw movable tiles) is on, draw the movable tiles
                    if (movableTilesToolStripMenuItem.Checked && engine.GetByXY(x, y) == Tile.MOVEABLETILE)
                    {
                        g.DrawRectangle(penGreen, x * boxSize, y * boxSize, boxSize, boxSize);
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            engine = new KaroEngineWrapper();

            pictureBox1.Invalidate();

            btnDoMove.Enabled = true;
            btn.Enabled = true;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!this.gameOver)
            {
                if (clickedFirst.X == -1)
                {
                    if (engine.GetGameState() == GameState.INSERTION)
                    {
                        engine.InsertByXY((e.X - 1) / this.boxSize, (e.Y - 1) / this.boxSize);
                    }
                    else if (engine.GetGameState() == GameState.PLAYING)
                    {
                        Tile tempTile = engine.GetByXY((e.X - 1) / this.boxSize, (e.Y - 1) / this.boxSize);
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
                                possibleMoves = engine.GetPossibleMoves(clickedFirst.X, clickedFirst.Y, clickedTile.X, clickedTile.Y);
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
                        engine.DoMove((clickedFirst.Y * BOARDWIDTH) + clickedFirst.X, (clickedSecond.Y * BOARDWIDTH) + clickedSecond.X, -1);
                    }
                    else
                    {
                        engine.DoMove((clickedFirst.Y * BOARDWIDTH) + clickedFirst.X, (clickedSecond.Y * BOARDWIDTH) + clickedSecond.X, (clickedTile.Y * BOARDWIDTH) + clickedTile.X);
                    }

                    clickedTile = new Point(-1, -1);
                    clickedFirst = new Point(-1, -1);
                    clickedSecond = new Point(-1, -1);
                    possibleMoves = null;
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

        #region Menu Toolstrip Actions
        private void tileNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tileNumbersToolStripMenuItem.Checked)
            {
                tileNumbersToolStripMenuItem.Checked = false;
            }
            else
            {
                tileNumbersToolStripMenuItem.Checked = true;
            }
            pictureBox1.Invalidate();
        }

        private void movableTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (movableTilesToolStripMenuItem.Checked)
            {
                movableTilesToolStripMenuItem.Checked = false;
            }
            else
            {
                movableTilesToolStripMenuItem.Checked = true;
            }
            pictureBox1.Invalidate();
        }
        #endregion

        // Undo
        private void btn_Click(object sender, EventArgs e)
        {
            engine.UndoLastMove();
            UpdateGUI();
        }

        // Generate random move(s)
        private void button1_Click(object sender, EventArgs e)
        {
            engine = new KaroEngineWrapper();
            UpdateGUI();

            engine.InsertByXY(5, 4);
            engine.InsertByXY(6, 4);
            engine.InsertByXY(7, 4);
            engine.InsertByXY(8, 4);
            engine.InsertByXY(9, 4);

            engine.InsertByXY(5, 5);
            engine.InsertByXY(6, 5);
            engine.InsertByXY(7, 5);
            engine.InsertByXY(8, 5);
            engine.InsertByXY(9, 5);

            engine.InsertByXY(5, 6);
            engine.InsertByXY(6, 6);

            Application.DoEvents();

            int times = int.Parse(textBox1.Text);
            int moves = 0;

            DateTime startTijd = DateTime.Now;
            TimeSpan timeDiff = DateTime.Now - DateTime.Now;

            for (int i = 0; i < times; i++)
            {
                engine.CalculateComputerMove();
                moves++;
                UpdateGUI();
                Application.DoEvents();

                if (engine.GetGameState() == GameState.GAMEFINISHED)
                {
                   timeDiff = DateTime.Now - startTijd;
                    ShowWinning(moves, (float)timeDiff.TotalSeconds, false);
                    break;
                }
            }

            timeDiff = DateTime.Now - startTijd; 

            this.txtMessageLog.Text = "Moves:\t" + moves + "\r\n\r\n" + this.txtMessageLog.Text; 
            this.txtMessageLog.Text = "Avarage:\t" + (timeDiff.TotalSeconds / times) + " Seconds \r\n" + this.txtMessageLog.Text;
            this.txtMessageLog.Text = "Total:\t" + timeDiff.TotalSeconds + " Seconds \r\n" + this.txtMessageLog.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            engine = new KaroEngineWrapper();
            UpdateGUI();

            DateTime startTijd = DateTime.Now;
            TimeSpan timeDiff = DateTime.Now - DateTime.Now;

            int white = 0;
            int red = 0;
            int draw = 0;

            bool bdraw = false;

            int games = int.Parse(textBox2.Text);
            
            for (int i = 0; i < games; i++)
            {
                newGameToolStripMenuItem_Click(sender, e);
                int moves = 0;
                startTijd = DateTime.Now;

                while (engine.GetGameState() != GameState.GAMEFINISHED)
                {
                    if (moves >= 180)
                    {
                        bdraw = true;
                        break;
                    }

                    engine.CalculateComputerMove();
                    moves++;
                    UpdateGUI();
                    Application.DoEvents();
                }


                textBox2.Text = "" + (int.Parse(textBox2.Text) -1);

                if (bdraw)
                    draw++;
                else
                {
                    if (engine.GetTurn() == Player.WHITE)
                        red++;
                    if (engine.GetTurn() == Player.RED)
                        white++;
                }
                bdraw = false;

                timeDiff = DateTime.Now - startTijd;
                ShowWinning(moves, (float)timeDiff.TotalSeconds, bdraw);
            }

            this.txtMessageLog.Text = "Draw\t" + draw + "\r\n\r\n" + this.txtMessageLog.Text;
            this.txtMessageLog.Text = "RED:\t"+ red + "\r\n" + this.txtMessageLog.Text;
            this.txtMessageLog.Text = "WHITE:\t" + white + " \r\n" + this.txtMessageLog.Text;
            this.txtMessageLog.Text = "Played:\t" + games + "\r\n" + this.txtMessageLog.Text;

            textBox2.Text = "" + games; 
        }

        private void ShowWinning(int moves, float total, bool draw)
        {
            String winningPlayer = "";

            if(engine.GetTurn() == Player.WHITE)
                winningPlayer = "RED";
            if(engine.GetTurn() == Player.RED)
                winningPlayer = "WHITE";
            if (draw)
                winningPlayer = "DRAW";
            
            this.txtMessageLog.Text = "Moves:\t" + moves + "\r\n\r\n" + this.txtMessageLog.Text;
            this.txtMessageLog.Text = "Seconds:\t" + total + "\r\n" +this.txtMessageLog.Text;
            this.txtMessageLog.Text = winningPlayer + " WINS\r\n" + this.txtMessageLog.Text;

            btnDoMove.Enabled = false;
            btn.Enabled = false;
        }
    }
}

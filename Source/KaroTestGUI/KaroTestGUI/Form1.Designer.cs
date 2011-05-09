namespace KaroTestGUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblEvalScore = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vieuwsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileNumbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movableTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDoMove = new System.Windows.Forms.Button();
            this.btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.txtMessageLog = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 23);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(426, 426);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // lblEvalScore
            // 
            this.lblEvalScore.AutoSize = true;
            this.lblEvalScore.Location = new System.Drawing.Point(12, 452);
            this.lblEvalScore.Name = "lblEvalScore";
            this.lblEvalScore.Size = new System.Drawing.Size(60, 13);
            this.lblEvalScore.TabIndex = 1;
            this.lblEvalScore.Text = "Eval score:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.vieuwsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(709, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newGameToolStripMenuItem
            // 
            this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            this.newGameToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.newGameToolStripMenuItem.Text = "New Game";
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Q";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // vieuwsToolStripMenuItem
            // 
            this.vieuwsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileNumbersToolStripMenuItem,
            this.movableTilesToolStripMenuItem});
            this.vieuwsToolStripMenuItem.Name = "vieuwsToolStripMenuItem";
            this.vieuwsToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.vieuwsToolStripMenuItem.Text = "Views";
            // 
            // tileNumbersToolStripMenuItem
            // 
            this.tileNumbersToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tileNumbersToolStripMenuItem.Name = "tileNumbersToolStripMenuItem";
            this.tileNumbersToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.tileNumbersToolStripMenuItem.Text = "Show Tile Numbers";
            this.tileNumbersToolStripMenuItem.Click += new System.EventHandler(this.tileNumbersToolStripMenuItem_Click);
            // 
            // movableTilesToolStripMenuItem
            // 
            this.movableTilesToolStripMenuItem.Name = "movableTilesToolStripMenuItem";
            this.movableTilesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.movableTilesToolStripMenuItem.Text = "Show Movable Tiles";
            this.movableTilesToolStripMenuItem.Click += new System.EventHandler(this.movableTilesToolStripMenuItem_Click);
            // 
            // btnDoMove
            // 
            this.btnDoMove.Location = new System.Drawing.Point(557, 51);
            this.btnDoMove.Name = "btnDoMove";
            this.btnDoMove.Size = new System.Drawing.Size(140, 23);
            this.btnDoMove.TabIndex = 3;
            this.btnDoMove.Text = "Compute Computer Move";
            this.btnDoMove.UseVisualStyleBackColor = true;
            this.btnDoMove.Click += new System.EventHandler(this.btnDoMove_Click);
            // 
            // btn
            // 
            this.btn.Location = new System.Drawing.Point(557, 183);
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(140, 23);
            this.btn.TabIndex = 4;
            this.btn.Text = "button1";
            this.btn.UseVisualStyleBackColor = true;
            this.btn.Click += new System.EventHandler(this.btn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(554, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 25);
            this.label1.TabIndex = 5;
            this.label1.Text = "Player turn:";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox2.Location = new System.Drawing.Point(557, 127);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(131, 50);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // txtMessageLog
            // 
            this.txtMessageLog.Location = new System.Drawing.Point(476, 275);
            this.txtMessageLog.Multiline = true;
            this.txtMessageLog.Name = "txtMessageLog";
            this.txtMessageLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessageLog.Size = new System.Drawing.Size(221, 187);
            this.txtMessageLog.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(473, 259);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "MessageLog";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(559, 213);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(138, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(559, 243);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(138, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Automatic";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(534, 213);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(19, 20);
            this.textBox1.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 474);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMessageLog);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn);
            this.Controls.Add(this.btnDoMove);
            this.Controls.Add(this.lblEvalScore);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(725, 512);
            this.MinimumSize = new System.Drawing.Size(725, 512);
            this.Name = "Form1";
            this.Text = "Karo Test GUI";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblEvalScore;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button btnDoMove;
        private System.Windows.Forms.Button btn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ToolStripMenuItem vieuwsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileNumbersToolStripMenuItem;
        private System.Windows.Forms.TextBox txtMessageLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem movableTilesToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;

    }
}


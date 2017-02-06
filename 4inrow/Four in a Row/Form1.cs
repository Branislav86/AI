using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Four_in_a_Row
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            this.Width = 906;
            this.Height = 628;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(200, 100);
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.BackColor = Color.LightBlue;
            this.Show();
            this.Focus();

            //Set graphic stuff

            //  Surface is het tekenpapier
            surface = new Bitmap(pnlMain.Width, pnlMain.Height);

            // Device is the pen which draws the graphics
            device = pnlMain.CreateGraphics();

            drawall();
        }

        //Global Variables
        Bitmap surface;
        Graphics device;
        Graphics Backdevice;
        Board gameboard = null;
        
        //Form events
        private void Form1_Load(object sender, EventArgs e)
        {
            //Set correct form size
           
                        
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            surface.Dispose();
            device.Dispose();
        }

        gamestate currentDbgState = null;
        gamestate[,] dbgStateArray = null;

        // 1. Drawing
        private void drawDbg(gamestate state, Font font, String text, SolidBrush brush)
        {
            int x = state.Column;
            int y = state.Row;

            int xx = 8 + x * state.squares[x, y].SizeH;
            int yy = 8 + y * state.squares[x, y].SizeV;

            String score = "";
            if (Math.Abs(state.AIValue) == gameboard.WINNING_SCORE)
            {
                if (state.AIValue == -gameboard.WINNING_SCORE)
                    score = "WIN 2";
                else
                    score = "WIN 1";
            }
            else
                score = state.AIValue.ToString();

            device.DrawString(score+text, font, brush, xx, yy);

            dbgStateArray[x, y] = state;
        }

        private void drawall()
        {
            if(gameboard != null)
                gameboard.draw(device);

            if (cbDebug.Checked)
            {
                Font font = new Font("Arial", 14, FontStyle.Bold);
                SolidBrush clrPrev = new SolidBrush(Color.Orange);
                SolidBrush clrCurr = new SolidBrush(Color.LightGreen);
                SolidBrush clrNext = new SolidBrush(Color.Red);
                //SolidBrush blu = new SolidBrush(Color.Blue);
                                
                for (int i = 0; i < gameboard.Width; i++)
                    for (int j = 0; j < gameboard.Height; j++)
                        dbgStateArray[i, j] = null;

                gamestate state;
                
                // prev
                int prevCnt = -1;
                state = currentDbgState.Parent;
                while (state != null)
                {
                    drawDbg(state, font, Environment.NewLine + "(" + prevCnt.ToString() + ")", clrPrev);

                    prevCnt--;

                    state = state.Parent;
                }

                // curr
                drawDbg(currentDbgState, font, "", clrCurr);

                // next
                for (int i = 0; i < currentDbgState.children.Count; i++)
                {
                    drawDbg(currentDbgState.children[i], font, "", clrNext);
                }
            }

            //Here we copy the backbuffer (drawing paper) to the pen
            device = Graphics.FromImage(surface);

            //Here we draw the surface to the screen
            Backdevice = pnlMain.CreateGraphics();
            Backdevice.DrawImage(surface, 0, 0, pnlMain.Width, pnlMain.Height);

            //Here we fix the overdraw
            device.Clear(Color.LightBlue);
        }
        
        private void cbDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDebug.Checked)
            {
                currentDbgState = gameboard.AIRootState;
                if (dbgStateArray == null)
                {
                    dbgStateArray = new gamestate[gameboard.Width, gameboard.Height];
                }
            }
            drawall();
        }

        private void pnlMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameboard == null)
                return;

            decimal X = (e.X) / gameboard.squares[0, 0].SizeH;
            decimal Y = (e.Y) / gameboard.squares[0, 0].SizeV;
            X = Math.Floor(X);
            Y = Math.Floor(Y);

            int xSquare = Convert.ToInt32(X);
            int ySquare = Convert.ToInt32(Y);

            if (cbDebug.Checked)
            {
                if (dbgStateArray[xSquare, ySquare] != null)
                {
                    currentDbgState = dbgStateArray[xSquare, ySquare];
                }
            }
            else if(gameboard.gameRun)
            {
                //Check Correct click
                if (gameboard.CheckCorrectClick(xSquare))
                {
                    gameboard.placeSquare(xSquare, ySquare, gameboard.activePlayer);

                    //Player 2 moves (AI move)
                    if (gameboard.activePlayer == 2)
                    {
                        gameboard.placeSquareAI(label1, xSquare, (int)nudMaxDepth.Value);
                    }
                    button2.Enabled = true;
                }
            }
            drawall();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String text = "";
            int NrSquaresX = gameboard.squares.GetLength(0);
            int NrSquaresY = gameboard.squares.GetLength(1);

            //MessageBox.Show(this.squares[0,0].SizeH.ToString());

            for (int x = 0; x < NrSquaresX; x++)
            {
                for (int y = 0; y < NrSquaresY; y++)
                {
                    if (gameboard.squares[x, y].Owner != 0)
                    {
                        text += "squares[" + x.ToString() + ", " + y.ToString() + "].Owner = " + gameboard.squares[x, y].Owner.ToString() + ";" + Environment.NewLine;
                    }
                }
            }
            tbDumpBoard.Text = text;
            tbDumpBoard.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gameboard.squares[gameboard.lastMyMove.X, gameboard.lastMyMove.Y].Owner = 0;
            gameboard.squares[gameboard.lastAIMove.X, gameboard.lastAIMove.Y].Owner = 0;
            button2.Enabled = false;
            gameboard.gameRun = true;
            drawall();
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            gameboard = new Board((int)nudWidth.Value, (int)nudHeight.Value, 60);
            gameboard.gameRun = true;
            drawall();
        }

        private void nudMaxDepth_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}

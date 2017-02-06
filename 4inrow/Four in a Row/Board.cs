using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Four_in_a_Row
{
    class Board
    {
        bool writeDbg = false;
        String dbgText = "";
        public Point lastMyMove, lastAIMove;

        //Local variables
        int pixelsizeW, pixelsizeH;
        int sizeWidth;
        int sizeHeight;
        int spacer;
        int activeplayer;
        Color backcolor;
        public Square[,] squares;
        //AI States
        public gamestate AIRootState;
        bool gamerun;

        public int WINNING_SCORE = 10000;

        //======================//
        //      Constructor     //
        //======================//

        public Board(int sizeW, int sizeH, int pixels)
        {
            spacer = 50;
            sizeWidth = sizeW;
            sizeHeight = sizeH;
            pixelsizeW = pixels * sizeW;
            pixelsizeH = pixels * sizeH;
            backcolor = Color.Black;
            activePlayer = 1;
            squares = new Square[sizeWidth, sizeHeight];

            for (int x = 0; x < squares.GetLength(0); x++)
            {
                for (int y = 0; y < squares.GetLength(1); y++)
                {
                    squares[x, y] = new Square(pixels, 0);
                }

            }

            //Create game AI states
            //AIStates = new gamestate[SearchDepth + 1, System.Convert.ToInt32(Math.Pow(System.Convert.ToDouble(sizeWidth), System.Convert.ToDouble(SearchDepth)))];

            //squares[1, 7].Owner = 1;
            //squares[2, 7].Owner = 2;
            //squares[4, 7].Owner = 2;
            //squares[1, 6].Owner = 1;
            //squares[2, 6].Owner = 1;
            //squares[4, 6].Owner = 1;

            ////Debug test
            //squares[2, 3].Owner = 1;
            //squares[3, 3].Owner = 2;
            //squares[3, 2].Owner = 2;
            //squares[0, 3].Owner = 2;
            //squares[0, 2].Owner = 2;
            //squares[1, 3].Owner = 2;
            //squares[1, 2].Owner = 2;
            //squares[1, 1].Owner = 1;
            //squares[0, 1].Owner = 1;

            //Debug test 2
            //squares[0, 2].Owner = 1;
            //squares[0, 3].Owner = 2;
            //squares[1, 1].Owner = 2;
            //squares[1, 2].Owner = 1;
            //squares[1, 3].Owner = 1;
            //squares[2, 2].Owner = 1;
            //squares[2, 3].Owner = 2;
            //squares[2, 1].Owner = 2;

            //Debug test 3
            //squares[0, 5].Owner = 1;
            //squares[1, 5].Owner = 1;
            //squares[3, 5].Owner = 1;

            //Debug test 4
            //squares[0, 3].Owner = 1;
            //squares[1, 3].Owner = 1;
            //squares[0, 2].Owner = 2;
            //squares[1, 2].Owner = 2;

        }

        //======================//
        //      Properties      //
        //======================//

        public int Spacer
        {
            get { return spacer; }
            set { spacer = value; }
        }

        public bool gameRun 
        {
            get { return gamerun; }
            set { gamerun = value; }
        }


        public int activePlayer
        {
            get { return activeplayer; }
            set { activeplayer = value; }
        }

        public int widthPixels
        {
            get { return pixelsizeW; }
            set { pixelsizeW = value; }
        }

        public int Width
        {
            get { return sizeWidth; }
            set { sizeWidth = value; }
        }

        public int Height
        {
            get { return sizeHeight; }
            set { sizeHeight = value; }
        }

        public int heightPixels
        {
            get { return pixelsizeH; }
            set { pixelsizeH = value; }
        }

        public SolidBrush backBrush
        {
            get
            {
                SolidBrush backbrush = new SolidBrush(backcolor);
                return backbrush;
            }

        }

        //=====================//
        //      Methods        //
        //=====================//

        public Boolean CheckCorrectClick(int x)
        {
            // Check for playing fields bounds
            if ((x > -1) && (x < sizeWidth))
            { }
            else
            {
                MessageBox.Show("Click inside the playingfield");
                return false;
            }

            // Check for free row
            if (squares[x, 0].Owner != 0)
            {
                MessageBox.Show("This column is full");
                return false;
            }

            return true;
        } //Checks whether the mouseclick made by user is correct

        public void placeSquare(int x, int y, int player)
        {
            // Color square
            for (int i = sizeHeight - 1; i > -1; i--)
            {
                if (squares[x, i].Owner == 0)
                {
                    squares[x, i].Owner = player;
                    y = i;
                    break;
                }

            }

            lastMyMove.X = x;
            lastMyMove.Y = y;

            // Check win
            if (checkWin(x, y, player, squares, 3))
            {
                gamewon();
            }

            // Change active player
            if (player == 1)
            {
                activeplayer = 2;
            }
            else if (player == 2)
            {
                activeplayer = 1;
            }


        }

        public bool checkFull()
        {
            for (int y = 0; y < sizeHeight; y++)
            {
                for (int x = 0; x < sizeWidth; x++)
                {
                    if (squares[x, y].Owner == 0)
                    {
                        return false;
                    }
                }
            }
            // If no square has owner 0, then the board is full
            return true;
        }

        public int GetScore(int x, int y, int player, Square[,] Field)
        {
            int score = 0;

            for (int orient = 0; orient < 4; orient++)
            {
                // horizontal
                for (int i = 0; i < 4; i++)
                {
                    int missCount = 4;
                    int startX, startY;
                    if (orient == 0)
                    {
                        startX = x - 3 + i;
                        startY = y;
                    }
                    else if (orient == 1)
                    {
                        startX = x;
                        startY = y - 3 + i;
                    }
                    else if (orient == 2)
                    {
                        startX = x - 3 + i;
                        startY = y + 3 - i;
                    }
                    else
                    {
                        startX = x - 3 + i;
                        startY = y - 3 + i;
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        int xx, yy;
                        if (orient == 0)
                        {
                            xx = startX + j;
                            yy = startY;
                        }
                        else if (orient == 1)
                        {
                            xx = startX;
                            yy = startY + j;
                        }
                        else if (orient == 2)
                        {
                            xx = startX + j;
                            yy = startY - j;
                        }
                        else
                        {
                            xx = startX + j;
                            yy = startY + j;
                        }
                        if (xx < 0 || xx >= sizeWidth || yy < 0 || yy >= sizeHeight)
                        {
                            j = 4;
                            missCount = 4;
                            continue;
                        }

                        int owner = Field[xx, yy].Owner;
                        if (owner != 0 && owner != player)
                        {
                            j = 4;
                            missCount = 4;
                            continue;
                        }
                        if (owner == player)
                            missCount--;
                        else if (owner == 0)
                        {
                            if (yy != sizeHeight - 1)
                            {
                                if (Field[xx, yy + 1].Owner == 0)
                                    missCount++;
                            }
                        }
                    }

                    if (missCount == 0)
                        return WINNING_SCORE;
                    else if (missCount == 1)
                        score += 100;
                    else if (missCount == 2)
                        score += 25;
                }
            }

            return score;
        }

        public bool checkWin(int column, int row, int player, Square[,] Field, int targetCount)
        {
            //int target = 3; //The maximum numbers of stones to be in line, not counting current stone
            int count = 0; //The count of stones in line
            int check = 0; //The amount to add or remove from the stone to check

            //--------------------//
            // 1. Check vertical  //
            //--------------------//

            do
            {
                count++;
            }
            while (row + count < sizeHeight && Field[column, row + count].Owner == player);

            if (count > targetCount)
            {
                return true;
            }

            count = 0;
            check = 0;

            //---------------------//
            // 2. Check horizontal //
            //---------------------//

            //First left
            do
            {
                count++;
            }
            while (column - count >= 0 && Field[column - count, row].Owner == player);

            if (count > targetCount)
            {
                return true;
            }

            count -= 1; //Make sure you dont double count the center piece. Deduct it here.

            //Then right
            do
            {
                count++;
                check++;
            }
            while (column + check < sizeWidth && Field[column + check, row].Owner == player);

            if (count > targetCount)
            {
                return true;
            }

            count = 0;
            check = 0;

            //------------------------//
            //  3. Check diagonal     //
            //  Lefttop - rightbottom //
            //------------------------//

            //To the top
            do
            {
                count++;
            }
            while (column - count >= 0 && row - count >= 0 && Field[column - count, row - count].Owner == player);

            if (count > targetCount)
            {
                return true;
            }

            count -= 1; //Make sure you dont double count the center piece. Deduct it here.

            //To the bottom
            do
            {
                count++;
                check++;
            }
            while (column + check < sizeWidth && row + check < sizeHeight && Field[column + check, row + check].Owner == player);

            if (count > targetCount)
            {
                return true;
            }

            count = 0;
            check = 0;

            //------------------------//
            //  3. Check diagonal     //
            //  Righttop - leftbottom //
            //------------------------//

            //To the top
            do
            {
                count++;
            }
            while (column + count < sizeWidth && row - count >= 0 && Field[column + count, row - count].Owner == player);

            if (count > targetCount)
            {
                return true;
            }

            count -= 1; //Make sure you dont double count the center piece. Deduct it here.

            //To the bottom
            do
            {
                count++;
                check++;
            }
            while (column - check >= 0 && row + check < sizeHeight && Field[column - check, row + check].Owner == player);

            if (count > targetCount)
            {
                return true;
            }

            return false;
        }

        public void gamewon()
        {
            MessageBox.Show("Player " + activeplayer.ToString() + " has won!");
            gameRun = false;
        }

        public void draw(Graphics device)
        {
            // Draw board background
            device.FillRectangle(this.backBrush, 0, 0, this.widthPixels, this.heightPixels);

            //Draw a small colored circle for each square
            int NrSquaresX = this.squares.GetLength(0);
            int NrSquaresY = this.squares.GetLength(1);

            //MessageBox.Show(this.squares[0,0].SizeH.ToString());

            for (int x = 0; x < NrSquaresX; x++)
            {
                for (int y = 0; y < NrSquaresY; y++)
                {
                    SolidBrush SquareBrush = new SolidBrush(this.squares[x, y].Color);
                    //device.FillRectangle(Brushes.Green, this.Spacer + x*this.squares[x,y].SizeH, this.Spacer+y*this.squares[x,y].SizeV , this.squares[x,y].SizeH, this.squares[x,y].SizeV);
                    device.FillEllipse(SquareBrush,
                        /*this.Spacer + */8 + x * this.squares[x, y].SizeH,
                        /* this.Spacer + */8 + y * this.squares[x, y].SizeV,
                        this.squares[x, y].SizeH - 16,
                        this.squares[x, y].SizeV - 16);
                    //MessageBox.Show(this.squares[x,y].Color.ToString());
                    //MessageBox.Show((this.Spacer + x * this.squares[x, y].SizeH).ToString());
                }

            }
        }

        //=============================//
        //          AI Methods         //
        //=============================//

        //1. The general movement function, just places the square and times the thinking time.
        public void placeSquareAI(Label label1, int ColumnUsed, int MaxDepth)
        {
            //Start timer
            Stopwatch Timer = new Stopwatch();
            Timer.Start();

            //Check if moves can be made
            if (checkFull())
            {
                MessageBox.Show("Table is full!");
            }
            else
            {
                // Clear created gamestates
                if(AIRootState != null)
                    ClearAIStates(AIRootState);

                //determine column

                //  Create gamestate
                AIRootState = new gamestate(squares, ColumnUsed, sizeWidth, sizeHeight);

                //  Determine AI move
                AIMove AI = new AIMove();
                AI = RecursiveMiniMaxAI(0, MaxDepth, AIRootState, 2); //Recursive Mini Max Move

                //Place token
                int row = placeTokenAI(AI.Move);

                //Check win
                if (checkWin(AI.Move, row, activeplayer, squares, 3))
                {
                    gamewon();
                }

                //Change player
                activeplayer = 1;

            }

            Timer.Stop();
            label1.Text = "Time elapsed: " + Timer.ElapsedMilliseconds.ToString() + " ms";
            Timer.Reset();
        }

        //1.1. Supportive function that just places the token.
        public int placeTokenAI(int column)
        {
            for (int i = (sizeHeight - 1); i >= 0; i--)
            {
                if (squares[column, i].Owner == 0)
                {
                    squares[column, i].Owner = activeplayer;

                    lastAIMove.X = column;
                    lastAIMove.Y = i;

                    return i;
                }
            }
            return -1;
        }

        //1.2 Supportive function that clears all AIStates.
        public void ClearAIStates(gamestate state)
        {
            for (int i = 0; i < state.children.Count; i++)
            {
                ClearAIStates(state.children[i]);
            }
            state.children.Clear();
        }

        public void DumpState(gamestate state)
        {
            Bitmap bmp = new Bitmap(widthPixels + 2 * this.spacer, heightPixels + 2 * this.spacer);
            Graphics device = Graphics.FromImage(bmp);
            draw(device);

            gamestate currState = state;
            Font font = new Font("Arial", 30);
            String fileName = "";
            SolidBrush sb1 = new SolidBrush(Color.Red);
            SolidBrush sb2 = new SolidBrush(Color.Blue);

            int cnt = 1;
            while (currState != null)
            {
                currState = currState.Parent;
                cnt++;
            }

            currState = state;
            while (currState != null)
            {
                //device.FillEllipse(SquareBrush, , , this.squares[x, y].SizeH - 16, this.squares[x, y].SizeV - 16);
                int x = currState.Column;
                int y = 0;
                while (y < sizeHeight)
                {
                    if (currState.squares[x, y].Owner != 0)
                        break;
                    y++;
                }

                int xx = this.Spacer + 8 + x * this.squares[x, y].SizeH;
                int yy = this.Spacer + 8 + y * this.squares[x, y].SizeV;
                SolidBrush sb;
                if (currState.squares[x, y].Owner == 1)
                    sb = sb1;
                else
                    sb = sb2;
                device.DrawString((cnt - 1).ToString(), font, sb, xx, yy);

                fileName += currState.Column.ToString();

                currState = currState.Parent;

                cnt--;
            }

            bmp.Save("A" + fileName + ".png");
            bmp.Dispose();
        }

        //C. AI movement three, recursive minimax
        public AIMove RecursiveMiniMaxAI(int Depth, int MaxDepth, gamestate StateOfBoard, int Player)
        {
            AIMove ReturnMove = new AIMove();

            //BestScore & Move
            AIMove BestMove = new AIMove(-1, 0);
            if (Player == 2) // Minimizing
                BestMove.Score = 99999;
            else
                BestMove.Score = -99999;

            if (Depth == MaxDepth)
            //We are at the bottom of the search space and evaluate this level
            {
                ReturnMove.Score = EvaluateBoard(StateOfBoard, StateOfBoard.Column);
                ReturnMove.Move = StateOfBoard.Column;
            }

            else
            {
                int score = EvaluateBoard(StateOfBoard, StateOfBoard.Column);

                if (Math.Abs(score) == WINNING_SCORE)
                {
                    ReturnMove.Score = score;
                    ReturnMove.Move = StateOfBoard.Column;
                }
                else
                {
                    //Check for possible moves
                    List<int> MoveList = new List<int>();
                    List<AIMove> EqualGoodReturns = new List<AIMove>();
                    for (int column = 0; column < squares.GetLength(0); column++)
                    {
                        if (StateOfBoard.squares[column, 0].Owner == 0)
                        {
                            MoveList.Add(column);
                        }
                    }

                    //Execute a minimax on each child
                    for (int k = 0; k < MoveList.Count; k++)
                    {
                        //Make a move and create a new gamestate
                        gamestate newState = new gamestate(StateOfBoard, MoveList[k], sizeWidth, sizeHeight);
                        StateOfBoard.children.Add(newState);

                        //Place a token in the new Gamestate
                        PlaceToken(newState, MoveList[k], Player);

                        //Call minimax on the child
                        ReturnMove = RecursiveMiniMaxAI(Depth + 1, MaxDepth, newState, Player == 1 ? 2 : 1);

                        newState.AIValue = ReturnMove.Score;

                        if (Player == 2)  // Minimizing
                        {
                            if (ReturnMove.Score < BestMove.Score)
                            {
                                EqualGoodReturns.Clear();
                                EqualGoodReturns.Add(new AIMove(MoveList[k], ReturnMove.Score));
                                BestMove.Score = ReturnMove.Score;
                                BestMove.Move = MoveList[k]; //Changed this from ReturnMove
                            }
                            else if (ReturnMove.Score == BestMove.Score)
                                EqualGoodReturns.Add(new AIMove(MoveList[k], ReturnMove.Score));
                        }
                        else
                        {
                            //Evaluate return
                            if (ReturnMove.Score > BestMove.Score)
                            {
                                EqualGoodReturns.Clear();
                                EqualGoodReturns.Add(new AIMove(MoveList[k], ReturnMove.Score));
                                BestMove.Score = ReturnMove.Score;
                                BestMove.Move = MoveList[k]; //Changed this from ReturnMove
                            }
                            else if (ReturnMove.Score == BestMove.Score)
                                EqualGoodReturns.Add(new AIMove(MoveList[k], ReturnMove.Score));
                        }

                    }
                    if (EqualGoodReturns.Count == 1 || EqualGoodReturns.Count == 0)
                    {
                        ReturnMove = BestMove;
                    }
                    else
                    {
                        Random rnd = new Random();
                        ReturnMove = EqualGoodReturns[rnd.Next(EqualGoodReturns.Count)];
                    }
                }
            }

            //string Data2;
            //Data2 = ReturnMove.Score.ToString();
            //Data2 += "/";
            //Data2 += ReturnMove.Move.ToString();
            //Trace.WriteLine("ReturnScore & Move: " + Data2 + " || At Depth " + Depth.ToString());

            if (writeDbg)
            {
                if (Depth == MaxDepth && ReturnMove.Score != 0)
                {
                    //for (int i = 0; i < Depth; i++)
                    //    dbgText += "\t";
                    gamestate currState = StateOfBoard;
                    while (currState != null)
                    {
                        dbgText += "AI[" + currState.AIValue + "] COL[" + currState.Column + "]\t";
                        currState = currState.Parent;
                    }
                    //dbgText += Environment.NewLine;
                    dbgText += "P:" + Player.ToString() + "  Col:" + StateOfBoard.Column.ToString() + "  BestMove:" + ReturnMove.Move.ToString() + "  BestScore:" + ReturnMove.Score.ToString() + Environment.NewLine;

                    if (ReturnMove.Score > 100 && false)
                        DumpState(StateOfBoard);
                }
            }

            return ReturnMove;
        }

        //C1. Supportive function that checks all moves in a gamestate
        // public List<int> GenerateListOfMoves(gamestate StateOfBoard)
        // {

        // return list;
        //}

        //C2. Supportive function that returns the first empty gamestate on a given depth

        //C3. Supportive function that places the token in the new Gamestate
        public void PlaceToken(gamestate Board, int Column, int Player)
        {
            for (int i = sizeHeight - 1; i >= 0; i--)
            {
                if (Board.squares[Column, i].Owner == 0)
                {
                    Board.squares[Column, i].Owner = Player;
                    break;
                }
            }
        }

        //C4. Evaluates the board for a given gamestate
        public int EvaluateBoard(gamestate Board, int ColumnUsed)
        {
            //Determine the highest placed fiche and call checkwin on that fiche
            for (int y = 0; y < sizeHeight; y++)
            {
                if (Board.squares[ColumnUsed, y].Owner != 0)
                {
                    int score = GetScore(ColumnUsed, y, Board.squares[ColumnUsed, y].Owner, Board.squares);
                    
                    if (Board.squares[ColumnUsed, y].Owner == 1)
                        return score;
                    else
                        return -score;
                }
            }

            return 0;
        }
    }
}
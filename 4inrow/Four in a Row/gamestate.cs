using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Four_in_a_Row
{
    class gamestate
    {
        //Local variables
        public Square[,] squares;
        gamestate parent;
        public List<gamestate> children = new List<gamestate>();
        int columnused;
        int rowused;
        int aivalue;
        string id;
        
        //Constructors
        public gamestate(gamestate ParentState, int column, int sizeW, int sizeH)
        {
            // Set parent state
            parent = ParentState;

            //Set ID
            id = parent.ID;
            id += "-";
            id += column.ToString();

            // Set squares
            squares = new Square[sizeW, sizeH];

            for (int x = 0; x < sizeW; x++)
            {
                for (int y = 0; y < sizeH; y++)
                {
                    squares[x, y] = new Square(60, ParentState.squares[x, y].Owner);
                }
            }

            // Set columns used
            columnused = column;

            rowused = 0;
            while (rowused < sizeH)
            {
                if (squares[columnused, rowused].Owner != 0)
                    break;
                rowused++;
            }
            rowused--;

        }  // Used to set a state based on a parent

        public gamestate(Square[,] CurrentSquares, int column, int sizeW, int sizeH)
        {
            //Set squares
            squares = new Square[sizeW, sizeH];

            for (int x = 0; x < sizeW; x++)
            {
                for (int y = 0; y < sizeH; y++)
                {
                    squares[x, y] = new Square(60, CurrentSquares[x, y].Owner);
                    //MessageBox.Show(squares[x, y].Owner.ToString());
                }
            }

            // Set columns used
            columnused = column;

            rowused = 0;
            while (rowused < sizeH)
            {
                if (squares[columnused, rowused].Owner != 0)
                    break;
                rowused++;
            }

        } // Used to set initial state
        
        //Properties
        public int AIValue
        {
            get { return aivalue; }
            set { aivalue = value; }
        }

        public gamestate Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public int Column
        {
            get { return columnused; }
            set { columnused = value; }
        }

        public int Row
        {
            get { return rowused; }
            set { rowused = value; }
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        //Methods


    }
}

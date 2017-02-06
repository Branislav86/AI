using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Four_in_a_Row
{
    class AIMove
    {
        int move = 0;
        int score = 0;

        public AIMove()
        {
            move = score = 0;
        }

        public AIMove(int move, int score)
        {
            this.move = move;
            this.score = score;
        }

        public int Move
        {
            get { return move; }
            set { move = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

    }
}

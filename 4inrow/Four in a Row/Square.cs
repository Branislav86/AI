using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Four_in_a_Row
{
    class Square
    {
        //Local variables
        int size;
        int owner;

        //Constructor
        public Square(int pixels, int Owner)
        {
            //Random rnd2 = new Random();
            //int randomCalc = unique * rnd2.Next();
            //Random rnd = new Random(randomCalc);
            //owner = rnd.Next(0, 3);

            size = pixels;
            owner = Owner;
        }

        //Properties
        public int SizeH
        {
            get{return size;}
            set{size = value;}
        }

        public int SizeV
        {
            get { return size; }
            set { size = value; }
        }

        public int Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public Color Color
        {
            get
            {
                Color color;
                
                switch (owner)
                {
                    case 0:
                        color = Color.White;
                        break;
                    case 1:
                        color = Color.Crimson;
                        break;
                    case 2:
                        color = Color.RoyalBlue;
                        break;
                    default:
                        color = Color.White;
                        break;
                }

                return color;
            }
        }

    }
}

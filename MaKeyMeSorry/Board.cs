using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace MaKeyMeSorry
{
    class Board{


        private List<Square> theBoard;

        // These could be organized as a vector of vectors of Squares 
        // with the equivalent color = to its representative enum
        private List<Square> blueBase;
        private List<Square> yellowBase;
        private List<Square> greenBase;
        private List<Square> redBase; 


        //Again this could be a vector of int's if it seems to work better.
        private int blueStart;
        private int yellowStart;
        private int greenStart;
        private int redStart;


        private static int slideSize1;
        private static int slideSize2;

        private static int numSquares;

        // Not sure if we need
        public void update_board(){
            // TODO Write update_board
        }

        // Returns the start square number for the inputted color equivalent
        public int get_start_square(int numColor){
            // TODO Write get_start_square
            return -1;
        }

        public void execute_slide(int squareNum, int pawnNum){
            // TODO Write execute_slide
        }

        public Square get_square_at(int index)
        {
            //TODO Write get_square_at
            return null;
        }

        public List<Square> get_my_base(Color myColor)
        {
            switch (myColor)
            {
                case Color.BLUE:
                    return blueBase;
                case Color.GREEN:
                    return greenBase;
                case Color.RED:
                    return redBase;
                case Color.YELLOW:
                    return yellowBase;
                default:
                    Debug.WriteLine("error getting base for my color!");
                    return null;
            }
        }

        public Board(){
            // TODO Write Board constructor
        }


    }
}

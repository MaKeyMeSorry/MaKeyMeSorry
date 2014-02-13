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
            switch(numColor)
            {
                case 0:
                    return -1;
                case 1:
                    return -1;
                case 2:
                    return 4;
                case 3:
                    return -1;
                default:
                    return -1;
            }
            return -1;
        }

        public void execute_slide(int startSlide, Pawn pawn){
            int index = startSlide;
            // note: this assumes that there may be other pawns on the 
            // start slide location and they will be moved to sorry. If
            // for some reason your pawn is on the start location it will
            // be sent to start then sent to the end of slide but user will
            // never see
            while(theBoard[index].get_Type() != SquareKind.SLIDE_END)
            {
                if(theBoard[index].get_has_pawn())
                {
                    theBoard[index].get_pawn_in_square().sorry();
                }
                index++;
            }
            if(theBoard[index].get_Type() == SquareKind.SLIDE_END)
            {
                if (theBoard[index].get_has_pawn())
                {
                    theBoard[index].get_pawn_in_square().sorry();
                }
            }
        }

        public Square get_square_at(int index)
        {
            return theBoard[index];
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
            int index = 0;
            theBoard = new List<Square>();
            for (int i = 0; i < 4; i++)
            {
                theBoard.Add(new Square(index++, SquareKind.REGULAR, Color.WHITE));
                theBoard.Add(new Square(index++, SquareKind.SLIDE_START, (Color)i));
                theBoard.Add(new Square(index++, SquareKind.HOMEC, (Color)i));
                theBoard.Add(new Square(index++, SquareKind.REGULAR, (Color)i));
                theBoard.Add(new Square(index++, SquareKind.STARTC, (Color)i));
                theBoard.Add(new Square(index++, SquareKind.REGULAR, Color.WHITE));
                theBoard.Add(new Square(index++, SquareKind.REGULAR, Color.WHITE));
                theBoard.Add(new Square(index++, SquareKind.REGULAR, Color.WHITE));
                theBoard.Add(new Square(index++, SquareKind.REGULAR, Color.WHITE));
                theBoard.Add(new Square(index++, SquareKind.SLIDE_START, (Color)i));
                theBoard.Add(new Square(index++, SquareKind.REGULAR, (Color)i));
                theBoard.Add(new Square(index++, SquareKind.REGULAR, (Color)i));
                theBoard.Add(new Square(index++, SquareKind.REGULAR, (Color)i));
                theBoard.Add(new Square(index++, SquareKind.SLIDE_END, (Color)i));
                theBoard.Add(new Square(index++, SquareKind.REGULAR, Color.WHITE));
            }
            print_Board();
        }

        public void print_Board()
        {
            foreach (Square square in theBoard)
            {
                Debug.WriteLine("index: " + square.get_index() + " type: " + square.get_Type() + " Color: " + square.get_color());
            }
        }
        


    }
}

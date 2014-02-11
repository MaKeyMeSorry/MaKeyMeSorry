using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaKeyMeSorry
{

    public enum SquareKind {
        REGULAR,
        SLIDE_START,
        SLIDE_END,
        STARTSQ, // Start square for a player
        HOMESQ,  // Home square for a player
        STARTC,  // Square that connects to start square.
        HOMEC    // Square that connects to home slide.
    };

    class Square{

        private bool hasPawn;
        private Pawn pawnInSquare;
        private SquareKind type;
        private Color color; // for if it is a slide square, which color slide it is

        public Square()
        {
            // TODO Write Square Constructor
        }

        public bool can_place_pawn(Pawn pawn){
            if(!hasPawn)
                return true;
            if(pawn.get_color() == pawnInSquare.get_color()){
                return false;
            }
            return true;
        }

        public void place_pawn(Pawn pawn){
            if(hasPawn){
                    pawnInSquare.sorry();
            }
            pawnInSquare = pawn;
        }

        public SquareKind get_Type()
        {
            return type;
        }

    }

}

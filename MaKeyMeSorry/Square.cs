using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaKeyMeSorry
{

    public enum SquareKind
    {
        REGULAR,
        SLIDE_START,
        SLIDE_END,
        STARTSQ, // Start square for a player
        HOMESQ,  // Home square for a player
        STARTC,  // Square that connects to start square.
        HOMEC,    // Square that connects to home slide.
        SAFE
    };

    public class Square
    {

        private bool hasPawn;
        private Pawn pawnInSquare;
        private SquareKind type;
        private Color color; // for if it is a slide square, which color slide it is
        private int index;

        public Square(int index, SquareKind type)
        {
            this.index = index;
            this.type = type;
            this.color = Color.BLUE;
            // TODO Write Square Constructor
        }

        public Square(int index, SquareKind type, Color color)
        {
            this.index = index;
            this.type = type;
            this.color = color;
            this.hasPawn = false;
        }

        public bool can_place_pawn(Pawn pawn)
        {
            if (!hasPawn)
                return true;
            if (pawn.get_color() == pawnInSquare.get_color() && this.type != SquareKind.HOMESQ)
            {
                return false;
            }
            return true;
        }

        public void place_pawn(Pawn pawn)
        {
            if (hasPawn && this.type != SquareKind.HOMESQ)
            {
                pawnInSquare.sorry();
            }
            pawnInSquare = pawn;
            set_has_pawn(true);
        }

        public SquareKind get_Type()
        {
            return type;
        }

        public int get_index()
        {
            return index;
        }

        public Color get_color()
        {
            return color;
        }

        public bool get_has_pawn()
        {
            return hasPawn;
        }

        public Pawn get_pawn_in_square()
        {
            return pawnInSquare;
        }

        public void set_has_pawn(bool hasPawn)
        {
            this.hasPawn = hasPawn;
        }

    }

}

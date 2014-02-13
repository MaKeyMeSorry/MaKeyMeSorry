using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaKeyMeSorry
{

    public enum Color { RED, BLUE, GREEN, YELLOW};

    class Pawn {
        private Color color;

        // Current Square the pawn is on. NULL if at start
        private Square current_location;

        public Pawn(Color pawnColor, Square pawnSquare)
        {
            color = pawnColor;
            current_location = pawnSquare;
        }

        public Color get_color()
        {
            return color;
        }

        public Square get_current_location()
        {
            return current_location;
        }

        public void move_to(Square square)
        {
            current_location = square;
        }

        public bool is_home()
        {
            if (current_location.get_Type() == SquareKind.HOMESQ)
            {
                return true;
            }
            return false;
        }

        public bool is_start()
        {
            if (current_location == null)
            {
                return true;
            }
            return false;
        }

        // Sends this pawn back to start
        // current location set to null
        public void sorry()
        {
            current_location = null;
        }

    }
}

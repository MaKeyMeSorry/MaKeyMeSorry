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
            // TODO Write move_to
        }

        public bool is_home()
        {
            // TODO Write is_home
            return false;
        }

        public bool is_start()
        {
            // TODO Write is_start
            return false;
        }

        // Sends this pawn back to start
        public void sorry()
        {
            // TODO Write sorry
        }

    }
}

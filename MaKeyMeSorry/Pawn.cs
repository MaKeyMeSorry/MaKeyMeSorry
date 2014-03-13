using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaKeyMeSorry
{

    public enum Color { RED, BLUE, YELLOW, GREEN, WHITE };

    public class Pawn
    {
        private Color color;

        // Current Square the pawn is on. NULL if at start
        private Square current_location;
        private int id;
        bool in_safe_zone;

        public Pawn(Color pawnColor, Square pawnSquare, int id)
        {
            color = pawnColor;
            current_location = pawnSquare;
            this.id = id;
            in_safe_zone = false;
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
            if (current_location != null)
            {
                current_location.set_has_pawn(false);
            }
            current_location = square;
            square.place_pawn(this);
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

        public bool is_in_safe_zone()
        {
            return in_safe_zone;
        }

        public void set_in_safe_zone(bool isSafe)
        {
            in_safe_zone = isSafe;
        }



        // Sends this pawn back to start
        // current location set to null
        public void sorry()
        {
            current_location = null;
        }

        public int get_id()
        {
            return id;
        }

    }
}

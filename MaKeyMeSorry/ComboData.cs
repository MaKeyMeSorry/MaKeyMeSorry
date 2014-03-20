using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaKeyMeSorry
{
    public class ComboData
    {

        public enum move { FORWARD, BACKWARD, SORRY, SWAP };
        public Square square_location;
        public move move_choice;

        public ComboData(move move_choice, Square square_location)
        {
            this.move_choice = move_choice;
            this.square_location = square_location;
        }

        public override string ToString()
        {
            return square_location.get_index().ToString();
        }

    }
}

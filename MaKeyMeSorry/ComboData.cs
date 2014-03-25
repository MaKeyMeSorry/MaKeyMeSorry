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
        public int current_choice_number;
        public int total_choice_number;

        public ComboData(move move_choice, Square square_location, int current_choice_number, int total_choice_number)
        {
            this.move_choice = move_choice;
            this.square_location = square_location;
            this.current_choice_number = current_choice_number;
            this.total_choice_number = total_choice_number;
        }

        public override string ToString()
        {
            return "Choice " + current_choice_number.ToString() + " of " + total_choice_number.ToString();
        }

    }
}

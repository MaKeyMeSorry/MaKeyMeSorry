using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MaKeyMeSorry
{
    public enum CardType { START = 0, FORWARD = 1, BACKWARD = 2, SWAP = 3, SORRY = 4, SPLIT = 5 };

    class Card
    {

        private CardType type;
        private int value;
        private int[] card_features = new int[6];

        // Constructor for a Card
        public Card(CardType type, int value)
        {
            if (value == 2)
            {
                card_features[(int)CardType.START] = 1;
                card_features[(int)CardType.FORWARD] = 1;
                card_features[(int)CardType.BACKWARD] = 0;
                card_features[(int)CardType.SWAP] = 0;
                card_features[(int)CardType.SORRY] = 0;
                card_features[(int)CardType.SPLIT] = 0;
            }
            else if (value == 3)
            {

                // TODO Finish Card Constructor

            }
        }

        public ~Card(){
            // TODO Write Card Destructor
        }

        public bool get_start()
        {
            if (card_features[(int)CardType.START] == 1)
            {
                return true;
            }
            else if (card_features[(int)CardType.START] == 0)
            {
                return false;
            }
            else
            {
                // TODO Print Error
                return false;
            }
        }

        //returns value to move forward or 0 otherwise
        public int can_move_forward()
        {
            if (card_features[(int)CardType.FORWARD] == 1)
            {
                return value;
            }
            return 0;
        }

        // returns value to move backward or 0 otherwise
        public int can_move_backward()
        {
            if (card_features[(int)CardType.BACKWARD] == 1)
            {
                if (value == 10)
                {
                    return 1;
                }
                return value;
            }
            return 0;
        }


        public bool can_split_up()
        {
            // TODO Write can_split_up
            return false;
        }

        public bool can_swap()
        {
            // TODO Write can_swap
            return false;
        }

        public bool can_sorry()
        {
            // TODO Write can_sorry
            return false;
        }

        //flip card over to see its value
        private void flip_Card()
        {
            // TODO Write flip_Card
        }

    };

}

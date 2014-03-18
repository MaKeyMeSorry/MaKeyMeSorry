using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MaKeyMeSorry
{
    public enum CardType { START = 0, FORWARD = 1, BACKWARD = 2, SWAP = 3, SORRY = 4, SPLIT = 5 };

    public class Card
    {

        private int value;
        private bool[] card_features = new bool[6];

        // Constructor for a Card
        public Card(int card_value)
        {
            /*
             *  Quant 5: "1" Move a pawn from Start or move a pawn 1 space forward
             *  Quant 4: "2" Move a pawn from start or move a pawn 2 spaces forward. Drawing a "2" entitles 
             *          the player to draw again at the end of his or her turn. If you cannot use "2", you 
             *          can still draw again
             *  Quant 4: "3" Move a pawn 3 spaces forward
             *  Quant 4: "4" Move a pawn 4 spaces backwards
             *  Quant 4: "5" Move a pawn 5 spaces forward
             *  Quant 4: "7" Move one pawn 7 spaces or split the 7 spaces between two pawns.
             *              Cannot be split into 6 and 1 or 5 and 2. Entire 7 must be used or forfeit turn
             *  Quant 4: "8" Move a pawn 8 spaces forward
             *  Quant 4: "10" Move a pawn 10 spaces forward or 1 space backward. If a player cannot go forward 
             *          10 spaces, must go backwards 1 space
             *  Quant 4: "11" Move 11 spaces forward or switch places with one opposing pawn. A player that 
             *          cannot move 11 spaces isnt forced to switch with opponent. Instead they can forfeit
             *  Quant 4: "12" Move 12 spaces forward
             *  Quant 4: "SORRY!" Move any one pawn from the start to a square occupied by any opponent, 
             *          sending that pawn back to its own start. If there are no pawns on the player's start, 
             *          or no opponent's on any squares, the turn is lost.  If an enemy's pawn is swapped 
             *          while it is in front of your HOME, your pawn is switched exactly where your enemy's pawn is, not at your HOME.
             */

            value = card_value;
            for (int i = 0; i < 6; i++)
            {
                card_features[i] = false;
            }

            card_features[(int)CardType.FORWARD] = true; 

                if (value == 1)
                {
                    card_features[(int)CardType.START] = true;
                    //card_features[(int)CardType.FORWARD] = true;
                }
                else if (value == 2)
                {
                    card_features[(int)CardType.START] = true;
                    //card_features[(int)CardType.FORWARD] = true; 
                }
                else if (value == 3)
                {
                    //card_features[(int)CardType.FORWARD] = true; 
                }
                else if (value == 4)
                {
                    card_features[(int)CardType.FORWARD] = false; 
                    card_features[(int)CardType.BACKWARD] = true;
                }
                else if (value == 5)
                {
                    //card_features[(int)CardType.FORWARD] = true; 
                }
                else if (value == 7)
                {
                    //card_features[(int)CardType.FORWARD] = true; 
                    card_features[(int)CardType.SPLIT] = true;
                }
                else if (value == 8)
                {
                    //card_features[(int)CardType.FORWARD] = true; 
                }
                else if (value == 10)
                {
                    card_features[(int)CardType.BACKWARD] = true; 
                }
                else if (value == 11)
                {
                    //card_features[(int)CardType.FORWARD] = true; 
                    card_features[(int)CardType.SWAP] = true;
                }
                else if (value == 12)
                {
                    //card_features[(int)CardType.FORWARD] = true; 
                }
                else if (value == 13)
                {
                    card_features[(int)CardType.FORWARD] = false;
                    card_features[(int)CardType.SORRY] = true;
                }
                else
                {
                    throw new Exception("Invalid value given to card");
                }
        }

        public bool get_start()
        {
            return card_features[(int)CardType.START];
        }

        //returns value to move forward or 0 otherwise
        public int can_move_forward()
        {
            if (card_features[(int)CardType.FORWARD])
            {
                return value;
            }
            return 0;
        }

        // returns value to move backward or 0 otherwise
        // special case: 10 returns 1 for backwards move
        public int can_move_backward()
        {
            if (card_features[(int)CardType.BACKWARD])
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
            return card_features[(int)CardType.SPLIT];

        }

        public bool can_swap()
        {
            return card_features[(int)CardType.SWAP];
        }

        public bool can_sorry()
        {
            return card_features[(int)CardType.SORRY];
        }

        public int get_value(){
            return value;
        }

    };

}

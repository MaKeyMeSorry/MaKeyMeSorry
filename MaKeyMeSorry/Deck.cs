using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaKeyMeSorry
{
    class Deck {
        
        private static int MAXCARDS = 45; 
        //Array of cards (45 is the count) deck holds
        private Card[] cards=  new Card[MAXCARDS];  
        //The index of the card on the TOP of the deck
        private int curCard; 

        // Deck Constructor
        public Deck()
        {
            curCard = 0;
            init_Deck();
            shuffle_Deck();
        }

        public ~Deck(){
            // TODO: Write Deck Deconstructor
        }

        // draw_card: draws a card from the deck, should auto shuffle the deck if curCard == MAXCARDS  
        // and set curCard = 0;
        public Card draw_card(){
            if(curCard == MAXCARDS){
                curCard = 0;
                shuffle_Deck();
            }
            return(cards[curCard++]);
        } 

        private void init_Deck(){
            // TODO Write init_Deck
        }

        //shuffles deck
        private void shuffle_Deck(){
            // TODO Write shuffle_Deck
        }
    }
}

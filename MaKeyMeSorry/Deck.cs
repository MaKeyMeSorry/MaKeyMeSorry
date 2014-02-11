using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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

        public Deck(bool enableTesting)
        {
            curCard = 0;
            Debug.WriteLine("Intializing Deck");
            init_Deck();
            Debug.WriteLine("Deck Intialized. Printing contents");
            print_deck();
            Debug.WriteLine("Shuffling Deck");
            shuffle_Deck();
            Debug.WriteLine("Deck shuffled. Printing contents");
            print_deck();
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

        // Initializes a deck with 5 1s and 4 of everything else
        private void init_Deck(){
            int curIndex = 0;

            for (int i = 0; i < 4; i++)
            {
                Card oneCard = new Card(1);
                cards[curIndex++] = oneCard;

                Card twoCard = new Card(2);
                cards[curIndex++] = twoCard;

                Card threeCard = new Card(3);
                cards[curIndex++] = threeCard;

                Card fourCard = new Card(4);
                cards[curIndex++] = fourCard;

                Card fiveCard = new Card(5);
                cards[curIndex++] = fiveCard;

                Card sevenCard = new Card(7);
                cards[curIndex++] = sevenCard;

                Card eightCard = new Card(8);
                cards[curIndex++] = eightCard;

                Card tenCard = new Card(10);
                cards[curIndex++] = tenCard;

                Card elevenCard = new Card(11);
                cards[curIndex++] = elevenCard;

                Card twelveCard = new Card(12);
                cards[curIndex++] = twelveCard;

                Card sorryCard = new Card(13);
                cards[curIndex++] = sorryCard;
            }

            Card extraOneCard = new Card(1);
            cards[curIndex++] = extraOneCard;
        }


        // Shuffles the deck
        private void shuffle_Deck(){
            Card[] currentDeck = cards;
            cards = new Card[MAXCARDS];
            List<int> freeIndices = new List<int>(MAXCARDS);
            Random rand = new Random();

            for (int idx = 0; idx < MAXCARDS; idx++)
                freeIndices.Add(idx);

            foreach (Card card in currentDeck)
            {
                int indexOfNewIdx = rand.Next(freeIndices.Count);
                int newIdxOfCard = freeIndices[indexOfNewIdx];

                cards[newIdxOfCard] = card;
                freeIndices.Remove(newIdxOfCard);

            } 
        }

        private void print_deck()
        {
            for(int i=0; i<MAXCARDS; i++)
            {
                Debug.WriteLine("Card at index: {0} has the value: {1}", i, cards[i].get_value());
            }
        }

    }

}

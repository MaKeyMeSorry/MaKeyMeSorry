using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaKeyMeSorry
{
    class Game{

            // Holds the number of human players in game
            private int numHumanPlayers;

            // Make default constructor private so only 
            // the correct constructor is used.
            private Game();

            //helper function for get_move_options
            private Square get_move(Pawn pawn, Card card)
            {
                // TODO Write get_move
                return null;
            }

            // Could add getters and setters if we don't want
            // these to be public, but might be a pain to access then
            public List<Player> players;
            public Board board;
            public Deck deck;

            // Constructs the Game class with numHumans 
            // representing the number of human players.
            // Sets numHumanPlayers
            public Game(int numHumans)
            {
                // TODO Write Game constructor
            }

            public ~Game()
            {
                // TODO Write Game deconstructor
            }

            // Returns the player who has won the game.
            // Returns NULL if there currently is no winner
            public Player get_winner()
            {
                // TODO Write get_winner
                return null;
            }

            // Returns number of human players
            public int get_num_human_players()
            {
                // TODO Write get_num_human_players
                return -1;
            }

           //this should be the meat of the entire game i thinkkkk? haha 
            public Tuple<Pawn, List<Square>> get_move_options(Color player, Card card)
            {
                // TODO Write get_move_options
                return null;
            }
    }
}

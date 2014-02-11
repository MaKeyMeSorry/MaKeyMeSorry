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
            private Game() { }

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

            // Returns the player with the specified color
            public Player get_player(Color playerColor)
            {
                foreach(Player player in players) {
                    if (player.get_pawn_color() == playerColor) 
                    {
                        return player;
                    }
                }
                //error occurred
                return null;
            }

           //this should be the meat of the entire game i thinkkkk? haha 
            public List<Tuple<Pawn, List<Square>>> get_move_options(Color playerColor, Card card)
            {
                List<Tuple<Pawn, List<Square>>> allChoices = new List<Tuple<Pawn, List<Square>>>();
                List<Square> choices = new List<Square>();
                Player myPlayer = get_player(playerColor);
                
                // if it is a start card (1 or 2), then grab
                // the first pawn from start as a choice. if
                // this pawn is selected in UI then set to not at start
                if(card.get_start()) 
                {
                    if (myPlayer.get_pawn_from_start() != null)
                    {
                        choices.Add(board.get_square_at(board.get_start_square((int)playerColor) + card.can_move_forward()));
                    }
                    allChoices.Add(new Tuple<Pawn,List<Square>>(myPlayer.get_pawn_from_start(), choices));
                }
                
                foreach(Pawn pawn in myPlayer.get_active_pawns()) {
                    choices.Clear();

                    if(card.can_move_forward() != 0) {
                        // forward
                    }
                    if(card.can_move_backward() != 0) {
                        // backward
                    }
                    // swap and sorry combined because they used same code to grab all enemy pawns.
                    // differences only occur after the choice has been chosen on UI.
                    if(card.can_swap() || card.can_sorry())
                    {
                        foreach(Player player in players)
                        {
                            if(player != myPlayer)
                            {
                                foreach(Pawn enemyPawn in player.get_active_pawns())
                                {
                                    choices.Add(enemyPawn.get_current_location());
                           
                                    // if(card.can_sorry()) and if(card.can_swap()) 
                                    // implementations for actions after the choice has
                                    // been made on UI.
                                }
                            }
                        }
                    }
                    allChoices.Add(new Tuple<Pawn, List<Square>(pawn, choices));
                }
                return allChoices;
            }
    }
}

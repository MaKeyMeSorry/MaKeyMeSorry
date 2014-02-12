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
            public int MAXSQUARES = 60;

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
                int moveLocation;
                int startSquareIndex = board.get_start_square((int)playerColor);

                // TODO If a choice is slected then check if that spot is a slide and if you need 
                // to send any pawns on the slide to start and then move your pawn to end of the slide.

                // TODO add event triggers to each of the choices added with a certain type. After the event
                // is sletected, then more checks will occur and the UI will then be updated.
                
                // If it is a start card (1 or 2), then grab the first pawn from start as a
                // choice. If this pawn is selected in UI, then set to not at start for that pawn.
                if(card.get_start()) 
                {
                    if (myPlayer.get_pawn_from_start() != null)
                    {
                        moveLocation = (startSquareIndex + card.can_move_forward()) % MAXSQUARES;
                        if (board.get_square_at(moveLocation).can_place_pawn(myPlayer.get_pawn_from_start())) 
                        {
                            choices.Add(board.get_square_at(moveLocation));
                            allChoices.Add(new Tuple<Pawn, List<Square>>(myPlayer.get_pawn_from_start(), choices));
                        }
                    }
                }
                
                foreach(Pawn pawn in myPlayer.get_active_pawns()) {
                    choices.Clear();

                    
                    if(card.can_move_forward() != 0) {
                        bool pawnJumpedBoard = false;  //bool if the pawn jumped map like from 59 to 3
                        int homeConnect;
                        
                        moveLocation = (pawn.get_current_location().get_index() + card.can_move_forward()) % MAXSQUARES;
                        homeConnect = startSquareIndex - 2;

                        // Check to make sure that home connect isn't on the other side of board -- will not need if
                        // we set up board starting from the corners. Leaving in code for now.
                        if(homeConnect < 0) 
                        {
                            homeConnect += MAXSQUARES;
                        }

                        // If the forward move goes past your color home connect 
                        if(moveLocation > MAXSQUARES) {
                            pawnJumpedBoard = true;
                            moveLocation = moveLocation % MAXSQUARES;
                        }

                        // Check if the forward move jumped the array from 61 to 0. This will allow us 
                        if (pawn.get_current_location().get_index() > homeConnect && moveLocation > homeConnect)
                        {
                            if (pawnJumpedBoard)
                            {
                                // pawn passed homeConnect
                                if ((moveLocation - homeConnect) <= 6)
                                {
                                    choices.Add(board.get_my_base(playerColor)[moveLocation - homeConnect]);
                                }
                            }
                            else
                            {
                                //pawn didn't pass homeConnect
                                if (board.get_square_at(moveLocation).can_place_pawn(pawn))
                                {
                                    choices.Add(board.get_square_at(moveLocation));
                                }
                            }
                        } else if(pawn.get_current_location().get_index() <= homeConnect && moveLocation > homeConnect) {
                            //pawn passed homeConnect
                            if ((moveLocation - homeConnect) <= 6)
                            {
                                choices.Add(board.get_my_base(playerColor)[moveLocation - homeConnect]);
                            }
                        } else {
                            // normal move from space > homeConnect to space < homeConnect 
                            // or space < homeConnect to space < homeConnect
                            if (board.get_square_at(moveLocation).can_place_pawn(pawn))
                            {
                                choices.Add(board.get_square_at(moveLocation));
                            }
                        }


                    }

                    if(card.can_move_backward() != 0) {
                        moveLocation = pawn.get_current_location().get_index() - card.can_move_backward();
                        if (moveLocation < 0)
                        {
                            moveLocation += MAXSQUARES;
                        }
                        if (board.get_square_at(moveLocation).can_place_pawn(pawn))
                        {
                            choices.Add(board.get_square_at(moveLocation));
                        }
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
                                    // TODO Check if enemy pawn is in safe zone...inside home slide.
                                    choices.Add(enemyPawn.get_current_location());
                           
                                    // if(card.can_sorry()) and if(card.can_swap()) 
                                    // implementations for actions after the choice has
                                    // been made on UI.
                                }
                            }
                        }
                    }
                    allChoices.Add(new Tuple<Pawn, List<Square>>(pawn, choices));
                }
                return allChoices;
            }
    }
}

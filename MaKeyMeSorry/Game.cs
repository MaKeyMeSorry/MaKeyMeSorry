﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace MaKeyMeSorry
{
    public class Game
    {

        // Holds the number of human players in game
        private int numHumanPlayers;

        private int start_index;

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
        public bool forfeit_enabled;

        // Constructs the Game class with numHumans 
        // representing the number of human players.
        // Sets numHumanPlayers
        public Game(int numHumans, List<Player> playerList, int player1_index)
        {
            board = new Board();
            deck = new Deck(true);
            
            players = playerList;
            forfeit_enabled = false;
            numHumanPlayers = numHumans;

            start_index = player1_index;

            /*
            players = new List<Player>();
            Player redPlayer = new Player(Color.RED, true, true);
            Player bluePlayer = new Player(Color.BLUE, true, true);
            Player yellowPlayer = new Player(Color.YELLOW, true, true);
            Player greenPlayer = new Player(Color.GREEN, true, true);

            players.Add(redPlayer);
            players.Add(bluePlayer);
            players.Add(yellowPlayer);
            players.Add(greenPlayer);
            */
        }

        public int get_start_index()
        {
            return start_index;
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
            return numHumanPlayers;
        }

        // Returns the player with the specified color
        public Player get_player(Color playerColor)
        {
            foreach (Player player in players)
            {
                if (player.get_pawn_color() == playerColor)
                {
                    return player;
                }
            }
            //error occurred
            return null;
        }

        //this should be the meat of the entire game i thinkkkk? haha 
        public List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>> get_move_options(Color playerColor, Card card)
        {
            List<Tuple<Pawn, List<Tuple<Square, ComboData.move>>>> allChoices = new List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>>();
            List<Tuple<Square,ComboData.move>> choices = new List<Tuple<Square,ComboData.move>>();
            Player myPlayer = get_player(playerColor);
            int moveLocation;
            int startSquareIndex = board.get_start_square(playerColor);
            forfeit_enabled = false;

            // TODO If a choice is slected then check if that spot is a slide and if you need 
            // to send any pawns on the slide to start and then move your pawn to end of the slide.

            // TODO add event triggers to each of the choices added with a certain type. After the event
            // is sletected, then more checks will occur and the UI will then be updated.

            // If it is a start card (1 or 2), then grab the first pawn from start as a
            // choice. If this pawn is selected in UI
            choices.Clear();
            if (card.get_start() || card.can_sorry())
            {
                if (myPlayer.get_pawn_from_start() != null)
                {
                    if (card.can_sorry())
                    {
                        foreach (Player player in players)
                        {
                            if (player != myPlayer)
                            {
                                foreach (Pawn enemyPawn in player.get_active_pawns())
                                {
                                    if (!enemyPawn.is_in_safe_zone())
                                    {
                                        choices.Add(new Tuple<Square,ComboData.move>(enemyPawn.get_current_location(),ComboData.move.SORRY));
                                    }
                                    // TODO Check if enemy pawn is in safe zone...inside home slide.

                                    // if(card.can_sorry()) 
                                    //     add event trigger for this choice to a sorry event
                                    // if(card.can_swap())
                                    //     add event trigger for this choice to a swap event
                                    // implementations for actions after the choice has
                                    // been made on UI.
                                }
                            }
                        }
                        if (choices.Count != 0)
                        {
                            allChoices.Add(new Tuple<Pawn, List<Tuple<Square,ComboData.move>>>(myPlayer.get_pawn_from_start(), new List<Tuple<Square,ComboData.move>>(choices)));
                        }
                    }
                    else
                    {

                        moveLocation = (startSquareIndex + card.can_move_forward() - 1) % MAXSQUARES;
                        if (board.get_square_at(moveLocation).can_place_pawn(myPlayer.get_pawn_from_start()))
                        {
                            //choices.Add(board.get_square_at(moveLocation));
                            choices.Add(new Tuple<Square, ComboData.move>(board.get_square_at(moveLocation), ComboData.move.SORRY));
                            //allChoices.Add(new Tuple<Pawn, List<Square>>(myPlayer.get_pawn_from_start(), new List<Square>(choices)));
                            allChoices.Add(new Tuple<Pawn, List<Tuple<Square, ComboData.move>>>(myPlayer.get_pawn_from_start(), new List<Tuple<Square, ComboData.move>>(choices)));

                            //Debug.WriteLine("START CHOICE GO TO: " + choices.ElementAt(0).get_index());
                            /*foreach (Tuple<Pawn, List<Square>> pawnChoice in allChoices)
                            {
                                if (pawnChoice.Item1.is_start())
                                {
                                    Debug.WriteLine("Pawn ...location START");

                                }
                                else
                                {
                                    Debug.WriteLine("Pawn ...location" + pawnChoice.Item1.get_current_location().get_index());

                                }
                                foreach (Square testSquare in pawnChoice.Item2)
                                {
                                    Debug.WriteLine("square location" + testSquare.get_index());
                                }
                            }*/
                        }

                    }
                }
            }

            foreach (Pawn pawn in myPlayer.get_active_pawns())
            {
                choices.Clear();
                Debug.WriteLine("PAWNNNNNNNNNNNNAGE");

                if (pawn.is_in_safe_zone())
                {

                    if (card.can_move_forward() > 0 && card.can_move_forward() <= 5)
                    {
                        if ((pawn.get_current_location().get_index() % 6) + card.can_move_forward() < 6)
                        {
                            //if land at home don't check to add
                            if ((pawn.get_current_location().get_index() % 6) + card.can_move_forward() == 6)
                            {
                                //choices.Add(board.get_square_at(pawn.get_current_location().get_index() + card.can_move_forward()));
                                choices.Add(new Tuple<Square, ComboData.move>(board.get_square_at(pawn.get_current_location().get_index() + card.can_move_forward()), ComboData.move.FORWARD));


                            }
                            else
                            {
                                if (board.get_square_at(pawn.get_current_location().get_index() + card.can_move_forward()).can_place_pawn(pawn))
                                {
                                    //choices.Add(board.get_square_at(pawn.get_current_location().get_index() + card.can_move_forward()));
                                    choices.Add(new Tuple<Square, ComboData.move>(board.get_square_at(pawn.get_current_location().get_index() + card.can_move_forward()), ComboData.move.FORWARD));

                                }
                            }

                        }

                    } else if (card.can_move_backward() != 0)
                    {
                        moveLocation = pawn.get_current_location().get_index() - card.can_move_backward();
                        if(moveLocation < (60+(6*(int)playerColor)))
                        {
                            moveLocation = board.get_safe_square(playerColor) - (((60 + (6 * (int)playerColor)) - 1) - moveLocation);
                        }
                        if (moveLocation < 0)
                        {
                            moveLocation += MAXSQUARES;
                        }
                        if (board.get_square_at(moveLocation).can_place_pawn(pawn))
                        {
                            //choices.Add(board.get_square_at(moveLocation));
                            choices.Add(new Tuple<Square, ComboData.move>(board.get_square_at(moveLocation), ComboData.move.BACKWARD));
                        }
                    } else if(card.can_sorry())// || card.can_swap()) //add swap here
                    {
                        foreach (Player player in players)
                        {
                            if (player != myPlayer)
                            {
                                foreach (Pawn enemyPawn in player.get_active_pawns())
                                {
                                    if(!enemyPawn.is_in_safe_zone())
                                    {
                                        //choices.Add(enemyPawn.get_current_location());
                                        choices.Add(new Tuple<Square, ComboData.move>(enemyPawn.get_current_location(), ComboData.move.SORRY));

                                    }
                                    // TODO Check if enemy pawn is in safe zone...inside home slide.

                                    // if(card.can_sorry()) 
                                    //     add event trigger for this choice to a sorry event
                                    // if(card.can_swap())
                                    //     add event trigger for this choice to a swap event
                                    // implementations for actions after the choice has
                                    // been made on UI.
                                }
                            }
                        }
                    }
                    

                }
                else
                {
                    if (card.can_move_forward() != 0)
                    {
                        bool pawnJumpedBoard = false;  //bool if the pawn jumped map like from 59 to 3
                        int homeConnect;

                        if(card.can_move_forward() == 11)
                        {
                            forfeit_enabled = true;
                        }

                        moveLocation = (pawn.get_current_location().get_index() + card.can_move_forward());
                        homeConnect = startSquareIndex - 2;

                        // Check to make sure that home connect isn't on the other side of board -- will not need if
                        // we set up board starting from the corners. Leaving in code for now.
                        if (homeConnect < 0)
                        {
                            homeConnect += MAXSQUARES;
                        }

                        // If the forward move goes past your color home connect 
                        if (moveLocation >= MAXSQUARES)
                        {
                            pawnJumpedBoard = true;
                            moveLocation = moveLocation % MAXSQUARES;
                        }

                        // Check if the forward move jumped the array from 61 to 0. This will allow us 
                        if (pawn.get_current_location().get_index() > homeConnect && moveLocation > homeConnect)
                        {
                            Debug.WriteLine("before pawn jump check ");
                            if (pawnJumpedBoard)
                            {

                                // pawn passed homeConnect
                                if ((moveLocation - homeConnect) <= 6)
                                {
                                    Debug.WriteLine("passed home connect 1 ");

                                    if (board.get_square_at((((int)pawn.get_color()) * 6) + 59 + (moveLocation - homeConnect)).can_place_pawn(pawn))
                                    {
                                        if(card.can_move_forward() == 11)
                                        {
                                            forfeit_enabled = false;
                                        }
                                        //choices.Add(board.get_square_at((((int)pawn.get_color()) * 6) + 59 + (moveLocation - homeConnect)));
                                        choices.Add(new Tuple<Square, ComboData.move>(board.get_square_at((((int)pawn.get_color()) * 6) + 59 + (moveLocation - homeConnect)), ComboData.move.FORWARD));

                                    }
                                    //choices.Add(board.get_my_base(playerColor)[moveLocation - homeConnect]);
                                    //used bottom for testing...top is correct once we implement the home connector squares
                                    //choices.Add(board.get_square_at(board.get_start_square(playerColor)));
                                }
                            }
                            else
                            {
                                //pawn didn't pass homeConnect
                                Debug.WriteLine("didn't passed home connect 1 ");

                                if (board.get_square_at(moveLocation).can_place_pawn(pawn))
                                {
                                    if (card.can_move_forward() == 11)
                                    {
                                        forfeit_enabled = false;
                                    }
                                    //choices.Add(board.get_square_at(moveLocation));
                                    choices.Add(new Tuple<Square, ComboData.move>(board.get_square_at(moveLocation), ComboData.move.FORWARD));

                                }
                            }
                        }
                        else if (pawn.get_current_location().get_index() <= homeConnect && moveLocation > homeConnect)
                        {
                            //pawn passed homeConnect
                            Debug.WriteLine("passed home connect 2 ");

                            if ((moveLocation - homeConnect) <= 6)
                            {
                                Debug.WriteLine("passed home connect 2 ");

                                if (board.get_square_at((((int)pawn.get_color()) * 6) + 59 + (moveLocation - homeConnect)).can_place_pawn(pawn))
                                {
                                    if (card.can_move_forward() == 11)
                                    {
                                        forfeit_enabled = false;
                                    }
                                    //choices.Add(board.get_square_at((((int)pawn.get_color()) * 6) + 59 + (moveLocation - homeConnect)));
                                    choices.Add(new Tuple<Square, ComboData.move>(board.get_square_at((((int)pawn.get_color()) * 6) + 59 + (moveLocation - homeConnect)), ComboData.move.FORWARD));

                                }
                                //choices.Add(board.get_my_base(playerColor)[moveLocation - homeConnect]);
                                //used bottom for testing...top is correct once we implement the home connector squares
                                //choices.Add(board.get_square_at(board.get_start_square(playerColor)));
                            }
                        }
                        else
                        {
                            // normal move from space > homeConnect to space < homeConnect 
                            // or space < homeConnect to space < homeConnect
                            Debug.WriteLine("normal move ");

                            if (board.get_square_at(moveLocation).can_place_pawn(pawn))
                            {
                                if (card.can_move_forward() == 11)
                                {
                                    forfeit_enabled = false;
                                }
                                //choices.Add(board.get_square_at(moveLocation));
                                choices.Add(new Tuple<Square, ComboData.move>(board.get_square_at(moveLocation), ComboData.move.FORWARD));

                            }
                        }


                    }

                    if (card.can_move_backward() != 0)
                    {
                        moveLocation = pawn.get_current_location().get_index() - card.can_move_backward();
                        if (moveLocation < 0)
                        {
                            moveLocation += MAXSQUARES;
                        }
                        if (board.get_square_at(moveLocation).can_place_pawn(pawn))
                        {
                            //choices.Add(board.get_square_at(moveLocation));
                            choices.Add(new Tuple<Square, ComboData.move>(board.get_square_at(moveLocation), ComboData.move.FORWARD));

                        }
                    }

                    // swap and sorry combined because they used same code to grab all enemy pawns.
                    // differences only occur after the choice has been chosen on UI.
                    if (card.can_sorry())// || card.can_swap()) //add swap here
                    {
                        foreach (Player player in players)
                        {
                            if (player != myPlayer)
                            {
                                foreach (Pawn enemyPawn in player.get_active_pawns())
                                {
                                    if(!enemyPawn.is_in_safe_zone())
                                    {
                                        //choices.Add(enemyPawn.get_current_location());
                                        choices.Add(new Tuple<Square, ComboData.move>(enemyPawn.get_current_location(), ComboData.move.SORRY));
                                    }
                                    // TODO Check if enemy pawn is in safe zone...inside home slide.

                                    // if(card.can_sorry()) 
                                    //     add event trigger for this choice to a sorry event
                                    // if(card.can_swap())
                                    //     add event trigger for this choice to a swap event
                                    // implementations for actions after the choice has
                                    // been made on UI.
                                }
                            }
                        }
                    }
                    if (card.can_swap())
                    {
                        foreach (Player player in players)
                        {
                            if (player != myPlayer)
                            {
                                foreach (Pawn enemyPawn in player.get_active_pawns())
                                {
                                    if (!enemyPawn.is_in_safe_zone())
                                    {
                                        //choices.Add(enemyPawn.get_current_location());
                                        choices.Add(new Tuple<Square, ComboData.move>(enemyPawn.get_current_location(), ComboData.move.SWAP));

                                    }
                                }
                            }
                        }
                    }
                }

                if(choices.Count != 0)
                {
                    //allChoices.Add(new Tuple<Pawn, List<Square>>(pawn, new List<Square>(choices)));
                    allChoices.Add(new Tuple<Pawn, List<Tuple<Square, ComboData.move>>>(pawn, new List<Tuple<Square, ComboData.move>>(choices)));

                }

            }
            /*foreach (Tuple<Pawn, List<Square>> pawnChoice in allChoices)
            {
                if (pawnChoice.Item1.is_start())
                {
                    Debug.WriteLine("Pawn ...location START");

                }
                else
                {
                    Debug.WriteLine("Pawn ...location " + pawnChoice.Item1.get_current_location().get_index());

                }
                foreach (Square testSquare in pawnChoice.Item2)
                {
                    Debug.WriteLine("square location " + testSquare.get_index());
                }
            }*/
            return allChoices;
        }
    }
}

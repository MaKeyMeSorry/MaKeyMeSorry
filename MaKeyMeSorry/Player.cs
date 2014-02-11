using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaKeyMeSorry
{
    class Player {

       private Color pawnColor;
       private int numPawnsLeft;
       private List<Pawn> pawns; 
       // Used for distinction between normal player and AI
       private bool isHuman;

       // Ensures default constructor cannot be used
       private Player() { }


       // Constructs Player class with the color specified
       // and if the player is a bot or not
       public Player(Color color, bool isHuman)
       {
           // TODO Write Player Constructor
       }

       // Getters
       public int get_num_pawns_at_start()
       {
           // TODO Write get_num_pawns_at_start
           return -1;
       }

       public int get_Num_Pawns_at_home()
       {
           // TODO Write get_num_pawns_at_home
           return -1;
       }

       //returns NULL if no pawns are at start
       public Pawn get_pawn_from_start()
       {
           // TODO Write get_pawn_from_start
           return null;
       }

       //return empty vector if none active
       public List<Pawn> get_active_pawns()
       {
           // TODO Write get_active_pawns
           return null;
       }

       public Color get_pawn_color()
       {
           return pawnColor;
       }

       public bool get_is_human()
       {
           return isHuman;
       }
    }
}

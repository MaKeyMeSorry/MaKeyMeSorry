using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace MaKeyMeSorry
{
    public class Player
    {

        private Color pawnColor;
        private int numPawnsLeft;
        private List<Pawn> pawns;
        // Used for distinction between normal player and AI
        private bool isHuman;

        // Holds players name
        string name;

        // Ensures default constructor cannot be used
        private Player() { }


        // Constructs Player class with the color specified
        // and if the player is a bot or not
        public Player(string name, Color pawnColor, bool isHuman, bool enableTesting)
        {
            this.pawnColor = pawnColor;
            this.isHuman = isHuman;
            this.name = name;
            setNumPawnsLeft(5);
            pawns = new List<Pawn>();
            for (int i = 0; i < 4; i++)
            {
                pawns.Add(new Pawn(pawnColor, null, i));
            }
            if (enableTesting)
            {
                Debug.WriteLine("Number of pawns at start: " + get_num_pawns_at_start().ToString());
                Debug.WriteLine("Player Color: " + pawnColor);
                Debug.WriteLine("Player Name: " + name);
                Debug.WriteLine("Player is human: " + isHuman.ToString());

            }
        }

        // Getters
        public string get_player_name()
        {
            return name;
        }


        public int get_num_pawns_at_start()
        {
            int numAtStart = 0;
            foreach (Pawn pawn in pawns)
            {
                if (pawn.is_start())
                {
                    numAtStart++;
                }
            }
            return numAtStart;
        }

        public int get_Num_Pawns_at_home()
        {
            int numAtHome = 0;
            foreach (Pawn pawn in pawns)
            {
                if (pawn.is_home())
                {
                    numAtHome++;
                }
            }
            return numAtHome;
        }

        // returns NULL if no pawns are at start
        // NOTE: game should make call to update this pawn to is_active if
        // this pawn was chosen to move from UI
        public Pawn get_pawn_from_start()
        {
            foreach (Pawn pawn in pawns)
            {
                if (pawn.is_start())
                {
                    return pawn;
                }
            }
            return null;
        }

        //return empty vector if none active
        public List<Pawn> get_active_pawns()
        {
            List<Pawn> activePawns = new List<Pawn>();
            foreach (Pawn pawn in pawns)
            {
                if (!pawn.is_start() && !pawn.is_home())
                {
                    activePawns.Add(pawn);
                }
            }
            return activePawns;
        }

        public Color get_pawn_color()
        {
            return pawnColor;
        }

        public bool get_is_human()
        {
            return isHuman;
        }

        public void setNumPawnsLeft(int numPawnsLeft)
        {
            this.numPawnsLeft = numPawnsLeft;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elphysics
{
    class BusyInGame : DBase<BusyInGame>
    {
        private Guid Player_Guid;
        public Player Player
        {
            get { return Player.GetItemById(Player_Guid); }
            set { Player_Guid = value.GUID; }
        }
        private Guid Game_Guid;
        public Game Game
        {
            get { return Game.GetItemById(Game_Guid); }
            set { Game_Guid = value.GUID; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elphysics
{
    public class Player : DBase<Player>
    {

        private string _Name;
        private short _Balls_in_pockets;

        // Вернем всех игроков, которые играют в текущей игре.
        private Guid game_guid;
        public Game Game
        {
            get { return Game.GetItemById(game_guid); }
            set { game_guid = value.GUID; }
        }

        public static List<Player> GetPlayersInCurrentGame(Game game)
        {
            var temp = new List<Player>();
            foreach (var player in MyItems)
                if (player.Game == game)
                    temp.Add(player);
            return temp;
        }

        // Вернем все ходы, которые совершил игрок
        public List<Turn> GetTurns
        {
            get
            {
                List<Turn> temp = new List<Turn>();
                foreach(var t in Turn.MyItems)
                {
                    if (t.Player == this)
                        temp.Add(t);
                }
                return temp;
            }
        }

        public Player(String name)
        {
            Name = name;
        }

        public string Name
        {
            get {  return _Name; }
            set { _Name = value; }
        }

        public short Balls_in_pockets
        {
            get { return _Balls_in_pockets; }
            set { _Balls_in_pockets = value; }
        }

    }
}

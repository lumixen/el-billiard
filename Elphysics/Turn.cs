using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elphysics
{
    public class Turn : DBase<Turn>
    {
        private Guid _Player;
        private double _angle;
        private int _force;

        public Turn()
        {
            _angle = 0;
            _force = 0;
        }
        public Player Player
        {
            get { return Player.GetItemById(_Player); }
            set { _Player = value.GUID; }
        }
        private Guid _Game;

        public Game Game
        {
            get { return Game.GetItemById(_Game); }
            set { _Game = value.GUID; }
        }

        public double angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        public int force
        {
            get { return _force; }
            set { _force = value; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elphysics
{
    public class ElEventArgs: EventArgs
    {
        public enum result_status { _HasWhiteFault, _HasBlackFault, _StepHasFinished, _BallKicked, NoBallsKickedFault, WinInGame };
        private result_status _current_status;

        public result_status current_status
        {
            set { _current_status = value; }
            get { return _current_status; }
        }


    }
}

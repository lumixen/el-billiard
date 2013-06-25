using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElControls
{
    public class ElPowerChanged : EventArgs
    {
        private int _Value;

        public int Value
        {
           get { return _Value; }
           set { _Value = value; }
        }
    }
}

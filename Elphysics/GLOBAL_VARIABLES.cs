using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Elphysics
{
    class GLOBAL_VARIABLES
    {
        private double _friction_k = 0.011D;
        private double _BALL_RADIUS;
        private byte _Number_Balls_American = 9;
        private Point[] _Pockets = {new Point(5,5), new Point(347,0), new Point(675,8), new Point(8,413), new Point(348, 415), new Point(673, 410)};
        private List<Point> _Ball_American_Positions = new List<Point>
        {   
            new Point(50, 150),
            new Point(50, 180),
            new Point(50, 210),
            new Point(50, 240),
            new Point(80, 165),
            new Point(80, 195),
            new Point(80, 225),
            new Point(110, 195),
            new Point(400,195) 
        };
        private CBall.BType[] _Ball_American_Types =
        {   CBall.BType.RED,
            CBall.BType.RED,
            CBall.BType.RED,
            CBall.BType.BLACK,
            CBall.BType.RED,
            CBall.BType.RED,
            CBall.BType.RED,
            CBall.BType.RED,
            CBall.BType.WHITE 
        };
        
        public bool HasWelts;


        public GLOBAL_VARIABLES()
        {
            if (File.Exists("StartPositions.txt"))
            {
                _Ball_American_Positions.Clear();
                using (StreamReader rf = File.OpenText("StartPositions.txt"))
                {
                    string temp = "";
                    Regex separator = new Regex(",");
                    while ((temp = rf.ReadLine()) != null)
                    {
                        string[] points = separator.Split(temp);
                        _Ball_American_Positions.Add(new Point(Convert.ToInt32(points[0]), Convert.ToInt32(points[1])));
                    }
                }
            }
        }

        public Point[] Pockets
        {
            get { return _Pockets; }
            set { this.Pockets = value; }
        }

        public List<Point> Ball_American_Positions
        {
            get { return _Ball_American_Positions; }
            set { this._Ball_American_Positions = value; }
        }

        public CBall.BType[] Ball_American_Types
        {
            get { return _Ball_American_Types; }
            set { this._Ball_American_Types = value; }
        }

        public byte Number_Balls_American
        {
            get { return _Number_Balls_American; }
            set { _Number_Balls_American = value; }
        }

        public double Friction_k
        {
            get { return _friction_k; }
            set { _friction_k = value; }
        }

        public double BALL_RADIUS
        {
            get { return _BALL_RADIUS; }
            set { _BALL_RADIUS = value; }
        }
    }
}

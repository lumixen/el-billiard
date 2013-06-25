using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elphysics
{
    public class CWall
    {
          public enum E_Wall { NONE, X1, Y1, X2, Y2 };

          private  double ix1;
          private  double ix2;
          private  double iy1;
          private  double iy2;

          public CWall()
          {
              ix1 = ix2 = iy1 = iy2 = 0.0f;
          }

          public CWall(double sx1, double sy1, double sx2, double sy2)
          {
              ix1 = sx1;
              iy1 = sy1;
              ix2 = sx2;
              iy2 = sy2;
          }

          public double X1
          {
              get { return this.ix1; }
              private set { this.ix1 = value; }
          }
          public double Y1
          {
              get { return this.iy1; }
              private set { this.iy1 = value; }
          }
          public double X2
          {
              get { return this.ix2; }
              private set { this.ix2 = value; }
          }
          public double Y2
          {
              get { return this.iy2; }
              private set { this.iy2 = value; }
          }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elphysics
{
    public class CollisionSolveFunctions
    {

        private double square(double m)
        {
            return m * m;
        }
        public CCollision FindTimeUntilBallColidesWithWall(CBall b, CWall w)
        {
            // set local variables
            double timeToCollision = 0.0D;
            CWall.E_Wall wWall = CWall.E_Wall.NONE;
            CCollision clsn = new CCollision();
            // Collison with right wall
            if (b.VX < 0.0D)
            {
                double t = (w.X1 + b.R - b.X) / b.VX;
                if (t < 0.0D)
                {
                    timeToCollision = t;
                    wWall = CWall.E_Wall.X1;
                }
            }
            // Collision with wall Y1
            if (b.VY < 0.0D)
            {
                double t = (b.R - b.Y + w.Y1) / b.VY;
                if (t < 0.0D)
                {
                    if ((wWall == CWall.E_Wall.NONE) || t < timeToCollision)
                    {
                        timeToCollision = t;
                        wWall = CWall.E_Wall.Y1;
                    }
                }
            }
            // Collision with wall X2
            if (b.VX > 0.0D)
            {
                double t = (w.X2 - b.R - b.X) / b.VX;
                if (t < 0.0D)
                {
                    if ((wWall == CWall.E_Wall.NONE) || (t < timeToCollision))
                    {
                        timeToCollision = t;
                        wWall = CWall.E_Wall.X2;

                    }
                }
            }
            // Collision with wall Y2
            if (b.VY > 0.0D)
            {
                double t = (w.Y2 - b.R - b.Y) / b.VY;
                if (t < 0.0D)
                {
                    if (wWall == CWall.E_Wall.NONE || (t < timeToCollision))
                    {
                        timeToCollision = t;
                        wWall = CWall.E_Wall.Y2;
                    }
                }
            }
            if (wWall != CWall.E_Wall.NONE)
            {
                clsn.SetCollisionWithWall(wWall, timeToCollision);
                clsn.b1 = b;
            }
            return clsn;
        }

        public CCollision FindTimeUntilBallCollidesWithBall(CBall b1, CBall b2)
        {
            CCollision clsn = new CCollision();
            if (square(b1.X - b2.X) + square(b1.Y - b2.Y) < square(b1.R + b2.R))        // Detect collision
            {                                                                           // If detect count time, util collision was detected
                //Coeff a
                double a = square((b1.VX - b2.VX)) + square((b1.VY - b2.VY));
                // Coeff b
                double b = 2.0 * (b1.X - b2.X) * (b1.VX - b2.VX) + (b1.Y - b2.Y) * (b1.VY - b2.VY);
                // Coeff c
                double c = square(b1.X - b2.X) + square(b1.Y - b2.Y) - square(b1.R + b2.R);

                double det = square(b) - 4.0D * a * c;
                if (!(det < 0))
                {
                    if (a != 0.0D)
                    {
                        double t = (-b - Math.Sqrt(det)) / (2.0D * a);
                        clsn.b1 = b1;
                        clsn.b2 = b2;
                        clsn.SetCollisionWithBall(t);
                    }
                }
            }
            return clsn;
        }

        // Calculate Collision with wall

        public void doCollisionWithWall(CCollision collision)
        {
            switch (collision.getCollisonWall())
            {
                case (CWall.E_Wall.X1):
                    collision.b1.VX = Math.Abs(collision.b1.VX);
                    break;
                case (CWall.E_Wall.Y1):
                    collision.b1.VY = Math.Abs(collision.b1.VY);
                    break;
                case (CWall.E_Wall.X2):
                    collision.b1.VX = -(Math.Abs(collision.b1.VX));
                    break;
                case (CWall.E_Wall.Y2):
                    collision.b1.VY = -(Math.Abs(collision.b1.VY));
                    break;
            }
        }

        // ---------- Calculate new velocities vectors ------------ //
 
        public void doCollisionWithTwoBalls(CCollision collision)
        {
            if ((collision.b1.M == 0.0D) && (collision.b2.M == 0.0D)) return;

            Vector2D v_n = collision.b2.XY - collision.b1.XY;
            Vector2D v_un = v_n.unitVector();
            Vector2D v_ut = new Vector2D(-v_un.Y, v_un.X);

            double v1n = v_un * collision.b1.VXY;
            double v1t = v_ut * collision.b1.VXY;
            double v2n = v_un * collision.b2.VXY;
            double v2t = v_ut * collision.b2.VXY;

            double v1tPrime = v1t;
            double v2tPrime = v2t;

            double v1nPrime = (v1n * (collision.b1.M - collision.b2.M) + 2.0D * collision.b2.M * v2n) / (collision.b1.M + collision.b2.M);
            double v2nPrime = (v2n * (collision.b2.M - collision.b1.M) + 2.0D * collision.b1.M * v1n) / (collision.b1.M + collision.b2.M);

            // Calculate new vectors

            Vector2D v_v1nPrime = new Vector2D(v1nPrime * v_un);
            Vector2D v_v1tPrime = new Vector2D(v1tPrime * v_ut);
            Vector2D v_v2nPrime = new Vector2D(v2nPrime * v_un);
            Vector2D v_v2tPrime = new Vector2D(v2tPrime * v_ut);

            // Calculate new velocities

            collision.b1.VX = v_v1nPrime.X + v_v1tPrime.X;
            collision.b1.VY = v_v1nPrime.Y + v_v1tPrime.Y;
            collision.b2.VX = v_v2nPrime.X + v_v2tPrime.X;
            collision.b2.VY = v_v2nPrime.Y + v_v2tPrime.Y;
        }

        public double distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public bool intersect_collision(double X1, double Y1, double X2, double Y2, double R1, double R2)
        {
            if (square(X1 - X2) + square(Y1 - Y2) < square(R1 + R2))
                return true;
            return false;
        }

    }
}

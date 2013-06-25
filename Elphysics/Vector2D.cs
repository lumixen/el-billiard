using System;
using System.Collections.Generic;
using System.Linq;

namespace Elphysics
{
    public struct Vector2D
    {
        //public members (variables)
        public double X;
        public double Y;

        public Vector2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2D(Vector2D n)
        {
            this.X = n.X;
            this.Y = n.Y;
        }

        private double magnitude()
        {
            return Math.Sqrt(this.X*this.X + this.Y*this.Y);
        }

        public Vector2D unitVector()
        {
            double mag = this.magnitude();
            if (mag != 0.0D)
            {
                return (new Vector2D(this.X / mag, this.Y / mag));
            }
            else 
                return  (new Vector2D());
        }

        public static Vector2D operator +(Vector2D left, Vector2D right)
        {
            Vector2D result;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            return result;
        }
        public static Vector2D operator +(Vector2D left, double _val)
        {
            Vector2D result;
            result.X = left.X + _val;
            result.Y = left.Y + _val;
            return result;
        }

        public static Vector2D operator -(Vector2D left, Vector2D right)
        {
            return (new Vector2D(left.X - right.X, left.Y - right.Y));
        }

        public static double operator *(Vector2D left, Vector2D right)
        {
            return (left.X * right.X + left.Y * right.Y);
        }

        // Multiplication by a scalar
        public static Vector2D operator *(Vector2D left, double c)
        {
            return (new Vector2D(left.X * c, left.Y * c));
        }

        public static Vector2D operator *(double c, Vector2D left)
        {
            return (left*c);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", this.X, this.Y);
        }
    }
}

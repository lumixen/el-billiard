using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Elphysics
{
    [Serializable]
    public class CBall : DBase<CBall>
    {
        // Возвращаем все коллизии, в которых участвовал шар
        public List<CCollision> Collisions
        {
            get
            {
                var res = new List<CCollision>();
                foreach (var b in BusyInCollision.MyItems)
                    if (b.CBall == this)
                        res.Add(b.CCollision);
                return res;
            }
        }

        public CBall()
        {
            this.mass = 0;
            this.radius = 0;
        }

        public CBall(Vector2D _position, Vector2D _velocity, double _mass, double _radius, BType _ball_type)
        {
            this.position = _position;
            this.velocity = _velocity;
            this.mass = _mass;
            this.radius = _radius;
            this.ball_type = _ball_type;
        }


        // Get & Set Methods

        public double X        { get { return position.X; }
                                set { this.position.X = value; }
                              }
        public double Y        { get { return position.Y; }
                                set { this.position.Y = value; } 
                              }
        public double VX       { get { return velocity.X; }
                                set { this.velocity.X = value; }
                              }
        public double VY       { get { return velocity.Y; }
                                set { this.velocity.Y = value; }
                              }
        public double M     { get { return this.mass; }
                         set { this.mass = value; }
                       }
        public double R     { get { return this.radius; }
                         set { this.radius = value; } 
                            }
        public BType Type     { get { return this.ball_type; }
                            set { this.ball_type = value; }
                            }
        public Vector2D XY   { get { return position; }       // set X & Y at the same time
                             set { this.position = value; }
                           }
        public Vector2D VXY  { get { return velocity; }       // set VX & VY at the same time
                             set { this.velocity = value; }
                             }
        

        // int - type of ball
        // 0 - means White ball
        // 1 - black ball
        // 2 - red ball
        // 3 - yellow ball
        public void SetBallType(BType type)
        {
            ball_type = type;
        }

        public bool IsBallWhite() { return ball_type == BType.WHITE; }
        public bool IsBallBlack() { return ball_type == BType.BLACK; } 
          
        public void UpdateBallPosition(double dt)
        {
            this.X = this.X + this.VX * dt; 
            this.Y = this.Y + this.VY * dt;
        }

        public void ApplyForce(double angle, double force)
        {
            this.VX = force * (Math.Cos(angle));
            this.VY = force * (Math.Sin(angle));
        }

        public void ClearVelocity()
        {
            this.VX = 0.0D;
            this.VY = 0.0D;
        }

        // ----- Variables ----- //
        [NonSerialized()]private Vector2D position;
        [NonSerialized()]private Vector2D velocity;
        double mass;
        double radius;
        private BType ball_type;
        public enum BType { WHITE, BLUE, RED, BLACK, WHITE_TRANSPARENT };
    }
}

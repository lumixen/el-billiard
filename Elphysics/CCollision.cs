using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elphysics
{
    public class CCollision : DBase<CCollision>
    {

        // ------------------------------------------------- //

        private CBall _b1;
        private CBall _b2;

        // --------------------------- //

        private enum Type { NONE, WALL, BALL, POCKET };
        private Type collisionType;

        private CWall.E_Wall Collision_Wall;
        private double timeToCollision;

        public CCollision() 
        { 
            this.timeToCollision = 0.0D; 
            this.collisionType = Type.NONE;
        }

        public void SetCollisionWithWall(CWall.E_Wall w, double t)
        {
            Collision_Wall  = w;
            timeToCollision = t;
            collisionType   = Type.WALL;
        }

        public void SetCollisionWithBall(double t)
        {
            timeToCollision = t;
            collisionType   = Type.BALL;
            Collision_Wall  = CWall.E_Wall.NONE;
        }

        public void SetCollisionWithPocket()
        {
            collisionType   = Type.POCKET;
        }

        public void resetState()
        {
            collisionType   = Type.NONE;
            Collision_Wall  = CWall.E_Wall.NONE;
            timeToCollision = 0;
        }

        public CBall b1
        {
            get { return _b1; }
            set { _b1 = value; }
        }

        public CBall b2
        {
            get { return _b2; }
            set { _b2 = value; }
        }

        public bool ballHasNoCollision()        { return collisionType == Type.NONE; }
        public bool ballHasCollisionWithWall()  { return collisionType == Type.WALL; }
        public bool ballHasCollisonWithBall()   { return collisionType == Type.BALL; }
        public bool ballHasCollisionWithPocket() { return collisionType == Type.POCKET; }
       
        public CWall.E_Wall getCollisonWall()  { return Collision_Wall; }
        public double getTimeToCollision()     { return timeToCollision; }

    }
}

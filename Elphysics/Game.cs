using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;

namespace Elphysics
{
    public class Game : DBase<Game>
    {
        public event EventHandler<ElEventArgs> El_Event;
        private GLOBAL_VARIABLES GLOBAL     = new GLOBAL_VARIABLES();
        private List<CBall> balls;      //all balls
        private CWall Walls;
        private Turn turn;
        private byte balls_num;
        public enum GameStatus { InGame, NotInGame, Paused };
        private GameStatus _gameStatus;
        // Functions classes
        private CollisionSolveFunctions solve = new CollisionSolveFunctions();

        bool no_collisions = true;
        bool DemoMode      = false;
        // ------ Get methods --------- //

        // Возвращает все игры, в которых участвует игрок.

        public List<Game> PlayerInGames(Player player)
        {
                List<Game> temp = new List<Game>();
                foreach (var g in BusyInGame.MyItems)
                    if (g.Player == player)
                        temp.Add(g.Game);
                return temp;
        }

        public List<Player> PlayersInGame
        {
            get
            {
                List<Player> temp = new List<Player>();
                foreach (var g in BusyInGame.MyItems)
                    if (g.Game == this)
                        temp.Add(g.Player);
                return temp;
            }
        }

        public CBall getBallById(byte i)
        {
            return balls[i];
        }

        public CBall getBallByType(CBall.BType _type)
        {
            for (byte i = 0; i < balls_num; i++)
            {
                if (getBallById(i).Type == _type)
                    return getBallById(i);
            }
            return null;
        }

        public byte getBallCount { get { return this.balls_num; } }

        // Constructor
        public Game(bool _demo_mode)
        {
            DemoMode = _demo_mode;
            balls = new List<CBall>();
            this.balls_num = 0;
            gameStatus = GameStatus.NotInGame;
        }

        // public set methods;

        public void GenerateAmericanBilliard(double BALL_DIAMETER)
        {
            GLOBAL.BALL_RADIUS = BALL_DIAMETER / 2;
            for (byte i = 0; i < GLOBAL.Ball_American_Positions.Count; i++)
            {
                this.addBall(new CBall(new Vector2D(GLOBAL.Ball_American_Positions[i].X, GLOBAL.Ball_American_Positions[i].Y), new Vector2D(), 12.0D, BALL_DIAMETER - 8.0D, GLOBAL.Ball_American_Types[i]));
            }
        }
        public void DeleteCurrentGame()
        {
            balls.Clear();
            balls_num = 0;
            turn = null;
        }
        public GameStatus gameStatus
        {
            get { return _gameStatus; }
            set { _gameStatus = value; }
        }

        // ----- Game Events ------- //
        public void SetEvent(ElEventArgs events)
        {
            EventHandler<ElEventArgs> temp = El_Event;
            if (temp != null)
                temp(this, events);
        }

        public void HitCue(Turn _turn)
        {
            turn = _turn;
            this.getBallByType(CBall.BType.WHITE).ApplyForce(turn.angle + Math.PI, turn.force);
        }


        public void addWalls(CWall w)
        {
            GLOBAL.HasWelts = true;
            this.moveWalls(w);
        }

        public void addBall(CBall newBall)
        {
            balls.Add(newBall);
            this.balls_num++;     //balls.Count;
        }
        public Point CalculatePreCollision(Point[] p, double angle, double cue_x_center, double cue_y_center, Size size)
        {
                    double _cue_line_k = -Math.Tan(angle);
                    bool k_negative = false;
                    if (_cue_line_k < 0)
                        k_negative = true;
                    // А - числитель
                    double A = Math.Abs(_cue_line_k) % 1;
                    A = A * 1e16;
                    // В - знаменатель
                    double B = 1e16;
                    // N - целая часть числа
                    int N  = (int)_cue_line_k;
                    if (k_negative)
                    {
                        A = -A;
                    }
                    A = (N * B) + A;
                    // Определение коеффициента С. Для того, что бы уравнение прямой соответствовало рисунку, 
                    // необходимо прибавить также радиус (cue_y_center, cue_x_center - координаты левого верхнего
                    // угла прямоугольника, который  впоследствии применяю для создания круга.
                    double C =  (-B * (cue_y_center + GLOBAL.BALL_RADIUS) - A * (cue_x_center + GLOBAL.BALL_RADIUS));
                    // Пересечение с рамками.
                    int X1 = (int)-((C + B * 10) / A);
                    int X2 = (int)-((C + B * size.Height) / A);
                    int Y1 = (int)-((C + A * 10) / B);
                    int Y2 = (int)-((C + A * size.Width) / B);
                    p[0].X = X1; p[0].Y = Y1; p[1].X = X2; p[1].Y = Y2;
                    int temp_x = Int32.MaxValue, temp_y = Int32.MaxValue;
                    double h = Double.PositiveInfinity;  
                    // Процесс нахождения расстояния от центра шара до траектории полета битка
                    for (byte i = 0; i < getBallCount; i++)
                    {
                        double temp_h;
                        double temp_distance;
                        double current_distance = Double.PositiveInfinity;
                        if (getBallById(i).Type != CBall.BType.WHITE)
                        {
                            temp_h =  Math.Abs(A * (getBallById(i).X + GLOBAL.BALL_RADIUS) + B * (getBallById(i).Y + GLOBAL.BALL_RADIUS) + C) / Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2));
                            temp_distance = solve.distance(getBallById(i).X + GLOBAL.BALL_RADIUS, getBallById(i).Y + GLOBAL.BALL_RADIUS, cue_x_center + GLOBAL.BALL_RADIUS, cue_y_center + GLOBAL.BALL_RADIUS);
                            if ((temp_h < GLOBAL.BALL_RADIUS) && (temp_distance < current_distance))
                            {
                                current_distance = temp_distance;
                                h = temp_h;
                                temp_x = (int)(getBallById(i).X + GLOBAL.BALL_RADIUS);
                                temp_y = (int)(getBallById(i).Y + GLOBAL.BALL_RADIUS);
                            }
                        }
                    }
                    // Находим точки соприкосновения шара и линии удара битка (ордината)
                    if (h != Double.PositiveInfinity)
                    {
                        double a = (1 + Math.Pow((B / A), 2));
                        double b = (2 * B * (A * temp_x + C) / (Math.Pow(A, 2)) - 2 * temp_y);
                        double c = Math.Pow((C / A), 2) + (2 * temp_x * C / A) - Math.Pow((GLOBAL.BALL_RADIUS), 2) + Math.Pow(temp_x, 2) + Math.Pow(temp_y, 2);
                        double det = Math.Sqrt(Math.Pow(b, 2) - 4.0D * a * c);
                        double point_y1 = (-b + det) / (2.0D * a);
                        double point_y2 = (-b - det) / (2.0D * a);
                        double point_x1 = -((B * point_y1 + C) / A);
                        double point_x2 = -((B * point_y2 + C) / A);
                        if (solve.distance(point_x1, point_y1, cue_x_center + GLOBAL.BALL_RADIUS, cue_y_center + GLOBAL.BALL_RADIUS) < solve.distance(point_x2, point_y2, cue_x_center + GLOBAL.BALL_RADIUS, cue_y_center + GLOBAL.BALL_RADIUS))
                            return new Point((int)point_x1, (int)point_y1);
                        else
                            return new Point((int)point_x2,(int)point_y2);
                    }
                    else 
                        return new Point() ;
        }

        public void UpdatePhysics(double dt)
        {
                ElEventArgs _event = new ElEventArgs();
                AdvanceBallPositions(dt);
                this.CollisionSolver(dt);
                if (this.TestOnFinish() && (gameStatus == GameStatus.InGame))
                {

                    /*
                    //  ---- Serialize ---- //
                    Stream stream = File.Open("CBall.xml", FileMode.Create);
                    SoapFormatter saver = new SoapFormatter();
                    saver.Serialize(stream, getBallByType(CBall.BType.WHITE));
                    stream.Position = 0;
                    CBall _local_ball = getBallByType(CBall.BType.WHITE);
                    _local_ball = (CBall)saver.Deserialize(stream);
                    stream.Close();
                    _local_ball = null;
                 
                    // ----- End of serailization ---- // 
                    */
                    ClearAllVelocities();
                    //Если имело место столкновение за время dt
                    if (no_collisions)
                        _event.current_status = ElEventArgs.result_status.NoBallsKickedFault;
                    else
                        _event.current_status = ElEventArgs.result_status._StepHasFinished;
                    this.SetEvent(_event);
                    // Сбрасываем настройки
                    no_collisions = true;
                }

                CBall.ClearMyList();
                CCollision.ClearMyList();
                BusyInCollision.ClearMyList();
        }

        // ----- END OF PUBLIC METHODS   //

        // ----- PRIVATE METHODS 

        private void moveWalls(CWall newWalls)
        {
            Walls = newWalls;
            GLOBAL.HasWelts = true;
        }

        private void ClearAllVelocities()
        {
            for (byte i = 0; i < balls_num; i++)
            {
                balls[i].ClearVelocity();
            }
        }

        /// <summary>
        /// Delete i ball
        /// </summary>
        private void deleteBall(CBall b)
        {
            balls.Remove(b);
            balls_num--;
        }

        private void AdvanceBallPositions(double dt)
        {
            for (short i = 0; i < balls_num; i++)
            {
                balls[i].UpdateBallPosition(dt);
                //  ------- Apply some forces (like Friction) ------ //
                if (!DemoMode)
                    this.ApplyFrictionForce(balls[i]);
                // ------------------------------------------------- //
            }
        }

        private void ApplyFrictionForce(CBall b)
        {
            b.VX -= (b.VX * GLOBAL.Friction_k);
            b.VY -= (b.VY * GLOBAL.Friction_k);
        }

        // Function test step on finish & returns true, if have finish step;

        private bool TestOnFinish()
        {
            bool _finish = true;
            for (byte i = 0; i < balls_num; i++)
            {
                if (Math.Abs(balls[i].VX) + Math.Abs(balls[i].VY) > 1000*GLOBAL.Friction_k)
                    _finish = false;
            }
            return _finish;
        }

        // ------------------------- //



        private void CollisionSolver(double dt)
        {
            ElEventArgs _event = new ElEventArgs();
            double tElapsed = 0.0D;
            CCollision c = new CCollision();
                for (short i = 0; i < balls_num; i++)
                {
                    if (tElapsed > -dt)
                    {
                        c = DetectECollison();
                        if (c.ballHasNoCollision()) 
                            break;
                        this.AdvanceBallPositions(c.getTimeToCollision());

                        if (c.ballHasCollisionWithWall())
                        {
                            solve.doCollisionWithWall(c);
                        }
                        if (c.ballHasCollisonWithBall())
                        {
                            no_collisions = false;
                            solve.doCollisionWithTwoBalls(c);
                        }

                        if (!DemoMode)
                        {
                            if (c.ballHasCollisionWithPocket())
                            {
                                // Если фол
                                int white_x_temp = GLOBAL.Ball_American_Positions[GLOBAL.Number_Balls_American - 1].X;
                                int white_y_temp = GLOBAL.Ball_American_Positions[GLOBAL.Number_Balls_American - 1].Y;
                                if (c.b1.Type == CBall.BType.WHITE)
                                {
                                    this.ClearAllVelocities();
                                    // Вставить мяч
                                    for (byte _i = 0; _i < balls_num; _i++)
                                    {
                                        if (solve.intersect_collision((GLOBAL.Ball_American_Positions[GLOBAL.Number_Balls_American - 1].X), getBallById(_i).X, GLOBAL.Ball_American_Positions[GLOBAL.Number_Balls_American - 1].Y, getBallById(_i).Y, GLOBAL.BALL_RADIUS, GLOBAL.BALL_RADIUS))
                                        {
                                            white_x_temp += (int)GLOBAL.BALL_RADIUS * 2;
                                            _i--;
                                        }
                                        else
                                            this.getBallByType(CBall.BType.WHITE).XY = new Vector2D(white_x_temp, white_y_temp);
                                    }
                                    _event.current_status = Elphysics.ElEventArgs.result_status._HasWhiteFault;
                                    this.SetEvent(_event);
                                    break;
                                }
                                else
                                {
                                    if ((c.b1.Type == CBall.BType.BLACK) && (balls_num != 2))
                                    {
                                        this.ClearAllVelocities();
                                        for (byte _i = 0; _i < balls_num; _i++)
                                        {
                                            if (solve.intersect_collision((GLOBAL.Ball_American_Positions[GLOBAL.Number_Balls_American - 1].X), getBallById(_i).X, GLOBAL.Ball_American_Positions[GLOBAL.Number_Balls_American - 1].Y, getBallById(_i).Y, GLOBAL.BALL_RADIUS, GLOBAL.BALL_RADIUS))
                                            {
                                                white_x_temp += (int)GLOBAL.BALL_RADIUS * 2;
                                                _i--;
                                            }
                                            else
                                                this.getBallByType(CBall.BType.WHITE).XY = new Vector2D(white_x_temp, white_y_temp);
                                        }
                                        this.getBallByType(CBall.BType.BLACK).XY = new Vector2D(GLOBAL.Ball_American_Positions[GLOBAL.Number_Balls_American - 2].X, GLOBAL.Ball_American_Positions[GLOBAL.Ball_American_Positions.Count - 2].Y);
                                        _event.current_status = Elphysics.ElEventArgs.result_status._HasBlackFault;
                                        this.SetEvent(_event);
                                        break;
                                    }
                                    turn.Player.Balls_in_pockets++;
                                    _event.current_status = Elphysics.ElEventArgs.result_status._BallKicked;
                                    this.SetEvent(_event);
                                }
                                this.deleteBall(c.b1);
                            }
                        }
                        this.AdvanceBallPositions(-c.getTimeToCollision());
                        tElapsed += c.getTimeToCollision();
                    }
                    else
                        return;
                }
            if (balls_num == 0)
            {
                _event.current_status = ElEventArgs.result_status.WinInGame;
                SetEvent(_event);
            }
        }

        private CCollision DetectECollison()
        {
            CCollision ecollision = new CCollision();

            ecollision = findCollisionTwoBalls();

            if (GLOBAL.HasWelts)
            {
                CCollision cWalls   = findCollisionWithWall();
                CCollision cPocket  = findCollisionWithPocket();
                if (cWalls.ballHasCollisionWithWall())
                {
                    if (cWalls.getTimeToCollision() < ecollision.getTimeToCollision())
                    {
                        ecollision = cWalls;
                    }
                }
                if (!DemoMode)
                {
                    if (cPocket.ballHasCollisionWithPocket())
                    {
                        if (!ecollision.ballHasCollisonWithBall())
                        {
                            ecollision = cPocket;
                        }
                    }
                }
            }
            return ecollision;
        }

        // -------- Finding collisions with walls ----- //

        private CCollision findCollisionWithWall()
        {
            CCollision ecollision = new CCollision();
            // No walls? No collisions
            if (!GLOBAL.HasWelts) 
                return ecollision;
            for (byte i = 0; i < balls_num; i++)
            {
                CCollision c = new CCollision();
                c = solve.FindTimeUntilBallColidesWithWall(balls[i], this.Walls);
                if (c.ballHasCollisionWithWall())
                {
                    if (!ecollision.ballHasNoCollision() || c.getTimeToCollision() < ecollision.getTimeToCollision())
                    {
                        ecollision = c;
                    }
                }
            }
            return ecollision;
        }

        // ------ Finding collisions with balls ------ //

        private CCollision findCollisionTwoBalls()
        {
            CCollision ecollision = new CCollision();
            if (balls_num == 0) return ecollision; //No balls
            for (byte i = 0; i < this.getBallCount - 1; i++)
            {
                for (byte j = 0; j < this.getBallCount; j++)
                {
                    CCollision c = new CCollision();
                    c = solve.FindTimeUntilBallCollidesWithBall(balls[i], balls[j]);
                    if (c.ballHasCollisonWithBall())
                    {
                        if (!ecollision.ballHasNoCollision() || c.getTimeToCollision() < ecollision.getTimeToCollision())
                        {
                            // --- ELEMENT OF OOP  ---- //
                            BusyInCollision busy = new BusyInCollision();
                            busy.CBall = c.b2;
                            busy.CCollision = c;
                            // ------------- //
                            ecollision = c;
                        }
                    }
                }
            }
            return ecollision;
        }

        //  ----- Finding Collision With Pockets ----- // 

        private CCollision findCollisionWithPocket()
        {
            CCollision ecollision = new CCollision();
            for (short i = 0; i < balls_num; i++)
            {
                for (short j = 0; j < GLOBAL.Pockets.Length; j++)
                {
                    if (Math.Pow((balls[i].X - GLOBAL.Pockets[j].X), 2) + Math.Pow((balls[i].Y - GLOBAL.Pockets[j].Y), 2) < Math.Pow((balls[i].R + 15) + 2,2))
                    {
                        ecollision.SetCollisionWithPocket();
                        ecollision.b1 = balls[i];
                    }
                }
            }
            return ecollision;
        }

    }

}

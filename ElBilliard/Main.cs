using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Elphysics;
using System.IO;
using System.Threading;

namespace ElBilliard
{
    public partial class Main : Form
    {
        GameResultForm gameResultForm = new GameResultForm();
        System.Windows.Forms.Timer Mtimer;
        Game game;
        Turn turn;
        Player player1;
        Player player2;
        Image LogoScreen;
        Image Red_Ball, White_Ball, Black_Ball, Menu_Anim_Ball;

        List<string> player_names = new List<string>();
        double timeAtLastUpdate;
        bool draw_cue = true;
        double BALL_DIAMETER = 20.0D;
        double GAME_SPEED = 1000.0D;
        bool has_kicked_ball = false;

        // --------------------------------------------------- //
        public Main()
        {
            Mtimer = new System.Windows.Forms.Timer();
            Mtimer.Interval = 15;
            Load                += new EventHandler(Form1_Load);
            Shown               += new EventHandler(Form1_Shown);
            Paint               += new PaintEventHandler(Form1_Paint);
            FormClosed          += new FormClosedEventHandler(Form1_Closed);
            Mtimer.Tick         += new EventHandler(Mtimer_Tick);
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Paint += new PaintEventHandler(State_RePaint);      
            int loading_result;
            if ((loading_result = LoadingData()) != 0)
            {
                MessageBox.Show("Loading Error 0x" + loading_result);
                Close();
            }
            ShowMenu();
            label4.Visible = false;
        }

        private int LoadingData()
        {
            if (File.Exists("LogoScreen.jpg"))
                LogoScreen = Image.FromFile("LogoScreen.jpg");
            else
            {
                return 1;
            }
            if (File.Exists("game_dat/red_ball.png") &&
                File.Exists("game_dat/white_ball.png") &&
                File.Exists("game_dat/black_ball.png") &&
                File.Exists("game_dat/menu_anim_ball.png")
                )
            {
                White_Ball     = Image.FromFile("game_dat/white_ball.png");
                Red_Ball       = Image.FromFile("game_dat/red_ball.png");
                Black_Ball     = Image.FromFile("game_dat/black_ball.png");
                Menu_Anim_Ball = Image.FromFile("game_dat/menu_anim_ball.png");
            }
            else
            {
                return 2;   // Generating erros 
            }
            if (File.Exists("game.dat"))
            {
                using (StreamReader sr = new StreamReader("game.dat"))
                {
                    string temp_string;
                    while ((!sr.EndOfStream) && ((temp_string = sr.ReadLine()) != "[]"))
                        player_names.Add(temp_string);
                }
            }
            else
            {
                player_names.Add("Max");
                player_names.Add("P2");
            }

            return 0;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //Send Signal to LoadLogo
            if (Program.rEvent != null)
                Program.rEvent.Set();
        }

        private void Form1_Closed(object sender, EventArgs e)
        {
            if (File.Exists("TempLog.txt"))
                File.Delete("TempLog.txt");
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (game.gameStatus == Game.GameStatus.NotInGame)
            {
                if (LogoScreen != null)
                {
                    e.Graphics.DrawImage(LogoScreen, 0, 0);
                }
                Animate(e);
            }
        }
        private void Animate(PaintEventArgs e)
        {
            Draw_Balls(e);
        }

        private void Draw_Balls(PaintEventArgs e)
        {
            CBall current_ball = new CBall();
            for (byte i = 0; i < game.getBallCount; i++)
            {
                current_ball = game.getBallById(i);
                switch (current_ball.Type)
                {
                    case CBall.BType.WHITE:
                        e.Graphics.DrawImage(White_Ball, (int)current_ball.X, (int)current_ball.Y);
                        break;
                    case CBall.BType.RED:
                        e.Graphics.DrawImage(Red_Ball, (int)current_ball.X, (int)current_ball.Y);
                        break;
                    case CBall.BType.BLACK:
                        e.Graphics.DrawImage(Black_Ball, (int)current_ball.X, (int)current_ball.Y);
                        break;
                    case CBall.BType.WHITE_TRANSPARENT:
                        e.Graphics.DrawImage(Menu_Anim_Ball, (int)current_ball.X, (int)current_ball.Y);
                        break;
                }
            }
        }

        private void LogoClosedFunction(object sender, EventArgs e)
        {
            Close();
            this.Activate();
        }

        private void SetVStatusScreen(bool state)
        {
            pictureBox1.Visible = state;
            label1.Visible = state;
            label2.Visible = state;
            label3.Visible = state;
            label5.Visible = state;
            label6.Visible = state;
            label7.Visible = state;
            elPowerControl2.Visible = state;
        }

        private void MenuVisible(bool state)
        {
            linkLabel1.Visible = state;
            linkLabel2.Visible = state;
        }

        // ------- EVENT DELEGATES --------- //

        private void Mtimer_Tick(object sender, EventArgs e)
        {
            double timeSinceLastFrame;
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            if (timeAtLastUpdate == 0)
                timeSinceLastFrame = 10;
            else
                timeSinceLastFrame = currentTime.TotalMilliseconds - timeAtLastUpdate;

            if (timeSinceLastFrame > 15)
                timeSinceLastFrame = 10;
            timeAtLastUpdate = currentTime.TotalMilliseconds;
            game.UpdatePhysics(timeSinceLastFrame / this.GAME_SPEED);
            switch (game.gameStatus)
            {
                case Game.GameStatus.InGame:
                    pictureBox1.Invalidate();
                    break;
                case Game.GameStatus.NotInGame:
                    Invalidate();
                    break;
            }
        }

        private void Game_Events(object sender, ElEventArgs e)
        {
            switch (e.current_status)
            {
                case ElEventArgs.result_status._HasWhiteFault:
                    label4.Visible = true;
                    this.label4.Text = "Fault!".ToString();
                    NextPlayer();
                break;
                case ElEventArgs.result_status._HasBlackFault:
                    label4.Visible = true;
                    label4.Text = "Fault!".ToString();
                    NextPlayer();
                break;
                case ElEventArgs.result_status._StepHasFinished:
                    if (has_kicked_ball)
                    {
                        has_kicked_ball = false;
                    }
                    else
                    {
                        NextPlayer();
                    }
                break;
                case ElEventArgs.result_status.NoBallsKickedFault:
                    label4.Visible = true;
                    label4.Text = "Fault!".ToString();
                    NextPlayer();
                break;
                case ElEventArgs.result_status._BallKicked:
                    has_kicked_ball = true;
                    label3.Text = turn.Player.Balls_in_pockets.ToString();
                return;
                case ElEventArgs.result_status.WinInGame:
                    gameResultForm.Tlabel1 = player1.Name;
                    gameResultForm.Show();
                    ShowMenu();
                    return;

            }
            StopStep();
            // ----------- CREATING LOG FILE --------- //
            using (StreamWriter wr = File.AppendText("TempLog.txt"))
            {
                wr.WriteLine(DateTime.Now.ToString("yyyy:mm:dd:ss")+ " " + this.label2.Text);
                wr.WriteLine("NextStep");
            }

        }

        private void State_RePaint(object sender, PaintEventArgs e)
        {
            switch (game.gameStatus)
            {
                case Game.GameStatus.InGame:
                    // ------ DRAWING TABLE ------ //
                    Draw_Balls(e);
                    //  ------ Draw a bead on White ball  ------- //
                    if (draw_cue)
                    {
                        short _cue_x_center = Convert.ToInt16(game.getBallByType(CBall.BType.WHITE).X);
                        short _cue_y_center = Convert.ToInt16(game.getBallByType(CBall.BType.WHITE).Y);
                        double _cue_dx = Math.Cos(turn.angle);
                        double _cue_dy = Math.Sin(turn.angle);
                        Pen pen = new Pen(Color.Brown, 2.0f);
                        e.Graphics.DrawLine(pen, _cue_x_center + Convert.ToInt32((BALL_DIAMETER / 2 + 3) * _cue_dx) + 10, _cue_y_center + Convert.ToInt32((BALL_DIAMETER / 2 + 3) * _cue_dy) + 10, _cue_x_center + Convert.ToInt32(200 * _cue_dx), _cue_y_center + Convert.ToInt32(200 * _cue_dy));
                        pen.Color = Color.White;
                        pen.Width = 0.5f;
                        /* Point[] intersect = new Point[2];
                        Point collision_point = game.CalculatePreCollision(intersect, turn.angle, _cue_x_center, _cue_y_center, this.Size);
                        if (collision_point != Point.Empty)
                        {
                            try
                            {
                                e.Graphics.DrawLine(pen, collision_point.X, collision_point.Y, _cue_x_center + 10, _cue_y_center + 10);
                            }
                            catch { }
                        } */

                    }
                    break;
            }

        }

        private void Cue_ReCountAngle(Point point)
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            if (currentTime.TotalMilliseconds - timeAtLastUpdate > 10)
            {
                turn.angle = Math.Atan2(point.Y - game.getBallByType(CBall.BType.WHITE).Y, point.X - game.getBallByType(CBall.BType.WHITE).X);
                pictureBox1.Invalidate();
                timeAtLastUpdate = currentTime.TotalMilliseconds;
            }
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (game.gameStatus == Game.GameStatus.InGame)
            {
                if (draw_cue)
                {
                    Cue_ReCountAngle(new Point(e.X, e.Y));
                }
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            label4.Visible = false;
            if (draw_cue)
            {
                game.HitCue(turn);
                Mtimer.Start();
                draw_cue = false;
            }
        }

        private void elPowerControl2_ValueOnChanged(object sender, ElControls.ElPowerChanged e)
        {
            turn.force = e.Value*10;
        }

        private void NextPlayer()
        {
            if (turn.Player.Equals(player1))
                turn.Player = player2;
            else
                turn.Player = player1;
        }

        private void StopStep()
        {
            label2.Text = turn.Player.Name.ToString();
            label3.Text = turn.Player.Balls_in_pockets.ToString();
            draw_cue = true;
            Mtimer.Stop();
            timeAtLastUpdate = 0.0D;
        }

        private void ShowMenu()
        {
            game = null;
            Player.ClearMyList();
            turn    = null;
            player1 = null;
            player2 = null;
            SetVStatusScreen(false);
            MenuVisible(true);
            label4.Visible = false;
            game = new Game(true);
            CWall w = new CWall(15, 15, this.Width - 20, this.Height - 40);
            game.addWalls(w);
            // Создание анимации в меню.
            CBall animate_ball = new CBall();
            Random o_rand = new Random();
            for (int i = 0; i < o_rand.Next(6, 15); i++)
            {
                game.addBall(new CBall(new Vector2D((double)(o_rand.Next(500) + 20), (double)(o_rand.Next(350) + 20)), new Vector2D((double)(o_rand.Next(-200,200) + 20), (double)(o_rand.Next(-200, 200) + 20)), 10.0D, BALL_DIAMETER/2, CBall.BType.WHITE_TRANSPARENT));
            }
            Mtimer.Start();
            Invalidate();
        }

        private void GameStart()
        {
            game = new Game(false);
            game.El_Event += new EventHandler<ElEventArgs>(Game_Events);

            CWall w = new CWall(2, 2, pictureBox1.Width - 22, pictureBox1.Height - 22);
            game.addWalls(w);

            game.GenerateAmericanBilliard(BALL_DIAMETER);
            turn = new Turn();
            player1 = new Player(player_names[0]);
            player2 = new Player(player_names[1]);

            turn.force = 500;
            elPowerControl2.Power = 50;
            turn.Player = player1;
            label2.Text = turn.Player.Name.ToString();
            label3.Text = turn.Player.Balls_in_pockets.ToString();
            draw_cue = true;
            SetVStatusScreen(true);
            MenuVisible(false);
            game.gameStatus = Game.GameStatus.InGame;
            Mtimer.Stop();
            Invalidate();
        }


        private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMenu();
            GameStart();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GameStart();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void finishGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMenu();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (gameResultForm.Enabled)
                gameResultForm.Visible = false;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options optionsDialog = new Options();
            if (optionsDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < optionsDialog.names.Length; i++)
                {
                    player_names[i] = optionsDialog.names[i];
                }
            }
        }

    }
}

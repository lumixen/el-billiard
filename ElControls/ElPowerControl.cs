using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ElControls
{
    public partial class ElPowerControl : UserControl
    {

        // ------------------------ ////
        private Color _BorderColor = Color.Black;

        public event EventHandler<ElPowerChanged> ValueOnChanged;

        // ------- GET/SET METHODS ------------- //

        public Color BorderColor
        {
            get { return _BorderColor;  }
            set { _BorderColor = value; }
        }

        public int Power
        {
            get { return pictureBox1.Size.Width; }
            set { if ((value < 100) & (!(value < 0)))
                       pictureBox1.Size = new Size(value, pictureBox1.Height); 
                }
        }

        private void SetEvent(ElPowerChanged _event)
        {
            EventHandler<ElPowerChanged> temp = ValueOnChanged;
            if (temp != null)
                temp(this, _event);
        }


        public ElPowerControl()
        {
            Paint += new PaintEventHandler(ElPowerControl_Paint);
            Load  +=new EventHandler(ElPowerControl_Load);

            InitializeComponent();

        }

        private void ElPowerControl_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(_BorderColor, 2.0f);
            Rectangle BorderRectangle = ClientRectangle;
            e.Graphics.DrawRectangle(pen, BorderRectangle);
        }

        private void ElPowerControl_Load(object sender, EventArgs e)
        {
            ClientSize = new Size(pictureBox1.Width + 2, pictureBox1.Height + 2);
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void ElPowerControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X < this.ClientSize.Width-1)
                {
                    pictureBox1.ClientSize = new Size(e.X, pictureBox1.ClientSize.Height);
                }
                ElPowerChanged _event = new ElPowerChanged();
                _event.Value = pictureBox1.Size.Width;
                SetEvent(_event);
                Invalidate();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X < this.ClientSize.Width-1)
                {
                    pictureBox1.ClientSize = new Size(e.X, pictureBox1.ClientSize.Height);
                }
                ElPowerChanged _event = new ElPowerChanged();
                _event.Value = pictureBox1.Size.Width;
                SetEvent(_event);
                Invalidate();
            }

        }
    }
}

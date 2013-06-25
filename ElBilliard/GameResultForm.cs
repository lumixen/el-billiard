using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ElBilliard
{
    public partial class GameResultForm : Form
    {
        public GameResultForm()
        {
            InitializeComponent();
        }

        public string Tlabel1
        {
            get { return label1.Text; }
            set { label1.Text = (value + " Win").ToString(); }
        }

        private void GameResultForm_MouseClick(object sender, MouseEventArgs e)
        {
            this.Visible = false;
        }
    }
}

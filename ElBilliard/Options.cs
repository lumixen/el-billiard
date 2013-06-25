using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ElBilliard
{
    public partial class Options : Form
    {
        private string[] _names = new string[2];
        public string[] names
        {
            get { return _names; }
        }

        public Options()
        {
            InitializeComponent();

            if (File.Exists("game.dat"))
            {
                using (StreamReader sr = new StreamReader("game.dat"))
                {
                    _names[0] = sr.ReadLine();
                    _names[1] = sr.ReadLine();
                    textBox1.Text = _names[0];
                    textBox2.Text = _names[1];
                }
            }
            else
            {
                textBox1.Text = "Player1";
                textBox2.Text = "Player2";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _names[0] = textBox1.Text;
            _names[1] = textBox2.Text;
            using (StreamWriter sw = new StreamWriter("game.dat", false))
            {
                sw.WriteLine(_names[0]); sw.WriteLine(_names[1]); sw.WriteLine("[]");
            }
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

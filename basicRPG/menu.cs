using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace basicRPG
{
    public partial class menu : Form
    {
        public menu()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            basicRPG rpgGame = new basicRPG();
            rpgGame.FormClosed += new FormClosedEventHandler(btnQuit_Click);
            rpgGame.Show();
            this.Hide();
            

            
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void btnTutorial_Click(object sender, EventArgs e)
        {
            Tutorial gameTutorial = new Tutorial();
            gameTutorial.ShowDialog(this);
        }
    }
}

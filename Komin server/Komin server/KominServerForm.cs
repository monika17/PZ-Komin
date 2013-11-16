using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Komin
{
    public partial class KominServerForm : Form
    {
        public KominServerForm()
        {
            InitializeComponent();
        }

        private void label1_Paint(object sender, PaintEventArgs e)
        {
            label1.Text = KominNetworkErrors.UserAlreadyLoggedIn.ToString();
        }
    }
}

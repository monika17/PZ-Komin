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
        KominServer server;

        public KominServerForm()
        {
            InitializeComponent();
        }

        private void onStartStopListening(object sender, EventArgs e)
        {
            if (button1.Text == "start")
            {
                server.Start(comboBox1.Items[comboBox1.SelectedIndex].ToString(), int.Parse(textBox1.Text));
                comboBox1.Enabled = false;
                textBox1.Enabled = false;
                button1.Text = "stop";
            }
            else
            {
                server.Stop();
                comboBox1.Enabled = true;
                textBox1.Enabled = true;
                button1.Text = "start";
            }
        }

        private void onFormLoad(object sender, EventArgs e)
        {
            server = new KominServer();
            comboBox1.Items.AddRange(server.DetectIPAddresses());
            comboBox1.SelectedIndex = 0;
        }
    }
}

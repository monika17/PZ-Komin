using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;

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
            Regex regexp = new Regex("(0|(1([0-9]?[0-9])?)|(2((5[0-5]?)|([0-4]?[0-9]))?)|([3-9][0-9]?))\\.(0|(1([0-9]?[0-9])?)|(2((5[0-5]?)|([0-4]?[0-9]))?)|([3-9][0-9]?))\\.(0|(1([0-9]?[0-9])?)|(2((5[0-5]?)|([0-4]?[0-9]))?)|([3-9][0-9]?))\\.(0|(1([0-9]?[0-9])?)|(2((5[0-5]?)|([0-4]?[0-9]))?)|([3-9][0-9]?))");
            if (regexp.Matches(comboBox1.Text).Count != 1)
            {
                MessageBox.Show("Niepoprawny format adresu IP", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (button1.Text == "start")
            {
                if (textBox1.Text == "")
                    textBox1.Text = "666";
                server.Start(comboBox1.Text, int.Parse(textBox1.Text));
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
            KominServerDatabase db = new KominServerDatabase();
            List<GroupFileData> gfdl = db.GetGroupFiles();
            db.Disconnect();
        }
    }
}

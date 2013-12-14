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
        string to_log;

        public KominServerForm()
        {
            to_log = null;
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
                localstart.Enabled = false;
                button1.Text = "stop";
            }
            else
            {
                server.Stop();
                comboBox1.Enabled = true;
                textBox1.Enabled = true;
                localstart.Enabled = true;
                button1.Text = "start";
            }
        }

        private void onFormLoad(object sender, EventArgs e)
        {
            server = new KominServer();
            server.log = logger;
            comboBox1.Items.AddRange(server.DetectIPAddresses());
            comboBox1.SelectedIndex = 0;
        }

        private void onLocalhostStart(object sender, EventArgs e)
        {
            comboBox1.Text = "127.0.0.1";
            textBox1.Text = "666";
            onStartStopListening(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!server.IsRunning())
                return;
            connections_count.Text = ""+server.connections.Count;
            userlist.Items.Clear();
            grouplist.Items.Clear();
            DatabaseData db = KominServer.database.GetAllData();
            foreach (UserData ud in db.users)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = "" + ud.contact_id;
                lvi.SubItems.Add(ud.contact_name);
                lvi.SubItems.Add(""+ud.status);
                string s = "";
                foreach (ContactData c in ud.contacts)
                    s += c.contact_name + "  ";
                lvi.SubItems.Add(s);
                s = "";
                foreach (GroupData g in ud.groups)
                    s += g.group_name + "  ";
                lvi.SubItems.Add(s);
                userlist.Items.Add(lvi);
            }
        }

        public void logger(string msg)
        {
            if (to_log == null)
                to_log = "[" + DateTime.Now + "]" + msg + "\r\n";
            else
                to_log += "[" + DateTime.Now + "]" + msg + "\r\n";
        }

        private void onClearLog(object sender, EventArgs e)
        {
            logbox.Text = "";
        }

        private void onLogTimerUpdate(object sender, EventArgs e)
        {
            if (to_log != null)
                logbox.Text += to_log;
            to_log = null;
        }

        private void logbox_TextChanged(object sender, EventArgs e)
        {
            logbox.SelectionLength = 0;
            logbox.SelectionStart = logbox.Text.Length;
        }
    }
}

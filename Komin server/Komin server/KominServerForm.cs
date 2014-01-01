using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using System.IO;

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
            if (!DataTesters.TestIPAddress(comboBox1.Text))
            {
                MessageBox.Show("Niepoprawny format adresu IP", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (button1.Text == "start")
            {
                if (textBox1.Text == "")
                    textBox1.Text = "8888";
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
            textBox1.Text = "8888";
            onStartStopListening(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!server.IsRunning())
                return;
            connections_count.Text = "" + server.connections.Count;
            userlist.Items.Clear();
            grouplist.Items.Clear();
            DatabaseData db = KominServer.database.GetAllData();
            foreach (UserData ud in db.users)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = "" + ud.contact_id;
                lvi.SubItems.Add(ud.contact_name);
                lvi.SubItems.Add("" + ud.status);
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

            foreach (GroupData gd in db.groups)
            {
                if (gd == null) continue;
                ListViewItem lvi = new ListViewItem();
                lvi.Text = "" + gd.group_id;
                lvi.SubItems.Add(gd.group_name);
                lvi.SubItems.Add("" + gd.creators_id);
                lvi.SubItems.Add("" + gd.communication_type);
                string s = "";
                if (gd.members != null)
                    foreach (ContactData c in gd.members)
                        s += c.contact_name + "  ";
                lvi.SubItems.Add(s);
                grouplist.Items.Add(lvi);
            }
        }

        public void logger(string msg)
        {
            if (to_log == null)
                to_log = "[" + DateTime.Now + "]" + msg + "\r\n";
            else
                to_log = "[" + DateTime.Now + "]" + msg + "\r\n" + to_log;
        }

        private void onClearLog(object sender, EventArgs e)
        {
            logbox.Text = "";
        }

        private void onLogTimerUpdate(object sender, EventArgs e)
        {
            if (to_log != null)
                logbox.Text = to_log + logbox.Text;
            to_log = null;
        }

        private void logbox_TextChanged(object sender, EventArgs e)
        {
            logbox.SelectionLength = 0;
            logbox.SelectionStart = 0;
            button2.Enabled = (logbox.Text != "");
            savelog.Enabled = (logbox.Text != "");
        }

        private void sqlCommandText_TextChanged(object sender, EventArgs e)
        {
            sqlExec.Enabled = (sqlCommandText.Text != "");
        }

        private void readerexec_CheckedChanged(object sender, EventArgs e)
        {
            sqlReaderOutput.Enabled = readerexec.Checked;
        }

        private void sqlExec_Click(object sender, EventArgs e)
        {
            if (!readerexec.Checked)
            {
                KominServer.database.WaitForRequestExecution(KominServer.database.MakeExecuteRequest(sqlCommandText.Text));
                if (KominServer.database.error_during_req)
                    MessageBox.Show("Some error occured during execution", "SQL Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                KominServer.database.WaitForRequestExecution(KominServer.database.MakeReaderRequest(sqlCommandText.Text));
                if (KominServer.database.error_during_req)
                    MessageBox.Show("Some error occured during execution", "SQL Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    sqlReaderOutput.Clear();
                    System.Data.SqlClient.SqlDataReader rdr = KominServer.database.ReqRdr;
                    bool is_first = true;
                    while (rdr.Read())
                    {
                        if (is_first)
                        {
                            int i = 0;
                            try
                            {
                                while (true)
                                {
                                    sqlReaderOutput.Columns.Add(rdr.GetName(i));
                                    i++;
                                }
                            }
                            catch (Exception) { }
                            is_first = false;
                        }
                        ListViewItem lvi = new ListViewItem(rdr[0].ToString());
                        try
                        {
                            int i = 1;
                            while (true)
                            {
                                lvi.SubItems.Add(rdr[i].ToString());
                                i++;
                            }
                        }
                        catch (Exception) { }
                        sqlReaderOutput.Items.Add(lvi);
                    }
                    KominServer.database.MarkReaderRequestCompleted();
                }
            }
            sqlCommandText.SelectionStart = 0;
            sqlCommandText.SelectionLength = sqlCommandText.Text.Length;
            sqlCommandText.Focus();
        }

        private void savelog_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.CheckPathExists = true;
            sfd.DefaultExt = "log";
            sfd.DereferenceLinks = true;
            sfd.Filter = "Server log file (*.log)|*.log";
            sfd.FilterIndex = 0;
            sfd.InitialDirectory = ".\\";
            sfd.OverwritePrompt = true;
            sfd.RestoreDirectory = false;
            sfd.SupportMultiDottedExtensions = false;
            sfd.Title = "Select a file";
            sfd.ValidateNames = false;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter file = new StreamWriter(sfd.OpenFile());
                string text = logbox.Text;
                text = text.Replace("\r\n", "\n");
                text = text.Replace("\n\r", "\n");
                text = text.Replace("\r", "\n");
                file.Write(text);
                file.Close();
                MessageBox.Show("Log written to file", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

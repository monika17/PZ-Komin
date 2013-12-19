﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    }
}

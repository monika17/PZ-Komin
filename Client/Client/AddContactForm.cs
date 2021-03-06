﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Komin
{
    public partial class AddContactForm : Form
    {
        private KominClientSideConnection conn;
        private bool KominClientErrorOccured;

        public AddContactForm(KominClientSideConnection conn)
        {
            this.conn = conn;
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = false;
            contactValidityLabel.Text = "";
            timer1.Enabled = false;
            if (textBox1.Text != "")
                timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            KominClientErrorOccured = false;
            SomeError err_routine = conn.onError;
            conn.onError = onError_Ping;
            ContactData cd = conn.PingContactRequest(0, textBox1.Text);
            conn.onError = err_routine;
            if (KominClientErrorOccured)
            {
                contactValidityLabel.Text = "kontakt nie istnieje";
                contactValidityLabel.ForeColor = Color.FromArgb(255, 0, 0);
                timer1.Enabled = false;
                return;
            }
            if (cd != null)
            {
                contactValidityLabel.Text = "kontakt istnieje";
                contactValidityLabel.ForeColor = Color.FromArgb(0, 255, 0);
                button1.Enabled = true;
            }
            else
            {
                contactValidityLabel.Text = "kontakt nie istnieje";
                contactValidityLabel.ForeColor = Color.FromArgb(255, 0, 0);
            }
            timer1.Enabled = false;
        }

        private void onError_Ping(string err_text, KominNetworkPacket packet)
        {
            KominClientErrorOccured = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KominClientErrorOccured = false;
            SomeError err_routine = conn.onError;
            conn.onError = onError_Message;
            conn.AddContactToList(textBox1.Text);
            conn.onError = err_routine;
            if (KominClientErrorOccured)
                return;
            timer1.Enabled = false;
            this.Close();
        }

        private void onError_Message(string err_text, KominNetworkPacket packet)
        {
            KominClientErrorOccured = true;
            MessageBox.Show(err_text, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Close();
        }
    }
}

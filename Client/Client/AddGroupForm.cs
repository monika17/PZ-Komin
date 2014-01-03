using System;
using System.Drawing;
using System.Windows.Forms;

namespace Komin
{
    public partial class AddGroupForm : Form
    {
        private KominClientSideConnection conn;
        private bool KominClientErrorOccured;

        public AddGroupForm(KominClientSideConnection conn)
        {
            this.conn = conn;
            InitializeComponent();
        }

        private void groupnameTextBox_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = false;
            groupnameValidityLabel.Text = "";
            timer1.Enabled = false;
            if (groupnameTextBox.Text != "")
                timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!DataTesters.TestLoginOrGroupName(groupnameTextBox.Text))
            {
                groupnameValidityLabel.Text = "nazwa jest niepoprawna";
                groupnameValidityLabel.ForeColor = Color.FromArgb(255, 0, 0);
                timer1.Enabled = false;
                return;
            }
            KominClientErrorOccured = false;
            SomeError err_routine = conn.onError;
            conn.onError = onError_Ping;
            GroupData gd = conn.PingGroupRequest(groupnameTextBox.Text);
            conn.onError = err_routine;
            if (KominClientErrorOccured)
            {
                groupnameValidityLabel.Text = "grupa nie istnieje";
                groupnameValidityLabel.ForeColor = Color.FromArgb(0, 255, 0);
                button1.Enabled = true;
                timer1.Enabled = false;
                return;
            }
            if (gd != null)
            {
                groupnameValidityLabel.Text = "grupa istnieje";
                groupnameValidityLabel.ForeColor = Color.FromArgb(255, 0, 0);
            }
            else
            {
                groupnameValidityLabel.Text = "grupa nie istnieje";
                groupnameValidityLabel.ForeColor = Color.FromArgb(0, 255, 0);
                button1.Enabled = true;
            }
            timer1.Enabled = false;
        }

        private void onError_Ping(string err_text, KominNetworkPacket packet)
        {
            KominClientErrorOccured = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            uint group_caps = (uint)((text.Checked ? 0x008 : 0) + (voice.Checked ? 0x010 : 0) + (video.Checked ? 0x020 : 0) + (files.Checked ? 0x100 : 0));
            KominClientErrorOccured = false;
            SomeError err_routine = conn.onError;
            conn.onError = onError_Message;
            conn.CreateGroup(groupnameTextBox.Text, group_caps);
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

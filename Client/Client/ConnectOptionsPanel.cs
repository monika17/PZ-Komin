using System;
using System.Drawing;
using System.Windows.Forms;

namespace Komin
{
    public partial class ConnectOptionsPanel : UserControl
    {
        private KominClientSideConnection connection;
        private bool KominClientErrorOccured;
        private KominClientForm clientForm;
        public ConnectOptionsPanel(KominClientSideConnection connection, KominClientForm clientForm)
        {
            this.clientForm = clientForm;
            this.connection = connection;
            InitializeComponent();

            ConnectPanel.Location = new Point(0, 0);
            clientForm.AcceptButton = buttonConnect;

            //------- Debug
            textBoxhostIp.Text = "127.0.0.1";
            textBoxPort.Text = "8888";
            //-------
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (!DataTesters.TestIPAddress(textBoxhostIp.Text))
            {
                MessageBox.Show("Niepoprawny format adresu IP", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int port_no = Convert.ToInt32(textBoxPort.Text);
            if ((port_no < 1) || (port_no > 9999))
            {
                MessageBox.Show("Niepoprawny numer portu (poprawne są z przedziału [1, 9999])", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            KominClientErrorOccured = false;
            SomeError err_routine = connection.onError;
            connection.onError = onError;
            connection.Connect(textBoxhostIp.Text, port_no);
            connection.onError = err_routine;
            if (KominClientErrorOccured)
                return;
            ConnectPanel.Visible = false;
            LoginPanel.Location = new Point(0, 0);
            clientForm.AcceptButton = LoginButton;
            LoginPanel.Visible = true;
            LoginNametextBox.Focus();
        }

        private void onError(string err_text, KominNetworkPacket packet)
        {
            KominClientErrorOccured = true;
            MessageBox.Show(err_text, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            uint new_status = (uint)KominClientStatusCodes.Accessible;
            KominClientErrorOccured = false;
            SomeError err_routine = connection.onError;
            connection.onError = onError;
            connection.Login(LoginNametextBox.Text, LoginPasstextBox.Text, ref new_status);
            connection.onError = err_routine;
            if (KominClientErrorOccured)
                return;
            clientForm.LoginSuccess();
        }

        private void buttonNewUser_Click(object sender, EventArgs e)
        {
            AddUserForm addUserForm = new AddUserForm(connection);
            addUserForm.ShowDialog();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            KominClientErrorOccured = false;
            SomeError err_routine = connection.onError;
            connection.onError = onError;
            connection.Disconnect();
            connection.onError = err_routine;
            if (KominClientErrorOccured)
                return;
            ConnectPanel.Visible = true;
            ConnectPanel.Location = new Point(0, 0);
            LoginPanel.Visible = false;
            clientForm.AcceptButton = buttonConnect;
            textBoxhostIp.Focus();
            textBoxhostIp.Select(0, textBoxhostIp.Text.Length);
        }

        private void LoginNametextBox_TextChanged(object sender, EventArgs e)
        {
            LoginButton.Enabled = ((LoginNametextBox.Text != "") && (LoginPasstextBox.Text != ""));
        }

        private void LoginPasstextBox_TextChanged(object sender, EventArgs e)
        {
            LoginButton.Enabled = ((LoginNametextBox.Text != "") && (LoginPasstextBox.Text != ""));
        }

        private void textBoxhostIp_TextChanged(object sender, EventArgs e)
        {
            buttonConnect.Enabled = ((textBoxhostIp.Text != "") && (textBoxPort.Text != ""));
        }

        private void textBoxPort_TextChanged(object sender, EventArgs e)
        {
            buttonConnect.Enabled = ((textBoxhostIp.Text != "") && (textBoxPort.Text != ""));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RequestAcceptanceWaitingForm awf = new RequestAcceptanceWaitingForm(connection, 1, true);
            switch (awf.ShowDialog())
            {
                case DialogResult.Cancel:
                    MessageBox.Show("anulowano", "fgdshjfs", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case DialogResult.OK:
                    MessageBox.Show("akceptowano", "fjdafsd", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

    }
}

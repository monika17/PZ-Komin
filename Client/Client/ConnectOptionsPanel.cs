using System;
using System.Drawing;
using System.Windows.Forms;

namespace Komin
{
    public partial class ConnectOptionsPanel : UserControl
    {
        private KominClientSideConnection connection;
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
            try
            {
                connection.Connect(textBoxhostIp.Text, Convert.ToInt32(textBoxPort.Text));
            }
            catch (Exception)
            {
                MessageBox.Show("Nie udało się połączyć z serwerem", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ConnectPanel.Visible = false;
            LoginPanel.Location = new Point(0, 0);
            clientForm.AcceptButton = LoginButton;
            LoginPanel.Visible = true;
            LoginNametextBox.Focus();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            uint new_status = (uint)KominClientStatusCodes.Accessible;
            try
            {
                connection.Login(LoginNametextBox.Text, LoginPasstextBox.Text, ref new_status);
            }
            catch (KominClientErrorException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            clientForm.LoginSuccess();
        }

        private void buttonNewUser_Click(object sender, EventArgs e)
        {
            var addUserForm = new AddUserForm(connection);
            addUserForm.Visible = true;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            connection.Disconnect();
            ConnectPanel.Visible = true;
            ConnectPanel.Location = new Point(0, 0);
            LoginPanel.Visible = false;
            clientForm.AcceptButton = buttonConnect;
            textBoxhostIp.Focus();
        }

    }
}

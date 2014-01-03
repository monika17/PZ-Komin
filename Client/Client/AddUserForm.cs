using System;
using System.Drawing;
using System.Windows.Forms;

namespace Komin
{
    public partial class AddUserForm : Form
    {
        private KominClientSideConnection connection;
        private bool KominClientErrorOccured;

        public AddUserForm(KominClientSideConnection connection)
        {
            this.connection = connection;
            InitializeComponent();
            RegisterNameTextBoxToolTip.SetToolTip(RegisterNameTextBox, "Pierwsza litera, później ciąg złożyny z liter, cyfr i znaku podkreślenia _");
        }

        private void RegisterNameValidityTimer_Tick(object sender, EventArgs e)
        {
            if (!DataTesters.TestLoginOrGroupName(RegisterNameTextBox.Text))
            {
                RegisterNameAcceptableLabel.Text = "nazwa jest niepoprawna";
                RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(255, 0, 0);
                RegisterNameValidityTimer.Enabled = false;
                return;
            }
            KominClientErrorOccured = false;
            SomeError err_routine = connection.onError;
            connection.onError = onError;
            ContactData cd = connection.PingContactRequest(0, RegisterNameTextBox.Text);
            connection.onError = err_routine;
            if (KominClientErrorOccured)
            {
                RegisterNameAcceptableLabel.Text = "nazwa jest wolna";
                RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(0, 255, 0);
                RegisterNameValidityTimer.Enabled = false;
                return;
            }
            if (cd != null)
            {
                RegisterNameAcceptableLabel.Text = "nazwa jest zajęta";
                RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(255, 0, 0);
            }
            else
            {
                RegisterNameAcceptableLabel.Text = "nazwa jest wolna";
                RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(0, 255, 0);
            }
            RegisterNameValidityTimer.Enabled = false;
        }

        private void onError(string err_text, KominNetworkPacket packet)
        {
            KominClientErrorOccured = true;
        }

        private void RegisterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            RegisterButton.Enabled = ((RegisterNameTextBox.Text != "") && (RegisterPassTextBox.Text != ""));
            RegisterNameAcceptableLabel.Text = "";
            RegisterNameValidityTimer.Enabled = false;
            if (RegisterNameTextBox.Text != "")
                RegisterNameValidityTimer.Enabled = true;
        }

        private void RegisterPassTextBox_TextChanged(object sender, EventArgs e)
        {
            RegisterButton.Enabled = ((RegisterNameTextBox.Text != "") && (RegisterPassTextBox.Text != ""));
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            KominClientErrorOccured = false;
            SomeError err_routine = connection.onError;
            connection.onError = onError_Message;
            connection.CreateContact(RegisterNameTextBox.Text, RegisterPassTextBox.Text);
            connection.onError = err_routine;
            if (KominClientErrorOccured)
                return;
            Close();
        }

        private void onError_Message(string err_text, KominNetworkPacket packet)
        {
            KominClientErrorOccured = true;
            MessageBox.Show(err_text, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

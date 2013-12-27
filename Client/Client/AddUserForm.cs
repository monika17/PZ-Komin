using System;
using System.Drawing;
using System.Windows.Forms;

namespace Komin
{
    public partial class AddUserForm : Form
    {
        private KominClientSideConnection connection;
        public AddUserForm(KominClientSideConnection connection)
        {
            this.connection = connection;
            InitializeComponent();
            RegisterNameTextBoxToolTip.SetToolTip(RegisterNameTextBox, "Pierwsza litera duża, później ciąg złożyny z liter (dużych i małych), cyfr i znaku podkreślenia _");
        }

        private void RegisterNameValidityTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!DataTesters.TestLoginOrGroupName(RegisterNameTextBox.Text))
                {
                    RegisterNameAcceptableLabel.Text = "nazwa jest niepoprawna";
                    RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(255, 0, 0);
                    throw new Exception();
                }
                ContactData cd = connection.PingContactRequest(0, RegisterNameTextBox.Text);
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
            }
            catch (KominClientErrorException)
            {
                RegisterNameAcceptableLabel.Text = "nazwa jest wolna";
                RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(0, 255, 0);
            }
            catch (Exception) { }
            RegisterNameValidityTimer.Enabled = false;
        }

        private void RegisterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            RegisterNameAcceptableLabel.Text = "";
            RegisterNameValidityTimer.Enabled = false;
            if (RegisterNameTextBox.Text != "")
                RegisterNameValidityTimer.Enabled = true;
            RegisterButton.Enabled = ((RegisterNameTextBox.Text != "") && (RegisterPassTextBox.Text != ""));
        }

        private void RegisterButton_Paint(object sender, PaintEventArgs e)
        {
            RegisterButton.Enabled = ((RegisterNameTextBox.Text != "") && (RegisterPassTextBox.Text != ""));
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                //connection.CreateContact(RegisterNameTextBox.Text, RegisterPassTextBox.Text);
            }
            catch (KominClientErrorException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Close();
        }
    }
}

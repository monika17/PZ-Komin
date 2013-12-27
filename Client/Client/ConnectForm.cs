using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Komin
{
    public partial class ConnectForm : Form
    {
        private KominClientSideConnection connection;
        private KominClientForm clientForm;
        public ConnectForm(KominClientSideConnection connection, KominClientForm clientForm)
        {
            this.clientForm = clientForm;
            this.connection = connection;
            InitializeComponent();
            RegisterNameTextBoxToolTip.SetToolTip(RegisterNameTextBox, "Pierwsza litera duża, później ciąg złożyny z liter (dużych i małych), cyfr i znaku podkreślenia _");

            Height = 330;
            ConnectPanel.Location = new Point(9, 12);

            //------- Debug
            textBoxhostIp.Text = "127.0.0.1";
            textBoxPort.Text = "8888";
            //-------
        }

        private void ConnectPanel_Paint(object sender, PaintEventArgs e)
        {

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
            LoginPanel.Location = new Point(9, 12);
            LoginPanel.Visible = true;
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

        private void RegisterPassTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void RegisterButton_Paint(object sender, PaintEventArgs e)
        {
            RegisterButton.Enabled = ((RegisterNameTextBox.Text != "") && (RegisterPassTextBox.Text != ""));
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                connection.CreateContact(RegisterNameTextBox.Text, RegisterPassTextBox.Text);
            }
            catch (KominClientErrorException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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
            Close();
        }

        private void buttonNewUser_Click(object sender, EventArgs e)
        {
            LoginPanel.Visible = false;
            RegisterPanel.Location = new Point(9, 12);
            RegisterPanel.Visible = true;
        }
    }
}

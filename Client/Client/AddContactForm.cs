using System;
using System.Drawing;
using System.Windows.Forms;

namespace Komin
{
    public partial class AddContactForm : Form
    {
        private KominClientSideConnection conn;

        public AddContactForm(KominClientSideConnection conn)
        {
            this.conn = conn;
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            contactValidityLabel.Text = "";
            timer1.Enabled = false;
            if (textBox1.Text != "")
                timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                ContactData cd = conn.PingContactRequest(0, textBox1.Text);
                if (cd != null)
                {
                    contactValidityLabel.Text = "kontakt istnieje";
                    contactValidityLabel.ForeColor = Color.FromArgb(0, 255, 0);
                }
                else
                {
                    contactValidityLabel.Text = "kontakt nie istnieje";
                    contactValidityLabel.ForeColor = Color.FromArgb(255, 0, 0);
                }
            }
            catch (KominClientErrorException)
            {
                contactValidityLabel.Text = "kontakt nie istnieje";
                contactValidityLabel.ForeColor = Color.FromArgb(255, 0, 0);
            }
            timer1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                conn.AddContactToList(textBox1.Text);
            }
            catch (KominClientErrorException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            timer1.Enabled = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Close();
        }
    }
}

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
    public partial class GroupInviteForm : Form
    {
        private KominClientSideConnection conn;
        private bool KominClientErrorOccured;
        private ContactData contact; //contact to be invited

        public GroupInviteForm(KominClientSideConnection conn, ContactData contact)
        {
            this.conn = conn;
            this.contact = contact;
            InitializeComponent();
            //load groups
            foreach (GroupData gd in conn.userdata.groups)
                comboBox1.Items.Add(gd.group_name);
            comboBox1.SelectedIndex = 0;
            this.Text = "Zaproś " + contact.contact_name + " do grupy";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (GroupData gd in conn.userdata.groups)
                if (gd.group_name == ((string)comboBox1.SelectedItem))
                {
                    KominClientErrorOccured = false;
                    SomeError err_routine = conn.onError;
                    conn.onError = onError;
                    conn.JoinGroup(gd.group_id, true, contact.contact_id);
                    conn.onError = err_routine;
                    break;
                }
            this.Close();
        }

        private void onError(string err_text, KominNetworkPacket packet)
        {
            KominClientErrorOccured = true;
            MessageBox.Show(err_text, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

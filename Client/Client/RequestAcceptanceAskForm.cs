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
    public partial class RequestAcceptanceAskForm : Form
    {
        private KominClientSideConnection conn;
        private ContactData contact;
        private uint job_id;
        private int count;
        private bool closing;

        public RequestAcceptanceAskForm(KominClientSideConnection conn, ContactData contact, uint job_id, bool video_request)
        {
            this.conn = conn;
            this.contact = contact;
            this.job_id = job_id;
            closing = false;

            InitializeComponent();

            label1.Text = String.Format(label1.Text, contact.contact_name, (video_request ? "wideo" : "audio"));
            count = 15;
            acceptButton.Text = "Akceptuj (" + count + ")";
            //countdownTimer.Enabled = true;
        }

        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            countdownTimer.Enabled = false;
            count--;
            if (count > 0)
            {
                acceptButton.Text = "Akceptuj (" + count + ")";
                countdownTimer.Enabled = true;
            }
            else
            {
                cancelButton_Click(null, null);
            }
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            closing = true;
            conn.AcceptCall(contact.contact_id, job_id);
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            closing = true;
            conn.CloseCall(contact.contact_id, job_id);
            DialogResult = DialogResult.Cancel;
        }

        private void RequestAcceptanceAskForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closing)
                return;
            closing = true;
            cancelButton_Click(null, null);
        }
    }
}

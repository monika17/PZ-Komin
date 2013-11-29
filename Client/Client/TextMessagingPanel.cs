using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Komin
{
    public partial class TextMessagingPanel : UserControl
    {
        public uint receiver_id;
        public bool receiver_is_group;
        private KominClientSideConnection conn;

        public TextMessagingPanel(KominClientSideConnection conn, uint receiver_id, bool receiver_is_group)
        {
            this.receiver_id = receiver_id;
            this.receiver_is_group = receiver_is_group;
            this.conn = conn;
            InitializeComponent();
            this.Visible = true;
        }

        private void onTextInputContentChanged(object sender, EventArgs e)
        {
            textSendButton.Enabled = (textMessageInput.Text != "");
        }

        private void onTextSendClicked(object sender, EventArgs e)
        {
            TextMessage tmsg = conn.SendMessage(receiver_id, receiver_is_group, textMessageInput.Text);
            textMessageInput.Text = "";
            //insert loopback
            textMessageContainer.Text += "[" + tmsg.send_date + "]  " + conn.userdata.contact_name + ":\n" + tmsg.message + "\n";
        }
    }
}

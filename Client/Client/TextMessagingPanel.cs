using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Komin
{
    public partial class TextMessagingPanel : UserControl
    {
        public uint receiver_id;
        public bool receiver_is_group;
        private KominClientSideConnection conn;
        private bool KominClientErrorOccured;
        private System.Timers.Timer TextInsertTimer;
        private bool on_inserting;
        private string text_insert;
        private KominClientForm CallingForm;

        public TextMessagingPanel()
        {
            this.TextInsertTimer = new System.Timers.Timer(50);
            this.TextInsertTimer.Elapsed += TextMessagingPanel_Insert;
            this.TextInsertTimer.SynchronizingObject = this;
            this.on_inserting = false;
            this.text_insert = null;
            this.receiver_id = 0;
            this.receiver_is_group = false;
            this.conn = null;
            InitializeComponent();
            TextInsertTimer.Start();
            this.Visible = true;
        }

        public TextMessagingPanel(KominClientSideConnection conn, uint receiver_id, bool receiver_is_group, Form callingForm)
            : this()
        {
            this.receiver_id = receiver_id;
            this.receiver_is_group = receiver_is_group;
            this.conn = conn;
            this.CallingForm = callingForm as KominClientForm;
        }

        private void TextMessagingPanel_Insert(object sender, EventArgs e)
        {
            TextInsertTimer.Enabled = false;
            while (on_inserting) ;
            on_inserting = true;
            if (text_insert != null)
                textMessageContainer.Text += text_insert;
            text_insert = null;
            on_inserting = false;
            TextInsertTimer.Enabled = true;
        }

        public void InsertText(string text)
        {
            AddToArchive(text);
            if (text_insert == null)
                text_insert = text;
            else
                text_insert += text;
        }

        private void AddToArchive(string text)
        {
            var dirPath = "Archive/" + conn.userdata.contact_name;
            var path = dirPath + "/"+ receiver_id + ".txt";
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            try
            {
                using (var file = new StreamWriter(path, true))
                    file.Write(text);
            }
            catch
            {
                MessageBox.Show("Nie można zapisać w " + path, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void onTextInputContentChanged(object sender, EventArgs e)
        {
            textSendButton.Enabled = (textMessageInput.Text != "");
        }

        private void onTextSendClicked(object sender, EventArgs e)
        {
            KominClientErrorOccured = false;
            SomeError err_routine = conn.onError;
            conn.onError = onError;
            TextMessage tmsg = conn.SendMessage(receiver_id, receiver_is_group, textMessageInput.Text);
            conn.onError = err_routine;
            textMessageInput.Text = "";
            if (KominClientErrorOccured)
                return;
            //insert loopback
            if (!receiver_is_group)
                InsertText("[" + tmsg.send_date + "]  " + conn.userdata.contact_name + ":\r\n" + tmsg.message + "\r\n");
        }

        private void onError(string err_text, KominNetworkPacket packet)
        {
            KominClientErrorOccured = true;
            MessageBox.Show(err_text, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonAutoSend_Click(object sender, EventArgs e)
        {
            if (CallingForm.AcceptButton != textSendButton)
                CallingForm.AcceptButton = textSendButton;
            else
                CallingForm.AcceptButton = null;
        }

        private void textMessageContainer_TextChanged(object sender, EventArgs e)
        {
            textMessageContainer.SelectionStart = textMessageContainer.Text.Length;
            textMessageContainer.ScrollToCaret();
        }


    }
}

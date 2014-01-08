using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Komin
{
    /// <summary>
    /// Form that is used for asking about call acceptance (either audio and video).
    /// This form should be called during call requests instead calling KominClientSideConnection::Request...Call
    /// </summary>
    public partial class RequestAcceptanceWaitingForm : Form
    {
        private KominClientSideConnection conn;
        private uint contact_id;
        private bool request_type; //false - audio, true - video
        private bool cancel; //variable use for cancelling request
        private int result; //variable for storing request result value

        private Thread th;

        /// <param name="conn"></param>
        /// <param name="contact_id">Opposite side user id</param>
        /// <param name="video_request">false - audio call, true - video call</param>
        public RequestAcceptanceWaitingForm(KominClientSideConnection conn, uint contact_id, bool video_request)
        {
            this.conn = conn;
            this.contact_id = contact_id;
            this.request_type = video_request;

            InitializeComponent();

            result = -1;
            lifeTimer.Enabled = true;
            th = new Thread(thread_master);
            th.Start();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            cancel = true;
        }

        private void RequestAcceptanceWaitingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancelButton_Click(null, null);
        }

        private void thread_master()
        {
            cancel = false;
            //#####
            while (!cancel) ;
            result = 1;
            //#####
            if (request_type == false)
                ;//conn.RequestAudioCall(contact_id, ref cancel_var, ref result);
            else
                ;//conn.RequestVideoCall(contact_id, ref cancel_var, ref result);
            
            th.Abort();
        }

        private void lifeTimer_Tick(object sender, EventArgs e)
        {
            lifeTimer.Enabled = false;
            //check has result changed
            switch (result)
            {
                case 0: //accepted
                    DialogResult = DialogResult.OK;
                    this.Close();
                    break;
                case 1: //cancelled
                    DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;
                default:
                    lifeTimer.Enabled = true;
                    break;
            }
        }
    }
}

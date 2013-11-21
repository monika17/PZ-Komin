using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MessageWindow : Form
    {
        public MessageWindow()
        {
            InitializeComponent();
        }

        private void MessageWindow_Load(object sender, EventArgs e)
        {

        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {
            TextMessage.Text += WriteMessage.Text + "\n";
            WriteMessage.Clear();
        }

        private void WriteMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            TextMessage.Text += WriteMessage.Text + "\n";
            WriteMessage.Clear();
        }

    }
}

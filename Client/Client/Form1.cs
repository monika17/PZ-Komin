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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MainTabPanel.Controls.Clear();
            MainTabPanel.Controls.Add(LoginTab);
            RightMenu.Enabled = false;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            RightMenu.Enabled = true;

            UserInfo.Name = LoginNametextBox.Text;
            UserInfo.Password = LoginPasstextBox.Text;

            UserName.Text = "Nazwa: " + UserInfo.Name;
            UserStatus.Text = "Status: dostepny";

            MainTabPanel.Controls.Remove(LoginTab);
            MainTabPanel.Controls.Add(tmpTab);

            tmpTest.Text = "Zalogowany jako: " + UserInfo.Name;
        }

        private void tabTmp_Test(object sender, EventArgs e)
        {
            tmpTest.Text += UserInfo.Name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textMessageContainer.Text += "Ja:" +
                                         Environment.NewLine + "  " +
                                         textMessageBox.Text +
                                         Environment.NewLine;
            textMessageBox.Clear();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            MainTabPanel.Controls.Clear();
            tabSendTextMessage.Text = e.Node.Text;
            MainTabPanel.Controls.Add(tabSendTextMessage);
        }

        private void logout_Click(object sender, EventArgs e)
        {
            MainTabPanel.Controls.Clear();
            MainTabPanel.Controls.Add(LoginTab);
            RightMenu.Enabled = false;
        }
    }
}

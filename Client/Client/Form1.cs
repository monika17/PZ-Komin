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
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            UserInfo.Name = LoginNametextBox.Text;
            UserInfo.Password = LoginPasstextBox.Text;

            UserName.Text += UserInfo.Name;
            UserStatus.Text += " dostepny";

            MainTabPanel.Controls.Remove(LoginTab);
            MainTabPanel.Controls.Add(tmpTab);
        }

        private void tabTmp_Test(object sender, EventArgs e)
        {
            tmpTest.Text += UserInfo.Name + "\n";
        }
    }
}

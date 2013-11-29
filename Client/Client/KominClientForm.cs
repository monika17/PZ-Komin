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
    public partial class KominClientForm : Form
    {
        class ContactTreeTag
        {
            public uint id;
            public bool is_group;

            public ContactTreeTag(uint id, bool is_group)
            {
                this.id = id;
                this.is_group = is_group;
            }
        }

        private KominClientSideConnection connection;

        public KominClientForm()
        {
            connection = new KominClientSideConnection();
            //#####    to powinno byc zmienione      ########################################################################
            try
            {
                connection.Connect("127.0.0.1", 666);
            }
            catch (Exception)
            {
                MessageBox.Show("Nie udało się połączyć z serwerem", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            connection.onNewTextMessage = onNewMessage;
            InitializeComponent();
            MainTabPanel.TabPages.Clear();
            MainTabPanel.TabPages.Add(LoginTab);
            RightMenu.Enabled = false;
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

            UserName.Text = "Nazwa: " + connection.userdata.contact_name;
            //UserStatus.Text = "Status: dostępny";
            statusComboBox.SelectedIndex = (int)new_status;

            MainTabPanel.TabPages.Remove(LoginTab);
            MainTabPanel.TabPages.Add(HomePage);

            tmpTest.Text = "Zalogowany jako: " + connection.userdata.contact_name;

            //update contact and group list
            TreeNode contacts = treeView1.Nodes["Kontakty"];
            TreeNode groups = treeView1.Nodes["Grupy"];
            foreach (ContactData cd in connection.userdata.contacts)
            {
                TreeNode tn = new TreeNode(cd.contact_name);
                tn.Tag = new ContactTreeTag(cd.contact_id, false);
                contacts.Nodes.Add(tn);
            }
            foreach (GroupData gd in connection.userdata.groups)
            {
                TreeNode gtn = new TreeNode(gd.group_name);
                gtn.Tag = new ContactTreeTag(gd.group_id, true);
                foreach (ContactData cd in gd.members)
                {
                    TreeNode tn = new TreeNode(cd.contact_name);
                    tn.Tag = new ContactTreeTag(cd.contact_id, false);
                    gtn.Nodes.Add(tn);
                }
                groups.Nodes.Add(gtn);
            }

            RightMenu.Enabled = true;
        }

        private void tabTmp_Test(object sender, EventArgs e)
        {
            tmpTest.Text += UserInfo.Name;
        }

        private void treeView1_NodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OpenTabForContact(e.Node);
            /*if (e.Node.Text == "Kontakty" || e.Node.Text == "Grupy")
                return;
            if (MainTabPanel.TabPages[0] == HomePage)
            {
                MainTabPanel.TabPages.Clear();
                MainTabPanel.ContextMenuStrip = contactTabContextMenu;
            }

            //find is there a tab for this contact already opened
            foreach (TabPage tp in MainTabPanel.TabPages)
                if (tp.Text == e.Node.Text)
                {
                    MainTabPanel.SelectedTab = tp;
                    return;
                }

            //create new tab page for text messaging
            uint receiver_id = ((ContactTreeTag)e.Node.Tag).id;
            bool receiver_is_group = ((ContactTreeTag)e.Node.Tag).is_group;

            TextMessagingPanel tmp = new TextMessagingPanel(connection, receiver_id, receiver_is_group);
            tmp.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            tmp.Location = new System.Drawing.Point(3, 3);
            tmp.Name = "TextMessagingPanel";
            tmp.Size = new System.Drawing.Size(484, 386);
            tmp.TabStop = true;

            TabPage tpage = new TabPage();
            tpage.Location = new System.Drawing.Point(4, 22);
            tpage.Name = (receiver_is_group ? "G" : "C") + receiver_id;
            tpage.Size = new System.Drawing.Size(490, 392);
            tpage.TabIndex = 1;
            tpage.Text = e.Node.Text;
            tpage.UseVisualStyleBackColor = true;
            tpage.Controls.Add(tmp);
            MainTabPanel.TabPages.Add(tpage);
            MainTabPanel.SelectedTab = tpage;*/
        }

        private void logout_Click(object sender, EventArgs e)
        {
            MainTabPanel.TabPages.Clear();
            MainTabPanel.ContextMenuStrip = null;
            MainTabPanel.TabPages.Add(LoginTab);
            RightMenu.Enabled = false;
        }

        private void onCloseContactTabClick(object sender, EventArgs e)
        {
        }

        private void treeView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar=='\r')
                treeView1_NodeDoubleClick(sender, new TreeNodeMouseClickEventArgs(((TreeView)sender).SelectedNode, System.Windows.Forms.MouseButtons.Left, 2, 0, 0));
        }

        private void OpenTabForContact(TreeNode tn)
        {
            if (tn.Text == "Kontakty" || tn.Text == "Grupy")
                return;
            foreach (TabPage tabpage in MainTabPanel.TabPages)
                if (tabpage.Text == HomePage.Text)
                {
                    MainTabPanel.TabPages.Remove(tabpage);
                    break;
                }

            //find is there a tab for this contact already opened
            foreach (TabPage tp in MainTabPanel.TabPages)
                if (tp.Text == tn.Text)
                {
                    MainTabPanel.SelectedTab = tp;
                    return;
                }

            //create new tab page for text messaging
            uint receiver_id = ((ContactTreeTag)tn.Tag).id;
            bool receiver_is_group = ((ContactTreeTag)tn.Tag).is_group;

            TextMessagingPanel tmp = new TextMessagingPanel(connection, receiver_id, receiver_is_group);
            tmp.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            tmp.Location = new System.Drawing.Point(3, 3);
            tmp.Name = "TextMessagingPanel";
            tmp.Size = new System.Drawing.Size(484, 386);
            tmp.TabIndex = 1;
            tmp.TabStop = true;

            TabPage tpage = new TabPage();
            tpage.Location = new System.Drawing.Point(4, 22);
            tpage.Name = (receiver_is_group ? "G" : "C") + receiver_id;
            tpage.Size = new System.Drawing.Size(490, 392);
            tpage.TabIndex = 1;
            tpage.Text = tn.Text;
            tpage.UseVisualStyleBackColor = true;
            tpage.Controls.Add(tmp);
            MainTabPanel.TabPages.Add(tpage);
            MainTabPanel.SelectedTab = tpage;
            MainTabPanel.ContextMenuStrip = contactTabContextMenu;
        }

        private void onNewMessage(uint sender_id, uint receiver_id, bool receiver_is_group, TextMessage msg)
        {
            if (receiver_is_group == false) //if message is targeted to us not to group we're participating
            {
                if (receiver_id != connection.userdata.contact_id) //if it's not for us then discard
                    return;
                else
                {
                    //find sender on contact list, if not present then discard message
                    //######################### foreign contact notification should be sent instead of discarding
                    foreach (TreeNode tn in treeView1.Nodes["Kontakty"].Nodes)
                        if (((ContactTreeTag)tn.Tag).id == sender_id) //found
                        {
                            //find data in contact list
                            ContactData cd = null;
                            foreach (ContactData c in connection.userdata.contacts)
                                if (c.contact_id == sender_id)
                                {
                                    cd = c;
                                    break;
                                }
                            //open tab page for this contact
                            OpenTabForContact(tn);
                            //treeView1_NodeDoubleClick(treeView1, new TreeNodeMouseClickEventArgs(tn, System.Windows.Forms.MouseButtons.Left, 2, 0, 0));
                            //insert message
                            MainTabPanel.TabPages["C" + sender_id].Controls["TextMessagingPanel"].Controls["textMessageContainer"].Text += "[" + msg.send_date + "]  " + cd.contact_name + ":\n" + msg.message + "\n";
                            return;
                        }
                    //discard
                    return;
                }
            }
            else
            {
                //find group specified in receiver_id. if not found then discard
                foreach (TreeNode tn in treeView1.Nodes["Grupy"].Nodes)
                    if (((ContactTreeTag)tn.Tag).id == receiver_id) //found
                    {
                        //find data in group members list
                        GroupData gd = null;
                        ContactData cd = null;
                        foreach (GroupData g in connection.userdata.groups)
                            if (g.group_id == receiver_id)
                            {
                                gd = g;
                                break;
                            }
                        foreach (ContactData c in gd.members)
                            if (c.contact_id == sender_id)
                            {
                                cd = c;
                                break;
                            }
                        //open tab page for this group
                        OpenTabForContact(tn);
                        //treeView1_NodeDoubleClick(treeView1, new TreeNodeMouseClickEventArgs(tn, System.Windows.Forms.MouseButtons.Left, 2, 0, 0));
                        //insert message
                        MainTabPanel.TabPages["G" + receiver_id].Controls["TextMessagingPanel"].Controls["textMessageContainer"].Text += "[" + msg.send_date + "]  " + cd.contact_name + ":\n" + msg.message + "\n";
                        return;
                    }
                //discard
                return;
            }
        }

        private void onClientClosing(object sender, FormClosingEventArgs e)
        {
            connection.Disconnect();
        }
    }
}

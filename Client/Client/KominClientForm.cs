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
        //tab update
        private System.Timers.Timer TabUpdateTimer;
        private bool TabUpdateOnRun;
        private List<TabPage> remove_page; //tab pages to be removed on next redraw
        private List<TabPage> add_page; //tab pages to be added on next redraw
        private TabPage next_page; //tab page to make visible on next redraw. set to null to avoid changing
        private TreeNode clickedContactNode; //get Node after mouse click in contacts list

        public KominClientForm()
        {
            TabUpdateOnRun = false;
            TabUpdateTimer = new System.Timers.Timer(50);
            TabUpdateTimer.Elapsed += KominClientForm_TabUpdate;
            TabUpdateTimer.SynchronizingObject = this;
            remove_page = new List<TabPage>();
            add_page = new List<TabPage>();
            next_page = null;

            InitializeComponent();
            MainTabPanel.TabPages.Clear();
            MainTabPanel.TabPages.Add(LoginTab);
            RightMenu.Enabled = false;
            TabUpdateTimer.Start();

            //------- Debug
            textBoxhostIp.Text = "127.0.0.1";
            textBoxPort.Text = "8888";
            //-------

            //configure connection structure - don't attempt to connect yet
            connection = new KominClientSideConnection();
            connection.onNewTextMessage = onNewMessage;
            connection.onContactListChange = onContactListChange;

            var ConnectForm = new ConnectForm(connection, this);
            ConnectForm.Visible = true;
        }

        public void LoginSuccess()
        {
            uint new_status = (uint)KominClientStatusCodes.Accessible;

            UserName.Text = "Nazwa: " + connection.userdata.contact_name;
            //UserStatus.Text = "Status: dostępny";
            statusComboBox.SelectedIndex = (int)new_status;

            MainTabPanel.TabPages.Remove(LoginTab);
            MainTabPanel.TabPages.Add(HomePage);

            tmpTest.Text = "Zalogowany jako: " + connection.userdata.contact_name;

            //update contact and group list
            TreeNode contacts = treeView1.Nodes["Kontakty"];
            TreeNode groups = treeView1.Nodes["Grupy"];
            connection.userdata.contacts.Sort(ContactDataComparison_ByNameAsc);
            foreach (ContactData cd in connection.userdata.contacts)
            {
                TreeNode tn = new TreeNode(cd.contact_name);
                tn.Tag = new ContactTreeTag(cd.contact_id, false);
                tn.ContextMenuStrip = contextMenuStripContact;
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

        private void KominClientForm_TabUpdate(object sender, EventArgs e)
        {
            TabUpdateTimer.Enabled = false;
            while (TabUpdateOnRun) ;
            TabUpdateOnRun = true;
            foreach (TabPage tp in remove_page)
                try { MainTabPanel.TabPages.Remove(tp); }
                catch (Exception) { }
            remove_page.Clear();
            foreach (TabPage tp in add_page)
                try { MainTabPanel.TabPages.Add(tp); }
                catch (Exception) { }
            add_page.Clear();
            if (next_page != null)
                try { MainTabPanel.SelectedTab = next_page; }
                catch (Exception) { }
            next_page = null;
            TabUpdateOnRun = false;
            TabUpdateTimer.Enabled = true;
        }

        private void WaitForTabUpdate()
        {
            while (remove_page.Count > 0 || add_page.Count > 0 || next_page != null) ;
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
            connection.userdata.contacts.Sort(ContactDataComparison_ByNameAsc);
            foreach (ContactData cd in connection.userdata.contacts)
            {
                TreeNode tn = new TreeNode(cd.contact_name);
                tn.Tag = new ContactTreeTag(cd.contact_id, false);
                tn.ContextMenuStrip = contextMenuStripContact;
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
        }

        private void treeView1_NodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OpenTabForContact(e.Node);
        }

        private void logout_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Logout();
            }
            catch (KominClientErrorException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            remove_page.Clear();
            add_page.Clear();
            next_page = null;
            MainTabPanel.TabPages.Clear();
            MainTabPanel.ContextMenuStrip = null;
            MainTabPanel.TabPages.Add(LoginTab);
            UserName.Text = "Nazwa: ";
            statusComboBox.Text = "";
            treeView1.Nodes["Kontakty"].Nodes.Clear();
            treeView1.Nodes["Grupy"].Nodes.Clear();
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
                    remove_page.Add(tabpage);
                    break;
                }

            //find is there a tab for this contact already opened
            foreach (TabPage tp in MainTabPanel.TabPages)
                if (tp.Text == tn.Text)
                {
                    next_page = tp;
                    return;
                }

            //create new tab page for text messaging
            uint receiver_id = ((ContactTreeTag)tn.Tag).id;
            bool receiver_is_group = ((ContactTreeTag)tn.Tag).is_group;

            TextMessagingPanel tmp = new TextMessagingPanel(connection, receiver_id, receiver_is_group, this);
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
            add_page.Add(tpage);
            next_page = tpage;
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
                            WaitForTabUpdate();
                            //insert message
                            ((TextMessagingPanel)MainTabPanel.TabPages["C" + sender_id].Controls["TextMessagingPanel"]).InsertText("[" + msg.send_date + "]  " + cd.contact_name + ":\r\n" + msg.message + "\r\n");
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
                        WaitForTabUpdate();
                        //insert message
                        ((TextMessagingPanel)MainTabPanel.TabPages["G" + receiver_id].Controls["TextMessagingPanel"]).InsertText("[" + msg.send_date + "]  " + cd.contact_name + ":\r\n" + msg.message + "\r\n");
                        return;
                    }
                //discard
                return;
            }
        }

        private void onContactListChange(UserData new_ud)
        {
            connection.userdata.contacts = new_ud.contacts;

            TreeNode contacts = treeView1.Nodes["Kontakty"];

            //remove contacts
            List<int> to_remove = new List<int>();
            for (int i = 0; i < contacts.Nodes.Count; i++)
            {
                bool found = false;
                foreach (ContactData cd in connection.userdata.contacts)
                {
                    if (((ContactTreeTag)contacts.Nodes[i].Tag).id == cd.contact_id)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    to_remove.Insert(0, i);
            }
            foreach (int i in to_remove)
            {
                foreach (TabPage tp in MainTabPanel.TabPages)
                    if (tp.Text == contacts.Nodes[i].Text)
                    {
                        if (MainTabPanel.TabPages.Count == 1)
                        {
                            add_page.Add(HomePage);
                            next_page = HomePage;
                        }
                        remove_page.Add(tp);
                        WaitForTabUpdate();
                        break;
                    }
            }
            contacts.Nodes.Clear();

            //add contacts
            connection.userdata.contacts.Sort(ContactDataComparison_ByNameAsc);
            foreach (ContactData cd in connection.userdata.contacts)
            {
                TreeNode tn = new TreeNode(cd.contact_name);
                tn.Tag = new ContactTreeTag(cd.contact_id, false);
                tn.ContextMenuStrip = contextMenuStripContact;
                contacts.Nodes.Add(tn);
            }
        }

        int ContactDataComparison_ByNameAsc(ContactData _1, ContactData _2)
        {
            return _1.contact_name.CompareTo(_2.contact_name);
        }

        private void onClientClosing(object sender, FormClosingEventArgs e)
        {
            if(connection != null)
                connection.Disconnect();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Connect(textBoxhostIp.Text, Convert.ToInt32(textBoxPort.Text));
            }
            catch (Exception)
            {
                MessageBox.Show("Nie udało się połączyć z serwerem", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ConnectStatus.Text = "Połączono";
            buttonConnect.Enabled = false;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }

        private void treeView1_GetNode(object sender, TreeNodeMouseClickEventArgs e)
        {
            clickedContactNode = e.Node;
        }

        private void txtMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenTabForContact(this.clickedContactNode);
        }

        private void wyślijPlikToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = sendFileDialog.ShowDialog();
            //validate file
            //send file
        }

        private void addContactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddContactForm acf = new AddContactForm(connection);
            acf.ShowDialog();
        }

        private void RegisterNameValidityTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!DataTesters.TestLoginOrGroupName(RegisterNameTextBox.Text))
                {
                    RegisterNameAcceptableLabel.Text = "nazwa jest niepoprawna";
                    RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(255, 0, 0);
                    throw new Exception();
                }
                ContactData cd = connection.PingContactRequest(0, RegisterNameTextBox.Text);
                if (cd != null)
                {
                    RegisterNameAcceptableLabel.Text = "nazwa jest zajęta";
                    RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(255, 0, 0);
                }
                else
                {
                    RegisterNameAcceptableLabel.Text = "nazwa jest wolna";
                    RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(0, 255, 0);
                }
            }
            catch (KominClientErrorException)
            {
                RegisterNameAcceptableLabel.Text = "nazwa jest wolna";
                RegisterNameAcceptableLabel.ForeColor = Color.FromArgb(0, 255, 0);
            }
            catch (Exception) { }
            RegisterNameValidityTimer.Enabled = false;
        }

        private void RegisterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            RegisterNameAcceptableLabel.Text = "";
            RegisterNameValidityTimer.Enabled = false;
            if (RegisterNameTextBox.Text != "")
                RegisterNameValidityTimer.Enabled = true;
            RegisterButton.Enabled = ((RegisterNameTextBox.Text != "") && (RegisterPassTextBox.Text != ""));
        }

        private void RegisterPassTextBox_TextChanged(object sender, EventArgs e)
        {
            RegisterButton.Enabled = ((RegisterNameTextBox.Text != "") && (RegisterPassTextBox.Text != ""));
        }

        private void RegisterButton_Paint(object sender, PaintEventArgs e)
        {
            RegisterButton.Enabled = ((RegisterNameTextBox.Text != "") && (RegisterPassTextBox.Text != ""));
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                connection.CreateContact(RegisterNameTextBox.Text, RegisterPassTextBox.Text);
            }
            catch (KominClientErrorException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}

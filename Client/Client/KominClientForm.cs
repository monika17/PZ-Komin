using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq.Expressions;
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
        private bool KominClientErrorOccured;
        //tab update
        private System.Timers.Timer TabUpdateTimer;
        private bool TabUpdateOnRun;
        private List<TabPage> remove_page; //tab pages to be removed on next redraw
        private List<TabPage> add_page; //tab pages to be added on next redraw
        private TabPage next_page; //tab page to make visible on next redraw. set to null to avoid changing
        private TreeNode clickedContactNode; //get Node after mouse click in contacts list
        private List<ContactData> changed_contacts;
        private List<GroupData> changed_groups;
        private bool server_lost;
        private bool server_kick;
        private bool server_disconnect;
        private List<TabPage> added_text_tabs; 

        public KominClientForm()
        {
            TabUpdateOnRun = false;
            TabUpdateTimer = new System.Timers.Timer(50);
            TabUpdateTimer.Elapsed += KominClientForm_TabUpdate;
            TabUpdateTimer.SynchronizingObject = this;
            remove_page = new List<TabPage>();
            add_page = new List<TabPage>();
            next_page = null;
            changed_contacts = new List<ContactData>();
            changed_groups = new List<GroupData>();
            server_lost = false;
            server_kick = false;
            server_disconnect = false;
            added_text_tabs = new List<TabPage>();

            InitializeComponent();
            MainTabPanel.TabPages.Clear();
            MainTabPanel.TabPages.Add(LoginTab);
            RightMenu.Enabled = false;
            TabUpdateTimer.Start();

            //configure connection structure - don't attempt to connect yet
            connection = new KominClientSideConnection();
            connection.onError = onError;
            connection.onNewTextMessage = onNewMessage;
            connection.onNewAudioMessage = onNewAudioMessage;
            connection.onContactListChange = onContactListChange;
            connection.onStatusNotification = onStatusNotification_PreUpdate;
            connection.onGroupJoin = onGroupChange_PreUpdate;
            connection.onGroupInvite = onGroupInvite;
            connection.onGroupHolderChange = onGroupChange_PreUpdate;
            connection.onGroupClosed = onGroupClosed;
            connection.onGroupLeave = onGroupChange_PreUpdate;
            connection.onGroupKick = onGroupKick;
            connection.onServerLostConnection = onServerLostConnection_PreShow;
            connection.onServerLogout = onServerLogout_PreShow;
            connection.onServerDisconnected = onServerDisconnected_PreShow;
            ShowConnectOptionsOnLoginTab();
        }

        public void LoginSuccess()
        {
            AcceptButton = null;
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
                tn.ImageIndex = (int)cd.status;
                tn.SelectedImageIndex = (int)cd.status;
                contacts.Nodes.Add(tn);
            }
            connection.userdata.groups.Sort(GroupDataComparison_ByNameAsc);
            foreach (GroupData gd in connection.userdata.groups)
            {
                TreeNode gtn = new TreeNode(gd.group_name);
                gtn.Tag = new ContactTreeTag(gd.group_id, true);
                gtn.ContextMenuStrip = contextMenuStripGroup;
                gtn.ImageIndex = 6;
                gtn.SelectedImageIndex = 6;
                gd.members.Sort(ContactDataComparison_ByNameAsc);
                foreach (ContactData cd in gd.members)
                {
                    TreeNode tn = new TreeNode(cd.contact_name);
                    tn.Tag = new ContactTreeTag(cd.contact_id, false);
                    tn.ContextMenuStrip = contextMenuStripGroupMember;
                    tn.ImageIndex = (int)(cd.status + (gd.creators_id == cd.contact_id ? 6 : 0));
                    if (tn.ImageIndex == 6)
                        tn.ImageIndex = 0;
                    tn.SelectedImageIndex = (int)(cd.status + (gd.creators_id == cd.contact_id ? 6 : 0));
                    if (tn.SelectedImageIndex == 6)
                        tn.SelectedImageIndex = 0;
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
            if (server_lost)
                onServerLostConnection();
            if (server_kick)
                onServerLogout();
            if (server_disconnect)
                onServerDisconnected();
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
            foreach(TabPage tp in added_text_tabs)
                try
                {
                    WebBrowser wb = new WebBrowser();

                    wb.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom)| AnchorStyles.Left)| AnchorStyles.Right;
                    wb.Location = new System.Drawing.Point(3, 3);
                    wb.MinimumSize = new System.Drawing.Size(20, 20);
                    wb.Name = "textMessageContainer";
                    wb.ScriptErrorsSuppressed = true;
                    wb.Size = new System.Drawing.Size(475, 300);
                    wb.TabIndex = 5;
                    wb.DocumentText = "";
                    wb.Margin = new Padding(0, 0, 0, 0);
                    tp.Controls["TextMessagingPanel"].Controls["panel1"].Controls.Add(wb);
                    ((TextMessagingPanel)tp.Controls["TextMessagingPanel"]).textMessageContainer = wb;

                    var cssFileContent = System.IO.File.ReadAllText("style.css");
                    wb.Document.Write("<style>" + cssFileContent + " </style>");
                }
                catch (Exception)
                {
                }
            added_text_tabs.Clear();
            foreach (GroupData gd in changed_groups)
                onGroupChange(gd);
            changed_groups.Clear();
            foreach (ContactData cd in changed_contacts)
                onStatusNotification(cd);
            changed_contacts.Clear();
            TabUpdateOnRun = false;
            TabUpdateTimer.Enabled = true;
        }

        private void WaitForTabUpdate()
        {
            while (remove_page.Count > 0 || add_page.Count > 0 || next_page != null) ;
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
            KominClientErrorOccured = false;
            connection.Logout();
            if (KominClientErrorOccured)
                return;
            ResetPanel();
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            KominClientErrorOccured = false;
            connection.Disconnect();
            if (KominClientErrorOccured)
                return;
            ResetPanel();
            ShowConnectOptionsOnLoginTab();
        }

        private void ResetPanel()
        {
            remove_page.Clear();
            add_page.Clear();
            next_page = null;
            MainTabPanel.TabPages.Clear();
            MainTabPanel.ContextMenuStrip = null;
            MainTabPanel.TabPages.Add(LoginTab);
            UserName.Text = "Nazwa: ";
            statusComboBox.Text = "";
            treeView1.Nodes["Kontakty"].Collapse(false);
            treeView1.Nodes["Kontakty"].Nodes.Clear();
            treeView1.Nodes["Grupy"].Collapse(false);
            foreach (TreeNode gdn in treeView1.Nodes["Grupy"].Nodes)
                gdn.Nodes.Clear();
            treeView1.Nodes["Grupy"].Nodes.Clear();
            RightMenu.Enabled = false;
        }

        private void onCloseContactTabClick(object sender, EventArgs e)
        {
        }

        private void treeView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                treeView1_NodeDoubleClick(sender, new TreeNodeMouseClickEventArgs(((TreeView)sender).SelectedNode, System.Windows.Forms.MouseButtons.Left, 2, 0, 0));
        }

        private void ShowConnectOptionsOnLoginTab()
        {
            var connectOptionsPanel = new ConnectOptionsPanel(connection, this);
            connectOptionsPanel.Location = new System.Drawing.Point(75, 80);

            LoginTab.Controls.Clear();
            LoginTab.Controls.Add(connectOptionsPanel);
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
            //ContactData c = null; //###########################
            if (receiver_is_group)
                tpage.ImageIndex = 3;
            else
            {
                //find contact on contact list
                foreach (ContactData cd in connection.userdata.contacts)
                    if (cd.contact_id == receiver_id)
                    {
                        tpage.ImageIndex = (int)(cd.status & (uint)KominClientStatusCodes.Mask);
                        //c = cd; //#############################
                        break;
                    }
            }
            tpage.Controls.Add(tmp);
            added_text_tabs.Add(tpage);
            
            add_page.Add(tpage);
            next_page = tpage;
            MainTabPanel.ContextMenuStrip = contactTabContextMenu;

            /*AudioMessagingPanel amp = new AudioMessagingPanel(connection, receiver_id, receiver_is_group, tpage);
            amp.Enabled = true;
            amp.Visible = true;
            amp.Location = new System.Drawing.Point(0, 0);
            tpage.Controls.Add(amp);
            amp.AddContact(c, false);*/
        }

        private void onError(string err_text, KominNetworkPacket packet)
        {
            KominClientErrorOccured = true;
            MessageBox.Show(err_text, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            ((TextMessagingPanel)MainTabPanel.TabPages["C" + sender_id].Controls["TextMessagingPanel"]).InsertText(ReceiverHtmlText(msg.send_date, cd.contact_name, msg.message));
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
                        ((TextMessagingPanel)MainTabPanel.TabPages["G" + receiver_id].Controls["TextMessagingPanel"]).InsertText(ReceiverHtmlText(msg.send_date, cd.contact_name, msg.message));
                        return;
                    }
                //discard
                return;
            }
        }

        private string ReceiverHtmlText(DateTime sendDate, string contactName, string message)
        {
            return "<div class='receiveMessage'>" +
                   "<span class='dateFormat'>" + String.Format("{0:HH:mm:ss}", sendDate) + "</span>  " +
                   "<span class='name'>" + contactName + "</span>" +
                   "<span class='message'>: " + message + "</span><br>" +
                   "</div>";
        }

        private void onNewAudioMessage(uint sender, uint receiver, bool receiver_is_group, byte[] msg)
        {
            //check is there any opened audio messaging panel - if there isn't then no need to process message
            if (AudioMessagingPanel.Singleton == null)
                return;
            //check receiver data. exit if it is not for us
            if (receiver_is_group == false)
            {
                if (receiver != connection.userdata.contact_id)
                    return;
            }
            else
            {
                bool found = false;
                foreach (GroupData gd in connection.userdata.groups)
                {
                    if (gd.group_id == receiver)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return;
            }
            AudioMessagingPanel.Singleton.InsertMessage(sender, msg);
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
                tn.ImageIndex = (int)(cd.status & (uint)KominClientStatusCodes.Mask);
                tn.SelectedImageIndex = (int)(cd.status & (uint)KominClientStatusCodes.Mask);
                contacts.Nodes.Add(tn);
            }
        }

        private void onStatusNotification_PreUpdate(ContactData changed_contact)
        {
            changed_contacts.Add(changed_contact);
        }

        private void onStatusNotification(ContactData changed_contact)
        {
            TreeNode contacts = treeView1.Nodes["Kontakty"];
            TreeNode groups = treeView1.Nodes["Grupy"];

            //find contact on contact list
            foreach (ContactData cd in connection.userdata.contacts)
                if (cd.contact_id == changed_contact.contact_id)
                {
                    cd.status = changed_contact.status;
                    break;
                }

            //find contact on groups members list
            foreach (GroupData gd in connection.userdata.groups)
                foreach (ContactData cd in gd.members)
                    if (cd.contact_id == changed_contact.contact_id)
                    {
                        cd.status = changed_contact.status;
                        break;
                    }

            //find contact on contact list in tree view
            foreach (TreeNode tn in contacts.Nodes)
                if (tn.Text == changed_contact.contact_name)
                {
                    tn.ImageIndex = (int)(changed_contact.status & (uint)KominClientStatusCodes.Mask);
                    tn.SelectedImageIndex = (int)(changed_contact.status & (uint)KominClientStatusCodes.Mask);
                    break;
                }

            //find contact on groups members lists in tree view
            foreach (TreeNode gn in groups.Nodes)
            {
                GroupData gd = null;
                foreach (GroupData g in connection.userdata.groups)
                    if (g.group_name == gn.Text)
                    {
                        gd = g;
                        break;
                    }
                foreach (TreeNode tn in gn.Nodes)
                    if (tn.Text == changed_contact.contact_name)
                    {
                        tn.ImageIndex = (int)(changed_contact.status & (uint)KominClientStatusCodes.Mask) + (gd.creators_id == changed_contact.contact_id ? 6 : 0);
                        if (tn.ImageIndex == 6)
                            tn.ImageIndex = 0;
                        tn.SelectedImageIndex = (int)(changed_contact.status & (uint)KominClientStatusCodes.Mask) + (gd.creators_id == changed_contact.contact_id ? 6 : 0);
                        if (tn.SelectedImageIndex == 6)
                            tn.SelectedImageIndex = 0;
                        break;
                    }
            }

            //find contact in opened tabs
            foreach (TabPage tp in MainTabPanel.TabPages)
                if (tp.Name == "C" + changed_contact.contact_id)
                {
                    tp.ImageIndex = (int)(changed_contact.status & (uint)KominClientStatusCodes.Mask);
                    break;
                }
        }

        private void onGroupChange_PreUpdate(GroupData new_gd)
        {
            changed_groups.Add(new_gd);
        }

        private void onGroupChange(GroupData new_gd)
        {
            TreeNode groups = treeView1.Nodes["Grupy"];

            //remove groups
            List<int> to_remove = new List<int>();
            for (int i = 0; i < groups.Nodes.Count; i++)
            {
                bool found = false;
                foreach (GroupData gd in connection.userdata.groups)
                {
                    if (((ContactTreeTag)groups.Nodes[i].Tag).id == gd.group_id)
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
                    if (tp.Text == groups.Nodes[i].Text)
                    {
                        if (MainTabPanel.TabPages.Count == 1)
                        {
                            add_page.Add(HomePage);
                            next_page = HomePage;
                        }
                        remove_page.Add(tp);
                        break;
                    }
                groups.Nodes.RemoveAt(i);
            }

            //add groups
            connection.userdata.groups.Sort(GroupDataComparison_ByNameAsc);
            for (int i = 0; i < connection.userdata.groups.Count; i++)
            {
                GroupData gd = connection.userdata.groups[i];
                if ((groups.Nodes.Count < i + 1) || (groups.Nodes[i].Text != gd.group_name))
                {
                    TreeNode gtn = new TreeNode(gd.group_name);
                    gtn.Tag = new ContactTreeTag(gd.group_id, true);
                    gtn.ContextMenuStrip = contextMenuStripGroup;
                    gtn.ImageIndex = 6;
                    gtn.SelectedImageIndex = 6;
                    gd.members.Sort(ContactDataComparison_ByNameAsc);
                    foreach (ContactData cd in gd.members)
                    {
                        TreeNode tn = new TreeNode(cd.contact_name);
                        tn.Tag = new ContactTreeTag(cd.contact_id, false);
                        tn.ContextMenuStrip = contextMenuStripGroupMember;
                        tn.ImageIndex = (int)(cd.status + (gd.creators_id == cd.contact_id ? 6 : 0));
                        if (tn.ImageIndex == 6)
                            tn.ImageIndex = 0;
                        tn.SelectedImageIndex = (int)(cd.status + (gd.creators_id == cd.contact_id ? 6 : 0));
                        if (tn.SelectedImageIndex == 6)
                            tn.SelectedImageIndex = 0;
                        gtn.Nodes.Add(tn);
                    }
                    if (groups.Nodes.Count < i + 1)
                        groups.Nodes.Add(gtn);
                    else
                        groups.Nodes.Insert(i, gtn);
                }
                else //update group
                {
                    TreeNode gtn = groups.Nodes[i];
                    gtn.Nodes.Clear();
                    gd.members.Sort(ContactDataComparison_ByNameAsc);
                    foreach (ContactData cd in gd.members)
                    {
                        TreeNode tn = new TreeNode(cd.contact_name);
                        tn.Tag = new ContactTreeTag(cd.contact_id, false);
                        tn.ContextMenuStrip = contextMenuStripGroupMember;
                        tn.ImageIndex = (int)(cd.status + (gd.creators_id == cd.contact_id ? 6 : 0));
                        if (tn.ImageIndex == 6)
                            tn.ImageIndex = 0;
                        tn.SelectedImageIndex = (int)(cd.status + (gd.creators_id == cd.contact_id ? 6 : 0));
                        if (tn.SelectedImageIndex == 6)
                            tn.SelectedImageIndex = 0;
                        gtn.Nodes.Add(tn);
                    }
                }
            }
        }

        private bool onGroupInvite(GroupData gd, ContactData invitor)
        {
            switch (MessageBox.Show(invitor.contact_name + " zaprasza Cię do grupy " + gd.group_name, "Zaproszenie do grupy", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                case DialogResult.Yes:
                    return true;
                case DialogResult.No:
                    return false;
                default:
                    return false;
            }
        }

        private void onGroupClosed(GroupData gd)
        {
            onGroupChange_PreUpdate(gd);
            MessageBox.Show("Zarządca grupy " + gd.group_name + " rozwiązał ją.", "Powiadomienie", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void onGroupKick(GroupData gd)
        {
            onGroupChange_PreUpdate(gd);
            MessageBox.Show("Zostałeś wyrzucony/a z grupy " + gd.group_name + " przez jej zarządce", "Wyrzucenie z grupy", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void onServerLostConnection_PreShow()
        {
            server_lost = true;
        }

        private void onServerLostConnection()
        {
            server_lost = false;
            MessageBox.Show("Utracono połączenie z serwerem", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ResetPanel();
            ShowConnectOptionsOnLoginTab();
        }

        private void onServerLogout_PreShow()
        {
            server_kick = true;
        }

        private void onServerLogout()
        {
            server_kick = false;
            MessageBox.Show("Zostałeś wylogowany z serwera", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ResetPanel();
        }

        private void onServerDisconnected_PreShow()
        {
            server_disconnect = true;
        }

        private void onServerDisconnected()
        {
            server_disconnect = false;
            MessageBox.Show("Serwer zakończył połączenie", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ResetPanel();
            ShowConnectOptionsOnLoginTab();
        }

        int ContactDataComparison_ByNameAsc(ContactData _1, ContactData _2)
        {
            return _1.contact_name.CompareTo(_2.contact_name);
        }

        int GroupDataComparison_ByNameAsc(GroupData _1, GroupData _2)
        {
            return _1.group_name.CompareTo(_2.group_name);
        }

        private void onClientClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null)
                connection.Disconnect();
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

        private void deleteContactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Czy napewno chcesz usunąć kontakt?\n\nKontakt: " + clickedContactNode.Text, "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                KominClientErrorOccured = false;
                connection.RemoveContactFromList(clickedContactNode.Text);
                if (KominClientErrorOccured)
                    return;
            }
        }

        private void statusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            KominClientErrorOccured = false;
            connection.SetStatus((uint)statusComboBox.SelectedIndex);
            if (KominClientErrorOccured)
                return;
        }

        private void addGroupMemberToContacts(object sender, EventArgs e)
        {
            KominClientErrorOccured = false;
            connection.AddContactToList(clickedContactNode.Text);
            if (KominClientErrorOccured)
                return;
        }

        private void addNewGroupContextMenuItem(object sender, EventArgs e)
        {
            AddGroupForm agf = new AddGroupForm(connection);
            agf.ShowDialog();
        }

        private void inviteContactToGroup_Click(object sender, EventArgs e)
        {
            foreach (ContactData cd in connection.userdata.contacts)
                if (clickedContactNode.Text == cd.contact_name)
                {
                    GroupInviteForm gif = new GroupInviteForm(connection, cd);
                    gif.ShowDialog();
                    break;
                }
        }

        private void promoteToGroupLeader_Click(object sender, EventArgs e)
        {
            ContactTreeTag holder_tag = (ContactTreeTag)clickedContactNode.Tag;
            ContactTreeTag group_tag = (ContactTreeTag)clickedContactNode.Parent.Tag;
            KominClientErrorOccured = false;
            connection.ChangeGroupHolder(group_tag.id, holder_tag.id);
            if (KominClientErrorOccured)
                return;
        }

        private void removeGroupContextMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Czy napewno chcesz rozwiązać grupe " + clickedContactNode.Text + "?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                KominClientErrorOccured = false;
                connection.CloseGroup(((ContactTreeTag)clickedContactNode.Tag).id);
                if (KominClientErrorOccured)
                    return;
            }
        }

        private void leaveGroupContextMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Czy napewno chcesz opuścić grupe " + clickedContactNode.Text + "?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                KominClientErrorOccured = false;
                connection.LeaveGroup(((ContactTreeTag)clickedContactNode.Tag).id);
                if (KominClientErrorOccured)
                    return;
            }
        }

        private void kickFromGroupContextMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Czy napewno chcesz wyrzucić " + clickedContactNode.Text + " z grupy " + clickedContactNode.Parent.Text + "?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                KominClientErrorOccured = false;
                connection.LeaveGroup(((ContactTreeTag)clickedContactNode.Parent.Tag).id, true, ((ContactTreeTag)clickedContactNode.Tag).id);
                if (KominClientErrorOccured)
                    return;
            }
        }

        private void contextMenuStripContact_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menu = (ContextMenuStrip)sender;
            ContactTreeTag contact_tag = (ContactTreeTag)clickedContactNode.Tag;
            menu.Items["zaprośDoGrupyToolStripMenuItem1"].Enabled = (treeView1.Nodes["Grupy"].Nodes.Count > 0) && (contact_tag.id != connection.userdata.contact_id);
            //menu.Items["txtMessageToolStripMenuItem"].Enabled = (contact_tag.id != connection.userdata.contact_id);
            //menu.Items["audioMessageToolStripMenuItem"].Enabled = (contact_tag.id != connection.userdata.contact_id) && (clickedContactNode.SelectedImageIndex != 0);
            //menu.Items["videoMessageToolStripMenuItem"].Enabled = (contact_tag.id != connection.userdata.contact_id) && (clickedContactNode.SelectedImageIndex != 0);
        }

        private void contextMenuStripGroup_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menu = (ContextMenuStrip)sender;
            ContactTreeTag group_tag = (ContactTreeTag)clickedContactNode.Tag;
            bool holder = false;
            foreach (GroupData gd in connection.userdata.groups)
                if (gd.group_id == group_tag.id)
                {
                    holder = (connection.userdata.contact_id == gd.creators_id);
                    break;
                }
            menu.Items["rozwiążGrupeToolStripMenuItem"].Enabled = holder;
        }

        private void contextMenuStripGroupMember_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menu = (ContextMenuStrip)sender;
            ContactTreeTag group_tag = (ContactTreeTag)clickedContactNode.Parent.Tag;
            ContactTreeTag member_tag = (ContactTreeTag)clickedContactNode.Tag;
            bool self = (connection.userdata.contact_id == member_tag.id);
            bool holder = false;
            foreach (GroupData gd in connection.userdata.groups)
                if (gd.group_id == group_tag.id)
                {
                    holder = (connection.userdata.contact_id == gd.creators_id);
                    break;
                }
            menu.Items["awansujNaLideraGrupyToolStripMenuItem"].Enabled = holder && !self;
            menu.Items["wyrzućZGrupyToolStripMenuItem"].Enabled = holder && !self;
        }

        private void archiwumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var contact = clickedContactNode;
            var contactId = ((ContactTreeTag)contact.Tag).id;

            var archive = new ArchiveForm("Archive/" + connection.userdata.contact_name + "/" + contactId + ".txt");
            archive.Visible = true;
        }
    }
}

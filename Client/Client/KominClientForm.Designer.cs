namespace Komin
{
    partial class KominClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KominClientForm));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Kontakty", 4, 4);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Grupy", 5, 5);
            this.contextMenuStripContactsNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addContactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripGroups = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dołączDoGrupyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainTabPanel = new System.Windows.Forms.TabControl();
            this.LoginTab = new System.Windows.Forms.TabPage();
            this.HomePage = new System.Windows.Forms.TabPage();
            this.tmpTest = new System.Windows.Forms.Label();
            this.tabSendTextMessage = new System.Windows.Forms.TabPage();
            this.treeViewIcons = new System.Windows.Forms.ImageList(this.components);
            this.RightMenu = new System.Windows.Forms.Panel();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.statusComboBox = new System.Windows.Forms.ComboBox();
            this.logout = new System.Windows.Forms.Button();
            this.ContactGroupBox = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.UserStatus = new System.Windows.Forms.Label();
            this.UserName = new System.Windows.Forms.Label();
            this.contactTabContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zamknijToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripContact = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wyślijPlikToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ączNaRozmoweAudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.przełączNaRozmoweWideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.zaprośDoGrupyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.archiwumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteContactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSendMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.njnnjToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mmmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStripGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.opuśćGrupeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.group_txtMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.group_audioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.group_videoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.group_wyślijPlikToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.rozwiążGrupeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripGroupMember = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.awansujNaLideraGrupyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.dodajDoKontaktówToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.wyrzućZGrupyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripContactsNode.SuspendLayout();
            this.contextMenuStripGroups.SuspendLayout();
            this.MainTabPanel.SuspendLayout();
            this.HomePage.SuspendLayout();
            this.RightMenu.SuspendLayout();
            this.ContactGroupBox.SuspendLayout();
            this.contactTabContextMenu.SuspendLayout();
            this.contextMenuStripContact.SuspendLayout();
            this.contextMenuStripGroup.SuspendLayout();
            this.contextMenuStripGroupMember.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripContactsNode
            // 
            this.contextMenuStripContactsNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addContactToolStripMenuItem});
            this.contextMenuStripContactsNode.Name = "contextMenuStripAddContact";
            this.contextMenuStripContactsNode.Size = new System.Drawing.Size(142, 26);
            // 
            // addContactToolStripMenuItem
            // 
            this.addContactToolStripMenuItem.Name = "addContactToolStripMenuItem";
            this.addContactToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.addContactToolStripMenuItem.Text = "Dodaj kontakt";
            this.addContactToolStripMenuItem.Click += new System.EventHandler(this.addContactToolStripMenuItem_Click);
            // 
            // contextMenuStripGroups
            // 
            this.contextMenuStripGroups.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addGroupToolStripMenuItem,
            this.dołączDoGrupyToolStripMenuItem});
            this.contextMenuStripGroups.Name = "contextMenuStripGroups";
            this.contextMenuStripGroups.Size = new System.Drawing.Size(154, 48);
            // 
            // addGroupToolStripMenuItem
            // 
            this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
            this.addGroupToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.addGroupToolStripMenuItem.Text = "Dodaj grupę";
            this.addGroupToolStripMenuItem.Click += new System.EventHandler(this.addNewGroupContextMenuItem);
            // 
            // dołączDoGrupyToolStripMenuItem
            // 
            this.dołączDoGrupyToolStripMenuItem.Enabled = false;
            this.dołączDoGrupyToolStripMenuItem.Name = "dołączDoGrupyToolStripMenuItem";
            this.dołączDoGrupyToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.dołączDoGrupyToolStripMenuItem.Text = "Dołącz do grupy";
            // 
            // MainTabPanel
            // 
            this.MainTabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTabPanel.Controls.Add(this.LoginTab);
            this.MainTabPanel.Controls.Add(this.HomePage);
            this.MainTabPanel.Controls.Add(this.tabSendTextMessage);
            this.MainTabPanel.ImageList = this.treeViewIcons;
            this.MainTabPanel.Location = new System.Drawing.Point(12, 12);
            this.MainTabPanel.Name = "MainTabPanel";
            this.MainTabPanel.SelectedIndex = 0;
            this.MainTabPanel.Size = new System.Drawing.Size(498, 418);
            this.MainTabPanel.TabIndex = 0;
            // 
            // LoginTab
            // 
            this.LoginTab.Location = new System.Drawing.Point(4, 23);
            this.LoginTab.Name = "LoginTab";
            this.LoginTab.Padding = new System.Windows.Forms.Padding(3);
            this.LoginTab.Size = new System.Drawing.Size(490, 391);
            this.LoginTab.TabIndex = 0;
            this.LoginTab.Text = "Logowanie i Rejestracja";
            this.LoginTab.UseVisualStyleBackColor = true;
            // 
            // HomePage
            // 
            this.HomePage.Controls.Add(this.tmpTest);
            this.HomePage.Location = new System.Drawing.Point(4, 23);
            this.HomePage.Name = "HomePage";
            this.HomePage.Padding = new System.Windows.Forms.Padding(3);
            this.HomePage.Size = new System.Drawing.Size(490, 391);
            this.HomePage.TabIndex = 1;
            this.HomePage.Text = "Strona Główna";
            this.HomePage.UseVisualStyleBackColor = true;
            this.HomePage.ParentChanged += new System.EventHandler(this.tabTmp_Test);
            // 
            // tmpTest
            // 
            this.tmpTest.AutoSize = true;
            this.tmpTest.Location = new System.Drawing.Point(18, 24);
            this.tmpTest.Name = "tmpTest";
            this.tmpTest.Size = new System.Drawing.Size(24, 13);
            this.tmpTest.TabIndex = 0;
            this.tmpTest.Text = "test";
            // 
            // tabSendTextMessage
            // 
            this.tabSendTextMessage.Location = new System.Drawing.Point(4, 23);
            this.tabSendTextMessage.Name = "tabSendTextMessage";
            this.tabSendTextMessage.Size = new System.Drawing.Size(490, 391);
            this.tabSendTextMessage.TabIndex = 1;
            this.tabSendTextMessage.Text = "Text";
            this.tabSendTextMessage.UseVisualStyleBackColor = true;
            // 
            // treeViewIcons
            // 
            this.treeViewIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewIcons.ImageStream")));
            this.treeViewIcons.TransparentColor = System.Drawing.Color.White;
            this.treeViewIcons.Images.SetKeyName(0, "user_offline16x16.bmp");
            this.treeViewIcons.Images.SetKeyName(1, "user_invisible16x16.bmp");
            this.treeViewIcons.Images.SetKeyName(2, "user_busy16x16.bmp");
            this.treeViewIcons.Images.SetKeyName(3, "user_online16x16.bmp");
            this.treeViewIcons.Images.SetKeyName(4, "contacts.bmp");
            this.treeViewIcons.Images.SetKeyName(5, "groups.bmp");
            this.treeViewIcons.Images.SetKeyName(6, "group.bmp");
            this.treeViewIcons.Images.SetKeyName(7, "group_holder_invisible.bmp");
            this.treeViewIcons.Images.SetKeyName(8, "group_holder_busy.bmp");
            this.treeViewIcons.Images.SetKeyName(9, "group_holder_online.bmp");
            // 
            // RightMenu
            // 
            this.RightMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RightMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RightMenu.Controls.Add(this.DisconnectButton);
            this.RightMenu.Controls.Add(this.statusComboBox);
            this.RightMenu.Controls.Add(this.logout);
            this.RightMenu.Controls.Add(this.ContactGroupBox);
            this.RightMenu.Controls.Add(this.UserStatus);
            this.RightMenu.Controls.Add(this.UserName);
            this.RightMenu.Location = new System.Drawing.Point(512, 12);
            this.RightMenu.Name = "RightMenu";
            this.RightMenu.Size = new System.Drawing.Size(166, 421);
            this.RightMenu.TabIndex = 1;
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DisconnectButton.Location = new System.Drawing.Point(9, 57);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(77, 24);
            this.DisconnectButton.TabIndex = 5;
            this.DisconnectButton.Text = "Rozłącz";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // statusComboBox
            // 
            this.statusComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.statusComboBox.FormattingEnabled = true;
            this.statusComboBox.Items.AddRange(new object[] {
            "Niedostępny",
            "Niewidoczny",
            "Zaraz wracam",
            "Dostępny"});
            this.statusComboBox.Location = new System.Drawing.Point(52, 30);
            this.statusComboBox.Name = "statusComboBox";
            this.statusComboBox.Size = new System.Drawing.Size(111, 21);
            this.statusComboBox.TabIndex = 4;
            this.statusComboBox.SelectedIndexChanged += new System.EventHandler(this.statusComboBox_SelectedIndexChanged);
            // 
            // logout
            // 
            this.logout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logout.Location = new System.Drawing.Point(86, 57);
            this.logout.Name = "logout";
            this.logout.Size = new System.Drawing.Size(77, 24);
            this.logout.TabIndex = 3;
            this.logout.Text = "Wyloguj";
            this.logout.UseVisualStyleBackColor = true;
            this.logout.Click += new System.EventHandler(this.logout_Click);
            // 
            // ContactGroupBox
            // 
            this.ContactGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContactGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ContactGroupBox.Controls.Add(this.treeView1);
            this.ContactGroupBox.Location = new System.Drawing.Point(3, 87);
            this.ContactGroupBox.Name = "ContactGroupBox";
            this.ContactGroupBox.Size = new System.Drawing.Size(160, 331);
            this.ContactGroupBox.TabIndex = 2;
            this.ContactGroupBox.TabStop = false;
            this.ContactGroupBox.Text = "Kontakty";
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.treeViewIcons;
            this.treeView1.Location = new System.Drawing.Point(3, 16);
            this.treeView1.Name = "treeView1";
            treeNode1.ContextMenuStrip = this.contextMenuStripContactsNode;
            treeNode1.ImageIndex = 4;
            treeNode1.Name = "Kontakty";
            treeNode1.SelectedImageIndex = 4;
            treeNode1.Text = "Kontakty";
            treeNode2.ContextMenuStrip = this.contextMenuStripGroups;
            treeNode2.ImageIndex = 5;
            treeNode2.Name = "Grupy";
            treeNode2.SelectedImageIndex = 5;
            treeNode2.Text = "Grupy";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(154, 312);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_GetNode);
            this.treeView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeView1_KeyPress);
            // 
            // UserStatus
            // 
            this.UserStatus.AutoSize = true;
            this.UserStatus.Location = new System.Drawing.Point(6, 33);
            this.UserStatus.Name = "UserStatus";
            this.UserStatus.Size = new System.Drawing.Size(40, 13);
            this.UserStatus.TabIndex = 1;
            this.UserStatus.Text = "Status:";
            // 
            // UserName
            // 
            this.UserName.AutoSize = true;
            this.UserName.Location = new System.Drawing.Point(6, 11);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(46, 13);
            this.UserName.TabIndex = 0;
            this.UserName.Text = "Nazwa: ";
            // 
            // contactTabContextMenu
            // 
            this.contactTabContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zamknijToolStripMenuItem});
            this.contactTabContextMenu.Name = "contactTabContextMenu";
            this.contactTabContextMenu.Size = new System.Drawing.Size(112, 26);
            // 
            // zamknijToolStripMenuItem
            // 
            this.zamknijToolStripMenuItem.Enabled = false;
            this.zamknijToolStripMenuItem.Name = "zamknijToolStripMenuItem";
            this.zamknijToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.zamknijToolStripMenuItem.Text = "Zamknij";
            this.zamknijToolStripMenuItem.Click += new System.EventHandler(this.onCloseContactTabClick);
            // 
            // contextMenuStripContact
            // 
            this.contextMenuStripContact.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtMessageToolStripMenuItem,
            this.audioToolStripMenuItem,
            this.videoToolStripMenuItem,
            this.wyślijPlikToolStripMenuItem,
            this.toolStripSeparator1,
            this.ączNaRozmoweAudioToolStripMenuItem,
            this.przełączNaRozmoweWideoToolStripMenuItem,
            this.toolStripSeparator2,
            this.zaprośDoGrupyToolStripMenuItem1,
            this.toolStripSeparator8,
            this.archiwumToolStripMenuItem,
            this.toolStripSeparator3,
            this.deleteContactToolStripMenuItem});
            this.contextMenuStripContact.Name = "contextMenuStripContact";
            this.contextMenuStripContact.Size = new System.Drawing.Size(208, 226);
            this.contextMenuStripContact.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripContact_Opening);
            // 
            // txtMessageToolStripMenuItem
            // 
            this.txtMessageToolStripMenuItem.Name = "txtMessageToolStripMenuItem";
            this.txtMessageToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.txtMessageToolStripMenuItem.Text = "Rozmowa tekstowa";
            this.txtMessageToolStripMenuItem.Click += new System.EventHandler(this.txtMessageToolStripMenuItem_Click);
            // 
            // audioToolStripMenuItem
            // 
            this.audioToolStripMenuItem.Enabled = false;
            this.audioToolStripMenuItem.Name = "audioToolStripMenuItem";
            this.audioToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.audioToolStripMenuItem.Text = "Rozmowa audio";
            // 
            // videoToolStripMenuItem
            // 
            this.videoToolStripMenuItem.Enabled = false;
            this.videoToolStripMenuItem.Name = "videoToolStripMenuItem";
            this.videoToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.videoToolStripMenuItem.Text = "Rozmowa wideo";
            // 
            // wyślijPlikToolStripMenuItem
            // 
            this.wyślijPlikToolStripMenuItem.Name = "wyślijPlikToolStripMenuItem";
            this.wyślijPlikToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.wyślijPlikToolStripMenuItem.Text = "Wyślij plik";
            this.wyślijPlikToolStripMenuItem.Click += new System.EventHandler(this.wyślijPlikToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(204, 6);
            // 
            // ączNaRozmoweAudioToolStripMenuItem
            // 
            this.ączNaRozmoweAudioToolStripMenuItem.Enabled = false;
            this.ączNaRozmoweAudioToolStripMenuItem.Name = "ączNaRozmoweAudioToolStripMenuItem";
            this.ączNaRozmoweAudioToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.ączNaRozmoweAudioToolStripMenuItem.Text = "Przełącz na rozmowe audio";
            // 
            // przełączNaRozmoweWideoToolStripMenuItem
            // 
            this.przełączNaRozmoweWideoToolStripMenuItem.Enabled = false;
            this.przełączNaRozmoweWideoToolStripMenuItem.Name = "przełączNaRozmoweWideoToolStripMenuItem";
            this.przełączNaRozmoweWideoToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.przełączNaRozmoweWideoToolStripMenuItem.Text = "Przełącz na rozmowe wideo";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(204, 6);
            // 
            // zaprośDoGrupyToolStripMenuItem1
            // 
            this.zaprośDoGrupyToolStripMenuItem1.Name = "zaprośDoGrupyToolStripMenuItem1";
            this.zaprośDoGrupyToolStripMenuItem1.Size = new System.Drawing.Size(207, 22);
            this.zaprośDoGrupyToolStripMenuItem1.Text = "Zaproś do grupy";
            this.zaprośDoGrupyToolStripMenuItem1.Click += new System.EventHandler(this.inviteContactToGroup_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(204, 6);
            // 
            // archiwumToolStripMenuItem
            // 
            this.archiwumToolStripMenuItem.Enabled = false;
            this.archiwumToolStripMenuItem.Name = "archiwumToolStripMenuItem";
            this.archiwumToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.archiwumToolStripMenuItem.Text = "Archiwum";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(204, 6);
            // 
            // deleteContactToolStripMenuItem
            // 
            this.deleteContactToolStripMenuItem.Name = "deleteContactToolStripMenuItem";
            this.deleteContactToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.deleteContactToolStripMenuItem.Text = "Usuń kontakt";
            this.deleteContactToolStripMenuItem.Click += new System.EventHandler(this.deleteContactToolStripMenuItem_Click);
            // 
            // toolStripMenuItemSendMessage
            // 
            this.toolStripMenuItemSendMessage.Name = "toolStripMenuItemSendMessage";
            this.toolStripMenuItemSendMessage.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemSendMessage.Text = "toolStripMenuItem1";
            // 
            // njnnjToolStripMenuItem
            // 
            this.njnnjToolStripMenuItem.Name = "njnnjToolStripMenuItem";
            this.njnnjToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.njnnjToolStripMenuItem.Text = "njnnj";
            // 
            // mmmToolStripMenuItem
            // 
            this.mmmToolStripMenuItem.Name = "mmmToolStripMenuItem";
            this.mmmToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mmmToolStripMenuItem.Text = "mmm";
            // 
            // sendFileDialog
            // 
            this.sendFileDialog.FileName = "file";
            // 
            // contextMenuStripGroup
            // 
            this.contextMenuStripGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opuśćGrupeToolStripMenuItem,
            this.toolStripSeparator4,
            this.group_txtMessageToolStripMenuItem,
            this.group_audioToolStripMenuItem,
            this.group_videoToolStripMenuItem,
            this.group_wyślijPlikToolStripMenuItem,
            this.toolStripSeparator5,
            this.rozwiążGrupeToolStripMenuItem});
            this.contextMenuStripGroup.Name = "contextMenuStripGroup";
            this.contextMenuStripGroup.Size = new System.Drawing.Size(168, 148);
            this.contextMenuStripGroup.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripGroup_Opening);
            // 
            // opuśćGrupeToolStripMenuItem
            // 
            this.opuśćGrupeToolStripMenuItem.Name = "opuśćGrupeToolStripMenuItem";
            this.opuśćGrupeToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.opuśćGrupeToolStripMenuItem.Text = "Opuść grupe";
            this.opuśćGrupeToolStripMenuItem.Click += new System.EventHandler(this.leaveGroupContextMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(164, 6);
            // 
            // group_txtMessageToolStripMenuItem
            // 
            this.group_txtMessageToolStripMenuItem.Name = "group_txtMessageToolStripMenuItem";
            this.group_txtMessageToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.group_txtMessageToolStripMenuItem.Text = "Rozmowa tekstowa";
            this.group_txtMessageToolStripMenuItem.Click += new System.EventHandler(this.txtMessageToolStripMenuItem_Click);
            // 
            // group_audioToolStripMenuItem
            // 
            this.group_audioToolStripMenuItem.Enabled = false;
            this.group_audioToolStripMenuItem.Name = "group_audioToolStripMenuItem";
            this.group_audioToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.group_audioToolStripMenuItem.Text = "Rozmowa audio";
            // 
            // group_videoToolStripMenuItem
            // 
            this.group_videoToolStripMenuItem.Enabled = false;
            this.group_videoToolStripMenuItem.Name = "group_videoToolStripMenuItem";
            this.group_videoToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.group_videoToolStripMenuItem.Text = "Rozmowa wideo";
            // 
            // group_wyślijPlikToolStripMenuItem
            // 
            this.group_wyślijPlikToolStripMenuItem.Name = "group_wyślijPlikToolStripMenuItem";
            this.group_wyślijPlikToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.group_wyślijPlikToolStripMenuItem.Text = "Wyślij plik";
            this.group_wyślijPlikToolStripMenuItem.Click += new System.EventHandler(this.wyślijPlikToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(164, 6);
            // 
            // rozwiążGrupeToolStripMenuItem
            // 
            this.rozwiążGrupeToolStripMenuItem.Name = "rozwiążGrupeToolStripMenuItem";
            this.rozwiążGrupeToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.rozwiążGrupeToolStripMenuItem.Text = "Rozwiąż grupe";
            this.rozwiążGrupeToolStripMenuItem.Click += new System.EventHandler(this.removeGroupContextMenuItem_Click);
            // 
            // contextMenuStripGroupMember
            // 
            this.contextMenuStripGroupMember.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.awansujNaLideraGrupyToolStripMenuItem,
            this.toolStripSeparator7,
            this.dodajDoKontaktówToolStripMenuItem,
            this.toolStripSeparator6,
            this.wyrzućZGrupyToolStripMenuItem});
            this.contextMenuStripGroupMember.Name = "contextMenuStripGroupMember";
            this.contextMenuStripGroupMember.Size = new System.Drawing.Size(205, 104);
            this.contextMenuStripGroupMember.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripGroupMember_Opening);
            // 
            // awansujNaLideraGrupyToolStripMenuItem
            // 
            this.awansujNaLideraGrupyToolStripMenuItem.Name = "awansujNaLideraGrupyToolStripMenuItem";
            this.awansujNaLideraGrupyToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.awansujNaLideraGrupyToolStripMenuItem.Text = "Awansuj na zarządce grupy";
            this.awansujNaLideraGrupyToolStripMenuItem.Click += new System.EventHandler(this.promoteToGroupLeader_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(183, 6);
            // 
            // dodajDoKontaktówToolStripMenuItem
            // 
            this.dodajDoKontaktówToolStripMenuItem.Name = "dodajDoKontaktówToolStripMenuItem";
            this.dodajDoKontaktówToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.dodajDoKontaktówToolStripMenuItem.Text = "Dodaj do kontaktów";
            this.dodajDoKontaktówToolStripMenuItem.Click += new System.EventHandler(this.addGroupMemberToContacts);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(183, 6);
            // 
            // wyrzućZGrupyToolStripMenuItem
            // 
            this.wyrzućZGrupyToolStripMenuItem.Name = "wyrzućZGrupyToolStripMenuItem";
            this.wyrzućZGrupyToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.wyrzućZGrupyToolStripMenuItem.Text = "Wyrzuć z grupy";
            this.wyrzućZGrupyToolStripMenuItem.Click += new System.EventHandler(this.kickFromGroupContextMenuItem_Click);
            // 
            // KominClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 456);
            this.Controls.Add(this.RightMenu);
            this.Controls.Add(this.MainTabPanel);
            this.MinimumSize = new System.Drawing.Size(700, 480);
            this.Name = "KominClientForm";
            this.Text = "Komin v0.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.onClientClosing);
            this.contextMenuStripContactsNode.ResumeLayout(false);
            this.contextMenuStripGroups.ResumeLayout(false);
            this.MainTabPanel.ResumeLayout(false);
            this.HomePage.ResumeLayout(false);
            this.HomePage.PerformLayout();
            this.RightMenu.ResumeLayout(false);
            this.RightMenu.PerformLayout();
            this.ContactGroupBox.ResumeLayout(false);
            this.contactTabContextMenu.ResumeLayout(false);
            this.contextMenuStripContact.ResumeLayout(false);
            this.contextMenuStripGroup.ResumeLayout(false);
            this.contextMenuStripGroupMember.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabPanel;
        private System.Windows.Forms.TabPage LoginTab;
        private System.Windows.Forms.TabPage HomePage;
        private System.Windows.Forms.Panel RightMenu;
        private System.Windows.Forms.Label UserStatus;
        private System.Windows.Forms.Label UserName;
        private System.Windows.Forms.Label tmpTest;
        private System.Windows.Forms.GroupBox ContactGroupBox;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabPage tabSendTextMessage;
        private System.Windows.Forms.Button logout;
        private System.Windows.Forms.ContextMenuStrip contactTabContextMenu;
        private System.Windows.Forms.ToolStripMenuItem zamknijToolStripMenuItem;
        private System.Windows.Forms.ComboBox statusComboBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripContact;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSendMessage;
        private System.Windows.Forms.ToolStripMenuItem njnnjToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mmmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem txtMessageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem audioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wyślijPlikToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog sendFileDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripContactsNode;
        private System.Windows.Forms.ToolStripMenuItem addContactToolStripMenuItem;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.ToolStripMenuItem deleteContactToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGroups;
        private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ImageList treeViewIcons;
        private System.Windows.Forms.ToolStripMenuItem ączNaRozmoweAudioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem przełączNaRozmoweWideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem dołączDoGrupyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem archiwumToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGroup;
        private System.Windows.Forms.ToolStripMenuItem opuśćGrupeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem group_txtMessageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem group_audioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem group_videoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem group_wyślijPlikToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem rozwiążGrupeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGroupMember;
        private System.Windows.Forms.ToolStripMenuItem awansujNaLideraGrupyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem dodajDoKontaktówToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem wyrzućZGrupyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zaprośDoGrupyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
    }
}


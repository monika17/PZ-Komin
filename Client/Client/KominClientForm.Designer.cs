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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Kontakty");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Grupy");
            this.MainTabPanel = new System.Windows.Forms.TabControl();
            this.LoginTab = new System.Windows.Forms.TabPage();
            this.HomePage = new System.Windows.Forms.TabPage();
            this.tmpTest = new System.Windows.Forms.Label();
            this.tabSendTextMessage = new System.Windows.Forms.TabPage();
            this.RightMenu = new System.Windows.Forms.Panel();
            this.statusComboBox = new System.Windows.Forms.ComboBox();
            this.logout = new System.Windows.Forms.Button();
            this.ContactGroupBox = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStripContactsNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addContactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UserStatus = new System.Windows.Forms.Label();
            this.UserName = new System.Windows.Forms.Label();
            this.contactTabContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zamknijToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripContact = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wyślijPlikToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSendMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.njnnjToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mmmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.MainTabPanel.SuspendLayout();
            this.HomePage.SuspendLayout();
            this.RightMenu.SuspendLayout();
            this.ContactGroupBox.SuspendLayout();
            this.contextMenuStripContactsNode.SuspendLayout();
            this.contactTabContextMenu.SuspendLayout();
            this.contextMenuStripContact.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabPanel
            // 
            this.MainTabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTabPanel.Controls.Add(this.LoginTab);
            this.MainTabPanel.Controls.Add(this.HomePage);
            this.MainTabPanel.Controls.Add(this.tabSendTextMessage);
            this.MainTabPanel.Location = new System.Drawing.Point(12, 12);
            this.MainTabPanel.Name = "MainTabPanel";
            this.MainTabPanel.SelectedIndex = 0;
            this.MainTabPanel.Size = new System.Drawing.Size(498, 418);
            this.MainTabPanel.TabIndex = 0;
            // 
            // LoginTab
            // 
            this.LoginTab.Location = new System.Drawing.Point(4, 22);
            this.LoginTab.Name = "LoginTab";
            this.LoginTab.Padding = new System.Windows.Forms.Padding(3);
            this.LoginTab.Size = new System.Drawing.Size(490, 392);
            this.LoginTab.TabIndex = 0;
            this.LoginTab.Text = "Logowanie i Rejestracja";
            this.LoginTab.UseVisualStyleBackColor = true;
            // 
            // HomePage
            // 
            this.HomePage.Controls.Add(this.tmpTest);
            this.HomePage.Location = new System.Drawing.Point(4, 22);
            this.HomePage.Name = "HomePage";
            this.HomePage.Padding = new System.Windows.Forms.Padding(3);
            this.HomePage.Size = new System.Drawing.Size(490, 392);
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
            this.tabSendTextMessage.Location = new System.Drawing.Point(4, 22);
            this.tabSendTextMessage.Name = "tabSendTextMessage";
            this.tabSendTextMessage.Size = new System.Drawing.Size(490, 392);
            this.tabSendTextMessage.TabIndex = 1;
            this.tabSendTextMessage.Text = "Text";
            this.tabSendTextMessage.UseVisualStyleBackColor = true;
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
            this.treeView1.ContextMenuStrip = this.contextMenuStripContactsNode;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 16);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Kontakty";
            treeNode1.Text = "Kontakty";
            treeNode2.Name = "Grupy";
            treeNode2.Text = "Grupy";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.treeView1.Size = new System.Drawing.Size(154, 312);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_GetNode);
            this.treeView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeView1_KeyPress);
            // 
            // contextMenuStripContactsNode
            // 
            this.contextMenuStripContactsNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addContactToolStripMenuItem});
            this.contextMenuStripContactsNode.Name = "contextMenuStripAddContact";
            this.contextMenuStripContactsNode.Size = new System.Drawing.Size(148, 26);
            // 
            // addContactToolStripMenuItem
            // 
            this.addContactToolStripMenuItem.Name = "addContactToolStripMenuItem";
            this.addContactToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.addContactToolStripMenuItem.Text = "dodaj kontakt";
            this.addContactToolStripMenuItem.Click += new System.EventHandler(this.addContactToolStripMenuItem_Click);
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
            this.contactTabContextMenu.Size = new System.Drawing.Size(118, 26);
            // 
            // zamknijToolStripMenuItem
            // 
            this.zamknijToolStripMenuItem.Name = "zamknijToolStripMenuItem";
            this.zamknijToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.zamknijToolStripMenuItem.Text = "Zamknij";
            this.zamknijToolStripMenuItem.Click += new System.EventHandler(this.onCloseContactTabClick);
            // 
            // contextMenuStripContact
            // 
            this.contextMenuStripContact.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtMessageToolStripMenuItem,
            this.audioToolStripMenuItem,
            this.wideoToolStripMenuItem,
            this.wyślijPlikToolStripMenuItem});
            this.contextMenuStripContact.Name = "contextMenuStripContact";
            this.contextMenuStripContact.Size = new System.Drawing.Size(174, 92);
            this.contextMenuStripContact.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // txtMessageToolStripMenuItem
            // 
            this.txtMessageToolStripMenuItem.Name = "txtMessageToolStripMenuItem";
            this.txtMessageToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.txtMessageToolStripMenuItem.Text = "rozmowa tekstowa";
            this.txtMessageToolStripMenuItem.Click += new System.EventHandler(this.txtMessageToolStripMenuItem_Click);
            // 
            // audioToolStripMenuItem
            // 
            this.audioToolStripMenuItem.Enabled = false;
            this.audioToolStripMenuItem.Name = "audioToolStripMenuItem";
            this.audioToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.audioToolStripMenuItem.Text = "rozmowa audio";
            // 
            // wideoToolStripMenuItem
            // 
            this.wideoToolStripMenuItem.Enabled = false;
            this.wideoToolStripMenuItem.Name = "wideoToolStripMenuItem";
            this.wideoToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wideoToolStripMenuItem.Text = "rozmowa wideo";
            // 
            // wyślijPlikToolStripMenuItem
            // 
            this.wyślijPlikToolStripMenuItem.Name = "wyślijPlikToolStripMenuItem";
            this.wyślijPlikToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.wyślijPlikToolStripMenuItem.Text = "wyślij plik";
            this.wyślijPlikToolStripMenuItem.Click += new System.EventHandler(this.wyślijPlikToolStripMenuItem_Click);
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
            this.MainTabPanel.ResumeLayout(false);
            this.HomePage.ResumeLayout(false);
            this.HomePage.PerformLayout();
            this.RightMenu.ResumeLayout(false);
            this.RightMenu.PerformLayout();
            this.ContactGroupBox.ResumeLayout(false);
            this.contextMenuStripContactsNode.ResumeLayout(false);
            this.contactTabContextMenu.ResumeLayout(false);
            this.contextMenuStripContact.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripMenuItem wideoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wyślijPlikToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog sendFileDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripContactsNode;
        private System.Windows.Forms.ToolStripMenuItem addContactToolStripMenuItem;
        private System.Windows.Forms.Button DisconnectButton;
    }
}


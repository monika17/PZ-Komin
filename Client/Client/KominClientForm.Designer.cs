﻿namespace Komin
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
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Kontakty");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Grupy");
            this.MainTabPanel = new System.Windows.Forms.TabControl();
            this.LoginTab = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LoginGroupBox = new System.Windows.Forms.GroupBox();
            this.LoginNametextBox = new System.Windows.Forms.TextBox();
            this.LoginPass = new System.Windows.Forms.Label();
            this.LoginName = new System.Windows.Forms.Label();
            this.LoginButton = new System.Windows.Forms.Button();
            this.LoginPasstextBox = new System.Windows.Forms.TextBox();
            this.RegisterGroupBox = new System.Windows.Forms.GroupBox();
            this.RegisterNameTextBox = new System.Windows.Forms.TextBox();
            this.RegisterPassTextBox = new System.Windows.Forms.TextBox();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.RegisterNamelabel = new System.Windows.Forms.Label();
            this.RegisterPasslabel = new System.Windows.Forms.Label();
            this.HomePage = new System.Windows.Forms.TabPage();
            this.tmpTest = new System.Windows.Forms.Label();
            this.tabSendTextMessage = new System.Windows.Forms.TabPage();
            this.RightMenu = new System.Windows.Forms.Panel();
            this.statusComboBox = new System.Windows.Forms.ComboBox();
            this.logout = new System.Windows.Forms.Button();
            this.ContactGroupBox = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.UserStatus = new System.Windows.Forms.Label();
            this.UserName = new System.Windows.Forms.Label();
            this.contactTabContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zamknijToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainTabPanel.SuspendLayout();
            this.LoginTab.SuspendLayout();
            this.panel1.SuspendLayout();
            this.LoginGroupBox.SuspendLayout();
            this.RegisterGroupBox.SuspendLayout();
            this.HomePage.SuspendLayout();
            this.RightMenu.SuspendLayout();
            this.ContactGroupBox.SuspendLayout();
            this.contactTabContextMenu.SuspendLayout();
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
            this.LoginTab.Controls.Add(this.panel1);
            this.LoginTab.Location = new System.Drawing.Point(4, 22);
            this.LoginTab.Name = "LoginTab";
            this.LoginTab.Padding = new System.Windows.Forms.Padding(3);
            this.LoginTab.Size = new System.Drawing.Size(490, 392);
            this.LoginTab.TabIndex = 0;
            this.LoginTab.Text = "Logowanie i Rejestracja";
            this.LoginTab.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.LoginGroupBox);
            this.panel1.Controls.Add(this.RegisterGroupBox);
            this.panel1.Location = new System.Drawing.Point(33, 143);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(428, 104);
            this.panel1.TabIndex = 14;
            // 
            // LoginGroupBox
            // 
            this.LoginGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LoginGroupBox.Controls.Add(this.LoginNametextBox);
            this.LoginGroupBox.Controls.Add(this.LoginPass);
            this.LoginGroupBox.Controls.Add(this.LoginName);
            this.LoginGroupBox.Controls.Add(this.LoginButton);
            this.LoginGroupBox.Controls.Add(this.LoginPasstextBox);
            this.LoginGroupBox.Location = new System.Drawing.Point(0, 0);
            this.LoginGroupBox.Name = "LoginGroupBox";
            this.LoginGroupBox.Size = new System.Drawing.Size(184, 104);
            this.LoginGroupBox.TabIndex = 12;
            this.LoginGroupBox.TabStop = false;
            this.LoginGroupBox.Text = "Logowanie";
            // 
            // LoginNametextBox
            // 
            this.LoginNametextBox.Location = new System.Drawing.Point(55, 19);
            this.LoginNametextBox.Name = "LoginNametextBox";
            this.LoginNametextBox.Size = new System.Drawing.Size(123, 20);
            this.LoginNametextBox.TabIndex = 4;
            // 
            // LoginPass
            // 
            this.LoginPass.AutoSize = true;
            this.LoginPass.Location = new System.Drawing.Point(6, 48);
            this.LoginPass.Name = "LoginPass";
            this.LoginPass.Size = new System.Drawing.Size(42, 13);
            this.LoginPass.TabIndex = 3;
            this.LoginPass.Text = "Hasło: ";
            // 
            // LoginName
            // 
            this.LoginName.AutoSize = true;
            this.LoginName.Location = new System.Drawing.Point(6, 22);
            this.LoginName.Name = "LoginName";
            this.LoginName.Size = new System.Drawing.Size(43, 13);
            this.LoginName.TabIndex = 2;
            this.LoginName.Text = "Nazwa:";
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(87, 71);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(91, 26);
            this.LoginButton.TabIndex = 10;
            this.LoginButton.Text = "Zaloguj";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // LoginPasstextBox
            // 
            this.LoginPasstextBox.Location = new System.Drawing.Point(55, 45);
            this.LoginPasstextBox.Name = "LoginPasstextBox";
            this.LoginPasstextBox.Size = new System.Drawing.Size(123, 20);
            this.LoginPasstextBox.TabIndex = 5;
            this.LoginPasstextBox.UseSystemPasswordChar = true;
            // 
            // RegisterGroupBox
            // 
            this.RegisterGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RegisterGroupBox.Controls.Add(this.RegisterNameTextBox);
            this.RegisterGroupBox.Controls.Add(this.RegisterPassTextBox);
            this.RegisterGroupBox.Controls.Add(this.RegisterButton);
            this.RegisterGroupBox.Controls.Add(this.RegisterNamelabel);
            this.RegisterGroupBox.Controls.Add(this.RegisterPasslabel);
            this.RegisterGroupBox.Location = new System.Drawing.Point(244, 0);
            this.RegisterGroupBox.Name = "RegisterGroupBox";
            this.RegisterGroupBox.Size = new System.Drawing.Size(184, 104);
            this.RegisterGroupBox.TabIndex = 13;
            this.RegisterGroupBox.TabStop = false;
            this.RegisterGroupBox.Text = "Zarejestruj";
            // 
            // RegisterNameTextBox
            // 
            this.RegisterNameTextBox.Location = new System.Drawing.Point(55, 19);
            this.RegisterNameTextBox.Name = "RegisterNameTextBox";
            this.RegisterNameTextBox.Size = new System.Drawing.Size(123, 20);
            this.RegisterNameTextBox.TabIndex = 8;
            // 
            // RegisterPassTextBox
            // 
            this.RegisterPassTextBox.Location = new System.Drawing.Point(55, 45);
            this.RegisterPassTextBox.Name = "RegisterPassTextBox";
            this.RegisterPassTextBox.Size = new System.Drawing.Size(123, 20);
            this.RegisterPassTextBox.TabIndex = 9;
            this.RegisterPassTextBox.UseSystemPasswordChar = true;
            // 
            // RegisterButton
            // 
            this.RegisterButton.Location = new System.Drawing.Point(87, 71);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(91, 26);
            this.RegisterButton.TabIndex = 11;
            this.RegisterButton.Text = "Zarejestruj";
            this.RegisterButton.UseVisualStyleBackColor = true;
            // 
            // RegisterNamelabel
            // 
            this.RegisterNamelabel.AutoSize = true;
            this.RegisterNamelabel.Location = new System.Drawing.Point(6, 22);
            this.RegisterNamelabel.Name = "RegisterNamelabel";
            this.RegisterNamelabel.Size = new System.Drawing.Size(43, 13);
            this.RegisterNamelabel.TabIndex = 6;
            this.RegisterNamelabel.Text = "Nazwa:";
            // 
            // RegisterPasslabel
            // 
            this.RegisterPasslabel.AutoSize = true;
            this.RegisterPasslabel.Location = new System.Drawing.Point(6, 48);
            this.RegisterPasslabel.Name = "RegisterPasslabel";
            this.RegisterPasslabel.Size = new System.Drawing.Size(42, 13);
            this.RegisterPasslabel.TabIndex = 7;
            this.RegisterPasslabel.Text = "Hasło: ";
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
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 16);
            this.treeView1.Name = "treeView1";
            treeNode5.Name = "Kontakty";
            treeNode5.Text = "Kontakty";
            treeNode6.Name = "Grupy";
            treeNode6.Text = "Grupy";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6});
            this.treeView1.Size = new System.Drawing.Size(154, 312);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeDoubleClick);
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
            this.contactTabContextMenu.Size = new System.Drawing.Size(118, 26);
            // 
            // zamknijToolStripMenuItem
            // 
            this.zamknijToolStripMenuItem.Name = "zamknijToolStripMenuItem";
            this.zamknijToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.zamknijToolStripMenuItem.Text = "Zamknij";
            this.zamknijToolStripMenuItem.Click += new System.EventHandler(this.onCloseContactTabClick);
            // 
            // KominClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 442);
            this.Controls.Add(this.RightMenu);
            this.Controls.Add(this.MainTabPanel);
            this.MinimumSize = new System.Drawing.Size(700, 480);
            this.Name = "KominClientForm";
            this.Text = "Komin v0.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.onClientClosing);
            this.MainTabPanel.ResumeLayout(false);
            this.LoginTab.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.LoginGroupBox.ResumeLayout(false);
            this.LoginGroupBox.PerformLayout();
            this.RegisterGroupBox.ResumeLayout(false);
            this.RegisterGroupBox.PerformLayout();
            this.HomePage.ResumeLayout(false);
            this.HomePage.PerformLayout();
            this.RightMenu.ResumeLayout(false);
            this.RightMenu.PerformLayout();
            this.ContactGroupBox.ResumeLayout(false);
            this.contactTabContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabPanel;
        private System.Windows.Forms.TabPage LoginTab;
        private System.Windows.Forms.TabPage HomePage;
        private System.Windows.Forms.Panel RightMenu;
        private System.Windows.Forms.TextBox RegisterPassTextBox;
        private System.Windows.Forms.TextBox RegisterNameTextBox;
        private System.Windows.Forms.Label RegisterPasslabel;
        private System.Windows.Forms.Label RegisterNamelabel;
        private System.Windows.Forms.TextBox LoginPasstextBox;
        private System.Windows.Forms.TextBox LoginNametextBox;
        private System.Windows.Forms.Label LoginPass;
        private System.Windows.Forms.Label LoginName;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Label UserStatus;
        private System.Windows.Forms.Label UserName;
        private System.Windows.Forms.Label tmpTest;
        private System.Windows.Forms.GroupBox ContactGroupBox;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabPage tabSendTextMessage;
        private System.Windows.Forms.Button logout;
        private System.Windows.Forms.GroupBox RegisterGroupBox;
        private System.Windows.Forms.GroupBox LoginGroupBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip contactTabContextMenu;
        private System.Windows.Forms.ToolStripMenuItem zamknijToolStripMenuItem;
        private System.Windows.Forms.ComboBox statusComboBox;
    }
}

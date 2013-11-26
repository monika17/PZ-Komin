namespace Client
{
    partial class Form1
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
            this.MainTabPanel = new System.Windows.Forms.TabControl();
            this.LoginTab = new System.Windows.Forms.TabPage();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.LoginButton = new System.Windows.Forms.Button();
            this.RegisterPassTextBox = new System.Windows.Forms.TextBox();
            this.RegisterNameTextBox = new System.Windows.Forms.TextBox();
            this.RegisterPasslabel = new System.Windows.Forms.Label();
            this.RegisterNamelabel = new System.Windows.Forms.Label();
            this.LoginPasstextBox = new System.Windows.Forms.TextBox();
            this.LoginNametextBox = new System.Windows.Forms.TextBox();
            this.LoginPass = new System.Windows.Forms.Label();
            this.LoginName = new System.Windows.Forms.Label();
            this.RegisterLabel = new System.Windows.Forms.Label();
            this.LoginLabel = new System.Windows.Forms.Label();
            this.tmpTab = new System.Windows.Forms.TabPage();
            this.tmpTest = new System.Windows.Forms.Label();
            this.RightMenu = new System.Windows.Forms.Panel();
            this.UserStatus = new System.Windows.Forms.Label();
            this.UserName = new System.Windows.Forms.Label();
            this.ContactGroupBox = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.MainTabPanel.SuspendLayout();
            this.LoginTab.SuspendLayout();
            this.tmpTab.SuspendLayout();
            this.RightMenu.SuspendLayout();
            this.ContactGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabPanel
            // 
            this.MainTabPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTabPanel.Controls.Add(this.LoginTab);
            this.MainTabPanel.Location = new System.Drawing.Point(12, 12);
            this.MainTabPanel.Name = "MainTabPanel";
            this.MainTabPanel.SelectedIndex = 0;
            this.MainTabPanel.Size = new System.Drawing.Size(525, 427);
            this.MainTabPanel.TabIndex = 0;
            // 
            // LoginTab
            // 
            this.LoginTab.Controls.Add(this.RegisterButton);
            this.LoginTab.Controls.Add(this.LoginButton);
            this.LoginTab.Controls.Add(this.RegisterPassTextBox);
            this.LoginTab.Controls.Add(this.RegisterNameTextBox);
            this.LoginTab.Controls.Add(this.RegisterPasslabel);
            this.LoginTab.Controls.Add(this.RegisterNamelabel);
            this.LoginTab.Controls.Add(this.LoginPasstextBox);
            this.LoginTab.Controls.Add(this.LoginNametextBox);
            this.LoginTab.Controls.Add(this.LoginPass);
            this.LoginTab.Controls.Add(this.LoginName);
            this.LoginTab.Controls.Add(this.RegisterLabel);
            this.LoginTab.Controls.Add(this.LoginLabel);
            this.LoginTab.Location = new System.Drawing.Point(4, 22);
            this.LoginTab.Name = "LoginTab";
            this.LoginTab.Padding = new System.Windows.Forms.Padding(3);
            this.LoginTab.Size = new System.Drawing.Size(517, 401);
            this.LoginTab.TabIndex = 0;
            this.LoginTab.Text = "lgowoanie";
            this.LoginTab.UseVisualStyleBackColor = true;
            // 
            // RegisterButton
            // 
            this.RegisterButton.Location = new System.Drawing.Point(284, 158);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(91, 26);
            this.RegisterButton.TabIndex = 11;
            this.RegisterButton.Text = "zarejestruj";
            this.RegisterButton.UseVisualStyleBackColor = true;
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(80, 158);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(91, 26);
            this.LoginButton.TabIndex = 10;
            this.LoginButton.Text = "zaloguj";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // RegisterPassTextBox
            // 
            this.RegisterPassTextBox.Location = new System.Drawing.Point(284, 101);
            this.RegisterPassTextBox.Name = "RegisterPassTextBox";
            this.RegisterPassTextBox.Size = new System.Drawing.Size(100, 20);
            this.RegisterPassTextBox.TabIndex = 9;
            // 
            // RegisterNameTextBox
            // 
            this.RegisterNameTextBox.Location = new System.Drawing.Point(284, 76);
            this.RegisterNameTextBox.Name = "RegisterNameTextBox";
            this.RegisterNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.RegisterNameTextBox.TabIndex = 8;
            // 
            // RegisterPasslabel
            // 
            this.RegisterPasslabel.AutoSize = true;
            this.RegisterPasslabel.Location = new System.Drawing.Point(226, 108);
            this.RegisterPasslabel.Name = "RegisterPasslabel";
            this.RegisterPasslabel.Size = new System.Drawing.Size(40, 13);
            this.RegisterPasslabel.TabIndex = 7;
            this.RegisterPasslabel.Text = "hasło: ";
            // 
            // RegisterNamelabel
            // 
            this.RegisterNamelabel.AutoSize = true;
            this.RegisterNamelabel.Location = new System.Drawing.Point(223, 76);
            this.RegisterNamelabel.Name = "RegisterNamelabel";
            this.RegisterNamelabel.Size = new System.Drawing.Size(43, 13);
            this.RegisterNamelabel.TabIndex = 6;
            this.RegisterNamelabel.Text = "Nazwa:";
            // 
            // LoginPasstextBox
            // 
            this.LoginPasstextBox.Location = new System.Drawing.Point(80, 108);
            this.LoginPasstextBox.Name = "LoginPasstextBox";
            this.LoginPasstextBox.Size = new System.Drawing.Size(100, 20);
            this.LoginPasstextBox.TabIndex = 5;
            // 
            // LoginNametextBox
            // 
            this.LoginNametextBox.Location = new System.Drawing.Point(80, 83);
            this.LoginNametextBox.Name = "LoginNametextBox";
            this.LoginNametextBox.Size = new System.Drawing.Size(100, 20);
            this.LoginNametextBox.TabIndex = 4;
            // 
            // LoginPass
            // 
            this.LoginPass.AutoSize = true;
            this.LoginPass.Location = new System.Drawing.Point(22, 115);
            this.LoginPass.Name = "LoginPass";
            this.LoginPass.Size = new System.Drawing.Size(40, 13);
            this.LoginPass.TabIndex = 3;
            this.LoginPass.Text = "hasło: ";
            // 
            // LoginName
            // 
            this.LoginName.AutoSize = true;
            this.LoginName.Location = new System.Drawing.Point(19, 83);
            this.LoginName.Name = "LoginName";
            this.LoginName.Size = new System.Drawing.Size(43, 13);
            this.LoginName.TabIndex = 2;
            this.LoginName.Text = "Nazwa:";
            // 
            // RegisterLabel
            // 
            this.RegisterLabel.AutoSize = true;
            this.RegisterLabel.Location = new System.Drawing.Point(281, 53);
            this.RegisterLabel.Name = "RegisterLabel";
            this.RegisterLabel.Size = new System.Drawing.Size(60, 13);
            this.RegisterLabel.TabIndex = 1;
            this.RegisterLabel.Text = "Rejestracja";
            // 
            // LoginLabel
            // 
            this.LoginLabel.AutoSize = true;
            this.LoginLabel.Location = new System.Drawing.Point(77, 53);
            this.LoginLabel.Name = "LoginLabel";
            this.LoginLabel.Size = new System.Drawing.Size(59, 13);
            this.LoginLabel.TabIndex = 0;
            this.LoginLabel.Text = "Logowanie";
            // 
            // tmpTab
            // 
            this.tmpTab.Controls.Add(this.tmpTest);
            this.tmpTab.Location = new System.Drawing.Point(4, 22);
            this.tmpTab.Name = "tmpTab";
            this.tmpTab.Padding = new System.Windows.Forms.Padding(3);
            this.tmpTab.Size = new System.Drawing.Size(517, 401);
            this.tmpTab.TabIndex = 1;
            this.tmpTab.Text = "tmpTab";
            this.tmpTab.UseVisualStyleBackColor = true;
            this.tmpTab.ParentChanged += new System.EventHandler(this.tabTmp_Test);
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
            // RightMenu
            // 
            this.RightMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RightMenu.Controls.Add(this.ContactGroupBox);
            this.RightMenu.Controls.Add(this.UserStatus);
            this.RightMenu.Controls.Add(this.UserName);
            this.RightMenu.Location = new System.Drawing.Point(543, 34);
            this.RightMenu.Name = "RightMenu";
            this.RightMenu.Size = new System.Drawing.Size(145, 404);
            this.RightMenu.TabIndex = 1;
            // 
            // UserStatus
            // 
            this.UserStatus.AutoSize = true;
            this.UserStatus.Location = new System.Drawing.Point(15, 37);
            this.UserStatus.Name = "UserStatus";
            this.UserStatus.Size = new System.Drawing.Size(40, 13);
            this.UserStatus.TabIndex = 1;
            this.UserStatus.Text = "Status:";
            // 
            // UserName
            // 
            this.UserName.AutoSize = true;
            this.UserName.Location = new System.Drawing.Point(15, 15);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(46, 13);
            this.UserName.TabIndex = 0;
            this.UserName.Text = "Nazwa: ";
            // 
            // ContactGroupBox
            // 
            this.ContactGroupBox.Controls.Add(this.treeView1);
            this.ContactGroupBox.Location = new System.Drawing.Point(3, 66);
            this.ContactGroupBox.Name = "ContactGroupBox";
            this.ContactGroupBox.Size = new System.Drawing.Size(139, 335);
            this.ContactGroupBox.TabIndex = 2;
            this.ContactGroupBox.TabStop = false;
            this.ContactGroupBox.Text = "Kontakty";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(0, 19);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(138, 315);
            this.treeView1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 451);
            this.Controls.Add(this.RightMenu);
            this.Controls.Add(this.MainTabPanel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.MainTabPanel.ResumeLayout(false);
            this.LoginTab.ResumeLayout(false);
            this.LoginTab.PerformLayout();
            this.tmpTab.ResumeLayout(false);
            this.tmpTab.PerformLayout();
            this.RightMenu.ResumeLayout(false);
            this.RightMenu.PerformLayout();
            this.ContactGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabPanel;
        private System.Windows.Forms.TabPage LoginTab;
        private System.Windows.Forms.TabPage tmpTab;
        private System.Windows.Forms.Panel RightMenu;
        private System.Windows.Forms.TextBox RegisterPassTextBox;
        private System.Windows.Forms.TextBox RegisterNameTextBox;
        private System.Windows.Forms.Label RegisterPasslabel;
        private System.Windows.Forms.Label RegisterNamelabel;
        private System.Windows.Forms.TextBox LoginPasstextBox;
        private System.Windows.Forms.TextBox LoginNametextBox;
        private System.Windows.Forms.Label LoginPass;
        private System.Windows.Forms.Label LoginName;
        private System.Windows.Forms.Label RegisterLabel;
        private System.Windows.Forms.Label LoginLabel;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Label UserStatus;
        private System.Windows.Forms.Label UserName;
        private System.Windows.Forms.Label tmpTest;
        private System.Windows.Forms.GroupBox ContactGroupBox;
        private System.Windows.Forms.TreeView treeView1;
    }
}


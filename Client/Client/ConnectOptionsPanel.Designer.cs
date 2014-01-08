namespace Komin
{
    partial class ConnectOptionsPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoginPanel = new System.Windows.Forms.Panel();
            this.LoginGroupBox = new System.Windows.Forms.GroupBox();
            this.buttonNewUser = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.LoginNametextBox = new System.Windows.Forms.TextBox();
            this.LoginButton = new System.Windows.Forms.Button();
            this.LoginPass = new System.Windows.Forms.Label();
            this.LoginName = new System.Windows.Forms.Label();
            this.LoginPasstextBox = new System.Windows.Forms.TextBox();
            this.ConnectPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxhostIp = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.labelHostIp = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.LoginPanel.SuspendLayout();
            this.LoginGroupBox.SuspendLayout();
            this.ConnectPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoginPanel
            // 
            this.LoginPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LoginPanel.Controls.Add(this.LoginGroupBox);
            this.LoginPanel.Location = new System.Drawing.Point(103, 114);
            this.LoginPanel.Name = "LoginPanel";
            this.LoginPanel.Size = new System.Drawing.Size(328, 182);
            this.LoginPanel.TabIndex = 4;
            this.LoginPanel.Visible = false;
            // 
            // LoginGroupBox
            // 
            this.LoginGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LoginGroupBox.Controls.Add(this.buttonNewUser);
            this.LoginGroupBox.Controls.Add(this.BackButton);
            this.LoginGroupBox.Controls.Add(this.LoginNametextBox);
            this.LoginGroupBox.Controls.Add(this.LoginButton);
            this.LoginGroupBox.Controls.Add(this.LoginPass);
            this.LoginGroupBox.Controls.Add(this.LoginName);
            this.LoginGroupBox.Controls.Add(this.LoginPasstextBox);
            this.LoginGroupBox.Location = new System.Drawing.Point(12, 17);
            this.LoginGroupBox.Name = "LoginGroupBox";
            this.LoginGroupBox.Size = new System.Drawing.Size(313, 120);
            this.LoginGroupBox.TabIndex = 13;
            this.LoginGroupBox.TabStop = false;
            this.LoginGroupBox.Text = "Logowanie";
            // 
            // buttonNewUser
            // 
            this.buttonNewUser.Location = new System.Drawing.Point(199, 88);
            this.buttonNewUser.Name = "buttonNewUser";
            this.buttonNewUser.Size = new System.Drawing.Size(108, 26);
            this.buttonNewUser.TabIndex = 14;
            this.buttonNewUser.Text = "Dodaj nowe konto";
            this.buttonNewUser.UseVisualStyleBackColor = true;
            this.buttonNewUser.Click += new System.EventHandler(this.buttonNewUser_Click);
            // 
            // BackButton
            // 
            this.BackButton.Location = new System.Drawing.Point(6, 88);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(91, 26);
            this.BackButton.TabIndex = 15;
            this.BackButton.Text = "Wstecz";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // LoginNametextBox
            // 
            this.LoginNametextBox.Location = new System.Drawing.Point(131, 19);
            this.LoginNametextBox.Name = "LoginNametextBox";
            this.LoginNametextBox.Size = new System.Drawing.Size(123, 20);
            this.LoginNametextBox.TabIndex = 4;
            this.LoginNametextBox.TextChanged += new System.EventHandler(this.LoginNametextBox_TextChanged);
            // 
            // LoginButton
            // 
            this.LoginButton.Enabled = false;
            this.LoginButton.Location = new System.Drawing.Point(103, 88);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(90, 26);
            this.LoginButton.TabIndex = 10;
            this.LoginButton.Text = "Zaloguj";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // LoginPass
            // 
            this.LoginPass.AutoSize = true;
            this.LoginPass.Location = new System.Drawing.Point(81, 48);
            this.LoginPass.Name = "LoginPass";
            this.LoginPass.Size = new System.Drawing.Size(42, 13);
            this.LoginPass.TabIndex = 3;
            this.LoginPass.Text = "Hasło: ";
            // 
            // LoginName
            // 
            this.LoginName.AutoSize = true;
            this.LoginName.Location = new System.Drawing.Point(81, 22);
            this.LoginName.Name = "LoginName";
            this.LoginName.Size = new System.Drawing.Size(43, 13);
            this.LoginName.TabIndex = 2;
            this.LoginName.Text = "Nazwa:";
            // 
            // LoginPasstextBox
            // 
            this.LoginPasstextBox.Location = new System.Drawing.Point(131, 45);
            this.LoginPasstextBox.Name = "LoginPasstextBox";
            this.LoginPasstextBox.Size = new System.Drawing.Size(123, 20);
            this.LoginPasstextBox.TabIndex = 5;
            this.LoginPasstextBox.UseSystemPasswordChar = true;
            this.LoginPasstextBox.TextChanged += new System.EventHandler(this.LoginPasstextBox_TextChanged);
            // 
            // ConnectPanel
            // 
            this.ConnectPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ConnectPanel.Controls.Add(this.groupBox1);
            this.ConnectPanel.Location = new System.Drawing.Point(103, 3);
            this.ConnectPanel.Name = "ConnectPanel";
            this.ConnectPanel.Size = new System.Drawing.Size(328, 105);
            this.ConnectPanel.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.textBoxPort);
            this.groupBox1.Controls.Add(this.buttonConnect);
            this.groupBox1.Controls.Add(this.textBoxhostIp);
            this.groupBox1.Controls.Add(this.labelPort);
            this.groupBox1.Controls.Add(this.labelHostIp);
            this.groupBox1.Location = new System.Drawing.Point(25, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 93);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Połącz z serwerem";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(180, 22);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(76, 20);
            this.textBoxPort.TabIndex = 24;
            this.textBoxPort.TextChanged += new System.EventHandler(this.textBoxPort_TextChanged);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Enabled = false;
            this.buttonConnect.Location = new System.Drawing.Point(192, 59);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(87, 28);
            this.buttonConnect.TabIndex = 25;
            this.buttonConnect.Text = "Dalej";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // textBoxhostIp
            // 
            this.textBoxhostIp.Location = new System.Drawing.Point(58, 22);
            this.textBoxhostIp.Name = "textBoxhostIp";
            this.textBoxhostIp.Size = new System.Drawing.Size(76, 20);
            this.textBoxhostIp.TabIndex = 23;
            this.textBoxhostIp.TextChanged += new System.EventHandler(this.textBoxhostIp_TextChanged);
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(143, 25);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(31, 13);
            this.labelPort.TabIndex = 22;
            this.labelPort.Text = "port: ";
            // 
            // labelHostIp
            // 
            this.labelHostIp.AutoSize = true;
            this.labelHostIp.Location = new System.Drawing.Point(6, 25);
            this.labelHostIp.Name = "labelHostIp";
            this.labelHostIp.Size = new System.Drawing.Size(46, 13);
            this.labelHostIp.TabIndex = 21;
            this.labelHostIp.Text = "host IP: ";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(46, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ConnectOptionsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LoginPanel);
            this.Controls.Add(this.ConnectPanel);
            this.Name = "ConnectOptionsPanel";
            this.Size = new System.Drawing.Size(534, 335);
            this.LoginPanel.ResumeLayout(false);
            this.LoginGroupBox.ResumeLayout(false);
            this.LoginGroupBox.PerformLayout();
            this.ConnectPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel LoginPanel;
        private System.Windows.Forms.Button buttonNewUser;
        private System.Windows.Forms.GroupBox LoginGroupBox;
        private System.Windows.Forms.TextBox LoginNametextBox;
        private System.Windows.Forms.Label LoginPass;
        private System.Windows.Forms.Label LoginName;
        private System.Windows.Forms.TextBox LoginPasstextBox;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Panel ConnectPanel;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxhostIp;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Label labelHostIp;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
    }
}

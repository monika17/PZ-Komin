namespace Komin
{
    partial class ConnectForm
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
            this.ConnectPanel = new System.Windows.Forms.Panel();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxhostIp = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.labelHostIp = new System.Windows.Forms.Label();
            this.LoginPanel = new System.Windows.Forms.Panel();
            this.buttonNewUser = new System.Windows.Forms.Button();
            this.LoginGroupBox = new System.Windows.Forms.GroupBox();
            this.LoginNametextBox = new System.Windows.Forms.TextBox();
            this.LoginPass = new System.Windows.Forms.Label();
            this.LoginName = new System.Windows.Forms.Label();
            this.LoginPasstextBox = new System.Windows.Forms.TextBox();
            this.LoginButton = new System.Windows.Forms.Button();
            this.RegisterPanel = new System.Windows.Forms.Panel();
            this.RegisterGroupBox = new System.Windows.Forms.GroupBox();
            this.RegisterNameAcceptableLabel = new System.Windows.Forms.Label();
            this.RegisterNameTextBox = new System.Windows.Forms.TextBox();
            this.RegisterPassTextBox = new System.Windows.Forms.TextBox();
            this.RegisterNamelabel = new System.Windows.Forms.Label();
            this.RegisterPasslabel = new System.Windows.Forms.Label();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.RegisterNameValidityTimer = new System.Windows.Forms.Timer(this.components);
            this.RegisterNameTextBoxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ConnectPanel.SuspendLayout();
            this.LoginPanel.SuspendLayout();
            this.LoginGroupBox.SuspendLayout();
            this.RegisterPanel.SuspendLayout();
            this.RegisterGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectPanel
            // 
            this.ConnectPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ConnectPanel.Controls.Add(this.buttonConnect);
            this.ConnectPanel.Controls.Add(this.textBoxPort);
            this.ConnectPanel.Controls.Add(this.textBoxhostIp);
            this.ConnectPanel.Controls.Add(this.labelPort);
            this.ConnectPanel.Controls.Add(this.labelHostIp);
            this.ConnectPanel.Location = new System.Drawing.Point(9, 12);
            this.ConnectPanel.Name = "ConnectPanel";
            this.ConnectPanel.Size = new System.Drawing.Size(328, 80);
            this.ConnectPanel.TabIndex = 0;
            this.ConnectPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ConnectPanel_Paint);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(238, 49);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(87, 28);
            this.buttonConnect.TabIndex = 25;
            this.buttonConnect.Text = "Dalej";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(216, 14);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(76, 20);
            this.textBoxPort.TabIndex = 24;
            // 
            // textBoxhostIp
            // 
            this.textBoxhostIp.Location = new System.Drawing.Point(77, 14);
            this.textBoxhostIp.Name = "textBoxhostIp";
            this.textBoxhostIp.Size = new System.Drawing.Size(76, 20);
            this.textBoxhostIp.TabIndex = 23;
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(168, 17);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(31, 13);
            this.labelPort.TabIndex = 22;
            this.labelPort.Text = "port: ";
            // 
            // labelHostIp
            // 
            this.labelHostIp.AutoSize = true;
            this.labelHostIp.Location = new System.Drawing.Point(29, 17);
            this.labelHostIp.Name = "labelHostIp";
            this.labelHostIp.Size = new System.Drawing.Size(46, 13);
            this.labelHostIp.TabIndex = 21;
            this.labelHostIp.Text = "host IP: ";
            // 
            // LoginPanel
            // 
            this.LoginPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LoginPanel.Controls.Add(this.buttonNewUser);
            this.LoginPanel.Controls.Add(this.LoginGroupBox);
            this.LoginPanel.Controls.Add(this.LoginButton);
            this.LoginPanel.Location = new System.Drawing.Point(9, 98);
            this.LoginPanel.Name = "LoginPanel";
            this.LoginPanel.Size = new System.Drawing.Size(328, 182);
            this.LoginPanel.TabIndex = 1;
            this.LoginPanel.Visible = false;
            // 
            // buttonNewUser
            // 
            this.buttonNewUser.Location = new System.Drawing.Point(171, 126);
            this.buttonNewUser.Name = "buttonNewUser";
            this.buttonNewUser.Size = new System.Drawing.Size(121, 26);
            this.buttonNewUser.TabIndex = 14;
            this.buttonNewUser.Text = "Dodaj nowe konto";
            this.buttonNewUser.UseVisualStyleBackColor = true;
            this.buttonNewUser.Click += new System.EventHandler(this.buttonNewUser_Click);
            // 
            // LoginGroupBox
            // 
            this.LoginGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LoginGroupBox.Controls.Add(this.LoginNametextBox);
            this.LoginGroupBox.Controls.Add(this.LoginPass);
            this.LoginGroupBox.Controls.Add(this.LoginName);
            this.LoginGroupBox.Controls.Add(this.LoginPasstextBox);
            this.LoginGroupBox.Location = new System.Drawing.Point(48, 17);
            this.LoginGroupBox.Name = "LoginGroupBox";
            this.LoginGroupBox.Size = new System.Drawing.Size(220, 84);
            this.LoginGroupBox.TabIndex = 13;
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
            // LoginPasstextBox
            // 
            this.LoginPasstextBox.Location = new System.Drawing.Point(55, 45);
            this.LoginPasstextBox.Name = "LoginPasstextBox";
            this.LoginPasstextBox.Size = new System.Drawing.Size(123, 20);
            this.LoginPasstextBox.TabIndex = 5;
            this.LoginPasstextBox.UseSystemPasswordChar = true;
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(57, 126);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(91, 26);
            this.LoginButton.TabIndex = 10;
            this.LoginButton.Text = "Zaloguj";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // RegisterPanel
            // 
            this.RegisterPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RegisterPanel.Controls.Add(this.RegisterGroupBox);
            this.RegisterPanel.Controls.Add(this.RegisterButton);
            this.RegisterPanel.Location = new System.Drawing.Point(9, 286);
            this.RegisterPanel.Name = "RegisterPanel";
            this.RegisterPanel.Size = new System.Drawing.Size(328, 182);
            this.RegisterPanel.TabIndex = 2;
            this.RegisterPanel.Visible = false;
            // 
            // RegisterGroupBox
            // 
            this.RegisterGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RegisterGroupBox.Controls.Add(this.RegisterNameAcceptableLabel);
            this.RegisterGroupBox.Controls.Add(this.RegisterNameTextBox);
            this.RegisterGroupBox.Controls.Add(this.RegisterPassTextBox);
            this.RegisterGroupBox.Controls.Add(this.RegisterNamelabel);
            this.RegisterGroupBox.Controls.Add(this.RegisterPasslabel);
            this.RegisterGroupBox.Location = new System.Drawing.Point(72, 30);
            this.RegisterGroupBox.Name = "RegisterGroupBox";
            this.RegisterGroupBox.Size = new System.Drawing.Size(184, 92);
            this.RegisterGroupBox.TabIndex = 14;
            this.RegisterGroupBox.TabStop = false;
            this.RegisterGroupBox.Text = "Zarejestruj";
            // 
            // RegisterNameAcceptableLabel
            // 
            this.RegisterNameAcceptableLabel.AutoSize = true;
            this.RegisterNameAcceptableLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.RegisterNameAcceptableLabel.Location = new System.Drawing.Point(52, 68);
            this.RegisterNameAcceptableLabel.Name = "RegisterNameAcceptableLabel";
            this.RegisterNameAcceptableLabel.Size = new System.Drawing.Size(0, 13);
            this.RegisterNameAcceptableLabel.TabIndex = 12;
            // 
            // RegisterNameTextBox
            // 
            this.RegisterNameTextBox.Location = new System.Drawing.Point(55, 19);
            this.RegisterNameTextBox.Name = "RegisterNameTextBox";
            this.RegisterNameTextBox.Size = new System.Drawing.Size(123, 20);
            this.RegisterNameTextBox.TabIndex = 8;
            this.RegisterNameTextBox.TextChanged += new System.EventHandler(this.RegisterNameTextBox_TextChanged);
            // 
            // RegisterPassTextBox
            // 
            this.RegisterPassTextBox.Location = new System.Drawing.Point(55, 45);
            this.RegisterPassTextBox.Name = "RegisterPassTextBox";
            this.RegisterPassTextBox.Size = new System.Drawing.Size(123, 20);
            this.RegisterPassTextBox.TabIndex = 9;
            this.RegisterPassTextBox.UseSystemPasswordChar = true;
            this.RegisterPassTextBox.TextChanged += new System.EventHandler(this.RegisterPassTextBox_TextChanged);
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
            // RegisterButton
            // 
            this.RegisterButton.Location = new System.Drawing.Point(117, 128);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(91, 26);
            this.RegisterButton.TabIndex = 11;
            this.RegisterButton.Text = "Zarejestruj";
            this.RegisterButton.UseVisualStyleBackColor = true;
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            this.RegisterButton.Paint += new System.Windows.Forms.PaintEventHandler(this.RegisterButton_Paint);
            // 
            // RegisterNameValidityTimer
            // 
            this.RegisterNameValidityTimer.Interval = 1500;
            this.RegisterNameValidityTimer.Tick += new System.EventHandler(this.RegisterNameValidityTimer_Tick);
            // 
            // RegisterNameTextBoxToolTip
            // 
            this.RegisterNameTextBoxToolTip.IsBalloon = true;
            this.RegisterNameTextBoxToolTip.ToolTipTitle = "Prawidłowa forma nazwy:";
            // 
            // ConnectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 499);
            this.Controls.Add(this.RegisterPanel);
            this.Controls.Add(this.LoginPanel);
            this.Controls.Add(this.ConnectPanel);
            this.Name = "ConnectForm";
            this.Text = "ConnectForm";
            this.ConnectPanel.ResumeLayout(false);
            this.ConnectPanel.PerformLayout();
            this.LoginPanel.ResumeLayout(false);
            this.LoginGroupBox.ResumeLayout(false);
            this.LoginGroupBox.PerformLayout();
            this.RegisterPanel.ResumeLayout(false);
            this.RegisterGroupBox.ResumeLayout(false);
            this.RegisterGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ConnectPanel;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxhostIp;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Label labelHostIp;
        private System.Windows.Forms.Panel LoginPanel;
        private System.Windows.Forms.GroupBox LoginGroupBox;
        private System.Windows.Forms.TextBox LoginNametextBox;
        private System.Windows.Forms.Label LoginPass;
        private System.Windows.Forms.Label LoginName;
        private System.Windows.Forms.TextBox LoginPasstextBox;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Button buttonNewUser;
        private System.Windows.Forms.Panel RegisterPanel;
        private System.Windows.Forms.GroupBox RegisterGroupBox;
        private System.Windows.Forms.Label RegisterNameAcceptableLabel;
        private System.Windows.Forms.TextBox RegisterNameTextBox;
        private System.Windows.Forms.TextBox RegisterPassTextBox;
        private System.Windows.Forms.Label RegisterNamelabel;
        private System.Windows.Forms.Label RegisterPasslabel;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.Timer RegisterNameValidityTimer;
        private System.Windows.Forms.ToolTip RegisterNameTextBoxToolTip;

    }
}
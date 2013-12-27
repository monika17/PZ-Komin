namespace Komin
{
    partial class AddUserForm
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
            this.RegisterPanel = new System.Windows.Forms.Panel();
            this.RegisterGroupBox = new System.Windows.Forms.GroupBox();
            this.RegisterNameAcceptableLabel = new System.Windows.Forms.Label();
            this.RegisterNameTextBox = new System.Windows.Forms.TextBox();
            this.RegisterPassTextBox = new System.Windows.Forms.TextBox();
            this.RegisterNamelabel = new System.Windows.Forms.Label();
            this.RegisterPasslabel = new System.Windows.Forms.Label();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.RegisterNameTextBoxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.RegisterNameValidityTimer = new System.Windows.Forms.Timer(this.components);
            this.RegisterPanel.SuspendLayout();
            this.RegisterGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // RegisterPanel
            // 
            this.RegisterPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RegisterPanel.Controls.Add(this.RegisterGroupBox);
            this.RegisterPanel.Controls.Add(this.RegisterButton);
            this.RegisterPanel.Location = new System.Drawing.Point(16, 2);
            this.RegisterPanel.Name = "RegisterPanel";
            this.RegisterPanel.Size = new System.Drawing.Size(228, 136);
            this.RegisterPanel.TabIndex = 6;
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
            this.RegisterGroupBox.Location = new System.Drawing.Point(9, 3);
            this.RegisterGroupBox.Name = "RegisterGroupBox";
            this.RegisterGroupBox.Size = new System.Drawing.Size(184, 89);
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
            this.RegisterButton.Location = new System.Drawing.Point(60, 98);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(91, 26);
            this.RegisterButton.TabIndex = 11;
            this.RegisterButton.Text = "Dodaj";
            this.RegisterButton.UseVisualStyleBackColor = true;
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            this.RegisterButton.Paint += new System.Windows.Forms.PaintEventHandler(this.RegisterButton_Paint);
            // 
            // RegisterNameTextBoxToolTip
            // 
            this.RegisterNameTextBoxToolTip.IsBalloon = true;
            this.RegisterNameTextBoxToolTip.ToolTipTitle = "Prawidłowa forma nazwy:";
            // 
            // RegisterNameValidityTimer
            // 
            this.RegisterNameValidityTimer.Interval = 1500;
            this.RegisterNameValidityTimer.Tick += new System.EventHandler(this.RegisterNameValidityTimer_Tick);
            // 
            // AddUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 138);
            this.Controls.Add(this.RegisterPanel);
            this.Name = "AddUserForm";
            this.Text = "Dodaj użytkownika";
            this.RegisterPanel.ResumeLayout(false);
            this.RegisterGroupBox.ResumeLayout(false);
            this.RegisterGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel RegisterPanel;
        private System.Windows.Forms.GroupBox RegisterGroupBox;
        private System.Windows.Forms.Label RegisterNameAcceptableLabel;
        private System.Windows.Forms.TextBox RegisterNameTextBox;
        private System.Windows.Forms.TextBox RegisterPassTextBox;
        private System.Windows.Forms.Label RegisterNamelabel;
        private System.Windows.Forms.Label RegisterPasslabel;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.ToolTip RegisterNameTextBoxToolTip;
        private System.Windows.Forms.Timer RegisterNameValidityTimer;
    }
}
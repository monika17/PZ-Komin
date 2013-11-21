namespace Client
{
    partial class MessageWindow
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
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutMessageWindow = new System.Windows.Forms.TableLayoutPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.TextMessage = new System.Windows.Forms.TextBox();
            this.WriteMessage = new System.Windows.Forms.TextBox();
            this.SendMessageButton = new System.Windows.Forms.Button();
            this.MainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutMessageWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTabControl.Controls.Add(this.tabPage1);
            this.MainTabControl.Controls.Add(this.tabPage2);
            this.MainTabControl.Location = new System.Drawing.Point(0, 1);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(367, 515);
            this.MainTabControl.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutMessageWindow);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(359, 489);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Person1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutMessageWindow
            // 
            this.tableLayoutMessageWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutMessageWindow.AutoScroll = true;
            this.tableLayoutMessageWindow.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutMessageWindow.ColumnCount = 1;
            this.tableLayoutMessageWindow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.05286F));
            this.tableLayoutMessageWindow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.94714F));
            this.tableLayoutMessageWindow.Controls.Add(this.TextMessage, 0, 0);
            this.tableLayoutMessageWindow.Controls.Add(this.WriteMessage, 0, 2);
            this.tableLayoutMessageWindow.Controls.Add(this.SendMessageButton, 0, 1);
            this.tableLayoutMessageWindow.Location = new System.Drawing.Point(-4, 0);
            this.tableLayoutMessageWindow.Name = "tableLayoutMessageWindow";
            this.tableLayoutMessageWindow.RowCount = 4;
            this.tableLayoutMessageWindow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.36735F));
            this.tableLayoutMessageWindow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.632653F));
            this.tableLayoutMessageWindow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.tableLayoutMessageWindow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutMessageWindow.Size = new System.Drawing.Size(367, 489);
            this.tableLayoutMessageWindow.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(359, 489);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Person2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // TextMessage
            // 
            this.TextMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextMessage.Location = new System.Drawing.Point(3, 3);
            this.TextMessage.Multiline = true;
            this.TextMessage.Name = "TextMessage";
            this.TextMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextMessage.Size = new System.Drawing.Size(361, 360);
            this.TextMessage.TabIndex = 6;
            // 
            // WriteMessage
            // 
            this.WriteMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WriteMessage.Location = new System.Drawing.Point(3, 395);
            this.WriteMessage.Multiline = true;
            this.WriteMessage.Name = "WriteMessage";
            this.WriteMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.WriteMessage.Size = new System.Drawing.Size(361, 82);
            this.WriteMessage.TabIndex = 7;
            // 
            // SendMessageButton
            // 
            this.SendMessageButton.Location = new System.Drawing.Point(3, 369);
            this.SendMessageButton.Name = "SendMessageButton";
            this.SendMessageButton.Size = new System.Drawing.Size(75, 20);
            this.SendMessageButton.TabIndex = 8;
            this.SendMessageButton.Text = "Send";
            this.SendMessageButton.UseVisualStyleBackColor = true;
            // 
            // MessageWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 513);
            this.Controls.Add(this.MainTabControl);
            this.Name = "MessageWindow";
            this.Text = "Message";
            this.Load += new System.EventHandler(this.MessageWindow_Load);
            this.MainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutMessageWindow.ResumeLayout(false);
            this.tableLayoutMessageWindow.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMessageWindow;
        private System.Windows.Forms.TextBox TextMessage;
        private System.Windows.Forms.TextBox WriteMessage;
        private System.Windows.Forms.Button SendMessageButton;



    }
}


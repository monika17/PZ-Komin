namespace Komin
{
    public partial class TextMessagingPanel
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
            this.textSendButton = new System.Windows.Forms.Button();
            this.textMessageInput = new System.Windows.Forms.TextBox();
            this.textMessageContainer = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonAutoSend = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textSendButton
            // 
            this.textSendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textSendButton.Location = new System.Drawing.Point(570, 443);
            this.textSendButton.Name = "textSendButton";
            this.textSendButton.Size = new System.Drawing.Size(69, 27);
            this.textSendButton.TabIndex = 1;
            this.textSendButton.Text = "Wyślij";
            this.textSendButton.UseVisualStyleBackColor = true;
            this.textSendButton.Click += new System.EventHandler(this.onTextSendClicked);
            // 
            // textMessageInput
            // 
            this.textMessageInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textMessageInput.Location = new System.Drawing.Point(3, 443);
            this.textMessageInput.Multiline = true;
            this.textMessageInput.Name = "textMessageInput";
            this.textMessageInput.Size = new System.Drawing.Size(561, 58);
            this.textMessageInput.TabIndex = 0;
            this.textMessageInput.TextChanged += new System.EventHandler(this.onTextInputContentChanged);
            // 
            // textMessageContainer
            // 
            this.textMessageContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textMessageContainer.Location = new System.Drawing.Point(3, 3);
            this.textMessageContainer.Multiline = true;
            this.textMessageContainer.Name = "textMessageContainer";
            this.textMessageContainer.ReadOnly = true;
            this.textMessageContainer.Size = new System.Drawing.Size(636, 434);
            this.textMessageContainer.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.buttonAutoSend);
            this.panel1.Controls.Add(this.textMessageInput);
            this.panel1.Controls.Add(this.textSendButton);
            this.panel1.Controls.Add(this.textMessageContainer);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(642, 504);
            this.panel1.TabIndex = 6;
            // 
            // buttonAutoSend
            // 
            this.buttonAutoSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAutoSend.ForeColor = System.Drawing.SystemColors.GrayText;
            this.buttonAutoSend.Location = new System.Drawing.Point(570, 476);
            this.buttonAutoSend.Name = "buttonAutoSend";
            this.buttonAutoSend.Size = new System.Drawing.Size(69, 25);
            this.buttonAutoSend.TabIndex = 3;
            this.buttonAutoSend.Text = "auto enter";
            this.buttonAutoSend.UseVisualStyleBackColor = true;
            this.buttonAutoSend.Click += new System.EventHandler(this.buttonAutoSend_Click);
            // 
            // TextMessagingPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panel1);
            this.Name = "TextMessagingPanel";
            this.Size = new System.Drawing.Size(648, 510);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button textSendButton;
        private System.Windows.Forms.TextBox textMessageInput;
        private System.Windows.Forms.TextBox textMessageContainer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonAutoSend;
    }
}

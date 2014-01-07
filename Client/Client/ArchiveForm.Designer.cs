namespace Komin
{
    partial class ArchiveForm
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
            this.textMessageContainer = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // textMessageContainer
            // 
            this.textMessageContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textMessageContainer.Location = new System.Drawing.Point(12, 12);
            this.textMessageContainer.MinimumSize = new System.Drawing.Size(20, 20);
            this.textMessageContainer.Name = "textMessageContainer";
            this.textMessageContainer.ScriptErrorsSuppressed = true;
            this.textMessageContainer.Size = new System.Drawing.Size(459, 466);
            this.textMessageContainer.TabIndex = 6;
            // 
            // ArchiveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 490);
            this.Controls.Add(this.textMessageContainer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArchiveForm";
            this.Text = "Archiwum";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser textMessageContainer;

    }
}
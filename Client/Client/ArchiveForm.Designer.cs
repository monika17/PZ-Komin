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
            this.ArchiveContainer = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // ArchiveContainer
            // 
            this.ArchiveContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ArchiveContainer.Location = new System.Drawing.Point(12, 12);
            this.ArchiveContainer.Name = "ArchiveContainer";
            this.ArchiveContainer.ReadOnly = true;
            this.ArchiveContainer.Size = new System.Drawing.Size(459, 466);
            this.ArchiveContainer.TabIndex = 6;
            this.ArchiveContainer.Text = "";
            // 
            // ArchiveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 490);
            this.Controls.Add(this.ArchiveContainer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArchiveForm";
            this.Text = "Archiwum";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox ArchiveContainer;
    }
}
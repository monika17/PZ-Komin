namespace Komin
{
    partial class AddGroupForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupnameTextBox = new System.Windows.Forms.TextBox();
            this.groupnameValidityLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.text = new System.Windows.Forms.CheckBox();
            this.voice = new System.Windows.Forms.CheckBox();
            this.video = new System.Windows.Forms.CheckBox();
            this.files = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nazwa grupy:";
            // 
            // groupnameTextBox
            // 
            this.groupnameTextBox.Location = new System.Drawing.Point(89, 12);
            this.groupnameTextBox.Name = "groupnameTextBox";
            this.groupnameTextBox.Size = new System.Drawing.Size(132, 20);
            this.groupnameTextBox.TabIndex = 1;
            this.groupnameTextBox.TextChanged += new System.EventHandler(this.groupnameTextBox_TextChanged);
            // 
            // groupnameValidityLabel
            // 
            this.groupnameValidityLabel.AutoSize = true;
            this.groupnameValidityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupnameValidityLabel.Location = new System.Drawing.Point(86, 35);
            this.groupnameValidityLabel.Name = "groupnameValidityLabel";
            this.groupnameValidityLabel.Size = new System.Drawing.Size(0, 13);
            this.groupnameValidityLabel.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Rodzaj komunikacji:";
            // 
            // text
            // 
            this.text.AutoSize = true;
            this.text.Checked = true;
            this.text.CheckState = System.Windows.Forms.CheckState.Checked;
            this.text.Enabled = false;
            this.text.Location = new System.Drawing.Point(15, 73);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(69, 17);
            this.text.TabIndex = 4;
            this.text.Text = "tekstowa";
            this.text.UseVisualStyleBackColor = true;
            // 
            // voice
            // 
            this.voice.AutoSize = true;
            this.voice.Location = new System.Drawing.Point(15, 96);
            this.voice.Name = "voice";
            this.voice.Size = new System.Drawing.Size(67, 17);
            this.voice.TabIndex = 5;
            this.voice.Text = "głosowa";
            this.voice.UseVisualStyleBackColor = true;
            this.voice.CheckedChanged += new System.EventHandler(this.voice_CheckedChanged);
            // 
            // video
            // 
            this.video.AutoSize = true;
            this.video.Location = new System.Drawing.Point(15, 119);
            this.video.Name = "video";
            this.video.Size = new System.Drawing.Size(54, 17);
            this.video.TabIndex = 6;
            this.video.Text = "wideo";
            this.video.UseVisualStyleBackColor = true;
            this.video.CheckedChanged += new System.EventHandler(this.video_CheckedChanged);
            // 
            // files
            // 
            this.files.AutoSize = true;
            this.files.Checked = true;
            this.files.CheckState = System.Windows.Forms.CheckState.Checked;
            this.files.Location = new System.Drawing.Point(15, 142);
            this.files.Name = "files";
            this.files.Size = new System.Drawing.Size(100, 17);
            this.files.TabIndex = 7;
            this.files.Text = "wymiana plików";
            this.files.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(65, 167);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Dodaj grupe";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(146, 167);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Anuluj";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // AddGroupForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(233, 206);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.files);
            this.Controls.Add(this.video);
            this.Controls.Add(this.voice);
            this.Controls.Add(this.text);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupnameValidityLabel);
            this.Controls.Add(this.groupnameTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddGroupForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dodaj nową grupę";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox groupnameTextBox;
        private System.Windows.Forms.Label groupnameValidityLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox text;
        private System.Windows.Forms.CheckBox voice;
        private System.Windows.Forms.CheckBox video;
        private System.Windows.Forms.CheckBox files;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer timer1;
    }
}
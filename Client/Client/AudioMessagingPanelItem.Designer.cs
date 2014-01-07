namespace Komin
{
    partial class AudioMessagingPanelItem
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioMessagingPanelItem));
            this.panel1 = new System.Windows.Forms.Panel();
            this.volumeLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.volumeTrackBar = new System.Windows.Forms.TrackBar();
            this.userName = new System.Windows.Forms.Label();
            this.statePicture = new System.Windows.Forms.PictureBox();
            this.stateImages = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statePicture)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.volumeLabel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.volumeTrackBar);
            this.panel1.Controls.Add(this.userName);
            this.panel1.Controls.Add(this.statePicture);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.MinimumSize = new System.Drawing.Size(260, 55);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 55);
            this.panel1.TabIndex = 0;
            // 
            // volumeLabel
            // 
            this.volumeLabel.AutoSize = true;
            this.volumeLabel.Location = new System.Drawing.Point(225, 31);
            this.volumeLabel.Name = "volumeLabel";
            this.volumeLabel.Size = new System.Drawing.Size(21, 13);
            this.volumeLabel.TabIndex = 4;
            this.volumeLabel.Text = "0%";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Głośność:";
            // 
            // volumeTrackBar
            // 
            this.volumeTrackBar.Location = new System.Drawing.Point(103, 25);
            this.volumeTrackBar.Name = "volumeTrackBar";
            this.volumeTrackBar.Size = new System.Drawing.Size(116, 40);
            this.volumeTrackBar.TabIndex = 2;
            this.volumeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.volumeTrackBar.ValueChanged += new System.EventHandler(this.volumeTrackBar_ValueChanged);
            // 
            // userName
            // 
            this.userName.AutoSize = true;
            this.userName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.userName.Location = new System.Drawing.Point(40, 3);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(82, 19);
            this.userName.TabIndex = 1;
            this.userName.Text = "user_name";
            // 
            // statePicture
            // 
            this.statePicture.Location = new System.Drawing.Point(3, 3);
            this.statePicture.Name = "statePicture";
            this.statePicture.Size = new System.Drawing.Size(32, 32);
            this.statePicture.TabIndex = 0;
            this.statePicture.TabStop = false;
            // 
            // stateImages
            // 
            this.stateImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("stateImages.ImageStream")));
            this.stateImages.TransparentColor = System.Drawing.Color.White;
            this.stateImages.Images.SetKeyName(0, "0");
            this.stateImages.Images.SetKeyName(1, "1");
            this.stateImages.Images.SetKeyName(2, "2");
            this.stateImages.Images.SetKeyName(3, "3");
            this.stateImages.Images.SetKeyName(4, "4");
            this.stateImages.Images.SetKeyName(5, "5");
            this.stateImages.Images.SetKeyName(6, "6");
            this.stateImages.Images.SetKeyName(7, "7");
            this.stateImages.Images.SetKeyName(8, "8");
            this.stateImages.Images.SetKeyName(9, "9");
            this.stateImages.Images.SetKeyName(10, "10");
            this.stateImages.Images.SetKeyName(11, "11");
            this.stateImages.Images.SetKeyName(12, "12");
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // AudioMessagingPanelItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(260, 55);
            this.Name = "AudioMessagingPanelItem";
            this.Size = new System.Drawing.Size(260, 55);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statePicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox statePicture;
        private System.Windows.Forms.ImageList stateImages;
        private System.Windows.Forms.Label userName;
        private System.Windows.Forms.Label volumeLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar volumeTrackBar;
        private System.Windows.Forms.Timer timer1;
    }
}

using System;
using System.Windows.Forms;

namespace Komin
{
    public delegate void VolumeChanged(AudioMessagingPanelItem ampi, float new_volume);

    public partial class AudioMessagingPanelItem : UserControl
    {
        public VolumeChanged onVolumeChanged;
        private bool speeking_state;
        private int img;

        public AudioMessagingPanelItem()
        {
            onVolumeChanged = null;
            speeking_state = false;
            img = 0;
            InitializeComponent();
            volumeTrackBar.Value = 10;
        }

        private void volumeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            volumeLabel.Text = volumeTrackBar.Value * 10 + "%";
            if (onVolumeChanged != null)
                onVolumeChanged(this, volumeTrackBar.Value / 10.0f);
        }

        public void SetContactData(ContactData cd, bool group_holder = false)
        {
            userName.Text = cd.contact_name;
            img = (int)(cd.status & (uint)KominClientStatusCodes.MaxValue);
            if (group_holder && img != 0)
                img += 4;
            if (speeking_state && img != 0)
                img--;
            statePicture.Image = stateImages.Images["" + img];
            volumeTrackBar.Enabled = (img != 0);
            volumeLabel.Enabled = (img != 0);
        }

        public void MarkSpeaking()
        {
            speeking_state = true;
            timer1.Enabled = true;
            if ((img != 0) && (img % 2 == 0))
            {
                img--;
                statePicture.Image = stateImages.Images["" + img];
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            speeking_state = false;
            if ((img != 0) && (img % 2 == 1))
            {
                img++;
                statePicture.Image = stateImages.Images["" + img];
            }
        }
    }
}

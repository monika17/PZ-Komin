using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Komin
{
    public partial class AudioMessagingPanel : UserControl
    {
        private static AudioMessagingPanel singleton = null;
        public static AudioMessagingPanel Singleton
        {
            get { return singleton; }
            private set { singleton = value; }
        }

        struct Contact
        {
            public ContactData cd;
            public AudioMessagingPanelItem ampi;
        };
        List<Contact> contacts;

        KominAudioCodec codec;
        KominAudioInput audio_in;
        KominAudioOutput audio_out;

        KominClientSideConnection conn;
        uint receiver_id;
        bool receiver_is_group;
        TabPage tp;
        
        /// <summary>
        /// </summary>
        /// <param name="tp">TabPage with initialized instance of TextMessagingPanel</param>
        public AudioMessagingPanel(KominClientSideConnection conn, uint receiver_id, bool receiver_is_group, TabPage tp)
        {
            if (singleton != null)
                throw new Exception();
            singleton = this;

            InitializeComponent();
            this.Name = "AudioMessagingPanel";
            this.receiver_id = receiver_id;
            this.receiver_is_group = receiver_is_group;
            this.conn = conn;
            contacts = new List<Contact>();
            codec = new KominAudioCodec();
            audio_out = new KominAudioOutput();
            audio_in = new KominAudioInput();
            audio_in.onNewData = onNewAudioData;
            audio_in.StartRecording();

            this.HandleDestroyed += AudioMessagingPanel_Destroying;
            this.tp = tp;
            ((TextMessagingPanel)tp.Controls["TextMessagingPanel"]).FreeSpaceForAV(Size.Height);
        }

        private void onNewAudioData(byte[] buffer, int valid_count)
        {
            byte[] wave = new byte[valid_count];
            Buffer.BlockCopy(buffer, 0, wave, 0, valid_count);
            conn.SendAVMessage(receiver_id, receiver_is_group, codec.Encode(wave), null);
        }

        void AudioMessagingPanel_Destroying(object sender, EventArgs e)
        {
            ((TextMessagingPanel)tp.Controls["TextMessagingPanel"]).TakeSpaceFromAV(Size.Height);
            audio_in.Dispose();
            audio_out.Dispose();
            singleton = null;
        }

        public void AddContact(ContactData cd, bool group_holder)
        {
            Contact contact;
            contact.cd = cd;
            contact.ampi = new AudioMessagingPanelItem();
            contact.ampi.SetContactData(cd, group_holder);
            contact.ampi.Location = new Point((contacts.Count % 2) * contact.ampi.Width, (contacts.Count / 2) * contact.ampi.Height);
            contact.ampi.Enabled = true;
            contact.ampi.Visible = true;
            contact.ampi.Tag = cd;
            contact.ampi.onVolumeChanged = onVolumeChanged;

            audio_out.AddOut();
            contacts.Add(contact);
            panel1.Controls.Add(contact.ampi);
        }

        private void onVolumeChanged(AudioMessagingPanelItem ampi, float new_volume)
        {
            ContactData cd = (ContactData)ampi.Tag;
            for(int i=0; i<contacts.Count; i++)
            {
                if(cd.contact_id==contacts[i].cd.contact_id)
                {
                    audio_out.SetVolume(i, new_volume);
                    break;
                }
            }
        }

        private void CorrectPositions()
        {
            foreach (Contact c in contacts)
            {
                c.ampi.Location = new Point((contacts.Count % 2) * c.ampi.Width, (contacts.Count / 2) * c.ampi.Height);
            }
        }

        public void RemoveContact(ContactData cd)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                if (contacts[i].cd.contact_id == cd.contact_id)
                {
                    audio_out.RemoveOut(i);
                    panel1.Controls.RemoveAt(i);
                    contacts.RemoveAt(i);
                    CorrectPositions();
                    break;
                }
            }
        }

        public void SetContact(ContactData cd, bool group_holder)
        {
            foreach (Contact c in contacts)
            {
                if (c.cd.contact_id == cd.contact_id)
                {
                    c.ampi.SetContactData(cd, group_holder);
                    break;
                }
            }
        }

        public void InsertMessage(uint sender, byte[] wave)
        {
            for (int i=0; i<contacts.Count; i++)
            {
                if (contacts[i].cd.contact_id == sender)
                {
                    audio_out.InsertData(i, codec.Decode(wave));
                    contacts[i].ampi.MarkSpeaking();
                    break;
                }
            }
        }
    }
}

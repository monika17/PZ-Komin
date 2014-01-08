using System.Collections.Generic;
using System.Threading;
using NAudio;
using NAudio.Wave;

namespace Komin
{
    public delegate void NewDataCallback(byte[] buffer, int valid_count);

    public class KominAudioInput
    {
        Thread th;
        bool thread_working;

        WaveInEvent waveIn;
        List<int> requests; //0 - no request, 1 - start recording, 2 - stop recording, 4 - stop thread
        public NewDataCallback onNewData;
        int device_nr;

        public KominAudioInput(int device_nr = 0, NewDataCallback onNewData = null)
        {
            this.onNewData = onNewData;
            this.device_nr = device_nr;

            requests = new List<int>();
            th = new Thread(thread_master);
            th.Start();
        }

        ~KominAudioInput()
        {
            if (thread_working)
            {
                requests.Add(4);
                while (thread_working) ;
            }
        }

        /// <returns>true if test succeded</returns>
        public static bool HardwareTest()
        {
            WaveInEvent wevt = new WaveInEvent();
            wevt.BufferMilliseconds = 100;
            wevt.DeviceNumber = 0;
            wevt.WaveFormat = new WaveFormat(8000, 16, 1);
            try
            {
                wevt.StartRecording();
                wevt.StopRecording();
            }
            catch (MmException)
            {
                return false;
            }
            return true;
        }

        private void thread_master()
        {
            thread_working = true;

            WaveFormat format = new WaveFormat(8000, 16, 1);
            waveIn = new WaveInEvent();
            waveIn.BufferMilliseconds = 100;
            waveIn.DeviceNumber = device_nr;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.WaveFormat = format;

            int thread_request = 0;
            bool work = true;
            while (work)
            {
                if (requests.Count > 0)
                {
                    thread_request = requests[0];
                    requests.RemoveAt(0);
                }
                switch (thread_request)
                {
                    case 0:
                        break;
                    case 1:
                        waveIn.StartRecording();
                        break;
                    case 2:
                        waveIn.StopRecording();
                        break;
                    case 4:
                        work = false;
                        break;
                }
                thread_request = 0;
                Thread.Sleep(20);
            }

            waveIn.StopRecording();
            waveIn.Dispose();

            thread_working = false;
            th.Abort();
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (onNewData != null)
                onNewData(e.Buffer, e.BytesRecorded);
        }

        public void StartRecording()
        {
            requests.Add(1);
        }

        public void StopRecording()
        {
            requests.Add(2);
        }

        public void WaitForExec()
        {
            while (requests.Count > 0) ;
        }

        public void Dispose()
        {
            if (thread_working)
            {
                requests.Add(4);
                while (thread_working) ;
            }
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using NAudio;
using NAudio.Wave;

namespace Komin
{
    public class KominAudioOutput
    {
        Thread th;
        bool thread_working;

        class WOut
        {
            public WaveOut waveOut;
            public BufferedWaveProvider bwp;
        };
        List<WOut> outs;
        class Request
        {
            public int code;
            public object param;
            public Request(int code, object param)
            {
                this.code = code;
                this.param = param;
            }
        };
        List<Request> requests; //0 - no request, 1 - start playback, 2 - stop playback, 3 - set new volume, 4 - stop thread, 5 - add out, 6 - remove out, 7 - insert data

        public KominAudioOutput()
        {
            outs = new List<WOut>();
            requests = new List<Request>();
            th = new Thread(thread_master);
            th.Start();
        }

        ~KominAudioOutput()
        {
            if (thread_working)
            {
                requests.Add(new Request(4, null));
                while (thread_working) ;
            }
        }

        /// <returns>true if test succeded</returns>
        public static bool HardwareTest()
        {
            WOut wout = new WOut();
            wout.bwp = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            wout.waveOut = new WaveOut();
            wout.waveOut.Volume = 1.0f;
            try
            {
                wout.waveOut.Init(wout.bwp);
                wout.waveOut.Play();
                wout.waveOut.Stop();
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

            Request thread_request = null;
            bool work = true;
            while (work)
            {
                if (requests.Count > 0)
                {
                    thread_request = requests[0];
                    requests.RemoveAt(0);
                }
                if(thread_request!=null)
                    switch (thread_request.code)
                    {
                        case 0: //nop
                            break;
                        case 1: //start
                            outs[(int)thread_request.param].waveOut.Play();
                            break;
                        case 2: //stop
                            outs[(int)thread_request.param].waveOut.Stop();
                            break;
                        case 3: //set volume
                            outs[(int)((object[])thread_request.param)[0]].waveOut.Volume = (float)((object[])thread_request.param)[1];
                            break;
                        case 4: //exit thread
                            if(outs.Count>0)
                            {
                                for (int i = 0; i < outs.Count; i++)
                                    requests.Add(new Request(6, 0));
                                requests.Add(new Request(4, null));
                            }
                            else
                                work = false;
                            break;
                        case 5: //add out
                            {
                                WOut wout = new WOut();
                                wout.bwp = new BufferedWaveProvider(format);
                                wout.waveOut = new WaveOut();
                                wout.waveOut.Volume = 1.0f;
                                wout.waveOut.Init(wout.bwp);
                                wout.waveOut.Play();
                                outs.Add(wout);
                                break;
                            }
                        case 6: //remove out (param: nr of out to remove)
                            {
                                int wout_nr = (int)thread_request.param;
                                outs[wout_nr].waveOut.Stop();
                                outs[wout_nr].waveOut.Dispose();
                                outs.RemoveAt(wout_nr);
                                break;
                            }
                        case 7: //insert data
                            {
                                int wout_nr = (int)((object[])thread_request.param)[0];
                                byte[] wave = (byte[])((object[])thread_request.param)[1];
                                outs[wout_nr].bwp.AddSamples(wave, 0, wave.Length);
                                break;
                            }
                    }
                thread_request = null;
                Thread.Sleep(20);
            }

            thread_working = false;
            th.Abort();
        }

        public void SetVolume(int out_nr, float volume)
        {
            requests.Add(new Request(3, new object[] { out_nr, volume }));
            WaitForExec();
        }

        public void Play(int out_nr)
        {
            requests.Add(new Request(1, out_nr));
            WaitForExec();
        }

        public void Stop(int out_nr)
        {
            requests.Add(new Request(2, out_nr));
            WaitForExec();
        }

        private void WaitForExec()
        {
            while (requests.Count > 0) ;
        }

        public int AddOut() //returns new out id
        {
            requests.Add(new Request(5, null));
            WaitForExec();
            return outs.Count - 1;
        }

        public void RemoveOut(int out_nr)
        {
            requests.Add(new Request(6, out_nr));
            WaitForExec();
        }

        public void InsertData(int out_nr, byte[] wave)
        {
            requests.Add(new Request(7, new object[] { out_nr, wave }));
        }

        public void Dispose()
        {
            if (thread_working)
            {
                requests.Add(new Request(4, null));
                while (thread_working) ;
            }
        }
    }
}

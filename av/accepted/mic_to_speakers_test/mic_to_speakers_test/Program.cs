using System;
using System.Collections.Generic;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Utils;

namespace mic_to_speakers_test
{
    class Program
    {
        static WaveInEvent waveIn;
        static WaveOut waveOut;
        static WaveOut waveOut2;
        static BufferedWaveProvider waveBP;
        static BufferedWaveProvider waveBP2;
        static Gsm610ChatCodec codec;

        static void Main(string[] args)
        {
            codec = new Gsm610ChatCodec();

            WaveFormat format = new WaveFormat(8000, 16, 1);

            waveIn = new WaveInEvent();
            waveIn.BufferMilliseconds = 50;
            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.WaveFormat = format;
            waveIn.StartRecording();

            waveBP = new BufferedWaveProvider(format);
            waveBP2 = new BufferedWaveProvider(format);

            waveOut = new WaveOut();
            waveOut.Volume = 1.0f;
            waveOut.Init(waveBP);
            waveOut.Play();

            waveOut2 = new WaveOut();
            waveOut2.Volume = 1.0f;
            waveOut2.Init(waveBP2);
            //waveOut2.Play();

            Console.WriteLine("Press any key to change volume...");
            Console.ReadKey(true);
            waveOut.Volume = 0.1f;

            Console.Write("Press any key to stop...");
            Console.ReadKey(true);

            waveIn.StopRecording();
            waveOut.Stop();
            //waveOut2.Stop();
            
            //clean up
            waveIn.Dispose();
            waveOut.Dispose();
            waveOut2.Dispose();
            codec.Dispose();
        }

        static void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] encoded = codec.Encode(e.Buffer, 0, e.BytesRecorded);
            //here send encoded into the cable
            //...
            //network
            //...
            //here take encoded out of the cable
            byte[] decoded = codec.Decode(encoded, 0, encoded.Length);

            //fake alternative
            byte[] dec = new byte[decoded.Length];
            decoded.CopyTo(dec, (long)0);

            //try to estimate silence
            double max = 0;
            for (int i = 0; i < dec.Length / 2; i++)
            {
                int sample = Math.Abs(dec[2 * i] + ((int)dec[2 * i + 1]) * 256);
                if (max < Decibels.LinearToDecibels(sample / 65536.0))
                    max = Decibels.LinearToDecibels(sample / 65536.0);
            }
            double threshold = -0.5;
            bool silence = (max < threshold);
            Console.WriteLine("max: {0}   sizeof(enc): {1}   sizeof(dec): {2}", max, encoded.Length, decoded.Length);

            waveBP.AddSamples(decoded, 0, decoded.Length);
            //waveBP2.AddSamples(dec, 0, dec.Length);
        }
    }
}

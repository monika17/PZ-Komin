using System;
using NAudio.Wave;
using NAudio.Wave.Compression;
using NAudio;

namespace mic_to_speakers_test
{
    public class Gsm610ChatCodec : IDisposable
    {
        private readonly WaveFormat encodeFormat;
        private AcmStream encodeStream;
        private AcmStream decodeStream;
        private int decodeSourceBytesLeftovers;
        private int encodeSourceBytesLeftovers;

        public Gsm610ChatCodec()
        {
            RecordFormat = new WaveFormat(8000, 16, 1);
            encodeFormat = new Gsm610WaveFormat();
        }

        /// <summary>
        /// Friendly Name for this codec
        /// </summary>
        public string Name { get { return "GSM 6.10"; } }

        /// <summary>
        /// Tests whether the codec is available on this system
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                // determine if this codec is installed on this PC
                bool available = true;
                try
                {
                    using (new AcmStream(RecordFormat, encodeFormat)) { }
                    using (new AcmStream(encodeFormat, RecordFormat)) { }
                }
                catch (MmException)
                {
                    available = false;
                }
                return available;
            }
        }

        /// <summary>
        /// Bitrate
        /// </summary>
        public int BitsPerSecond
        {
            get
            {
                return encodeFormat.AverageBytesPerSecond * 8;
            }
        }

        /// <summary>
        /// Preferred PCM format for recording in (usually 8kHz mono 16 bit)
        /// </summary>
        public WaveFormat RecordFormat { get; private set; }

        /// <summary>
        /// Encodes a block of audio
        /// </summary>
        public byte[] Encode(byte[] data, int offset, int length)
        {
            if (encodeStream == null)
            {
                encodeStream = new AcmStream(RecordFormat, encodeFormat);
            }
            return Convert(encodeStream, data, offset, length, ref encodeSourceBytesLeftovers);
        }

        /// <summary>
        /// Decodes a block of audio
        /// </summary>
        public byte[] Decode(byte[] data, int offset, int length)
        {
            if (decodeStream == null)
            {
                decodeStream = new AcmStream(encodeFormat, RecordFormat);
            }
            return Convert(decodeStream, data, offset, length, ref decodeSourceBytesLeftovers);
        }

        private static byte[] Convert(AcmStream conversionStream, byte[] data, int offset, int length, ref int sourceBytesLeftovers)
        {
            int bytesInSourceBuffer = length + sourceBytesLeftovers;
            Array.Copy(data, offset, conversionStream.SourceBuffer, sourceBytesLeftovers, length);
            int sourceBytesConverted;
            int bytesConverted = conversionStream.Convert(bytesInSourceBuffer, out sourceBytesConverted);
            sourceBytesLeftovers = bytesInSourceBuffer - sourceBytesConverted;
            if (sourceBytesLeftovers > 0)
            {
                // shift the leftovers down
                Array.Copy(conversionStream.SourceBuffer, sourceBytesConverted, conversionStream.SourceBuffer, 0, sourceBytesLeftovers);
            }
            byte[] encoded = new byte[bytesConverted];
            Array.Copy(conversionStream.DestBuffer, 0, encoded, 0, bytesConverted);
            return encoded;
        }

        public void Dispose()
        {
            if (encodeStream != null)
            {
                encodeStream.Dispose();
                encodeStream = null;
            }
            if (decodeStream != null)
            {
                decodeStream.Dispose();
                decodeStream = null;
            }
        }
    }
}
using CriMw.CriGears.Audio.Media.IO.HCA;
using NAudio.Wave;
using MustafaUğuz.PES2017.Sound.NAudio.Lame;
using MustafaUğuz.Utility.System;
using System.IO;

namespace MustafaUğuz.PES2017.Sound
{
    public class Conversion
    {
        private const string HCADecoderPath = @"HCAtoWAV.exe";
        private const string ADXDecoderPath = @"ADXtoWAV.exe";
        private const string ADXEncoderPath = @"WAVtoADX.exe";

        public static void HCAtoWAV(string inputFilePath)
        {
            Diagnostics.Process(HCADecoderPath, "\"" + inputFilePath + "\"", Path.GetDirectoryName(HCADecoderPath));
        }

        public static void WAVtoHCA(string inputFilePath, string outputFilePath)
        {
            HcaEncoderLite hcaEnc = new HcaEncoderLite();
            hcaEnc.Encode(inputFilePath, outputFilePath, Quality.MIDDLE, 0, HcaEncoderLite.KeyCode);
        }

        public static void WAVtoHCA(string inputFilePath, string outputFilePath, Quality quality)
        {
            HcaEncoderLite hcaEnc = new HcaEncoderLite();
            hcaEnc.Encode(inputFilePath, outputFilePath, quality, 0, HcaEncoderLite.KeyCode);
        }
        
        public static void ADXtoWAV(string inputFilePath)
        {
            Diagnostics.Process(ADXDecoderPath, "\"" + inputFilePath + "\"", Path.GetDirectoryName(ADXDecoderPath));
        }

        public static void WAVtoADX(string inputFilePath)
        {
            Diagnostics.Process(ADXEncoderPath, "\"" + inputFilePath + "\"", Path.GetDirectoryName(ADXEncoderPath));
        }

        public static byte[] MP3toWAV(byte[] inputBytes, WaveFormat waveFormat = null)
        {
            MemoryStream outputStream = new MemoryStream();

            using (WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(new MemoryStream(inputBytes))))
            using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveFormat ?? waveStream.WaveFormat))
            {
                byte[] bytes = new byte[waveStream.Length];
                waveStream.Read(bytes, 0, (int)waveStream.Length);
                waveFileWriter.Write(bytes, 0, bytes.Length);
                waveFileWriter.Flush();
            }

            return outputStream.GetBuffer();
        }

        public static byte[] MP3toWAV(Stream inputStream, WaveFormat waveFormat = null)
        {
            MemoryStream outputStream = new MemoryStream();

            using (WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(inputStream)))
            using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveFormat ?? waveStream.WaveFormat))
            {
                byte[] bytes = new byte[waveStream.Length];
                waveStream.Read(bytes, 0, (int)waveStream.Length);
                waveFileWriter.Write(bytes, 0, bytes.Length);
                waveFileWriter.Flush();
            }

            return outputStream.GetBuffer();
        }

        public static byte[] MP3toWAV(string inputFilePath, WaveFormat waveFormat = null)
        {
            MemoryStream outputStream = new MemoryStream();

            using (WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(inputFilePath)))
            using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveFormat ?? waveStream.WaveFormat))
            {
                byte[] bytes = new byte[waveStream.Length];
                waveStream.Read(bytes, 0, (int)waveStream.Length);
                waveFileWriter.Write(bytes, 0, bytes.Length);
                waveFileWriter.Flush();
            }

            return outputStream.GetBuffer();
        }

        public static byte[] WAVtoMP3(byte[] inputBytes, WaveFormat waveFormat = null, int bitRate = 128)
        {
            var output = new MemoryStream();

            using (var reader = new WaveFileReader(new MemoryStream(inputBytes)))
            using (var writer = new LameMP3FileWriter(output, waveFormat ?? reader.WaveFormat, bitRate))
                reader.CopyTo(writer);

            return output.GetBuffer();
        }

        public static byte[] WAVtoMP3(Stream inputStream, WaveFormat waveFormat = null, int bitRate = 128)
        {
            var output = new MemoryStream();

            using (var reader = new WaveFileReader(inputStream))
            using (var writer = new LameMP3FileWriter(output, waveFormat ?? reader.WaveFormat, bitRate))
                reader.CopyTo(writer);

            return output.GetBuffer();
        }

        public static byte[] WAVtoMP3(string inputFilePath, WaveFormat waveFormat = null, int bitRate = 128)
        {
            var output = new MemoryStream();

            using (var reader = new WaveFileReader(inputFilePath))
            using (var writer = new LameMP3FileWriter(output, waveFormat ?? reader.WaveFormat, bitRate))
                reader.CopyTo(writer);

            return output.GetBuffer();
        }

        public static void WAVSetting(string inputFilePath, WaveFormat waveFormat = null)
        {
            MemoryStream outputStream = new MemoryStream();
            using (WaveStream inputStream = new WaveFileReader(inputFilePath))
            using (WaveFileWriter outputWriter = new WaveFileWriter(outputStream, waveFormat == null ? inputStream.WaveFormat : waveFormat))
            {
                byte[] bytes = new byte[inputStream.Length];
                inputStream.Read(bytes, 0, (int)inputStream.Length);
                outputWriter.Write(bytes, 0, bytes.Length);
                outputWriter.Flush();
            }
            File.Delete(inputFilePath);
            File.WriteAllBytes(inputFilePath, outputStream.ToArray());
        }
    }
}

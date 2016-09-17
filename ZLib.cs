using Ionic.Zlib;
using System;
using System.Net;
using System.IO;
using MustafaUğuz.Utility.System;

namespace MustafaUğuz.Utility
{
    public class ZLib
    {
        private static byte[] header = new byte[] { 0, 1, 1, 0x57, 0x45, 0x53, 0x59, 0x53 };
        private static byte[] magic = new byte[] { 0x57, 0x45, 0x53, 0x59, 0x53 };

        private static bool IsZlibbed(Stream inputStream)
        {
            var inputReader = new BinaryReader(inputStream);
            inputReader.BaseStream.Position = 3;
            if (inputStream.Length > 8)
            {
                var fileHeader = inputReader.ReadBytes(5);

                inputReader.BaseStream.Position = 0;
                return fileHeader.PadLeft(8).ToUInt64() == magic.PadLeft(8).ToUInt64();
            }

            else
            {
                inputReader.BaseStream.Position = 0;
                return false;
            }
        }

        public static byte[] Encrypt(Stream inputStream, bool pcInput)
        {
            return Encrypt(inputStream, CompressionLevel.Level3, pcInput);
        }

        public static byte[] Decrypt(Stream inputStream, bool pcInput)
        {
            using (var inputReader = new BinaryReader(inputStream))
            using (var input = new ZlibStream(inputStream, CompressionMode.Decompress, false))
            using (var outputStream = new MemoryStream())
            using (var output = new BinaryWriter(outputStream))
            {
                inputReader.BaseStream.Position = 12;
                int num = pcInput ? inputReader.ReadInt32() : IPAddress.NetworkToHostOrder(inputReader.ReadInt32());

                byte[] buffer = new byte[0x2000];
                int count = 1;
                outputStream.SetLength(num);
                while ((num > 0) && (count > 0))
                {
                    count = input.Read(buffer, 0, Math.Min(num, buffer.Length));
                    output.Write(buffer, 0, count);
                    num -= count;
                }

                return outputStream.ToArray();
            }
        }

        public static byte[] DecryptIfZlibbed(Stream inputStream, bool pcInput)
        {
            byte[] inputBytes = new byte[0];
            using (var memoryStream = new MemoryStream())
            {
                inputStream.CopyTo(memoryStream);
                inputBytes = memoryStream.ToArray();
            }

            inputStream.Position = 0;

            if (IsZlibbed(inputStream))
            {
                using (var inputReader = new BinaryReader(inputStream))
                using (var input = new ZlibStream(inputStream, CompressionMode.Decompress, false))
                using (var outputStream = new MemoryStream())
                using (var output = new BinaryWriter(outputStream))
                {
                    inputReader.BaseStream.Position = 12;
                    int num = pcInput ? inputReader.ReadInt32() : IPAddress.NetworkToHostOrder(inputReader.ReadInt32());

                    byte[] buffer = new byte[0x2000];
                    int count = 1;
                    outputStream.SetLength(num);
                    while ((num > 0) && (count > 0))
                    {
                        count = input.Read(buffer, 0, Math.Min(num, buffer.Length));
                        output.Write(buffer, 0, count);
                        num -= count;
                    }

                    return outputStream.ToArray();
                }
            }

            else
                return inputBytes;
        }

        public static byte[] Encrypt(Stream inputStream, CompressionLevel compressionLevel, bool pcInput)
        {
            using (var input = new ZlibStream(inputStream, CompressionMode.Compress, compressionLevel, false))
            using (var outputStream = new MemoryStream())
            using (var output = new BinaryWriter(outputStream))
            {
                output.Seek(16, SeekOrigin.Current);
                byte[] buffer = new byte[inputStream.Length];
                int length = input.Read(buffer, 0, buffer.Length);
                output.Write(buffer, 0, length);

                output.Seek(0, SeekOrigin.Begin);
                output.Write(header);

                if (pcInput)
                {
                    output.Write((int)output.BaseStream.Length - 16);
                    output.Write((int)inputStream.Position);
                }
                else
                {
                    output.Write(IPAddress.NetworkToHostOrder((int)output.BaseStream.Length - 16));
                    output.Write(IPAddress.NetworkToHostOrder((int)inputStream.Position));
                }


                var bytes = outputStream.ToArray();
                Array.Resize(ref bytes, (int)output.BaseStream.Length);
                return bytes;
            }
        }

        public static bool IsZlibbed(byte[] inputBytes)
        {
            return IsZlibbed(new MemoryStream(inputBytes));
        }

        public static bool IsZlibbed(string inputFilePath)
        {
            return IsZlibbed(File.OpenRead(inputFilePath));
        }

        public static byte[] Encrypt(byte[] inputBytes, bool pcInput)
        {
            return Encrypt(new MemoryStream(inputBytes), pcInput);
        }

        public static byte[] Encrypt(byte[] inputBytes, CompressionLevel compressionLevel, bool pcInput)
        {
            return Encrypt(new MemoryStream(inputBytes), compressionLevel, pcInput);
        }

        public static void Encrypt(string inputFilePath, bool pcInput)
        {
            File.WriteAllBytes(inputFilePath, Encrypt(File.OpenRead(inputFilePath), pcInput));
        }

        public static void Encrypt(string inputFilePath, CompressionLevel compressionLevel, bool pcInput)
        {
            File.WriteAllBytes(inputFilePath, Encrypt(File.OpenRead(inputFilePath), compressionLevel, pcInput));
        }

        public static void Encrypt(string inputFilePath, string outputFilePath, bool pcInput)
        {
            File.WriteAllBytes(outputFilePath, Encrypt(File.OpenRead(inputFilePath), pcInput));
        }

        public static void Encrypt(string inputFilePath, string outputFilePath, CompressionLevel compressionLevel, bool pcInput)
        {
            File.WriteAllBytes(outputFilePath, Encrypt(File.OpenRead(inputFilePath), compressionLevel, pcInput));
        }

        public static byte[] Decrypt(byte[] inputBytes, bool pcInput)
        {
            return Decrypt(new MemoryStream(inputBytes), pcInput);
        }

        public static byte[] DecryptIfZlibbed(byte[] inputBytes, bool pcInput)
        {
            if (IsZlibbed(inputBytes))
                return Decrypt(new MemoryStream(inputBytes), pcInput);
            else
                return inputBytes;
        }

        public static void Decrypt(string inputFilePath, bool pcInput)
        {
            File.WriteAllBytes(inputFilePath, Decrypt(File.OpenRead(inputFilePath), pcInput));
        }

        public static void DecryptIfZlibbed(string inputFilePath, bool pcInput)
        {
            if (IsZlibbed(inputFilePath))
                File.WriteAllBytes(inputFilePath, Decrypt(File.OpenRead(inputFilePath), pcInput));
        }

        public static void Decrypt(string inputFilePath, string outputFilePath, bool pcInput)
        {
            File.WriteAllBytes(outputFilePath, Decrypt(File.OpenRead(inputFilePath), pcInput));
        }

        public static void DecryptIfZlibbed(string inputFilePath, string outputFilePath, bool pcInput)
        {
            if (IsZlibbed(inputFilePath))
                File.WriteAllBytes(outputFilePath, Decrypt(File.OpenRead(inputFilePath), pcInput));
        }
    }
}

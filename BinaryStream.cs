using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MustafaUğuz.Utility.System.IO
{
    public class BinaryReader : global::System.IO.BinaryReader
    {
        bool BitMode = false;
        int Int32Value = 0;
        uint UInt32Value = 0;
        int BitPosition = 0;
        Encoding encoding;

        public BinaryReader(Stream stream) : base(stream, Encoding.UTF8)
        {
            encoding = Encoding.UTF8;
        }

        public BinaryReader(Stream stream, Encoding encoding) : base(stream, encoding)
        {
            this.encoding = encoding;
        }

        public string ReadString(int count)
        {
            return ReadString(count, true);
        }

        public string ReadString(int count, bool trimEnd)
        {
            return encoding.GetString(ReadBytes(count)).TrimEnd('\0');
        }

        public long Seek(int offset)
        {
            return Seek(offset, SeekOrigin.Begin);
        }

        public long Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return BaseStream.Position = offset;
                case SeekOrigin.Current:
                    return BaseStream.Position += offset;
                case SeekOrigin.End:
                    return BaseStream.Position -= offset;
                default:
                    return BaseStream.Position;
            }
        }

        public int ReadBit(int count)
        {
            if (!BitMode)
            {
                Int32Value = ReadInt32();
                BitMode = true;
            }

            var result = (Int32Value << BitPosition) >> (32 - count);
            BitPosition += count;

            if (BitPosition == 32)
                BitMode = false;

            return result;
        }

        public uint ReadUBit(int count)
        {
            if (!BitMode)
            {
                UInt32Value = ReadUInt32();
                BitMode = true;
            }

            var result = (UInt32Value << BitPosition) >> (32 - count);
            BitPosition += count;

            if (BitPosition == 32)
            {
                BitPosition = 0;
                BitMode = false;
            }

            return result;
        }
    }

    public class BinaryWriter : global::System.IO.BinaryWriter
    {
        string bits = "";
        Encoding encoding;

        public BinaryWriter(Stream stream) : base(stream, Encoding.UTF8)
        {
            encoding = Encoding.UTF8;
        }

        public BinaryWriter(Stream stream, Encoding encoding) : base(stream, encoding)
        {
            this.encoding = encoding;
        }

        public long Seek(int offset)
        {
            return Seek(offset, SeekOrigin.Begin);
        }

        public void WriteString(string value)
        {
            var bytes = encoding.GetBytes(value);
            Write(bytes);
        }

        public void WriteString(string value, int capacity)
        {
            var bytes = encoding.GetBytes(value).PadRight(capacity);
            Write(bytes);
        }

        public void WriteBit(int value, int bitLength, bool bigEndian = true)
        {
            bits += value.ToBinary(bitLength);

            if (bits.Length == 32)
            {
                if (bigEndian)
                    Write(bits.BinaryToByteArray().Reverse().ToArray());
                else
                    Write(Convert.ToInt32(bits));

                bits = "";
            }
        }
    }
}

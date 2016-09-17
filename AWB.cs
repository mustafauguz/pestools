using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MustafaUğuz.PES2017.Sound
{
    public class AWBUtilities
    {
        public static decimal[] CreateIdentitiesList(int count)
        {
            List<decimal> identities = new List<decimal>(0);
            for (int i = 0; i < count; i++)
                identities.Add(i);
            return identities.ToArray();
        }
    }

    public class AWBReader: IDisposable
    {
        bool disposed = false;
        BinaryReader reader;
        long startPos = 0;
        List<decimal> identities;

        public decimal[] Identities { get { return identities.ToArray(); } }

        private List<decimal> StartOffsets { get; set; }

        private List<decimal> Sizes { get; set; }

        public AWBReader(Stream stream)
        {
            reader = new BinaryReader(stream);

            startPos = reader.BaseStream.Position;
            reader.BaseStream.Position += 5;

            var offsetLength = reader.ReadByte();
            var idLength = reader.ReadByte();
            reader.BaseStream.Position++;

            var soundCount = reader.ReadUInt32();
            reader.BaseStream.Position = startPos + 0x10;

            identities = new List<decimal>(0);
            StartOffsets = new List<decimal>(0);
            Sizes = new List<decimal>(0);

            for (int i = 0; i < soundCount; i++)
            {
                if (idLength == 1)
                    identities.Add((long)reader.ReadByte());
                else if (idLength == 2)
                    identities.Add((long)reader.ReadUInt16());
                else if (idLength == 4)
                    identities.Add((long)reader.ReadUInt32());
                else
                    identities.Add((long)reader.ReadUInt64());

                reader.BaseStream.Position = startPos + 0x10 + (soundCount * idLength) + (i * offsetLength);

                long sof = 0;
                long eof = 0;

                if (offsetLength == 1)
                {
                    sof = (long)reader.ReadByte();
                    eof = (long)reader.ReadByte();
                }
                else if (offsetLength == 2)
                {
                    sof = (long)reader.ReadUInt16();
                    eof = (long)reader.ReadUInt16();
                }
                else if (offsetLength == 4)
                {
                    sof = (long)reader.ReadUInt32();
                    eof = (long)reader.ReadUInt32();
                }
                else
                {
                    sof = (long)reader.ReadUInt64();
                    eof = (long)reader.ReadUInt64();
                }

                StartOffsets.Add(sof);
                Sizes.Add(eof - sof);

                reader.BaseStream.Position = startPos + 0x10 + ((i + 1) * idLength);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                reader.Dispose();
            }

            disposed = true;
        }

        ~AWBReader()
        {
            Dispose(false);
        }
        
        public byte[] GetSound(decimal identity)
        {
            var index = identities.IndexOf(identity);
            reader.BaseStream.Position = (long)(startPos + StartOffsets[index]);
            
            var sound = reader.ReadBytes((int)Sizes[index]);

            int skip = 0;

            while (sound[skip] == 0)
                skip++;
            
            return sound.Skip(skip).ToArray();
        }
    }

    public class AWBWriter : IDisposable
    {
        bool disposed = false;
        BinaryWriter writer;
        long startPos = 0;
        decimal[] _identities;
        int pos = 0;

        public AWBWriter(Stream stream, decimal[] identities)
        {
            _identities = identities;
            writer = new BinaryWriter(stream);

            startPos = writer.BaseStream.Position;
            writer.Write(BitConverter.GetBytes(0x4146533201040200).Reverse().ToArray());
            writer.Write((uint)identities.Length);
            writer.Write((uint)0x20);
            writer.Write(new byte[(identities.Length * 2) + ((identities.Length + 1) * 4)]);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                writer.Dispose();
            }

            disposed = true;
        }

        ~AWBWriter()
        {
            Dispose(false);
        }

        private byte[] getForAddBytes(long length, int align = 0x20)
        {
            var foradd = 32 - (length % align);
            return foradd == 32 ? new byte[0] : new byte[foradd];
        }

        public void WriteNextSound(byte[] bytes)
        {
            if (pos < _identities.Length)
            {
                writer.Seek((int)startPos + 0x10 + (pos * 2), SeekOrigin.Begin);
                writer.Write((ushort)_identities[pos]);

                writer.Seek((int)startPos + 0x10 + (_identities.Length * 2) + (pos * 4), SeekOrigin.Begin);
                writer.Write((uint)writer.BaseStream.Length);

                writer.Write(getForAddBytes(writer.Seek(0, SeekOrigin.End)));
                writer.Write(bytes);
            }
            pos++;
            if (pos == _identities.Length)
            {
                writer.Seek((int)startPos + 0x10 + (_identities.Length * 2) + (_identities.Length * 4), SeekOrigin.Begin);
                writer.Write((uint)writer.BaseStream.Length);
            }
        }
    }
}

using MustafaUğuz.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//NOT COMPLETED
//CLOSE FILE STREAMS AFTER USING
//THIS IS A EXAMPLE DATABASE READ/WRITE CODES

namespace MustafaUğuz.PES2017.Database
{
    public partial class Stadium
    {
        public Stadium()
        {

        }

        public Stadium(params object[] values)
        {
            Unknown1 = Convert.ToInt32(values[0]);
            Licensed = Convert.ToInt32(values[1]);
            CountryID = Convert.ToInt32(values[2]);
            Capacity = Convert.ToInt32(values[3]);
            ID = Convert.ToUInt16(values[4]);
            Region = Convert.ToByte(values[5]);
            Unknown2 = Convert.ToByte(values[6]);
            JapanaseName = Convert.ToString(values[7]);
            Name = Convert.ToString(values[8]);
            SystemName = Convert.ToString(values[9]);
        }

        public int Unknown1 { get; set; }

        public int Licensed { get; set; }

        public int CountryID { get; set; }

        public int Capacity { get; set; }

        public ushort ID { get; set; }

        public byte Region { get; set; }

        public byte Unknown2 { get; set; }

        public string JapanaseName { get; set; }

        public string Name { get; set; }

        public string SystemName { get; set; }
    }
    
    public class StadiumReader : System.IO.BinaryReader
    {
        int Loop = 272;

        public StadiumReader(string path) : base(new MemoryStream(ZLib.DecryptIfZlibbed(File.OpenRead(path))), Encoding.UTF8)
        {
            
        }

        public StadiumReader(byte[] bytes) : base(new MemoryStream(ZLib.DecryptIfZlibbed(bytes)), Encoding.UTF8)
        {

        }

        public StadiumReader(Stream stream) : base(new MemoryStream(ZLib.DecryptIfZlibbed(stream)), Encoding.UTF8)
        {

        }

        public Stadium ReadStadium(int index)
        {
            BaseStream.Position = index * Loop;
            
            return new Stadium(
                ReadUBit(2),
                ReadUBit(1),
                ReadUBit(9),
                ReadUBit(20),
                ReadUInt16(),
                ReadByte(),
                ReadByte(),
                ReadString(121),
                ReadString(121),
                ReadString(22)
            );
        }

        public int SectionCount => (int)BaseStream.Length / Loop;

        public List<Stadium> ReadStadiumRange(int index, int count) => Enumerable.Range(index, count).Select(p => ReadStadium(p)).ToList();

        public List<Stadium> ReadAllStadiums() => ReadStadiumRange(0, SectionCount);
    }

    public class StadiumWriter : System.IO.BinaryWriter
    {
        public StadiumWriter(string path) : base(File.Open(path, FileMode.Truncate, FileAccess.Write), Encoding.UTF8)
        {

        }

        public StadiumWriter() : base(new MemoryStream(), Encoding.UTF8)
        {

        }

        public StadiumWriter(Stream stream) : base(stream, Encoding.UTF8)
        {

        }

        public void WriteStadium(Stadium stadium)
        {
            WriteBit(stadium.Unknown1, 2);
            WriteBit(stadium.Licensed, 1);
            WriteBit(stadium.CountryID, 9);
            WriteBit(stadium.Capacity, 20);
            Write(stadium.ID);
            Write(stadium.Region);
            Write(stadium.Unknown2);
            WriteString(stadium.JapanaseName, 121);
            WriteString(stadium.Name, 121);
            WriteString(stadium.SystemName, 22);
        }
        
        public void WriteStadiums(List<Stadium> stadiums) => stadiums.ForEach(p => WriteStadium(p));
    }
}

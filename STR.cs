using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MustafaUğuz.Utility;
using System.Net;

namespace MustafaUğuz.Tools.PES2017
{
    public class Group
    {
        public short ID { get; set; }

        public string Name { get; set; }

        public List<Text> Texts { get; set; }
    }

    public class Text
    {
        public short ID { get; set; }

        public string Value { get; set; }
    }

    public class STRFile
    {
        public static List<Group> Read(string inputFilePath, bool pcInput)
        {
            using (var stream = File.OpenRead(inputFilePath))
                return Read(stream, pcInput);
        }

        public static List<Group> Read(byte[] inputBytes, bool pcInput)
        {
            return Read(new MemoryStream(inputBytes), pcInput);
        }

        public static List<Group> Read(Stream inputStream, bool pcInput)
        {
            using (var inputReader = new Utility.System.IO.BinaryReader(new MemoryStream(ZLib.DecryptIfZlibbed(inputStream, pcInput))))
            {
                var groupCount = pcInput ? inputReader.ReadInt32() : IPAddress.NetworkToHostOrder(inputReader.ReadInt32());
                var startOffset = pcInput ? inputReader.ReadInt32() : IPAddress.NetworkToHostOrder(inputReader.ReadInt32());

                inputReader.Seek((int)startOffset);

                List<Group> Groups = new List<Group>(0);

                for (int i = 0; i < groupCount; i++)
                {
                    var groupsInfoOffset = startOffset + (i * 12);

                    inputReader.Seek((int)groupsInfoOffset);
                    var groupStartOffset = pcInput ? inputReader.ReadInt32() : IPAddress.NetworkToHostOrder(inputReader.ReadInt32());
                    var groupSize = inputReader.ReadInt32();
                    var groupNameOffset = pcInput ? inputReader.ReadInt32() : IPAddress.NetworkToHostOrder(inputReader.ReadInt32());

                    inputReader.Seek((int)groupNameOffset);
                    var groupNameBytes = new List<byte>(0);
                    byte value = 255;
                    while (value != 0)
                        if ((value = inputReader.ReadByte()) != 0)
                            groupNameBytes.Add(value);
                    var groupName = Encoding.UTF8.GetString(groupNameBytes.ToArray());

                    inputReader.Seek((int)groupStartOffset);
                    var groupTextCount = pcInput ? inputReader.ReadInt32() : IPAddress.NetworkToHostOrder(inputReader.ReadInt32());
                    var groupID = pcInput ? inputReader.ReadInt16() : IPAddress.NetworkToHostOrder(inputReader.ReadInt16());
                    inputReader.ReadInt16();

                    var groupTexts = new List<Text>(0);

                    for (int f = 0; f < groupTextCount; f++)
                    {
                        var groupTextsInfoOffset = groupStartOffset + 8 + (f * 12);

                        inputReader.Seek((int)groupTextsInfoOffset);

                        var textID = pcInput ? inputReader.ReadInt16() : IPAddress.NetworkToHostOrder(inputReader.ReadInt16());
                        inputReader.ReadInt16();
                        var textSize = pcInput ? inputReader.ReadInt16() : IPAddress.NetworkToHostOrder(inputReader.ReadInt16());
                        var textLength = pcInput ? inputReader.ReadInt16() : IPAddress.NetworkToHostOrder(inputReader.ReadInt16());
                        var textOffset = (pcInput ? inputReader.ReadInt32() : IPAddress.NetworkToHostOrder(inputReader.ReadInt32())) + groupStartOffset;
                        inputReader.Seek((int)textOffset);
                        var textBytes = new List<byte>(0);
                        value = 255;
                        while (value != 0)
                            if ((value = inputReader.ReadByte()) != 0)
                                textBytes.Add(value);
                        var textValue = Encoding.UTF8.GetString(textBytes.ToArray());

                        groupTexts.Add(new Text()
                        {
                            ID = textID,
                            Value = textValue
                        });
                    }

                    Groups.Add(new Group()
                    {
                        ID = groupID,
                        Name = groupName,
                        Texts = groupTexts
                    });
                }

                return Groups;
            }
        }

        public static void Write(string inputFilePath, List<Group> groups, bool pcInput)
        {
            if (File.Exists(inputFilePath))
                File.Delete(inputFilePath);

            using (var stream = File.OpenWrite(inputFilePath))
                Write(stream, groups, pcInput);
        }

        public static void Write(Stream outputStream, List<Group> groups, bool pcInput)
        {
            using (var outputWriter = new Utility.System.IO.BinaryWriter(outputStream))
            {
                //Writing Header
                outputWriter.Write(pcInput ? groups.Count : IPAddress.NetworkToHostOrder(groups.Count));
                outputWriter.Write(pcInput ? 8 : IPAddress.NetworkToHostOrder(8));
                outputWriter.Write(new byte[groups.Count * 12]);

                var length = 8 + (groups.Count * 12);

                //Writing Group Names
                for (int i = 0; i < groups.Count; i++)
                {
                    long groupNameOffset = outputWriter.BaseStream.Position;

                    outputWriter.Seek(8 + (i * 12));
                    outputWriter.Seek(8, SeekOrigin.Current);
                    outputWriter.Write(pcInput ? (int)groupNameOffset : IPAddress.NetworkToHostOrder((int)groupNameOffset));

                    outputWriter.Seek((int)groupNameOffset);
                    var groupName = groups[i].Name;
                    var groupNameBytesLength = Encoding.UTF8.GetBytes(groupName).Length + 1;
                    outputWriter.WriteString(groupName, groupNameBytesLength);
                    length += groupNameBytesLength;
                }

                var kalan = length % 16;
                var add = kalan == 0 ? 0 : 16 - kalan;
                outputWriter.Write(new byte[add]);
                length += add;

                //Writing Groups
                for (int i = 0; i < groups.Count; i++)
                {
                    //Writing Group Header
                    long groupStartOffset = outputWriter.BaseStream.Position;

                    outputWriter.Seek(8 + (i * 12));
                    outputWriter.Write(pcInput ? (int)groupStartOffset : IPAddress.NetworkToHostOrder((int)groupStartOffset));

                    outputWriter.Seek((int)groupStartOffset);
                    outputWriter.Write(pcInput ? groups[i].Texts.Count : IPAddress.NetworkToHostOrder(groups[i].Texts.Count));
                    outputWriter.Write(pcInput ? groups[i].ID : IPAddress.NetworkToHostOrder(groups[i].ID));
                    outputWriter.Write((short)0);

                    outputWriter.Write(new byte[groups[i].Texts.Count * 12]);
                    length += 8 + (groups[i].Texts.Count * 12);

                    for (int f = 0; f < groups[i].Texts.Count; f++)
                    {
                        string value = groups[i].Texts[f].Value;
                        var valueBytesLength = Encoding.UTF8.GetBytes(value).Length + 1;
                        outputWriter.WriteString(value, valueBytesLength);

                        var lastPos = outputWriter.BaseStream.Position;

                        outputWriter.Seek((int)groupStartOffset + 8 + (f * 12));
                        outputWriter.Write(pcInput ? groups[i].Texts[f].ID : IPAddress.NetworkToHostOrder(groups[i].Texts[f].ID));
                        outputWriter.Write((short)0);
                        outputWriter.Write(pcInput ? (short)valueBytesLength : IPAddress.NetworkToHostOrder((short)valueBytesLength));
                        outputWriter.Write(pcInput ? (short)value.Length : IPAddress.NetworkToHostOrder((short)value.Length));
                        outputWriter.Write(pcInput ? ((int)lastPos - (int)valueBytesLength - (int)groupStartOffset) : IPAddress.NetworkToHostOrder(((int)lastPos - (int)valueBytesLength - (int)groupStartOffset)));

                        length += valueBytesLength;
                        outputWriter.Seek((int)lastPos);
                    }

                    outputWriter.Seek(8 + (i * 12));
                    outputWriter.Seek(4, SeekOrigin.Current);
                    outputWriter.Write(pcInput ? ((int)length - (int)groupStartOffset) : IPAddress.NetworkToHostOrder((int)length - (int)groupStartOffset));

                    outputWriter.Seek(length);

                    kalan = length % 16;
                    add = kalan == 0 ? 0 : 16 - kalan;
                    outputWriter.Write(new byte[add]);
                    length += add;
                }
            }
        }
    }
}

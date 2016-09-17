using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//NOT COMPLETED FULLY

namespace MustafaUğuz.Utility.System
{
    public static partial class Extensions
    {
        public enum Pad
        {
            None,
            Right,
            Left
        }

        public static short ToInt16(this byte[] value)
        {
            return BitConverter.ToInt16(value, 0);
        }
        public static short ToInt16(this byte[] value, int startIndex)
        {
            return BitConverter.ToInt16(value, startIndex);
        }

        public static int ToInt32(this byte[] value)
        {
            return BitConverter.ToInt32(value, 0);
        }
        public static int ToInt32(this byte[] value, int startIndex)
        {
            return BitConverter.ToInt32(value, startIndex);
        }

        public static long ToInt64(this byte[] value)
        {
            return BitConverter.ToInt64(value, 0);
        }
        public static long ToInt64(this byte[] value, int startIndex)
        {
            return BitConverter.ToInt64(value, startIndex);
        }

        public static ushort ToUInt16(this byte[] value)
        {
            return BitConverter.ToUInt16(value, 0);
        }
        public static ushort ToUInt16(this byte[] value, int startIndex)
        {
            return BitConverter.ToUInt16(value, startIndex);
        }

        public static uint ToUInt32(this byte[] value)
        {
            return BitConverter.ToUInt32(value, 0);
        }
        public static uint ToUInt32(this byte[] value, int startIndex)
        {
            return BitConverter.ToUInt32(value, startIndex);
        }

        public static ulong ToUInt64(this byte[] value)
        {
            return BitConverter.ToUInt64(value, 0);
        }
        public static ulong ToUInt64(this byte[] value, int startIndex)
        {
            return BitConverter.ToUInt64(value, startIndex);
        }

        public static byte[] PadRight(this byte[] value, int totalWidth)
        {
            var list = value.ToList();
            list.AddRange(new byte[totalWidth - value.Length]);
            return list.ToArray();
        }

        public static byte[] PadLeft(this byte[] value, int totalWidth)
        {
            var list = value.ToList();
            list.InsertRange(0, new byte[totalWidth - value.Length]);
            return list.ToArray();
        }
    }
}

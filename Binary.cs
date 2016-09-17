using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MustafaUğuz.Utility.System
{
    public static partial class Extensions
    {
        public static string ToBinary(this byte[] value)
        {
            return string.Concat(value.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        }

        public static byte[] BinaryToByteArray(this string value)
        {
            byte[] bytes = new byte[value.Length / 8];
            for (int i = 0; i < bytes.Length; ++i)
                bytes[i] = Convert.ToByte(value.Substring(8 * i, 8), 2);
            return bytes;
        }

        public static string ToBinary(this int value)
        {
            return Convert.ToString(value, 2);
        }

        public static string ToBinary(this int value, int padLeft)
        {
            return Convert.ToString(value, 2).PadLeft(padLeft, '0');
        }
    }
}

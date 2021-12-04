using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rfap_cs_lib
{
    public static class BitTools
    {
        public static List<byte> AddBytesToList(List<byte> list, byte[] append)
        {
            for(int i = 0; i < append.Length; i++)
            {
                list.Add(append[i]);
            }

            return list;
        }

        public static byte[] GetBytesBetweenArray(byte[] array, int start, int stop)
        {
            List<byte> result = new List<byte>();

            for(int i = 0; i < array.Length; i++)
            {
                if(i >= start && i < stop)
                {
                    result.Add(array[i]);
                }
            }

            return result.ToArray();
        }

        public static byte[] GetBytesFromInt(int value)
        {
            byte[] intBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }

            return intBytes;
        }

        public static int GetIntFromByte(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt32(bytes, 0);
        }
    }
}

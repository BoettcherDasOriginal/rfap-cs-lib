using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rfab_cs_lib
{
    public static class BitTools
    {
        public static byte[] AppendBytesToArray(byte[] array, byte[] append)
        {
            for(int i = 0; i < append.Length; i++)
            {
                array.Append(append[i]);
            }

            return array;
        }

        public static byte[] GetBytesBetweenArray(byte[] array, int start, int stop)
        {
            byte[] result = new byte[] { };

            for(int i = 0; i < array.Length; i++)
            {
                if(i > start && i < stop)
                {
                    result.Append(array[i]);
                }
            }

            return result;
        }
    }
}

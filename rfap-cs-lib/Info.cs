using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rfap_cs_lib
{
    static class Info
    {
        public static byte[] VERSION = new byte[2] { 0x00, 0x03 };

        public static int MAX_HEADER_LEN = 8 * 1024;

        public static int MAX_BYTES_SENT_AT_ONCE = 16 * 1024;
    }
}

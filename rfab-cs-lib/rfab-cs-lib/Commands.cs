using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rfab_cs_lib
{
    public static class Commands
    {
        public static byte[] CMD_PING             = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        public static byte[] CMD_DISCONNECT       = new byte[] { 0x01, 0x00, 0x00, 0x00 };
        public static byte[] CMD_INFO             = new byte[] { 0xa0, 0x01, 0x00, 0x00 };
        public static byte[] CMD_ERROR            = new byte[] { 0xff, 0xff, 0xff, 0xff };
        public static byte[] CMD_FILE_READ        = new byte[] { 0xf0, 0x02, 0x00, 0x00 };
        public static byte[] CMD_FILE_DELETE      = new byte[] { 0xf1, 0x01, 0x00, 0x00 };
        public static byte[] CMD_FILE_CREATE      = new byte[] { 0xf1, 0x02, 0x00, 0x00 };
        public static byte[] CMD_FILE_COPY        = new byte[] { 0xf1, 0x03, 0x00, 0x00 };
        public static byte[] CMD_FILE_MOVE        = new byte[] { 0xf1, 0x04, 0x00, 0x00 };
        public static byte[] CMD_FILE_WRITE       = new byte[] { 0xf2, 0x01, 0x00, 0x00 };
        public static byte[] CMD_DIRECTORY_READ   = new byte[] { 0xd0, 0x02, 0x00, 0x00 };
        public static byte[] CMD_DIRECTORY_DELETE = new byte[] { 0xd1, 0x01, 0x00, 0x00 };
        public static byte[] CMD_DIRECTORY_CREATE = new byte[] { 0xd1, 0x02, 0x00, 0x00 };
        public static byte[] CMD_DIRECTORY_COPY   = new byte[] { 0xd1, 0x03, 0x00, 0x00 };
        public static byte[] CMD_DIRECTORY_MOVE   = new byte[] { 0xd1, 0x04, 0x00, 0x00 };
    }
}

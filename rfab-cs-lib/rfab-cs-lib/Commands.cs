using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rfab_cs_lib
{
    public static class Commands
    {
        public static uint CMD_PING             = 0x00000000;
        public static uint CMD_DISCONNECT       = 0x01000000;
        public static uint CMD_INFO             = 0xa0010000;
        public static uint CMD_FILE_READ        = 0xf0020000;
        public static uint CMD_FILE_DELETE      = 0xf1010000;
        public static uint CMD_FILE_CREATE      = 0xf1020000;
        public static uint CMD_FILE_COPY        = 0xf1030000;
        public static uint CMD_FILE_MOVE        = 0xf1040000;
        public static uint CMD_FILE_WRITE       = 0xf2010000;
        public static uint CMD_DIRECTORY_READ   = 0xd0020000;
        public static uint CMD_DIRECTORY_DELETE = 0xd1010000;
        public static uint CMD_DIRECTORY_CREATE = 0xd1020000;
        public static uint CMD_DIRECTORY_COPY   = 0xd1030000;
        public static uint CMD_DIRECTORY_MOVE   = 0xd1040000;
    }
}

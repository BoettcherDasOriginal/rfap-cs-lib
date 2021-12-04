using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rfap_cs_lib
{
    public class Data
    {
        public byte[] Version { get; set; }
        public byte[] Command { get; set; }
        public Dictionary<string,string> Metadata { get; set; }
        public byte[] header_checksum { get; set; }
        public byte[] body { get; set; }
        public byte[] body_checksum { get; set; }

        public Data(byte[] Version, byte[] Command, Dictionary<string,string> Metadata, byte[] header_checksum, byte[] body, byte[] body_checksum)
        {
            this.Version = Version;
            this.Command = Command;
            this.Metadata = Metadata;
            this.header_checksum = header_checksum;
            this.body = body;
            this.body_checksum = body_checksum;
        }
    }
}

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
        public Dictionary<string, dynamic> Metadata { get; set; }
        public byte[] header_checksum { get; set; }
        public byte[] body { get; set; }
        public byte[] body_checksum { get; set; }

        public Data(byte[] Version, byte[] Command, Dictionary<string, dynamic> Metadata, byte[] header_checksum, byte[] body, byte[] body_checksum)
        {
            this.Version = Version;
            this.Command = Command;
            this.Metadata = Metadata;
            this.header_checksum = header_checksum;
            this.body = body;
            this.body_checksum = body_checksum;
        }
    }

    public class FileReadData
    {
        public bool ERROR { get; set; }
        public Dictionary<string, dynamic> Metadata { get; set; }
        public byte[] body { get; set; }

        public FileReadData(Dictionary<string, dynamic> Metadata, byte[] body)
        {
            this.Metadata = Metadata;
            this.body = body;
            ERROR = false;
        }
        public FileReadData(Dictionary<string, dynamic> Metadata, byte[] body, bool ERROR)
        {
            this.Metadata = Metadata;
            this.body = body;
            this.ERROR = ERROR;
        }
    }

    public class DirectoryReadData
    {
        public bool ERROR { get; set; }
        public Dictionary<string, dynamic> Metadata { get; set; }
        public List<string> decodeedBody { get; set; }

        public DirectoryReadData(Dictionary<string, dynamic> Metadata, List<string> decodeedBody)
        {
            this.Metadata= Metadata;
            this.decodeedBody = decodeedBody;
            ERROR = false;
        }
        public DirectoryReadData(Dictionary<string, dynamic> Metadata, List<string> decodeedBody, bool ERROR)
        {
            this.Metadata = Metadata;
            this.decodeedBody = decodeedBody;
            this.ERROR = ERROR;
        }
    }
}

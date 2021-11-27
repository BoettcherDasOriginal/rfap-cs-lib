using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using YamlDotNet.Serialization;

namespace rfab_cs_lib
{
    public class Client
    {
        TcpClient tcpClient = null;

        public bool Connect(IPAddress iPAddress, int port)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(iPAddress, port);
            return tcpClient.Connected;
        }

        public bool send_command(uint command, Dictionary<string, string> metaData, byte[] body)
        {
            if (tcpClient != null)
            {
                var yamlSerializer = new SerializerBuilder().Build();


                // send version

                tcpClient.Client.Send(Info.VERSION);


                // encode header

                byte[] header = new byte[] { };

                BitTools.AppendBytesToArray(header, BitConverter.GetBytes(command));
                BitTools.AppendBytesToArray(header, Encoding.UTF8.GetBytes(yamlSerializer.Serialize(metaData)));
                BitTools.AppendBytesToArray(header, new byte[32]);


                // send header

                tcpClient.Client.Send(BitConverter.GetBytes(header.Length));
                tcpClient.Client.Send(header);

                // send body
                if (body == null)
                {
                    body = new byte[0];
                }
                tcpClient.Client.Send(BitConverter.GetBytes(body.Length));
                tcpClient.Client.Send(body);
                tcpClient.Client.Send(new byte[32]);


                return true;
            }
            else
            {
                return false;
            }

        }

        public Data recv_command()
        {
            if(tcpClient != null)
            {
                var yamlDeserializer = new DeserializerBuilder().Build();


                // get version

                byte[] version = new byte[2];
                tcpClient.Client.Receive(version);


                // get header

                byte[] header_length = new byte[4];
                tcpClient.Client.Receive(header_length);

                byte[] header_raw = new byte[BitConverter.ToInt32(header_length, 0)];
                tcpClient.Client.Receive(header_raw);

                uint command = BitConverter.ToUInt32(header_raw, 0);
                Dictionary<string, string> metadata = yamlDeserializer.Deserialize<Dictionary<string,string>>(Encoding.UTF8.GetString(BitTools.GetBytesBetweenArray(header_raw, 0, header_raw.Length - 32)));
                byte[] header_checksum = BitTools.GetBytesBetweenArray(header_raw,header_raw.Length - 32, header_raw.Length);


                // get body

                byte[] body_length = new byte[4];
                tcpClient.Client.Receive(body_length);
                byte[] body = new byte[BitConverter.ToInt32(body_length, 0) - 32];
                tcpClient.Client.Receive(body);
                byte[] body_checksum = new byte[32];
                tcpClient.Client.Receive(body_checksum);


                return new Data(version,command,metadata,header_checksum,body,body_checksum);
            }
            else
            {
                return null;
            }
        }

        public bool rfab_ping()
        {
            if(tcpClient != null)
            {
                Dictionary<string,string> metadata = new Dictionary<string,string>();
                send_command(Commands.CMD_PING, metadata, null);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool rfab_disconnect()
        {
            if (tcpClient != null)
            {
                Dictionary<string, string> metadata = new Dictionary<string, string>();
                send_command(Commands.CMD_DISCONNECT, metadata, null);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using YamlDotNet.Serialization;
using System.Threading;

namespace rfap_cs_lib
{
    public class Client
    {
        TcpClient tcpClient = null;
        int waitForResponse = 0;
        byte[] supportetVersion = new byte[] { 0x00, 0x01 };

        public bool Connect(IPAddress iPAddress, int port, int waitForResponse)
        {
            tcpClient = new TcpClient();

            this.waitForResponse = waitForResponse;

            try { tcpClient.Connect(iPAddress, port); return tcpClient.Connected; }
            catch { tcpClient = null; return false; }
        }

        #region Send/Receive command

        public bool send_command(byte[] command, Dictionary<string, dynamic> metaData, byte[] body)
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                var yamlSerializer = new SerializerBuilder().Build();

                List<byte> data = new List<byte>();

                // add version

                data = BitTools.AddBytesToList(data, Info.VERSION);


                // encode header

                List<byte> header = new List<byte>();

                header = BitTools.AddBytesToList(header, command);
                header = BitTools.AddBytesToList(header, Encoding.UTF8.GetBytes(yamlSerializer.Serialize(metaData)));
                header = BitTools.AddBytesToList(header, new byte[32]);


                // add header

                data = BitTools.AddBytesToList(data, BitTools.GetBytesFromInt(header.ToArray().Length));
                data = BitTools.AddBytesToList(data, header.ToArray());


                // add body
                if (body == null)
                {
                    body = new byte[0];
                }

                data = BitTools.AddBytesToList(data, BitTools.GetBytesFromInt(body.Length + 32));
                data = BitTools.AddBytesToList(data, body);
                data = BitTools.AddBytesToList(data,new byte[32]);
                

                // send Data

                tcpClient.Client.Send(data.ToArray());

                return true;
            }
            else
            {
                return false;
            }

        }

        public Data recv_command()
        {
            if(tcpClient != null && tcpClient.Connected)
            {
                try
                {
                    var yamlDeserializer = new DeserializerBuilder().Build();

                    byte[] data = new byte[2 + 4 + (16 * 1024 * 1024) + 4 + (16 * 1024 * 1024)];
                    tcpClient.Client.Receive(data);


                    //get version

                    byte[] version = new byte[2];
                    version = BitTools.GetBytesBetweenArray(data, 0, 2);

                    

                    if(version[0] != Info.VERSION[0] && version[1] != Info.VERSION[1])
                    {
                        throw new Exception($"trying to receive packet of unsupported version (v{version[0]}.{version[1]})");
                    }


                    //get header

                    byte[] header_length = new byte[4];
                    header_length = BitTools.GetBytesBetweenArray(data, 2, 2 + 4);

                    int header_length_int = BitTools.GetIntFromByte(header_length);

                    byte[] header_raw = new byte[header_length_int];
                    header_raw = BitTools.GetBytesBetweenArray(data, 2 + 4, 2 + 4 + header_length_int);

                    byte[] command = BitTools.GetBytesBetweenArray(header_raw, 0, 4);
                    Dictionary<string, dynamic> metadata = yamlDeserializer.Deserialize<Dictionary<string, dynamic>>(Encoding.UTF8.GetString(BitTools.GetBytesBetweenArray(header_raw, 4, header_raw.Length - 32)));
                    byte[] header_checksum = BitTools.GetBytesBetweenArray(header_raw, header_raw.Length - 32, header_raw.Length);

                    // get body

                    byte[] body_length = new byte[4];
                    body_length = BitTools.GetBytesBetweenArray(data, 2 + 4 + BitTools.GetIntFromByte(header_length), 2 + 4 + BitTools.GetIntFromByte(header_length) + 4);

                    byte[] body = new byte[BitTools.GetIntFromByte(body_length) - 32];
                    body = BitTools.GetBytesBetweenArray(data, 2 + 4 + BitTools.GetIntFromByte(header_length) + 4, 2 + 4 + BitTools.GetIntFromByte(header_length) + 4 + BitTools.GetIntFromByte(body_length) - 32);

                    byte[] body_checksum = new byte[32];
                    body_checksum = BitTools.GetBytesBetweenArray(data, 2 + 4 + BitTools.GetIntFromByte(header_length) + 4 + BitTools.GetIntFromByte(body_length) - 32, 2 + 4 + BitTools.GetIntFromByte(header_length) + 4 + BitTools.GetIntFromByte(body_length));


                    // return data

                    return new Data(version, command, metadata, header_checksum, body, body_checksum);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR receiving data:");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("");

                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region IMPLEMENTATION OF RFAP COMMANDS

        public bool rfab_ping()
        {
            if(tcpClient != null && tcpClient.Connected)
            {
                Dictionary<string, dynamic> metadata = new Dictionary<string, dynamic>();
                send_command(Commands.CMD_PING, metadata, null);
                Thread.Sleep(waitForResponse);
                recv_command();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool rfab_disconnect()
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                Dictionary<string, dynamic> metadata = new Dictionary<string, dynamic>();
                send_command(Commands.CMD_DISCONNECT, metadata, null);
                tcpClient.Close();

                return true;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<string, dynamic> rfap_info(string path, bool verbose)
        {
            string[] requireDetails = new string[] { };
            if (verbose)
            {
                requireDetails = new string[] { "DirectorySize", "ElementsNumber" };
            }

            send_command(Commands.CMD_INFO, new Dictionary<string, dynamic>() { { "Path", path }, { "RequestDetails", requireDetails } }, null);
            Thread.Sleep(waitForResponse);
            Data data = recv_command();
            if (data != null)
            {
                return data.Metadata;
            }
            else
            {
                return null;
            }
        }

        // TO-DO

        // - rfap_file_read function
        // - rfap_directory_read function

        #endregion
    }
}

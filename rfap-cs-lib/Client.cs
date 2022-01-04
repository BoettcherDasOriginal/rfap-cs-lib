using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using YamlDotNet.Serialization;
using System.Security.Cryptography;
using System.Threading;

namespace rfap_cs_lib
{
    public class Client
    {
        TcpClient tcpClient = null;
        SHA256 sha256Hash = null;
        int waitForResponse = 0;
        byte[] supportetVersion = new byte[] { 0x00, 0x01 };

        /// <summary>
        /// Connects the rfap client with the specified address of the rfap server
        /// </summary>
        /// <param name="iPAddress"></param>
        /// <param name="port"></param>
        /// <param name="waitForResponse"></param>
        /// <returns>
        /// Returns true if the client has successfully connected
        /// </returns>
        public bool Connect(IPAddress iPAddress, int port, int waitForResponse)
        {
            tcpClient = new TcpClient();
            sha256Hash = SHA256.Create();

            this.waitForResponse = waitForResponse;

            try { tcpClient.Connect(iPAddress, port); return tcpClient.Connected; }
            catch { tcpClient = null; return false; }
        }

        #region Send/Receive command

        /// <summary>
        /// Sends the specified command, metadata and body to the connected rfap server
        /// </summary>
        /// <param name="command"></param>
        /// <param name="metaData"></param>
        /// <param name="body"></param>
        /// <returns>Returns false if something went wrong</returns>
        public bool send_packet(byte[] command, Dictionary<string, dynamic> metaData, byte[] body)
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                var yamlSerializer = new SerializerBuilder().Build();


                // HEADER


                List<byte> header_data = new List<byte>();

                header_data = BitTools.AddBytesToList(header_data, Info.VERSION);

                List<byte> header = new List<byte>();

                header = BitTools.AddBytesToList(header, command);
                header = BitTools.AddBytesToList(header, Encoding.UTF8.GetBytes(yamlSerializer.Serialize(metaData)));
                byte[] header_checksum = sha256Hash.ComputeHash(header.ToArray());
                header = BitTools.AddBytesToList(header, header_checksum);

                header_data = BitTools.AddBytesToList(header_data, BitTools.GetBytesFromInt(header.ToArray().Length));
                header_data = BitTools.AddBytesToList(header_data, header.ToArray());

                tcpClient.Client.Send(header_data.ToArray());


                // BODY


                if (body == null)
                {
                    body = new byte[0];
                }

                byte[] body_len = BitTools.GetBytesFromInt(body.Length + 32);
                byte[] body_checksum = sha256Hash.ComputeHash(body);

                tcpClient.Client.Send(body_len);

                int i = 0;
                List<byte> body_data = new List<byte>();
                body_data = BitTools.AddBytesToList(body_data, body);
                body_data = BitTools.AddBytesToList(body_data, body_checksum);

                while (true)
                {
                    if(i + Info.MAX_BYTES_SENT_AT_ONCE > body_data.ToArray().Length)
                    {
                        tcpClient.Client.Send(BitTools.GetBytesBetweenArray(body_data.ToArray(),i, body_data.ToArray().Length));
                        break;
                    }
                    tcpClient.Client.Send(BitTools.GetBytesBetweenArray(body_data.ToArray(), i, i + Info.MAX_BYTES_SENT_AT_ONCE));
                    i += Info.MAX_BYTES_SENT_AT_ONCE;
                }

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

                    //(byte[] data = new byte[2 + 4 + (16 * 1024 * 1024) + 4 + (16 * 1024 * 1024)];
                    //tcpClient.Client.Receive(data);


                    //get version

                    byte[] version = new byte[2];
                    tcpClient.Client.Receive(version);

                    if(version[0] != Info.VERSION[0] && version[1] != Info.VERSION[1])
                    {
                        throw new Exception($"trying to receive packet of unsupported version (v{version[0]}.{version[1]})");
                    }


                    //get header

                    byte[] header_length_raw = new byte[4];
                    tcpClient.Client.Receive(header_length_raw);
                    int header_length = BitTools.GetIntFromByte(header_length_raw);

                    byte[] header_raw = new byte[header_length];
                    tcpClient.Client.Receive(header_raw);

                    byte[] command = BitTools.GetBytesBetweenArray(header_raw, 0, 4);
                    Dictionary<string, dynamic> metadata = yamlDeserializer.Deserialize<Dictionary<string, dynamic>>(Encoding.UTF8.GetString(BitTools.GetBytesBetweenArray(header_raw, 4, header_raw.Length - 32)));
                    byte[] header_checksum = BitTools.GetBytesBetweenArray(header_raw, header_raw.Length - 32, header_raw.Length);


                    //get Body

                    byte[] body_length = new byte[4];
                    tcpClient.Client.Receive(body_length);

                    List<byte> body = new List<byte>();
                    while (body.ToArray().Length < BitTools.GetIntFromByte(body_length) - 32)
                    {
                        byte[] bytesReceived = new byte[Info.MAX_BYTES_SENT_AT_ONCE];
                        tcpClient.Client.Receive(bytesReceived);

                        body = BitTools.AddBytesToList(body, bytesReceived);
                    }

                    byte[] body_checksum = new byte[32];
                    tcpClient.Client.Receive(body_checksum);

                    // return data

                    return new Data(version, command, metadata, header_checksum, body.ToArray(), body_checksum);
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
                send_packet(Commands.CMD_PING, metadata, null);
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
                send_packet(Commands.CMD_DISCONNECT, metadata, null);
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

            send_packet(Commands.CMD_INFO, new Dictionary<string, dynamic>() { { "Path", path }, { "RequestDetails", requireDetails } }, null);
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

        public FileReadData rfap_file_read(string path)
        {
            send_packet(Commands.CMD_FILE_READ, new Dictionary<string, dynamic>() { { "Path", path } }, null);
            Thread.Sleep(waitForResponse);
            Data data = recv_command();

            bool ERROR = false;
            for (int i = 0; i < data.Metadata.Count; i++)
            {
                if (data.Metadata.ElementAt(i).Key == "ErrorCode" && data.Metadata.ElementAt(i).Value != 0)
                {
                    ERROR = true;
                }
            }

            if (ERROR) { return new FileReadData(data.Metadata, new byte[0], true); }
            else { return new FileReadData(data.Metadata, data.body); }
        }

        public DirectoryReadData rfap_directory_read(string path,bool verbose)
        {
            string[] requireDetails = new string[] { };
            if (verbose)
            {
                requireDetails = new string[] { "DirectorySize", "ElementsNumber" };
            }

            send_packet(Commands.CMD_DIRECTORY_READ, new Dictionary<string, dynamic>() { { "Path", path }, { "RequestDetails", requireDetails } }, null);
            Thread.Sleep (waitForResponse);
            Data directoryData = recv_command();

            bool ERROR = false;
            for (int i = 0; i < directoryData.Metadata.Count; i++)
            {
                if (directoryData.Metadata.ElementAt(i).Key == "ErrorCode" && directoryData.Metadata.ElementAt(i).Value != 0)
                {
                    ERROR = true;
                }
            }

            if (ERROR) 
            {
                return new DirectoryReadData(directoryData.Metadata, new List<string>(),true);
            }
            else
            {
                List<string> body = new List<string>();
                foreach (string str in Encoding.UTF8.GetString(directoryData.body).Split('\n'))
                {
                    if (str != "")
                    {
                        body.Add(str);
                    }
                }

                return new DirectoryReadData(directoryData.Metadata, body);
            }
        }

        #endregion
    }
}

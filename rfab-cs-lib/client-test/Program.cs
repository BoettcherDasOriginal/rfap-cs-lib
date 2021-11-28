using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rfab_cs_lib;
using System.Net;

namespace client_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            bool connected = client.Connect(IPAddress.Parse("127.0.0.1"), 6700);

            if (connected)
            {
                Console.WriteLine("Connected to 127.0.0.1:6700");

                Dictionary<string, string> metadata = new Dictionary<string, string>() { { "Path", "C:/GoServer" } };
                client.send_command(Commands.CMD_INFO, metadata, null);

                Data data = client.recv_command();
                for(int i = 0; i < data.Metadata.Count; i++)
                {
                    Console.WriteLine(data.Metadata.ElementAt(i));
                }
                

                client.rfab_ping();
                client.rfab_disconnect();

                Console.WriteLine("Disconnected from 127.0.0.1:6700");
            }
            else
            {
                Console.WriteLine("Can't connect to 127.0.0.1:6700");
            }

            Console.ReadLine();
        }
    }
}

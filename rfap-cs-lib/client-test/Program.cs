using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rfap_cs_lib;
using System.Net;

namespace client_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            bool connected = client.Connect(IPAddress.Parse("127.0.0.1"), 6700,100);

            if (connected)
            {
                Console.WriteLine("Connected to 127.0.0.1:6700");

                var header = client.rfap_info("/",true);
                if(header != null)
                {
                    for (int i = 0; i < header.Count; i++)
                    {
                        Console.WriteLine(header.ElementAt(i));
                    }
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

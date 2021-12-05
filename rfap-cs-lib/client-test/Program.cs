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
                        if(header.ElementAt(i).Value is List<object>)
                        {
                            List<object> list = (List<object>)header.ElementAt(i).Value;
                            if(list.Count > 0)
                            {
                                Console.WriteLine("[" + header.ElementAt(i).Key);
                                foreach (string str in list)
                                {
                                    Console.WriteLine(str);
                                }
                                Console.WriteLine("]");
                            }
                            else
                            {
                                Console.WriteLine("[" + header.ElementAt(i).Key + ", EMPTY]");
                            }
                        }
                        else
                        {
                            Console.WriteLine(header.ElementAt(i));
                        }
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

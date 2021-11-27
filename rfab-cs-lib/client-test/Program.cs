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
            client.Connect(IPAddress.Parse("127.0.0.1"), 6700);


            Console.ReadLine();
        }
    }
}

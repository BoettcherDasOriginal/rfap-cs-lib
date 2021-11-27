using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

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
    }
}

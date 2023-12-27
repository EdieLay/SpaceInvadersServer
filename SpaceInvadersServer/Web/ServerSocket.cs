using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SpaceInvadersServer
{
    internal class ServerSocket
    { 
        IPEndPoint endPoint { get; set; }
        Socket socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        public ServerSocket(IPAddress ip)
        {
            endPoint = new IPEndPoint(ip, 8791);
            socket.Bind(endPoint);
        }

        public byte[] RecievePacket()
        {
            throw new NotImplementedException();
        }

        public void SendPacket(byte[] packet)
        {
            throw new NotImplementedException();
        }
    }
}

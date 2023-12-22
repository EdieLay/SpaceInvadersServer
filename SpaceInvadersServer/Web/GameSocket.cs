using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class GameSocket
    {
        static int FREE_PORT = 8792;
        IPEndPoint endPoint { get; set; }
        Socket socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


        public GameSocket(IPAddress ip)
        {
            endPoint = new IPEndPoint(ip, FREE_PORT++);
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

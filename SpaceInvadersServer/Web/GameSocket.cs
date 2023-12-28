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
        IPEndPoint endPoint { get; set; }
        Socket socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


        public GameSocket(IPAddress ip, int port)
        {
            endPoint = new IPEndPoint(ip, port);
            socket.Bind(endPoint);
        }

        public byte[] RecievePacket()
        {
            EndPoint client = endPoint;
            byte[] buffer = new byte[128];
            socket.ReceiveFrom(buffer, ref client);
            PacketOpcode opcode = (PacketOpcode)buffer[0];
        }

        public void SendPacket(byte[] packet)
        {
            throw new NotImplementedException();
        }
    }
}

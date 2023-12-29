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
        EndPoint client;
        IPEndPoint endPoint { get; set; }
        Socket socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public delegate void SessionRespondDelegate(PlayerInput input);
        SessionRespondDelegate respondPlayerInput;


        public GameSocket(IPAddress ip, int port, SessionRespondDelegate respondPlayerInput)
        {
            endPoint = new IPEndPoint(ip, port);
            socket.Bind(endPoint);
            client = endPoint;
            byte[] buffer = new byte[32];
            socket.ReceiveFrom(buffer, ref client);
            socket.Connect(client); // возможно, из-за этого будут проблемы с UDP
            this.respondPlayerInput = respondPlayerInput;
            Thread receiving = new(StartReceiving);
            receiving.Start();
        }

        void StartReceiving()
        {
            while (true)
            {
                ReceivePacket();
            }
        }

        void ReceivePacket()
        {
            byte[] buffer = new byte[128];
            socket.ReceiveFrom(buffer, ref client);
            PacketOpcode opcode = (PacketOpcode)buffer[0];
            switch(opcode)
            {
                case PacketOpcode.KeyDown:
                    if (0 == buffer[1])
                        respondPlayerInput(PlayerInput.LeftKeyDown);
                    else 
                        respondPlayerInput(PlayerInput.RightKeyDown);
                    break;
                case PacketOpcode.KeyUp:
                    if (0 == buffer[1])
                        respondPlayerInput(PlayerInput.LeftKeyUp);
                    else
                        respondPlayerInput(PlayerInput.RightKeyUp);
                    break;
                case PacketOpcode.ShotKeyDown:
                    respondPlayerInput(PlayerInput.Shot);
                    break;
                default:
                    break;
            }
        }

        public void SendPacket(byte[] buffer)
        {
            socket.SendTo(buffer, client);
        }

        public void ShutdownAndClose()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}

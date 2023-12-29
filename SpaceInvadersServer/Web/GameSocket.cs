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
        EndPoint clientEP;
        IPEndPoint endPoint { get; set; }
        Socket socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public delegate void SessionRespondDelegate(PlayerInput input);
        SessionRespondDelegate respondPlayerInput;


        public GameSocket(IPEndPoint ipEP, EndPoint _clientEP, SessionRespondDelegate respondPlayerInput)
        {
            endPoint = ipEP;
            socket.Bind(endPoint);
            this.clientEP = _clientEP;
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
            //EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("ReceivePacket() clientEP: " + clientEP.ToString());
            Console.WriteLine("ReceivePacket() socket: " + socket.LocalEndPoint.ToString());
            socket.ReceiveFrom(buffer, ref clientEP);
            Console.WriteLine("after socket.ReceiveFrom");
            Console.WriteLine(clientEP);
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
            socket.SendTo(buffer, clientEP);
        }

        public void ShutdownAndClose()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}

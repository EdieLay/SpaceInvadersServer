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
        //IPEndPoint endPoint { get; set; }
        Socket socket { get; set; }

        public delegate void SessionRespondDelegate(PlayerInput input);
        SessionRespondDelegate respondPlayerInput;


        public GameSocket(Socket gameSocket, SessionRespondDelegate respondPlayerInput)
        {
            socket = gameSocket;
            clientEP = socket.RemoteEndPoint;
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
            Console.WriteLine("ReceivePacket() clientEP: " + clientEP.ToString());
            Console.WriteLine("ReceivePacket() socket: " + socket.LocalEndPoint.ToString());
            int numberOfBytes = socket.Receive(buffer);
            Console.WriteLine("after socket.ReceiveFrom: numberOfBytes = " + numberOfBytes.ToString());
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
            socket.Send(buffer);
            Console.WriteLine("GameSocket SendPacket..");
        }

        public void ShutdownAndClose()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}

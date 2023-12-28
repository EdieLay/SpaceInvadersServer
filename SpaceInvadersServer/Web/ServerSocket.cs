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
    enum PacketOpcode : byte
    {
        OpenNewSocket = 0, // Открытие нового сокета : с-к
        PressPlay = 1, // Нажата кнопка играть : к-с
        GameObjectsInfo = 2, // Инфа об игроке, пацанах и пулях : с-к
        KeyDown = 3, // Кнопка нажата (KeyDown) : к-с
        KeyUp = 4, // Кнопка отжата (KeyUp) : к-с
        ShotKeyDown = 5, // Кнопка выстрела (KeyDown) : к-с
        NewScore = 6, // Новый счёт при попадании : с-к
        PlayerDeath = 7, // Смерть игрока (конец игры) : с-к
    }
    internal class ServerSocket
    {
        static int FREE_PORT = 8792;

        public delegate void NewSessionHandler(int port);
        NewSessionHandler CreateNewSession;

        IPEndPoint endPoint { get; set; }
        Socket socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        public ServerSocket(IPAddress ip, NewSessionHandler create)
        {
            endPoint = new IPEndPoint(ip, 8791);
            socket.Bind(endPoint);
            CreateNewSession = create;
            Thread listenThread = new(StartPolling);
            listenThread.Start();
        }

        void StartPolling()
        {
            socket.Listen(20);

            while (true)
            {
                try
                {
                    Socket client = socket.Accept();

                    CreateNewSession(FREE_PORT);

                    byte[] message = new byte[3];
                    message[0] = (byte)PacketOpcode.OpenNewSocket;
                    short port = (short)FREE_PORT;
                    message[1] = (byte)(port >> 8);
                    message[2] = (byte)(port & 0xFF);

                    client.Send(message);

                    FREE_PORT++;

                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (SocketException)
                {

                }
            }
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

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
        PressPlay = 0, // Нажата кнопка играть : к-с
        GameObjectsInfo = 1, // Инфа об игроке, пацанах и пулях : с-к
        KeyDown = 2, // Кнопка нажата (KeyDown) : к-с
        KeyUp = 3, // Кнопка отжата (KeyUp) : к-с
        ShotKeyDown = 4, // Кнопка выстрела (KeyDown) : к-с
        NewScore = 5, // Новый счёт при попадании : с-к
        PlayerDeath = 6, // Смерть игрока (конец игры) : с-к
    }
    internal class ServerSocket
    {
        public delegate void NewSessionDelegate(Socket gameSck);
        NewSessionDelegate CreateNewSession; // это будет метод GameServer'а для создания новой сессии

        IPEndPoint endPoint { get; set; }
        Socket socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        public ServerSocket(IPAddress ip, NewSessionDelegate create)
        {
            endPoint = new IPEndPoint(ip, 8791);
            socket.Bind(endPoint);
            CreateNewSession = create; // получаем метод геймсервера в делегат
            Thread listenThread = new(StartPolling);
            listenThread.Start(); // начинаем принимать клиентов в новом потоке
        }

        void StartPolling()
        {
            socket.Listen(20);

            while (true)
            {
                try
                {
                    Socket client = socket.Accept(); // приняли клиента
                    if (client.RemoteEndPoint == null)
                    {
                        continue;
                    }
                    EndPoint clientEndPoint = client.RemoteEndPoint;
                    Console.WriteLine("StartPolling(): " + clientEndPoint.ToString());

                    byte[] message = new byte[3];

                    Console.WriteLine("StartPolling() socket: " + client.LocalEndPoint.ToString());

                    byte[] buffer = new byte[16];
                    client.Receive(buffer); // в этот момент клиент может отправить только сообщение о начале игры

                    // поэтому не делаю никаких проверок и сразу создаю новую сессию
                    CreateNewSession(client); // я хз, нужен LocalEndPoint или Remote
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"SocketException: {e.Message}");
                }
            }
        }
    }
}

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
        public delegate void NewSessionDelegate(IPEndPoint ipEP, EndPoint client);
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
                    message[0] = (byte)PacketOpcode.OpenNewSocket;

                    IPEndPoint ipEP = new(endPoint.Address, 0);
                    Console.WriteLine("StartPolling() socket: " + ipEP.ToString());
                    short port = (short)ipEP.Port;
                    message[1] = (byte)(port >> 8);
                    message[2] = (byte)(port & 0xFF);

                    client.Send(message); // отправили сообщение с новым портом

                    byte[] buffer = new byte[16];
                    client.Receive(buffer); // в этот момент клиент может отправить только сообщение о начале игры
                    
                    client.Shutdown(SocketShutdown.Both); // закрыли соединение
                    client.Close();

                    // поэтому не делаю никаких проверок и сразу создаю новую сессию
                    CreateNewSession(ipEP, clientEndPoint); // я хз, нужен LocalEndPoint или Remote
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"SocketException: {e.Message}");
                }
            }
        }
    }
}

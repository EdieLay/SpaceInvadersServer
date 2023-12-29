using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class GameServer // мб сделать класс синглтон
    {
        IPAddress ip;
        List<GameSession> sessions = new List<GameSession>();
        ServerSocket socket;

        public GameServer(IPAddress ip)
        {
            this.ip = ip;
            socket = new ServerSocket(ip, CreateNewSession); // сначала посмотри конструктор ServerSocket
            // потом - что делает CreatNewSession
        }

        void CreateNewSession(Socket gameSocket) // открывает новую игровую сессию
        {
            sessions.Add(new(gameSocket, CloseSession));
        }

        void CloseSession(GameSession session)
        {
            sessions.Remove(session);
        }
    }
}

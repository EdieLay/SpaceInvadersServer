using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            socket = new ServerSocket(ip, CreateNewSession);
        }

        void Init()
        {
            // начинает принимать сообщения в сокет
            // при получении сообщения создаёт новый GameSession, добавляет в sessions и запускает его
            // можно сделать другой вид сообщения, чтобы завершать сессию
            throw new NotImplementedException();
        }

        void CreateNewSession(int port)
        {
            sessions.Add(new(ip, port));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class GameServer // мб сделать класс синглтон
    {
        List<GameSession> sessions = new List<GameSession>();
        ServerSocket socket;

        void Init()
        {
            // начинает принимать сообщения в сокет
            // при получении сообщения создаёт новый GameSession, добавляет в sessions и запускает его
            // можно сделать другой вид сообщения, чтобы завершать сессию
            throw new NotImplementedException();
        }
    }
}

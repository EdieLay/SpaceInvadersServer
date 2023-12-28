using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class GameSession
    {
        const int TIMER_INTERVAL_MS = 30;
        GameSocket socket; // сокет для отправки и получения игровых данных
        PacketManager packetManager; // класс для конвертации отправляющихся и полученных данных
        BattleField battleField;

        public GameSession(IPAddress ip, int port)
        {
            socket = new GameSocket(ip, port);
            packetManager = new PacketManager();
            battleField = new BattleField();
            Thread game = new(StartGame);
            game.Start();
        }

        public void StartGame()
        {
            TimerCallback timerCallback = new(SendGameInfo);
            Timer timer = new(timerCallback, null, 0, TIMER_INTERVAL_MS);
        }

        void SendGameInfo(object? obj)
        {
            byte[] gameInfo = battleField.Update();
            socket.SendPacket(gameInfo);
        }
    }
}

using SpaceInvadersServer.GameObjects;
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
        const int TIMER_INTERVAL_MS = 60;
        GameSocket sck; // сокет для отправки и получения игровых данных
        PacketManager packetManager; // класс для конвертации отправляющихся и полученных данных
        BattleField battleField;

        public GameSession(IPAddress ip)
        {
            sck = new GameSocket(ip);
            packetManager = new PacketManager();
            battleField = new BattleField();
        }


        void Init()
        {
            // инициализация и запуск игрового процесса
            throw new NotImplementedException();
        }

        public void StartGame()
        {
            TimerCallback timerCallback = new(SendGameInfo);
            Timer timer = new(timerCallback, null, 0, TIMER_INTERVAL_MS);
        }

        void SendGameInfo(object? obj)
        {
            battleField.Update();
            byte[] message;

        }
    }
}

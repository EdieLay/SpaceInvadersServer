using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    enum PlayerInput
    {
        RightKeyDown,
        RightKeyUp,
        LeftKeyDown,
        LeftKeyUp,
        Shot
    }
    internal class GameSession
    {
        const int TIMER_INTERVAL_MS = 30;
        GameSocket socket; // сокет для отправки и получения игровых данных
        BattleField battleField; // игровое поле с врагами, игроком и пулями

        public delegate void CloseSessionDelegate(GameSession session); // делегат для закрытия этой сесии в GameServer
        CloseSessionDelegate CloseSession;

        public GameSession(Socket gameSocket, EndPoint clientEP, CloseSessionDelegate closeSession)
        {
            socket = new GameSocket(gameSocket, clientEP, RespondPlayerInput); // передаем в сокет метод, который будет вызываться для реагирования на ввод игрока
            battleField = new BattleField(SendScore); // передаем в баттлфилд метод, который будет вызываться при обновлении счёта
            CloseSession = closeSession;
            Thread game = new(StartGame);
            game.Start(); // начинаем игру в новом потоке
        }

        public void StartGame()
        {
            TimerCallback timerCallback = new(SendGameInfo); // вызываем SendGameInfo по тику таймера
            Timer timer = new(timerCallback, null, 0, TIMER_INTERVAL_MS);
        }

        void SendGameInfo(object? obj)
        {
            byte[] gameInfo = battleField.Update(); // метод обновляет игру и сразу формирует сообщение с инфой об игре
            socket.SendPacket(gameInfo); // отправляем инфу об игре
            if ((byte)PacketOpcode.PlayerDeath == gameInfo[0]) // если мы отправили сообщение о смерти игрока
            {
                socket.ShutdownAndClose(); // то закрываем сокет
                CloseSession(this); // и закрываем сессию
            }
        }

        void SendScore(int score) // метод для отправки счёта
        {
            byte[] buffer = new byte[5];
            buffer[0] = (byte)PacketOpcode.NewScore;
            buffer[1] = (byte)(score >> 24 & 0xFF);
            buffer[2] = (byte)(score >> 16 & 0xFF);
            buffer[3] = (byte)(score >> 8 & 0xFF);
            buffer[4] = (byte)(score & 0xFF);
            socket.SendPacket(buffer);
        }

        void RespondPlayerInput(PlayerInput input) // ответ на ввод игрока
        {
            switch (input)
            {
                case PlayerInput.RightKeyDown: // если нажал "вправо"
                    battleField.playerMovement = PlayerMovement.ToRight; // то идём вправо
                    break;
                case PlayerInput.RightKeyUp: // если отпустил "вправо", то сначала проверяем, не шёл ли он влево
                    if (PlayerMovement.ToRight == battleField.playerMovement)
                        battleField.playerMovement = PlayerMovement.Idle; // если он не шёл влево, то останавливаем
                    // эта логика нужна, если я нажму вправо, а потом, не отжимая, нажму влево
                    break;
                case PlayerInput.LeftKeyDown: // то же самое, что и с "вправо"
                    battleField.playerMovement = PlayerMovement.ToLeft;
                    break;
                case PlayerInput.LeftKeyUp:
                    if (PlayerMovement.ToLeft == battleField.playerMovement)
                        battleField.playerMovement = PlayerMovement.Idle;
                    break;
                case PlayerInput.Shot: // выстрел игрока
                    battleField.PlayerShot();
                    break;
                default:
                    break;
            }
        }
    }
}

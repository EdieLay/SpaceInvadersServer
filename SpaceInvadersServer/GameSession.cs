using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        BattleField battleField;

        public delegate void CloseSessionDelegate(GameSession session);
        CloseSessionDelegate CloseSession;

        public GameSession(IPAddress ip, int port, CloseSessionDelegate closeSession)
        {
            socket = new GameSocket(ip, port, RespondPlayerInput);
            battleField = new BattleField(SendScore);
            CloseSession = closeSession;
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
            if ((byte)PacketOpcode.PlayerDeath == gameInfo[0])
            {
                socket.ShutdownAndClose();
                CloseSession(this);
            }
        }

        void SendScore(int score)
        {
            byte[] buffer = new byte[5];
            buffer[0] = (byte)PacketOpcode.NewScore;
            buffer[1] = (byte)(score >> 24 & 0xFF);
            buffer[2] = (byte)(score >> 16 & 0xFF);
            buffer[3] = (byte)(score >> 8 & 0xFF);
            buffer[4] = (byte)(score & 0xFF);
            socket.SendPacket(buffer);
        }

        void RespondPlayerInput(PlayerInput input)
        {
            switch (input)
            {
                case PlayerInput.RightKeyDown:
                    battleField.playerMovement = PlayerMovement.ToRight;
                    break;
                case PlayerInput.RightKeyUp:
                    if (PlayerMovement.ToRight == battleField.playerMovement)
                        battleField.playerMovement = PlayerMovement.Idle;
                    break;
                case PlayerInput.LeftKeyDown:
                    battleField.playerMovement = PlayerMovement.ToLeft;
                    break;
                case PlayerInput.LeftKeyUp:
                    if (PlayerMovement.ToLeft == battleField.playerMovement)
                        battleField.playerMovement = PlayerMovement.Idle;
                    break;
                case PlayerInput.Shot:
                    break;
                default:
                    break;
            }
        }
    }
}

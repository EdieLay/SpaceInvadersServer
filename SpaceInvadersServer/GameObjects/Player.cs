using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class Player : IPackable
    {
        const int WIDTH = 20;
        const int HEIGHT = 15;
        const int SPEED = 10;

        readonly int _Y;
        readonly int FIELD_WIDTH; // ширина поля
        readonly int FIELD_HEIGHT; // высота поля

        int x;
        public int Center { get { return x + WIDTH / 2; } }
        public int Y { get { return _Y; } }

        public Player(int fieldWidth, int fieldHeight)
        {
            FIELD_WIDTH = fieldWidth; 
            FIELD_HEIGHT = fieldHeight;
            _Y = FIELD_HEIGHT - 4 * HEIGHT;
            x = FIELD_WIDTH / 2 - WIDTH / 2;
        }

        public void Move(bool toRight)
        {
            if (toRight)
                x = x + SPEED > FIELD_WIDTH + WIDTH ? FIELD_WIDTH + WIDTH : x + SPEED;
            else
                x = x - SPEED < 0 ? 0 : x - SPEED;
        }

        public bool CalculateBulletsCollision(List<Bullet> enemyBullets)
        {
            for (int i = 0; i < enemyBullets.Count; i++)
            {
                int bulX = enemyBullets[i].X;
                int bulY = enemyBullets[i].Y;
                int bulHeight = enemyBullets[i].HEIGHT;
                int bulWidth = enemyBullets[i].WIDTH;
                if (bulY + bulHeight > _Y && bulY < _Y + HEIGHT &&
                    bulX + bulWidth > x && bulX < x + WIDTH)
                {
                    enemyBullets.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        // пока что столкновение с пацанами реализовано просто, как заход на один игрек с игроком
        public bool CalculateEnemyCollision(int downBorder)
        {
            if (downBorder > _Y)
                return true;
            return false;
        }

        public byte[] GetInfo()
        {
            byte[] message = new byte[2];
            message[0] = (byte)(x >> 8);
            message[1] = (byte)(x & 0xFF);
            return message;
        }
    }
}

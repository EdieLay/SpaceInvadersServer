using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class Bullet : IPackable
    {
        const int FIELD_HEIGHT = 800;
        const int _WIDTH = 2;
        const int _HEIGHT = 5;
        public int WIDTH { get => _WIDTH; }
        public int HEIGHT { get => _HEIGHT; }
        readonly int SPEED;
        readonly bool IS_ENEMY;
        public bool IsAlive { get; set; }
        int x; // координаты левого верхнего угла пули
        int y;
        public int X { get => x; }
        public int Y { get => y; }

        public Bullet(int x, int y, bool isEnemy, bool isAlive = true)
        {
            this.x = x;
            this.y = y;
            IS_ENEMY = isEnemy;
            SPEED = isEnemy ? 7 : -7;
            IsAlive = isAlive;
        }

        public void Move()
        {
            if (x > FIELD_HEIGHT || x + HEIGHT < 0)
                IsAlive = false;
            x += SPEED;
        }

        public byte[] GetInfo()
        {
            byte[] message = new byte[5];
            message[0] = (byte)(x >> 8);
            message[1] = (byte)(x & 0xFF);
            message[2] = (byte)(y >> 8);
            message[3] = (byte)(y & 0xFF);
            message[4] = (byte)SPEED;
            return message;
        }
    }
}

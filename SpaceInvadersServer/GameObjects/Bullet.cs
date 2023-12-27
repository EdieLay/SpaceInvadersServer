using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class Bullet
    {
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
            SPEED = isEnemy ? 15 : -15;
            IsAlive = isAlive;
        }

        public void Move()
        {
            x += SPEED;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer.GameObjects
{
    internal class Bullet
    {
        public const int WIDTH = 2;
        public const int HEIGHT = 5;
        readonly int SPEED;
        readonly bool IS_ENEMY;
        int x; // координаты левого верхнего угла пули
        int y;
        public int X { get => x; }
        public int Y { get => y; }

        public Bullet(int x, int y, bool isEnemy)
        {
            this.x = x;
            this.y = y;
            IS_ENEMY = isEnemy;
            SPEED = isEnemy ? 15 : -15;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer.GameObjects
{
    internal class Bullet
    {
        const int WIDTH = 2;
        const int HEIGHT = 5;
        readonly int SPEED;
        int x;
        int y;

        public Bullet(int x, int y, bool isEnemy)
        {
            this.x = x;
            this.y = y;
            SPEED = isEnemy ? 15 : -15;
        }
    }
}

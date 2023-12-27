using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class Player
    {
        const int WIDTH = 20;
        const int HEIGHT = 15;
        const int SPEED = 10;

        readonly int Y;
        readonly int FIELD_WIDTH; // ширина поля
        readonly int FIELD_HEIGHT; // высота поля

        int x;

        public Player(int fieldWidth, int fieldHeight)
        {
            FIELD_WIDTH = fieldWidth; 
            FIELD_HEIGHT = fieldHeight;
            Y = FIELD_HEIGHT - 2 * HEIGHT;
            x = FIELD_WIDTH / 2 - WIDTH / 2;
        }

        public void Move(bool toRight)
        {
            if (toRight)
                x = x + SPEED > FIELD_WIDTH + WIDTH ? FIELD_WIDTH + WIDTH : x + SPEED;
            else
                x = x - SPEED < 0 ? 0 : x - SPEED;
        }
    }
}

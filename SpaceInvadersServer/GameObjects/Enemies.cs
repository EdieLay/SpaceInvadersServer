using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer.GameObjects
{
    internal class Enemies
    {
        const int ROWS = 5; // количество рядов пацанов
        const int COLS = 11; // количество столбцов пацанов
        const int WIDTH = 20; // ширина пацана
        const int HEIGHT = 15; // высота пацана
        readonly int FIELD_WIDTH; // ширина поля
        readonly int FIELD_HEIGHT; // высота поля
        readonly int GAP_X; // расстояние от правого конца пацана до следующего пацана
        readonly int GAP_Y; // расстояние от нижнего конйа пацана до следующего пацана
        bool[] enemies; // массив пацанов: true - жив, false - мертв
        int enemiesAlive;
        int offsetX; // смещение левого верхнего угла каждого пацана относительно его начальной позиции по икс
        int offsetY; // смещение левого верхнего угла каждого пацана относительно его начальной позиции по игрек
        int speed; // скорость пацанов по икс за тик таймера

        public Enemies(int fieldWidth, int fieldHeight)
        {
            enemies = new bool[ROWS * COLS]; // 5 строк по 11 пацанов
            enemiesAlive = ROWS * COLS;
            Array.Fill(enemies, true);
            offsetX = 0;
            offsetY = 0;
            CalculateSpeed();
            FIELD_WIDTH = fieldWidth;
            FIELD_HEIGHT = fieldHeight;
            GAP_X = 3;
            GAP_Y = 2 * HEIGHT;
        }

        public void Move()
        {
            offsetX += speed;
            if (offsetX + WIDTH + GAP_X >= FIELD_WIDTH)
            {
                offsetY += HEIGHT;
                offsetX = FIELD_WIDTH - 1;
                speed *= -1;
            }
            else if (offsetX < 0)
            {
                offsetY++;
                offsetX = 0;
                speed *= -1;
            }
        }

        void CalculateSpeed()
        {
            speed = (int)(243.0 / (enemiesAlive + 5.75));
        }
    }
}

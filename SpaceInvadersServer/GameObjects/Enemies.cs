using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer.GameObjects
{
    internal class Enemies
    {
        const int ROWS = 5; // количество строк пацанов
        const int COLS = 11; // количество столбцов пацанов
        const int WIDTH = 20; // ширина пацана
        const int HEIGHT = 15; // высота пацана
        const int GAP_X = 3; // расстояние от правого конца пацана до следующего пацана
        const int GAP_Y = HEIGHT; // расстояние от нижнего конйа пацана до следующего пацана
        readonly int FIELD_WIDTH; // ширина поля
        readonly int FIELD_HEIGHT; // высота поля
        bool[] enemies; // массив пацанов: true - жив, false - мертв
        int enemiesAlive;
        int offsetX; // смещение левого верхнего угла каждого пацана относительно его начальной позиции по икс
        int offsetY; // смещение левого верхнего угла каждого пацана относительно его начальной позиции по игрек
        int speed; // скорость пацанов по икс за тик таймера
        int downBorder;
        int upBorder;
        int leftBorder;
        int rightBorder;

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
            downBorder = ROWS * HEIGHT + (ROWS - 1) * GAP_Y;
            upBorder = 0;
            leftBorder = 0;
            rightBorder = COLS * WIDTH + (COLS - 1) * GAP_X;
        }

        public void Move()
        {
            offsetX += speed;
            if (offsetX + WIDTH * COLS + GAP_X * (COLS - 1) >= FIELD_WIDTH)
            {
                offsetY += HEIGHT;
                offsetX = FIELD_WIDTH - WIDTH * COLS + GAP_X * (COLS - 1);
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
            // enemiesAlive = 55 => speed = 4
            // enemiesAlive = 1  => speed = 36
            speed = (int)(243.0 / (enemiesAlive + 5.75));
        }

        public void CalculateBulletCollision(Bullet bullet)
        {
            int bulX = bullet.X;
            int bulY = bullet.Y;
            int bulWidth = bullet.WIDTH;
            int bulHeight = bullet.HEIGHT;
            if (bulY > downBorder || bulY + bulHeight < upBorder ||
                bulX + bulWidth < leftBorder || bulX > rightBorder)
                return;
            // добавить проверку на столкновение
        }

        void GetBorders()
        {
            // Высчитывание границ живых пацанов
        }
    }
}

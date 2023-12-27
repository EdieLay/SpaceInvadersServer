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

        int downBorderNum; // номер нижней границы пацанов
        int upBorderNum; // номер верхней
        int leftBorderNum; // номер левой
        int rightBorderNum; // номер правой

        int downBorder; // координата y нижней границы
        int upBorder; // координаты y верхней границы
        int leftBorder; // координата x левой границы
        int rightBorder; // координата x правой границы

        int wave; // текущая волна

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
            downBorderNum = ROWS - 1;
            upBorderNum = 0;
            leftBorderNum = 0;
            rightBorderNum = COLS - 1;
            downBorder = ROWS * HEIGHT + (ROWS - 1) * GAP_Y;
            upBorder = 0;
            leftBorder = 0;
            rightBorder = COLS * WIDTH + (COLS - 1) * GAP_X;
            wave = 1;
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
            speed = (int)((243.0 + (wave - 1.0) * 10) / (enemiesAlive + 5.75));
        }

        public int CalculateBulletCollision(Bullet bullet)
        {
            if (!bullet.IsAlive) return 0;

            int bulX = bullet.X;
            int bulY = bullet.Y;
            int bulWidth = bullet.WIDTH;
            int bulHeight = bullet.HEIGHT;
            if (bulY > downBorder || bulY + bulHeight < upBorder ||
                bulX + bulWidth < leftBorder || bulX > rightBorder)
                return 0;

            int x, y;
            for (int i = upBorderNum; i <= downBorderNum; i++) 
            {
                for (int j = leftBorderNum; j <= rightBorderNum; j++)
                {
                    if (!enemies[i * COLS + j]) continue;
                    x = j * (WIDTH + GAP_X);
                    y = i * (HEIGHT + GAP_Y);
                    if (x > bulX + bulWidth || x + WIDTH < bulX ||
                        y > bulY + bulHeight || y + HEIGHT < bulY)
                        continue;
                    enemies[i * COLS + j] = false;
                    enemiesAlive--;
                    bullet.IsAlive = false;
                    GetBorders();
                    CalculateSpeed();
                    if (enemiesAlive == 0)
                    {
                        StartNewWave();
                        return wave - 1;
                    }
                    return wave;
                }
            }
            return 0;
        }

        void GetBorders() // возможно передавать сюда номер убитого чела, чтобы относительно его считать
        {
            if (enemiesAlive == 0) return;
            // Высчитывание границ живых пацанов
            int i = upBorderNum * COLS + leftBorderNum;
            while (!enemies[i])
            {
                if (i % COLS == rightBorderNum)
                {
                    upBorderNum++;
                    i = upBorderNum * COLS + leftBorderNum;
                }
                else i++;
            }

            i = downBorderNum * COLS + leftBorderNum;
            while (!enemies[i])
            {
                if (i % COLS == rightBorderNum)
                {
                    downBorderNum--;
                    i = downBorderNum * COLS + leftBorderNum;
                }
                else i++;
            }

            i = upBorderNum * COLS + leftBorderNum;
            while (!enemies[i])
            {
                if (i / COLS == downBorderNum)
                {
                    leftBorderNum++;
                    i = upBorderNum * COLS + leftBorderNum;
                }
                i += COLS;
            }

            i = upBorderNum * COLS + rightBorderNum;
            while (!enemies[i])
            {
                if (i / COLS == downBorderNum)
                {
                    rightBorderNum--;
                    i = upBorderNum * COLS + rightBorderNum;
                }
                i += COLS;
            }

            upBorder = upBorderNum * (HEIGHT + GAP_Y);
            downBorder = downBorderNum * (HEIGHT + GAP_Y) + HEIGHT;
            leftBorder = leftBorderNum * (WIDTH + GAP_X);
            rightBorder = rightBorderNum * (WIDTH + GAP_X) + WIDTH;
        }

        void StartNewWave()
        {
            enemiesAlive = ROWS * COLS;
            Array.Fill(enemies, true);
            offsetX = 0;
            offsetY = 0;
            CalculateSpeed(); 
            downBorderNum = ROWS - 1;
            upBorderNum = 0;
            leftBorderNum = 0;
            rightBorderNum = COLS - 1;
            downBorder = ROWS * HEIGHT + (ROWS - 1) * GAP_Y;
            upBorder = 0;
            leftBorder = 0;
            rightBorder = COLS * WIDTH + (COLS - 1) * GAP_X;
            wave++;
        }
    }
}

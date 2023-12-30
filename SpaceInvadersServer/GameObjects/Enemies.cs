using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class Enemies : IPackable
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

        public int DownBorder { get { return downBorder; } }

        int wave; // текущая волна

        public Enemies(int fieldWidth, int fieldHeight)
        {
            enemies = new bool[ROWS * COLS]; // 5 строк по 11 пацанов
            enemiesAlive = ROWS * COLS;
            Array.Fill(enemies, true);
            offsetX = 0;
            offsetY = 0;
            speed = 1;
            CalculateSpeed();
            FIELD_WIDTH = fieldWidth;
            FIELD_HEIGHT = fieldHeight;
            downBorderNum = ROWS - 1;
            upBorderNum = 0;
            leftBorderNum = 0;
            rightBorderNum = COLS - 1;
            GetBorders();
            wave = 1;
        }

        public void Move()
        {
            offsetX += speed;
            if (rightBorder + offsetX >= FIELD_WIDTH)
            {
                offsetY += HEIGHT;
                offsetX = FIELD_WIDTH - WIDTH * (rightBorderNum + 1) - GAP_X * rightBorderNum;
                speed *= -1;
            }
            else if (leftBorder + offsetX < 0)
            {
                offsetY += HEIGHT;
                offsetX = 0 - WIDTH * leftBorderNum - GAP_X * leftBorderNum;
                speed *= -1;
            }
            //Console.WriteLine($"{leftBorder} {offsetX} {downBorder + offsetY}");
        }

        void CalculateSpeed()
        {
            // enemiesAlive = 55 => speed = 4
            // enemiesAlive = 1  => speed = 36
            speed = Math.Sign(speed) * (int)((243.0 + (wave - 1.0) * 10) / (enemiesAlive + 5.75)) / 2;
        }

        public int CalculateBulletCollision(Bullet bullet)
        {
            if (!bullet.IsAlive) return 0;

            int bulX = bullet.X;
            int bulY = bullet.Y;
            int bulWidth = bullet.WIDTH;
            int bulHeight = bullet.HEIGHT;
            if (bulY > downBorder + offsetY || bulY + bulHeight < upBorder + offsetY ||
                bulX + bulWidth < leftBorder + offsetX || bulX > rightBorder + offsetX)
                return 0;

            int x, y;
            for (int i = upBorderNum; i <= downBorderNum; i++) 
            {
                for (int j = leftBorderNum; j <= rightBorderNum; j++)
                {
                    if (!enemies[i * COLS + j]) continue;
                    x = j * (WIDTH + GAP_X) + offsetX;
                    y = i * (HEIGHT + GAP_Y) + offsetY;
                    if (x > bulX + bulWidth || x + WIDTH < bulX ||
                        y > bulY + bulHeight || y + HEIGHT < bulY)
                        continue;
                    // если пуля попала в пацна
                    Console.WriteLine($"Killed {i * COLS + j}");
                    enemies[i * COLS + j] = false; // убиваем пацана
                    enemiesAlive--; // уменьшаем кол-во пацанлв
                    bullet.IsAlive = false; // пля тоже теперь не жива
                    GetBorders(); // обновляем рамки пацанов
                    CalculateSpeed(); // обновляем скорость пацанов
                    if (enemiesAlive == 0) // если пацанов нет, то обновляем пацанов
                    {
                        StartNewWave();
                        return wave - 1;
                    }
                    return wave; // возвращаем текущую волну, как награду за попадание
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

        public (int, int) GetRandomFrontRowCoordinates() // возвращает координаты рандомного пацана, перед которым никого нет
        {
            var rand = new Random();
            while (true)
            {
                int col = rand.Next(leftBorderNum, rightBorderNum + 1);
                for (int i = downBorderNum; i >= upBorderNum; i--)
                {
                    if (enemies[i * COLS + col])
                        return (col * (WIDTH + GAP_X) + WIDTH / 2 + offsetX, i * (HEIGHT + GAP_Y) + HEIGHT + offsetY);
                }
            }
        }

        void StartNewWave() // новая волна
        {
            enemiesAlive = ROWS * COLS; // все пацаны теперь живы
            Array.Fill(enemies, true); // заполняем живыми пацанами массив
            offsetX = 0; // ресетим оффсеты пацанов
            offsetY = 0;
            CalculateSpeed(); // обновляем скорость пацанов
            downBorderNum = ROWS - 1; // обновляем рамки пацанов
            upBorderNum = 0;
            leftBorderNum = 0;
            rightBorderNum = COLS - 1;
            downBorder = ROWS * HEIGHT + (ROWS - 1) * GAP_Y;
            upBorder = 0;
            leftBorder = 0;
            rightBorder = COLS * WIDTH + (COLS - 1) * GAP_X;
            wave++; // увеличиваем счётчик волны
        }

        public byte[] GetInfo()
        {
            byte[] message = new byte[13];
            message[0] = (byte)(offsetX >> 8);
            message[1] = (byte)(offsetX & 0xFF);
            message[2] = (byte)(offsetY >> 8);
            message[3] = (byte)(offsetY & 0xFF);
            message[4] = (byte)(speed >> 8);
            message[5] = (byte)(speed & 0xFF);
            for (int i = 0; i < 7; i++)
            {
                byte alive = 0;
                for (int j = 0; j < 8; j++)
                {
                    alive = (byte)(alive << 1);
                    if (8 * i + j < ROWS * COLS)
                        if (enemies[8 * i + j])
                            alive |= 0b_0000_0001;
                }
                message[6 + i] = alive;
            }
            return message;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class BattleField : IPackable
    {
        const int WIDTH = 600;
        const int HEIGHT = 800;
        Enemies enemies;
        List<Bullet> enemyBullets;
        Bullet playerBullet;
        Player player;
        int score;
        int packetNum;

        public BattleField()
        {
            enemies = new Enemies(WIDTH, HEIGHT);
            enemyBullets = new List<Bullet>();
            playerBullet = new Bullet(0, 0, false, false);
            player = new Player(WIDTH, HEIGHT);
            score = 0;
            packetNum = 0;
        }

        public byte[] Update() // тут происходит вся логика игры
        {
            enemies.Move();
            for (int i = 0; i < enemyBullets.Count; i++)
                enemyBullets[i].Move();
            playerBullet.Move();
            // проверять, что зажата кнопка, и тогда => player.Move()
            score += enemies.CalculateBulletCollision(playerBullet);
            if (player.CalculateBulletsCollision(enemyBullets))
            {
                // закончить игру и отправить пакет
            }
            return GetInfo();
        }

        public void UpdatePlayer(/*params*/)
        {
            player.Move(/*params*/);
        }

        public byte[] GetInfo()
        {
            int playerBulletAlive = playerBullet.IsAlive ? 5 : 0;
            byte[] message = new byte[1 + 2 + 2 + 2 + 2 + 2 + 7 + 1 + playerBulletAlive + enemyBullets.Count * 5];
            message[0] = (byte)PacketOpcode.GameObjectsInfo;
            message[1] = (byte)(packetNum >> 8);
            message[2] = (byte)(packetNum & 0xFF);
            packetNum++;
            byte[] info = player.GetInfo();
            info.CopyTo(message, 3);
            info = enemies.GetInfo();
            info.CopyTo(message, 5);
            int bulletsNum = playerBullet.IsAlive ? 1 + enemyBullets.Count : enemyBullets.Count;
            message[18] = (byte)bulletsNum;
            int curIndex = 19;
            if (playerBullet.IsAlive) 
            {
                info = playerBullet.GetInfo();
                info.CopyTo(message, curIndex);
                curIndex += 5;
            }
            for (int i = 0; i < enemyBullets.Count; i++)
            {
                info = enemyBullets[i].GetInfo();
                info.CopyTo(message, curIndex);
                curIndex += 5;
            }
            return message;
        }
    }
}

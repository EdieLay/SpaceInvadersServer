using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    enum PlayerMovement
    {
        Idle,
        ToRight,
        ToLeft
    }
    internal class BattleField : IPackable
    {
        const int WIDTH = 600;
        const int HEIGHT = 800;
        Enemies enemies;
        List<Bullet> enemyBullets; // мб сделать интерфейс IBullet и сразу разделить на PlayerBullet и EnemyBullets??
        Bullet playerBullet;
        Player player;
        int score;
        int packetNum;
        public PlayerMovement playerMovement { get; set; }

        public delegate void SendScoreDelegate(int score);
        SendScoreDelegate SendScore;

        public BattleField(SendScoreDelegate sendScore)
        {
            enemies = new Enemies(WIDTH, HEIGHT);
            enemyBullets = new List<Bullet>();
            playerBullet = new Bullet(0, 0, false, false);
            player = new Player(WIDTH, HEIGHT);
            score = 0;
            packetNum = 0;
            playerMovement = PlayerMovement.Idle;
            SendScore = sendScore;
        }

        public byte[] Update() // тут происходит вся логика игры
        {
            enemies.Move();
            for (int i = 0; i < enemyBullets.Count; i++)
            {
                enemyBullets[i].Move();
                if (!enemyBullets[i].IsAlive)
                {
                    enemyBullets.RemoveAt(i);
                    i--;
                }
            }
            if (playerBullet.IsAlive) playerBullet.Move();
            UpdatePlayer();
            int prevScore = score;
            score += enemies.CalculateBulletCollision(playerBullet);
            if (score != prevScore) 
            {
                SendScore(score);
            }
            if (player.CalculateBulletsCollision(enemyBullets))
            {
                byte[] gameOver = new byte[1];
                gameOver[0] = (byte)PacketOpcode.PlayerDeath;
                return gameOver;
            }
            return GetInfo();
        }

        public void PlayerShot()
        {
            if (!playerBullet.IsAlive)
                playerBullet = new Bullet(player.Center, player.Y, false);
        }

        void UpdatePlayer()
        {
            switch (playerMovement)
            {
                case PlayerMovement.Idle:
                    break;
                case PlayerMovement.ToRight:
                    player.Move(true);
                    break;
                case PlayerMovement.ToLeft:
                    player.Move(false); 
                    break;
            }
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

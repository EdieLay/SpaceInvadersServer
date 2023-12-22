using SpaceInvadersServer.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class BattleField
    {
        const int WIDTH = 600;
        const int HEIGHT = 800;
        Enemies enemies;
        List<Bullet> enemyBullets;
        Bullet? playerBullet;

        public BattleField()
        {
            enemies = new Enemies(WIDTH, HEIGHT);
            enemyBullets = new List<Bullet>();
            playerBullet = null;
        }

        public void Update() // тут происходит вся логика игры
        {
            enemies.Move();
        }
    }
}

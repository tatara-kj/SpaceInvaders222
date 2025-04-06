using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceIntruder
{
    public class GameLevel
    {
        public int[] WaveEnemies = new int[3];
        public int FastEnemyChance = 0;
        public int ShieldedEnemyChance = 0;
        public int EnemySpeed = 8;

        public GameLevel(int[] waveEnemies, int fastEnemyChance, int shieldedEnemyChance, int enemySpeed) { 
            WaveEnemies = waveEnemies;
            FastEnemyChance = fastEnemyChance;
            ShieldedEnemyChance= shieldedEnemyChance;
            EnemySpeed = enemySpeed;
        }
    }
}

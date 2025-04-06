using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceIntruder
{
    public class Enemy
    {
        public int MoveDirection = 1;
        public int HP = 3;
        public float SpeedMultiplier = 1f;
        public float EnemyType = 0; // 0-Normalny, 1-Szybki, 2-Opancerzony
    }
}

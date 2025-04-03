using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceIntruder
{
    public class EnemyChaser
    {
        public float SavedPlayerLeftPos;

        public EnemyChaser(float playerLeftPos) {
            SavedPlayerLeftPos = playerLeftPos;
        }
    }
}

using System;

namespace PvPNet
{
    [Serializable]
    public class BattleResponseData
    {
        public bool IsBattleContinue
        {
            get; 
            set;
        }

        public string AttackMessageLog
        {
            get; 
            set;
        }

        public string EnemyAttackMessageLog
        {
            get; 
            set;
        }

        public int CurrentHp
        {
            get; 
            set;
        }

        public int EnemyCurrentHp
        {
            get; 
            set;
        }
    }
}
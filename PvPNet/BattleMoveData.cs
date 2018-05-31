using System;
using System.Security.Cryptography.X509Certificates;

namespace PvPNet
{
    [Serializable]
    public class BattleMoveData
    {
        public Guid BattleId
        {
            get; 
            set;
        }

        public BattleHitZone Attack
        {
            get; 
            set;
        }
        
        public BattleHitZone Defence
        {
            get; 
            set;
        }

        public string TargetName
        {
            get; 
            set;
        }
    }
}
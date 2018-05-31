using System;

namespace PvPNet
{
    [Serializable]
    public class NewBattleData
    {
        public string OponentName
        {
            get;
            set;
        }

        public Guid BattleId
        {
            get; 
            set;
        }
    }
}
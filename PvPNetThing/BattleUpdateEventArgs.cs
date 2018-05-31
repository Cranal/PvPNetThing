using System;

namespace PvPNetThing
{
    public class BattleUpdateEventArgs : EventArgs
    {
        public BattleStatusData BattleStatusData
        {
            get; 
            set;
        }

        public BattleUpdateEventArgs(BattleStatusData battleStatusData)
        {
            this.BattleStatusData = battleStatusData;
        }
    }
}
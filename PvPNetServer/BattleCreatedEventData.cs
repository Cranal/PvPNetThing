using System.Collections.Generic;
using PvPNet;

namespace PvPNetServer
{
    public class BattleCreatedEventData
    {
        public NewBattleData BattleData
        {
            get;
            set;
        }

        public BattleCreatedEventData(ConnectedClient combatant, NewBattleData battleData)
        {
            this.BattleData = battleData;
        }
    }
}
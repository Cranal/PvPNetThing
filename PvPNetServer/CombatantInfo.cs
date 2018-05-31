using PvPNet;

namespace PvPNetServer
{
    public class CombatantInfo
    {
        public ConnectedClient Client
        {
            get;
            set;
        }

        public int CurentHp
        {
            get;
            set;
        }

        public CombatantInfo(ConnectedClient client)
        {
            this.Client = client;
            this.CurentHp = 100;
        }

        public string CurrentTargetName
        {
            get; 
            set;
        }

        public void ClearBattleData()
        {
            this.BlockLocation = null;
            this.HitLocation = null;
            this.CurrentTargetName = null;
        }

        public BattleHitZone? HitLocation
        {
            get; 
            private set;
        }
        
        public BattleHitZone? BlockLocation
        {
            get; 
            private set;
        }

        public void SetMove(BattleHitZone hitLocation, BattleHitZone blockLocation, string targetName)
        {
            this.HitLocation = hitLocation;
            this.BlockLocation = blockLocation;
            this.CurrentTargetName = targetName;
        }

        public bool IsMoveMade
        {
            get
            {
                return this.HitLocation.HasValue && this.BlockLocation.HasValue;
            }
        }

        public void ClearMove()
        {
            this.HitLocation = null;
            this.BlockLocation = null;
        }
    }
}
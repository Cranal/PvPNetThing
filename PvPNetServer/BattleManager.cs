using System;
using System.Collections.Generic;
using System.Linq;
using PvPNet;

namespace PvPNetServer
{
    public class BattleManager
    {
        public event EventHandler<BattleCreatedEventData> BattleFoundEvent;
        
        private Queue<ConnectedClient> BattleQueue
        {
            get;
            set;
        }

        private List<Battle> ActiveBattles
        {
            get; 
            set;
        }

        public BattleManager()
        {
            this.BattleQueue = new Queue<ConnectedClient>();
            this.ActiveBattles = new List<Battle>(); 
        }

        public void ProcessBattleMove(ClientMessage data)
        {
            var opHandler = new OperationDataHandler();

            BattleMoveData moveData = opHandler.TranslateObject<BattleMoveData>(data.Operation.OperationData);

            Battle battleToProcess = this.ActiveBattles.FirstOrDefault(x => x.BattleId.Equals(moveData.BattleId));

            if (battleToProcess != null)
            {
                battleToProcess.MakeMove(data.Client, moveData.Attack, moveData.Defence, moveData.TargetName);

                if (battleToProcess.IsBattleEnded)
                {
                    this.ActiveBattles.Remove(battleToProcess);
                    battleToProcess = null;
                }
            }
        }

        public void FindBattle(ConnectedClient client)
        {
            if (!this.BattleQueue.Any())
            {
                this.BattleQueue.Enqueue(client);
                Console.WriteLine(string.Format("{0} added to the battle queue", client.ClientName));
                return;
            }

            //
            // If someone already looks for battle then match them in queue order.
            //

            var enemy = this.BattleQueue.Dequeue();
            
            var firstPlayer = new CombatantInfo(client);
            var secondPlayer = new CombatantInfo(enemy);

            Guid battleId = Guid.NewGuid();
            
            var battle = new Battle(firstPlayer, secondPlayer, battleId);
            
            this.ActiveBattles.Add(battle);
            
            NewBattleData data = new NewBattleData()
            {
                OponentName = enemy.ClientName,
                BattleId = battleId
            };
            
            this.OnBattleFound(client, data);
            
            data = new NewBattleData()
            {
                BattleId = battleId,
                OponentName = client.ClientName
            };
            
            this.OnBattleFound(enemy, data);
        }

        private void OnBattleFound(ConnectedClient combatant, NewBattleData data)
        {
            if (this.BattleFoundEvent != null)
                this.BattleFoundEvent(combatant, new BattleCreatedEventData(combatant, data));
        }

        public void DisconnectUserFromBattle(ConnectedClient userToDisconnect)
        {
            var disconnectedUserActiveBattle = this.ActiveBattles.FirstOrDefault(b => b.Combatants.Any(c => c.Client.Equals(userToDisconnect)));

            if (disconnectedUserActiveBattle == null)
                return;
            
            disconnectedUserActiveBattle.EndBattle(userToDisconnect);
            
            Console.WriteLine("Combatants disconnected from battle.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using PvPNet;

namespace PvPNetServer
{
    public class Battle
    {
        private List<CombatantInfo> combatants = new List<CombatantInfo>();
        
        public Guid BattleId
        {
            get;
            private set;
        }

        public bool IsBattleEnded
        {
            get; 
            private set;
        }

        public List<CombatantInfo> Combatants
        {
            get
            {
                return this.combatants;
            }
        }

        public void EndBattle(ConnectedClient lostUser)
        {
            var loser = combatants.FirstOrDefault(c => c.Client.Equals(lostUser));

            if (loser == null)
            {
                //
                // Cant end battle if this battle is not provided user battle.
                //
                return;
            }

            //
            // Check if client connected. If no then there is no point to end battle for him.
            //
            if (lostUser.IsClientConnected)
            {
                loser.Client.SendResponse(new ServerResponse()
                {
                    //
                    // IsSuccess basicaly used to show who won if battle is ended.
                    //
                    IsSuccess = false,
                    Operations = ClientOperationsType.SendBattleTurn,
                    ResponseData = new OperationDataHandler().GetBytes(new BattleResponseData
                    {
                        IsBattleContinue = false
                    })
                });
            }

            
            var winer = combatants.FirstOrDefault(c => !c.Client.Equals(lostUser));

            winer.Client.SendResponse(new ServerResponse()
            {
                //
                // IsSuccess basicaly used to show who won if battle is ended.
                //
                IsSuccess = true,
                Operations = ClientOperationsType.SendBattleTurn,
                ResponseData = new OperationDataHandler().GetBytes(new BattleResponseData
                {
                    IsBattleContinue = false
                })
            });
        }

        public Battle(CombatantInfo firstPlayer, CombatantInfo secondPlayer, Guid battleId)
        {
            this.combatants.Add(firstPlayer);
            this.combatants.Add(secondPlayer);
            this.BattleId = battleId;
        }

        public void MakeMove(ConnectedClient client, BattleHitZone hitLocation, BattleHitZone blockLocation, string targetName)
        {
            var combatant = this.combatants.First(cl => cl.Client == client);
            combatant.SetMove(hitLocation, blockLocation, targetName);
            
            this.CheckForEndOfRound();
        }

        private void CheckForEndOfRound()
        {
            if (this.combatants.All(x => x.IsMoveMade))
            {
                Dictionary<CombatantInfo, BattleResponseData> battleMoves = new Dictionary<CombatantInfo, BattleResponseData>();
                
                foreach (CombatantInfo combatant in this.combatants)
                {
                    BattleResponseData responseData = new BattleResponseData();
                    
                    CombatantInfo target =
                        this.combatants.FirstOrDefault(x => x.Client.ClientName.Equals(combatant.CurrentTargetName));

                    if (target == null)
                    {
                        //
                        // TODO: never should be null. But if null occurs then add handling here.
                        //
                    }

                    if (combatant.HitLocation != target.BlockLocation)
                    {
                        responseData.AttackMessageLog =
                            string.Format(
                                "You managed to hit the enemy in the {0} and he couldn't block it. Enemy took {1} damage",
                                combatant.HitLocation.Value.ToString("G"), (byte)combatant.HitLocation.Value);

                        target.CurentHp -= (byte) combatant.HitLocation.Value;
                        
                        responseData.EnemyCurrentHp = target.CurentHp;
                    }
                    else
                    {
                        responseData.AttackMessageLog =
                            string.Format(
                                "You tried to hit the enemy in the {0} but he blocked it. No damage.",
                                combatant.HitLocation.Value.ToString("G"));

                        responseData.EnemyCurrentHp = target.CurentHp;
                    }


                    if (target.HitLocation != combatant.BlockLocation)
                    {
                        responseData.EnemyAttackMessageLog = string.Format(
                            " Enemy hit you in the {0} while you tried to block {1}. You took {2} damage ",
                            target.HitLocation.Value.ToString("G"), combatant.BlockLocation.Value.ToString("G"),
                            (byte) target.HitLocation.Value);

                        responseData.CurrentHp = combatant.CurentHp - (byte) target.HitLocation.Value;
                    }
                    else
                    {
                        responseData.EnemyAttackMessageLog = string.Format(
                            " Enemy hit you in the {0} and you managed to block it. No damage ",
                            target.HitLocation.Value.ToString("G"));

                        responseData.CurrentHp = combatant.CurentHp;
                    }
                    
                    battleMoves.Add(combatant, responseData);
                }
               
                //
                // Check for battle end conditions before send the result.
                //

                foreach (KeyValuePair<CombatantInfo,BattleResponseData> battleMove in battleMoves)
                {
                    battleMove.Value.IsBattleContinue = battleMove.Key.CurentHp > 0
                                                        && this.combatants.First(x =>
                                                            x.Client.ClientName.Equals(battleMove.Key
                                                                .CurrentTargetName)).CurentHp > 0;

                    battleMove.Value.CurrentHp = battleMove.Key.CurentHp;
                    
                    battleMove.Key.Client.SendResponse(new ServerResponse()
                    {
                        //
                        // IsSuccess basicaly used to show who won if battle is ended.
                        //
                        IsSuccess = !battleMove.Value.IsBattleContinue && battleMove.Key.CurentHp > 0,
                        Operations = ClientOperationsType.SendBattleTurn,
                        ResponseData = new OperationDataHandler().GetBytes(battleMove.Value)
                    });
                }

                this.IsBattleEnded = battleMoves.Values.All(x => !x.IsBattleContinue);
                
                this.combatants.ForEach(x => x.ClearBattleData());
            }
        }
    }
}
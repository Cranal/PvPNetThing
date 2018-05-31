using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PvPNet;
using PvPNetThing.Annotations;

namespace PvPNetThing
{
    public class BattleViewModel : INotifyPropertyChanged
    {
        private Visibility isBattleVisible;
        private ClientEngine engine;
        private CommandHandler attackCommand;
        private List<string> battleLog;
        private int currentHp;
        private int enemyCurrentHp;
        
        public CommandHandler AttackCommand
        {
            get
            {
                return this.attackCommand ?? (this.attackCommand = new CommandHandler(() => this.PerformAttack(), true));
            }
        }

        public List<string> BattleLog
        {
            get
            {
                if (this.battleLog == null)
                    this.battleLog = new List<string>();
                
                return this.battleLog;
            }
            set
            {
                this.battleLog = value;
                this.OnPropertyChanged("BattleLog");
            }
        }

        public void AddMessageLog(string message)
        {
            this.BattleLog.Add(message);
            this.OnPropertyChanged("BattleLog");
        }

        private void PerformAttack()
        {
            BattleHitZone? attackZone = this.AttackTarget.GetHitZone();
            BattleHitZone? defenceZone = this.BlockTarget.GetHitZone();

            if (!attackZone.HasValue || !defenceZone.HasValue)
            {
                MessageBox.Show("You need to chose targets for attack and block before making the move.", "Choose move",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            BattleMoveData data = new BattleMoveData();
            data.Attack = attackZone.Value;
            data.Defence = defenceZone.Value;
            data.TargetName = this.EnemyName;
            this.engine.MakeBattleMove(data);
            
            //
            // Clear data for next move.
            //

            this.AttackTarget.ClearZones();
            this.BlockTarget.ClearZones();
        }

        public BattleViewModel(ClientEngine engine)
        {
            this.engine = engine;
            this.BlockTarget = new TargetViewModel();
            this.AttackTarget = new TargetViewModel();
        }
        
        public Visibility IsBattleVisible
        {
            get
            {
                return this.isBattleVisible;
            }
            set
            {
                this.isBattleVisible = value;
                this.OnPropertyChanged("IsBattleVisible");
                
            }
        }

        public string PlayerName
        {
            get
            {
                return this.engine.ClientName;
            }
        }
        
        public string EnemyName
        {
            get;
            set;
        }

        public int CurrentHp
        {
            get
            {
                return this.currentHp;
            }
            set
            {
                this.currentHp = value;
                this.OnPropertyChanged("CurrentHp");
            }
        }
        
        public int EnemyCurrentHp
        {
            get
            {
                return this.enemyCurrentHp;
            }
            set
            {
                this.enemyCurrentHp = value;
                this.OnPropertyChanged("EnemyCurrentHp");
            }
        }

        public TargetViewModel BlockTarget
        {
            get; 
            set;
        }
        
        public TargetViewModel AttackTarget
        {
            get; 
            set;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
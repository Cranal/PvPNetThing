using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PvPNetThing.Annotations;

namespace PvPNetThing
{
    public class BattleEndViewModel : INotifyPropertyChanged
    {
        private Visibility isVictoryVisible;
        private Visibility isLostVisible;
        private Visibility isBattleEndVisible;
        private CommandHandler mainScreenCommand;
        private ClientEngine engine;
        
        public CommandHandler MainScreenCommand
        {
            get
            {
                return mainScreenCommand ?? (mainScreenCommand = new CommandHandler(() => this.MoveToMainScreen(), true));
            }
        }

        public BattleEndViewModel(ClientEngine engine)
        {
            this.engine = engine;
            this.IsBattleEndVisible = Visibility.Collapsed;
            this.IsLostVisible = Visibility.Collapsed;
            this.IsVictoryVisible = Visibility.Collapsed;
        }

        private void MoveToMainScreen()
        {
            this.engine.BattleStatus = BattleStatus.NoBattle;
        }
        
        public Visibility IsBattleEndVisible
        {
            get
            {
                return this.isBattleEndVisible;
            }
            set
            {
                this.isBattleEndVisible = value;
                this.OnPropertyChanged("IsBattleEndVisible");
            }
        }
        
        public Visibility IsVictoryVisible
        {
            get
            {
                return this.isVictoryVisible;
            }
            set
            {
                this.isVictoryVisible = value;
                this.OnPropertyChanged("IsVictoryVisible");
            }
        }
        
        public Visibility IsLostVisible
        {
            get
            {
                return this.isLostVisible;
            }
            set
            {
                this.isLostVisible = value;
                this.OnPropertyChanged("IsLostVisible");
            }
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
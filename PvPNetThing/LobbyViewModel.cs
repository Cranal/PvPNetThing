using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PvPNetThing.Annotations;

namespace PvPNetThing
{
    public class LobbyViewModel : INotifyPropertyChanged
    {
        private CommandHandler findMatchCommand;
        private ClientEngine engine;
        private Visibility isVisible;
        
        public Visibility IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                if (this.IsVisible == value)
                    return;

                this.isVisible = value;
                this.OnPropertyChanged("IsVisible");
            }
        }

        public LobbyViewModel(ClientEngine engine)
        {
            this.IsVisible = Visibility.Collapsed;
            this.engine = engine;
        }
        
        public CommandHandler FindMatchCommand
        {
            get
            {
                return this.findMatchCommand ?? (this.findMatchCommand = new CommandHandler(() => this.FindMatch(), true));
            }
        }

        private void FindMatch()
        {
            this.engine.FindBatle();
            this.engine.BattleStatus = BattleStatus.LookingForBattle;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
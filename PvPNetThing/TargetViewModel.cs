using System.ComponentModel;
using System.Runtime.CompilerServices;
using PvPNet;
using PvPNetThing.Annotations;

namespace PvPNetThing
{
    public class TargetViewModel : INotifyPropertyChanged
    {
        public bool Head
        {
            get;
            set;
        }
        
        public bool Torso
        {
            get;
            set;
        }
        
        public bool Arms
        {
            get;
            set;
        }
        
        public bool Legs
        {
            get;
            set;
        }

        public BattleHitZone? GetHitZone()
        {
            if (this.Head)
            {
                return BattleHitZone.Head;
            }

            if (this.Torso)
            {
                return BattleHitZone.Torso;
            }

            if (this.Arms)
            {
                return BattleHitZone.Arms;
            }

            if (this.Legs)
            {
                return BattleHitZone.Legs;
            }

            return null;
        }

        public void ClearZones()
        {
            this.Arms = false;
            this.Head = false;
            this.Legs = false;
            this.Torso = false;
            this.OnPropertyChanged("Arms");
            this.OnPropertyChanged("Head");
            this.OnPropertyChanged("Legs");
            this.OnPropertyChanged("Torso");
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
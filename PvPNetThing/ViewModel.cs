
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using PvPNetThing.Annotations;

namespace PvPNetThing
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        private ClientEngine engine;
        
        private Visibility isLoginProcessVisible;
        
        private BattleViewModel battleViewModel;
        private BattleEndViewModel battleEndViewModel;
        
        public LoginViewModel LoginModel
        {
            get;
            private set;
        }
        
        public LobbyViewModel LobbyViewModel
        {
            get;
            private set;
        }

        public BattleViewModel BattleViewModel
        {
            get
            {
                return this.battleViewModel;
            }
            private set
            {
                this.battleViewModel = value;
                this.OnPropertyChanged("BattleViewModel");
            }
        }
        
        public BattleEndViewModel BattleEndViewModel
        {
            get
            {
                return this.battleEndViewModel;
            }
            private set
            {
                this.battleEndViewModel = value;
                this.OnPropertyChanged("BattleEndViewModel");
            }
        }

        public ViewModel()
        {
            this.engine = new ClientEngine();
            this.engine.PropertyChanged += this.EngineOnPropertyChanged;
            this.engine.BattleStatusChanged += EngineOnBattleStatusChanged;
            this.LoginModel = new LoginViewModel(this.engine);
            this.LobbyViewModel = new LobbyViewModel(this.engine);
            this.LobbyViewModel.IsVisible = Visibility.Collapsed;
            this.IsLoginProcessVisible = Visibility.Collapsed;
            this.BattleEndViewModel = new BattleEndViewModel(this.engine);
        }

        private void EngineOnBattleStatusChanged(object sender, BattleUpdateEventArgs e)
        {
            switch (e.BattleStatusData.BattleStatus)
            {
                case BattleStatus.Battle:
                    if (e.BattleStatusData.NewBattleData != null)
                    {
                        this.BattleViewModel.CurrentHp = 200;
                        this.BattleViewModel.EnemyCurrentHp = 200;
                        this.BattleViewModel.EnemyName = e.BattleStatusData.NewBattleData.OponentName;
                    }
                    else if (e.BattleStatusData.ResponseData != null)
                    {
                        this.BattleViewModel.CurrentHp = e.BattleStatusData.ResponseData.CurrentHp * 2;
                        this.BattleViewModel.EnemyCurrentHp = e.BattleStatusData.ResponseData.EnemyCurrentHp * 2;
                        this.BattleViewModel.AddMessageLog(e.BattleStatusData.ResponseData.AttackMessageLog);
                        this.BattleViewModel.AddMessageLog(e.BattleStatusData.ResponseData.EnemyAttackMessageLog);
                    }
                        
                    break;
            }
        }

        public Visibility IsLoginProcessVisible
        {
            get
            {
                return this.isLoginProcessVisible;
            }
            set
            {
                this.isLoginProcessVisible = value;
                this.OnPropertyChanged("IsLoginProcessVisible");
            }
        }

        private void EngineOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LoginStatus":
                    this.ChangeLoginView();
                    break;
                case "BattleStatus":
                    this.ChangeBattleView();
                    break;
            }
        }

        private void ChangeBattleView()
        {
            switch (this.engine.BattleStatus)
            {
                case BattleStatus.LookingForBattle:
                    this.IsLoginProcessVisible = Visibility.Visible;
                    this.LobbyViewModel.IsVisible = Visibility.Collapsed;
                    break;
                case BattleStatus.NoBattle:
                    this.IsLoginProcessVisible = Visibility.Collapsed;
                    this.BattleEndViewModel.IsBattleEndVisible = Visibility.Collapsed;
                    this.LobbyViewModel.IsVisible = Visibility.Visible;
                    break;
                case BattleStatus.Battle:
                    this.IsLoginProcessVisible = Visibility.Collapsed;
                    if (this.BattleViewModel == null)
                        this.BattleViewModel = new BattleViewModel(this.engine);
                    this.BattleViewModel.IsBattleVisible = Visibility.Visible;
                    break;
                case BattleStatus.Wait:
                    this.BattleViewModel.IsBattleVisible = Visibility.Collapsed;
                    this.IsLoginProcessVisible = Visibility.Visible;
                    break;
                case BattleStatus.Defeat:
                    this.IsLoginProcessVisible = Visibility.Collapsed;
                    this.BattleViewModel.IsBattleVisible = Visibility.Collapsed;
                    this.BattleViewModel = null;
                    this.battleEndViewModel.IsLostVisible = Visibility.Visible;
                    this.battleEndViewModel.IsVictoryVisible = Visibility.Collapsed;
                    this.BattleEndViewModel.IsBattleEndVisible = Visibility.Visible;
                    break;
                case BattleStatus.Victory:
                    this.IsLoginProcessVisible = Visibility.Collapsed;
                    this.BattleViewModel.IsBattleVisible = Visibility.Collapsed;
                    this.BattleViewModel = null;
                    this.battleEndViewModel.IsLostVisible = Visibility.Collapsed;
                    this.battleEndViewModel.IsVictoryVisible = Visibility.Visible;
                    this.BattleEndViewModel.IsBattleEndVisible = Visibility.Visible;
                    break;
            }
        }

        private void ChangeLoginView()
        {
            switch (this.engine.LoginStatus)
            {
                case LoginStatus.LoggedIn:
                    this.LoginModel.IsLogingVisible = Visibility.Collapsed;
                    this.IsLoginProcessVisible = Visibility.Collapsed;
                    this.LobbyViewModel.IsVisible = Visibility.Visible;
                    break;
                
                case LoginStatus.Loggining:
                    this.LoginModel.IsLogingVisible = Visibility.Collapsed;
                    this.IsLoginProcessVisible = Visibility.Visible;
                    this.LobbyViewModel.IsVisible = Visibility.Collapsed;
                    break;
                    
                case LoginStatus.LoginError:
                    this.IsLoginProcessVisible = Visibility.Collapsed;
                    this.LoginModel.IsLogingVisible = Visibility.Visible;
                    this.LoginModel.IsLoginError = Visibility.Visible;
                    this.LobbyViewModel.IsVisible = Visibility.Collapsed;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            this.engine.Dispose();
        }
    }
}
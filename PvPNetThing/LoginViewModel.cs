using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using PvPNetThing.Annotations;

namespace PvPNetThing
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private CommandHandler clickCommand;
        private CommandHandler createCommand;
        private string login;
        private string password;
        public event PropertyChangedEventHandler PropertyChanged;
        private ClientEngine engine;
        private Visibility isLogingVisible;
        private Visibility isLoginError;
        
        public Visibility IsLogingVisible
        {
            get
            {
                return this.isLogingVisible;           
            }
            set
            {
                this.isLogingVisible = value;
                this.OnPropertyChanged("IsLogingVisible");
            }
        }

        public Visibility IsLoginError
        {
            get
            {
                return this.isLoginError;           
            }
            set
            {
                this.isLoginError = value;
                this.OnPropertyChanged("IsLoginError");
            }
        }

        public LoginViewModel(ClientEngine engine)
        {
            this.engine = engine;
            this.IsLogingVisible = Visibility.Visible;
            this.IsLoginError = Visibility.Collapsed;
        }
        
        public CommandHandler ClickCommand
        {
            get
            {
                return clickCommand ?? (clickCommand = new CommandHandler(() => this.PerformLogin(), true));
            }
        }
        
        public CommandHandler CreateCommand
        {
            get
            {
                return this.createCommand ?? (this.createCommand  = new CommandHandler(() => this.CreateUser(), true));
            }
        }
        
        private void PerformLogin()
        {
             this.engine.PerformLogin(this.Login, this.Password);
        }

        private void CreateUser()
        {
            this.engine.CreateUser(this.Login, this.Password);
        }
        
        public string Login
        {
            get
            {
                return this.login;
            }
            set
            {
                this.login = value;
                this.OnPropertyChanged("Login");
            }
        }
        
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
                this.OnPropertyChanged("Password");
            }
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System;
using System.Windows.Input;

namespace PvPNetThing
{
    public class CommandHandler : ICommand
    {
        private Action action;
        private bool canExecute;
        public event EventHandler CanExecuteChanged;

        public CommandHandler(Action action, bool canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public void Execute(object parameter)
        {
            action();
        }
        
        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }
    }
}
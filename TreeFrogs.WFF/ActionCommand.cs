using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TreeFrogs.WFF
{
    public class ActionCommand: ICommand
    {
        private readonly Func<object, bool> canExecute;
        private readonly Action<object> execute;

        public ActionCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;

            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
